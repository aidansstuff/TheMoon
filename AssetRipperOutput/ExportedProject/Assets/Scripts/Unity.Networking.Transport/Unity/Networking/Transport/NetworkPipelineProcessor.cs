using System;
using System.Diagnostics;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Networking.Transport
{
	internal struct NetworkPipelineProcessor : IDisposable
	{
		public struct Concurrent
		{
			[ReadOnly]
			internal NativeArray<NetworkPipelineStage> m_StageCollection;

			[ReadOnly]
			internal NativeArray<byte> m_StaticInstanceBuffer;

			[ReadOnly]
			internal NativeList<PipelineImpl> m_Pipelines;

			[ReadOnly]
			internal NativeList<int> m_StageList;

			[ReadOnly]
			internal NativeList<int> m_AccumulatedHeaderCapacity;

			internal NativeQueue<UpdatePipeline>.ParallelWriter m_SendStageNeedsUpdateWrite;

			[ReadOnly]
			internal NativeArray<int> sizePerConnection;

			[ReadOnly]
			internal NativeList<byte> sharedBuffer;

			[ReadOnly]
			internal NativeList<byte> sendBuffer;

			[ReadOnly]
			internal NativeArray<long> m_timestamp;

			public int SendHeaderCapacity(NetworkPipeline pipeline)
			{
				return m_Pipelines[pipeline.Id - 1].headerCapacity;
			}

			public int PayloadCapacity(NetworkPipeline pipeline)
			{
				if (pipeline.Id > 0)
				{
					return m_Pipelines[pipeline.Id - 1].payloadCapacity;
				}
				return 0;
			}

			public unsafe int Send(NetworkDriver.Concurrent driver, NetworkPipeline pipeline, NetworkConnection connection, NetworkInterfaceSendHandle sendHandle, int headerSize)
			{
				if (sendHandle.data == IntPtr.Zero)
				{
					return -8;
				}
				int networkId = connection.m_NetworkId;
				int* unsafeReadOnlyPtr = (int*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(sendBuffer);
				unsafeReadOnlyPtr += networkId * sizePerConnection[0] / 4;
				if (Interlocked.CompareExchange(ref *unsafeReadOnlyPtr, 1, 0) != 0)
				{
					driver.AbortSend(sendHandle);
					return -7;
				}
				NativeList<UpdatePipeline> currentUpdates = new NativeList<UpdatePipeline>(128, Allocator.Temp);
				int result = ProcessPipelineSend(driver, 0, pipeline, connection, sendHandle, headerSize, currentUpdates);
				Interlocked.Exchange(ref *unsafeReadOnlyPtr, 0);
				for (int i = 0; i < currentUpdates.Length; i++)
				{
					m_SendStageNeedsUpdateWrite.Enqueue(currentUpdates[i]);
				}
				return result;
			}

			internal unsafe int ProcessPipelineSend(NetworkDriver.Concurrent driver, int startStage, NetworkPipeline pipeline, NetworkConnection connection, NetworkInterfaceSendHandle sendHandle, int headerSize, NativeList<UpdatePipeline> currentUpdates)
			{
				int num = headerSize;
				int num2 = sendHandle.size;
				NetworkPipelineContext ctx = default(NetworkPipelineContext);
				ctx.timestamp = m_timestamp[0];
				PipelineImpl p = m_Pipelines[pipeline.Id - 1];
				int networkId = connection.m_NetworkId;
				int systemHeaderSize = driver.MaxProtocolHeaderSize();
				bool flag = sendHandle.data == IntPtr.Zero;
				int num3 = 0;
				NativeList<int> resumeQ = new NativeList<int>(16, Allocator.Temp);
				int num4 = 0;
				InboundSendBuffer inboundBuffer = default(InboundSendBuffer);
				if (!flag)
				{
					inboundBuffer.bufferWithHeaders = (byte*)(void*)sendHandle.data + num + 1;
					inboundBuffer.bufferWithHeadersLength = sendHandle.size - num - 1;
					inboundBuffer.buffer = inboundBuffer.bufferWithHeaders + p.headerCapacity;
					inboundBuffer.bufferLength = inboundBuffer.bufferWithHeadersLength - p.headerCapacity;
				}
				while (true)
				{
					headerSize = p.headerCapacity;
					int num5 = p.sendBufferOffset + sizePerConnection[0] * networkId;
					int num6 = p.sharedBufferOffset + sizePerConnection[2] * networkId;
					if (startStage > 0)
					{
						if (inboundBuffer.bufferWithHeadersLength > 0)
						{
							UnityEngine.Debug.LogError("Can't start from a stage with a buffer");
							return -3;
						}
						for (int i = 0; i < startStage; i++)
						{
							num5 += (m_StageCollection[m_StageList[p.FirstStageIndex + i]].SendCapacity + 7) & -8;
							num6 += (m_StageCollection[m_StageList[p.FirstStageIndex + i]].SharedStateCapacity + 7) & -8;
							headerSize -= m_StageCollection[m_StageList[p.FirstStageIndex + i]].HeaderCapacity;
						}
					}
					for (int j = startStage; j < p.NumStages; j++)
					{
						int headerCapacity = m_StageCollection[m_StageList[p.FirstStageIndex + j]].HeaderCapacity;
						inboundBuffer.headerPadding = headerSize;
						headerSize -= headerCapacity;
						if (headerCapacity > 0 && inboundBuffer.bufferWithHeadersLength > 0)
						{
							NativeArray<byte> data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(inboundBuffer.bufferWithHeaders + headerSize, headerCapacity, Allocator.Invalid);
							ctx.header = new DataStreamWriter(data);
						}
						else
						{
							ctx.header = new DataStreamWriter(headerCapacity, Allocator.Temp);
						}
						InboundSendBuffer inboundSendBuffer = inboundBuffer;
						NetworkPipelineStage.Requests requests = NetworkPipelineStage.Requests.None;
						int num7 = ProcessSendStage(j, num5, num6, p, ref resumeQ, ref ctx, ref inboundBuffer, ref requests, systemHeaderSize);
						if ((requests & NetworkPipelineStage.Requests.Update) != 0)
						{
							AddSendUpdate(connection, j, pipeline, currentUpdates);
						}
						if (inboundBuffer.bufferWithHeadersLength == 0)
						{
							if ((requests & NetworkPipelineStage.Requests.Error) != 0 && !flag)
							{
								num2 = num7;
								num3 = num7;
							}
							break;
						}
						if (inboundBuffer.buffer != inboundSendBuffer.buffer)
						{
							UnsafeUtility.MemCpy(inboundBuffer.bufferWithHeaders + headerSize, ctx.header.AsNativeArray().GetUnsafeReadOnlyPtr(), ctx.header.Length);
						}
						if (ctx.header.Length < headerCapacity)
						{
							int num8 = headerCapacity - ctx.header.Length;
							UnsafeUtility.MemMove(inboundBuffer.buffer - num8, inboundBuffer.buffer, inboundBuffer.bufferLength);
						}
						inboundBuffer.buffer = inboundBuffer.bufferWithHeaders + headerSize;
						inboundBuffer.bufferLength = ctx.header.Length + inboundBuffer.bufferLength;
						num5 += (ctx.internalProcessBufferLength + 7) & -8;
						num6 += (ctx.internalSharedProcessBufferLength + 7) & -8;
					}
					if (inboundBuffer.bufferLength != 0)
					{
						DataStreamWriter writer;
						if (sendHandle.data != IntPtr.Zero && inboundBuffer.bufferWithHeaders == (byte*)(void*)sendHandle.data + num + 1)
						{
							if (inboundBuffer.buffer != inboundBuffer.bufferWithHeaders)
							{
								UnsafeUtility.MemMove(inboundBuffer.bufferWithHeaders, inboundBuffer.buffer, inboundBuffer.bufferLength);
								inboundBuffer.buffer = inboundBuffer.bufferWithHeaders;
							}
							((byte*)(void*)sendHandle.data)[num] = (byte)pipeline.Id;
							int size = num + 1 + inboundBuffer.bufferLength;
							sendHandle.size = size;
							if ((num2 = driver.CompleteSend(connection, sendHandle, hasPipeline: true)) < 0)
							{
								UnityEngine.Debug.LogWarning(FixedString.Format("CompleteSend failed with the following error code: {0}", num2));
							}
							sendHandle = default(NetworkInterfaceSendHandle);
						}
						else if (driver.BeginSend(connection, out writer) == 0)
						{
							writer.WriteByte((byte)pipeline.Id);
							writer.WriteBytes(inboundBuffer.buffer, inboundBuffer.bufferLength);
							if ((num2 = driver.EndSend(writer)) <= 0)
							{
								UnityEngine.Debug.Log(FixedString.Format("An error occurred during EndSend. ErrorCode: {0}", num2));
							}
						}
					}
					if (num4 >= resumeQ.Length)
					{
						break;
					}
					startStage = resumeQ[num4++];
					inboundBuffer = default(InboundSendBuffer);
				}
				if (sendHandle.data != IntPtr.Zero)
				{
					driver.AbortSend(sendHandle);
				}
				if (num3 >= 0)
				{
					return num2;
				}
				return num3;
			}

			private unsafe int ProcessSendStage(int startStage, int internalBufferOffset, int internalSharedBufferOffset, PipelineImpl p, ref NativeList<int> resumeQ, ref NetworkPipelineContext ctx, ref InboundSendBuffer inboundBuffer, ref NetworkPipelineStage.Requests requests, int systemHeaderSize)
			{
				int index = p.FirstStageIndex + startStage;
				NetworkPipelineStage networkPipelineStage = m_StageCollection[m_StageList[index]];
				ctx.accumulatedHeaderCapacity = m_AccumulatedHeaderCapacity[index];
				ctx.staticInstanceBuffer = (byte*)m_StaticInstanceBuffer.GetUnsafeReadOnlyPtr() + networkPipelineStage.StaticStateStart;
				ctx.staticInstanceBufferLength = networkPipelineStage.StaticStateCapcity;
				ctx.internalProcessBuffer = (byte*)sendBuffer.GetUnsafeReadOnlyPtr() + internalBufferOffset;
				ctx.internalProcessBufferLength = networkPipelineStage.SendCapacity;
				ctx.internalSharedProcessBuffer = (byte*)sharedBuffer.GetUnsafeReadOnlyPtr() + internalSharedBufferOffset;
				ctx.internalSharedProcessBufferLength = networkPipelineStage.SharedStateCapacity;
				requests = NetworkPipelineStage.Requests.None;
				int result = networkPipelineStage.Send.Ptr.Invoke(ref ctx, ref inboundBuffer, ref requests, systemHeaderSize);
				if ((requests & NetworkPipelineStage.Requests.Resume) != 0)
				{
					resumeQ.Add(in startStage);
				}
				return result;
			}
		}

		internal struct PipelineImpl
		{
			public int FirstStageIndex;

			public int NumStages;

			public int receiveBufferOffset;

			public int sendBufferOffset;

			public int sharedBufferOffset;

			public int headerCapacity;

			public int payloadCapacity;
		}

		internal struct UpdatePipeline
		{
			public NetworkPipeline pipeline;

			public int stage;

			public NetworkConnection connection;
		}

		public const int Alignment = 8;

		public const int AlignmentMinusOne = 7;

		private NativeArray<NetworkPipelineStage> m_StageCollection;

		private NativeArray<byte> m_StaticInstanceBuffer;

		private NativeList<int> m_StageList;

		private NativeList<int> m_AccumulatedHeaderCapacity;

		private NativeList<PipelineImpl> m_Pipelines;

		private NativeList<byte> m_ReceiveBuffer;

		private NativeList<byte> m_SendBuffer;

		private NativeList<byte> m_SharedBuffer;

		private NativeList<UpdatePipeline> m_ReceiveStageNeedsUpdate;

		private NativeList<UpdatePipeline> m_SendStageNeedsUpdate;

		private NativeQueue<UpdatePipeline> m_SendStageNeedsUpdateRead;

		private NativeArray<int> sizePerConnection;

		private NativeArray<long> m_timestamp;

		private const int SendSizeOffset = 0;

		private const int RecveiveSizeOffset = 1;

		private const int SharedSizeOffset = 2;

		public long Timestamp
		{
			get
			{
				return m_timestamp[0];
			}
			internal set
			{
				m_timestamp[0] = value;
			}
		}

		public int PayloadCapacity(NetworkPipeline pipeline)
		{
			if (pipeline.Id > 0)
			{
				return m_Pipelines[pipeline.Id - 1].payloadCapacity;
			}
			return 0;
		}

		public Concurrent ToConcurrent()
		{
			Concurrent result = default(Concurrent);
			result.m_StageCollection = m_StageCollection;
			result.m_StaticInstanceBuffer = m_StaticInstanceBuffer;
			result.m_Pipelines = m_Pipelines;
			result.m_StageList = m_StageList;
			result.m_AccumulatedHeaderCapacity = m_AccumulatedHeaderCapacity;
			result.m_SendStageNeedsUpdateWrite = m_SendStageNeedsUpdateRead.AsParallelWriter();
			result.sizePerConnection = sizePerConnection;
			result.sendBuffer = m_SendBuffer;
			result.sharedBuffer = m_SharedBuffer;
			result.m_timestamp = m_timestamp;
			return result;
		}

		public unsafe NetworkPipelineProcessor(NetworkSettings settings)
		{
			NetworkPipelineParams pipelineParameters = settings.GetPipelineParameters();
			int num = 0;
			for (int i = 0; i < NetworkPipelineStageCollection.m_stages.Count; i++)
			{
				num += NetworkPipelineStageCollection.m_stages[i].StaticSize;
				num = (num + 15) & -16;
			}
			m_StaticInstanceBuffer = new NativeArray<byte>(num, Allocator.Persistent);
			m_StageCollection = new NativeArray<NetworkPipelineStage>(NetworkPipelineStageCollection.m_stages.Count, Allocator.Persistent);
			num = 0;
			for (int j = 0; j < NetworkPipelineStageCollection.m_stages.Count; j++)
			{
				NetworkPipelineStage value = NetworkPipelineStageCollection.m_stages[j].StaticInitialize((byte*)m_StaticInstanceBuffer.GetUnsafePtr() + num, NetworkPipelineStageCollection.m_stages[j].StaticSize, settings);
				value.StaticStateStart = num;
				value.StaticStateCapcity = NetworkPipelineStageCollection.m_stages[j].StaticSize;
				m_StageCollection[j] = value;
				num += NetworkPipelineStageCollection.m_stages[j].StaticSize;
				num = (num + 15) & -16;
			}
			m_StageList = new NativeList<int>(16, Allocator.Persistent);
			m_AccumulatedHeaderCapacity = new NativeList<int>(16, Allocator.Persistent);
			m_Pipelines = new NativeList<PipelineImpl>(16, Allocator.Persistent);
			m_ReceiveBuffer = new NativeList<byte>(pipelineParameters.initialCapacity, Allocator.Persistent);
			m_SendBuffer = new NativeList<byte>(pipelineParameters.initialCapacity, Allocator.Persistent);
			m_SharedBuffer = new NativeList<byte>(pipelineParameters.initialCapacity, Allocator.Persistent);
			sizePerConnection = new NativeArray<int>(3, Allocator.Persistent);
			sizePerConnection[0] = 8;
			m_ReceiveStageNeedsUpdate = new NativeList<UpdatePipeline>(128, Allocator.Persistent);
			m_SendStageNeedsUpdate = new NativeList<UpdatePipeline>(128, Allocator.Persistent);
			m_SendStageNeedsUpdateRead = new NativeQueue<UpdatePipeline>(Allocator.Persistent);
			m_timestamp = new NativeArray<long>(1, Allocator.Persistent);
		}

		public void Dispose()
		{
			m_StageList.Dispose();
			m_AccumulatedHeaderCapacity.Dispose();
			m_ReceiveBuffer.Dispose();
			m_SendBuffer.Dispose();
			m_SharedBuffer.Dispose();
			m_Pipelines.Dispose();
			sizePerConnection.Dispose();
			m_ReceiveStageNeedsUpdate.Dispose();
			m_SendStageNeedsUpdate.Dispose();
			m_SendStageNeedsUpdateRead.Dispose();
			m_timestamp.Dispose();
			m_StageCollection.Dispose();
			m_StaticInstanceBuffer.Dispose();
		}

		public unsafe void initializeConnection(NetworkConnection con)
		{
			int num = (con.m_NetworkId + 1) * sizePerConnection[1];
			int num2 = (con.m_NetworkId + 1) * sizePerConnection[0];
			int num3 = (con.m_NetworkId + 1) * sizePerConnection[2];
			if (m_ReceiveBuffer.Length < num)
			{
				m_ReceiveBuffer.ResizeUninitialized(num);
			}
			if (m_SendBuffer.Length < num2)
			{
				m_SendBuffer.ResizeUninitialized(num2);
			}
			if (m_SharedBuffer.Length < num3)
			{
				m_SharedBuffer.ResizeUninitialized(num3);
			}
			UnsafeUtility.MemClear((byte*)m_ReceiveBuffer.GetUnsafePtr() + con.m_NetworkId * sizePerConnection[1], sizePerConnection[1]);
			UnsafeUtility.MemClear((byte*)m_SendBuffer.GetUnsafePtr() + con.m_NetworkId * sizePerConnection[0], sizePerConnection[0]);
			UnsafeUtility.MemClear((byte*)m_SharedBuffer.GetUnsafePtr() + con.m_NetworkId * sizePerConnection[2], sizePerConnection[2]);
			InitializeStages(con.m_NetworkId);
		}

		private unsafe void InitializeStages(int networkId)
		{
			for (int i = 0; i < m_Pipelines.Length; i++)
			{
				PipelineImpl pipelineImpl = m_Pipelines[i];
				int num = pipelineImpl.receiveBufferOffset + sizePerConnection[1] * networkId;
				int num2 = pipelineImpl.sendBufferOffset + sizePerConnection[0] * networkId;
				int num3 = pipelineImpl.sharedBufferOffset + sizePerConnection[2] * networkId;
				for (int j = pipelineImpl.FirstStageIndex; j < pipelineImpl.FirstStageIndex + pipelineImpl.NumStages; j++)
				{
					NetworkPipelineStage networkPipelineStage = m_StageCollection[m_StageList[j]];
					byte* sendProcessBuffer = (byte*)m_SendBuffer.GetUnsafePtr() + num2;
					int sendCapacity = networkPipelineStage.SendCapacity;
					byte* recvProcessBuffer = (byte*)m_ReceiveBuffer.GetUnsafePtr() + num;
					int receiveCapacity = networkPipelineStage.ReceiveCapacity;
					byte* sharedProcessBuffer = (byte*)m_SharedBuffer.GetUnsafePtr() + num3;
					int sharedStateCapacity = networkPipelineStage.SharedStateCapacity;
					byte* staticInstanceBuffer = (byte*)m_StaticInstanceBuffer.GetUnsafePtr() + networkPipelineStage.StaticStateStart;
					int staticStateCapcity = networkPipelineStage.StaticStateCapcity;
					networkPipelineStage.InitializeConnection.Ptr.Invoke(staticInstanceBuffer, staticStateCapcity, sendProcessBuffer, sendCapacity, recvProcessBuffer, receiveCapacity, sharedProcessBuffer, sharedStateCapacity);
					num2 += (sendCapacity + 7) & -8;
					num += (receiveCapacity + 7) & -8;
					num3 += (sharedStateCapacity + 7) & -8;
				}
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ValidateStages(params Type[] stages)
		{
			int num = Array.IndexOf(stages, typeof(ReliableSequencedPipelineStage));
			int num2 = Array.IndexOf(stages, typeof(FragmentationPipelineStage));
			if (num >= 0 && num2 >= 0 && num2 > num)
			{
				throw new InvalidOperationException("Cannot create pipeline with ReliableSequenced followed by Fragmentation stage. Should reverse their order.");
			}
		}

		public NetworkPipeline CreatePipeline(params Type[] stages)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int value = 0;
			int num4 = 0;
			PipelineImpl value2 = default(PipelineImpl);
			value2.FirstStageIndex = m_StageList.Length;
			value2.NumStages = stages.Length;
			for (int i = 0; i < stages.Length; i++)
			{
				int value3 = NetworkPipelineStageCollection.GetStageId(stages[i]).Index;
				m_StageList.Add(in value3);
				m_AccumulatedHeaderCapacity.Add(in value);
				num += (m_StageCollection[value3].ReceiveCapacity + 7) & -8;
				num3 += (m_StageCollection[value3].SendCapacity + 7) & -8;
				value += m_StageCollection[value3].HeaderCapacity;
				num2 += (m_StageCollection[value3].SharedStateCapacity + 7) & -8;
				if (num4 == 0)
				{
					num4 = m_StageCollection[value3].PayloadCapacity;
				}
			}
			value2.receiveBufferOffset = sizePerConnection[1];
			sizePerConnection[1] = sizePerConnection[1] + num;
			value2.sendBufferOffset = sizePerConnection[0];
			sizePerConnection[0] = sizePerConnection[0] + num3;
			value2.sharedBufferOffset = sizePerConnection[2];
			sizePerConnection[2] = sizePerConnection[2] + num2;
			value2.headerCapacity = value;
			value2.payloadCapacity = num4;
			m_Pipelines.Add(in value2);
			NetworkPipeline result = default(NetworkPipeline);
			result.Id = m_Pipelines.Length;
			return result;
		}

		public void GetPipelineBuffers(NetworkPipeline pipelineId, NetworkPipelineStageId stageId, NetworkConnection connection, out NativeArray<byte> readProcessingBuffer, out NativeArray<byte> writeProcessingBuffer, out NativeArray<byte> sharedBuffer)
		{
			if (pipelineId.Id < 1 || stageId.IsValid == 0)
			{
				writeProcessingBuffer = default(NativeArray<byte>);
				readProcessingBuffer = default(NativeArray<byte>);
				sharedBuffer = default(NativeArray<byte>);
				return;
			}
			PipelineImpl pipelineImpl = m_Pipelines[pipelineId.Id - 1];
			int num = pipelineImpl.receiveBufferOffset + sizePerConnection[1] * connection.InternalId;
			int num2 = pipelineImpl.sendBufferOffset + sizePerConnection[0] * connection.InternalId;
			int num3 = pipelineImpl.sharedBufferOffset + sizePerConnection[2] * connection.InternalId;
			bool flag = true;
			int i;
			for (i = pipelineImpl.FirstStageIndex; i < pipelineImpl.FirstStageIndex + pipelineImpl.NumStages; i++)
			{
				if (m_StageList[i] == stageId.Index)
				{
					flag = false;
					break;
				}
				num2 += (m_StageCollection[m_StageList[i]].SendCapacity + 7) & -8;
				num += (m_StageCollection[m_StageList[i]].ReceiveCapacity + 7) & -8;
				num3 += (m_StageCollection[m_StageList[i]].SharedStateCapacity + 7) & -8;
			}
			if (flag)
			{
				writeProcessingBuffer = default(NativeArray<byte>);
				readProcessingBuffer = default(NativeArray<byte>);
				sharedBuffer = default(NativeArray<byte>);
			}
			else
			{
				writeProcessingBuffer = ((NativeArray<byte>)m_SendBuffer).GetSubArray(num2, m_StageCollection[m_StageList[i]].SendCapacity);
				readProcessingBuffer = ((NativeArray<byte>)m_ReceiveBuffer).GetSubArray(num, m_StageCollection[m_StageList[i]].ReceiveCapacity);
				sharedBuffer = ((NativeArray<byte>)m_SharedBuffer).GetSubArray(num3, m_StageCollection[m_StageList[i]].SharedStateCapacity);
			}
		}

		internal unsafe void UpdateSend(NetworkDriver.Concurrent driver, out int updateCount)
		{
			int* unsafePtr = (int*)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(m_SendBuffer);
			for (int i = 0; i < m_SendBuffer.Length; i += sizePerConnection[0])
			{
				unsafePtr[i / 4] = 0;
			}
			NativeList<UpdatePipeline> currentUpdates = new NativeList<UpdatePipeline>(m_SendStageNeedsUpdateRead.Count + m_SendStageNeedsUpdate.Length, Allocator.Temp);
			UpdatePipeline item;
			while (m_SendStageNeedsUpdateRead.TryDequeue(out item))
			{
				if (driver.GetConnectionState(item.connection) == NetworkConnection.State.Connected)
				{
					AddSendUpdate(item.connection, item.stage, item.pipeline, currentUpdates);
				}
			}
			for (int j = 0; j < m_SendStageNeedsUpdate.Length; j++)
			{
				item = m_SendStageNeedsUpdate[j];
				if (driver.GetConnectionState(m_SendStageNeedsUpdate[j].connection) == NetworkConnection.State.Connected)
				{
					AddSendUpdate(item.connection, item.stage, item.pipeline, currentUpdates);
				}
			}
			updateCount = currentUpdates.Length;
			NativeList<UpdatePipeline> currentUpdates2 = new NativeList<UpdatePipeline>(128, Allocator.Temp);
			for (int k = 0; k < updateCount; k++)
			{
				item = currentUpdates[k];
				int num = ToConcurrent().ProcessPipelineSend(driver, item.stage, item.pipeline, item.connection, default(NetworkInterfaceSendHandle), 0, currentUpdates2);
				if (num < 0)
				{
					UnityEngine.Debug.LogWarning(FixedString.Format("ProcessPipelineSend failed with the following error code {0}.", num));
				}
			}
			for (int l = 0; l < currentUpdates2.Length; l++)
			{
				m_SendStageNeedsUpdateRead.Enqueue(currentUpdates2[l]);
			}
		}

		private static void AddSendUpdate(NetworkConnection connection, int stageId, NetworkPipeline pipelineId, NativeList<UpdatePipeline> currentUpdates)
		{
			UpdatePipeline updatePipeline = default(UpdatePipeline);
			updatePipeline.connection = connection;
			updatePipeline.stage = stageId;
			updatePipeline.pipeline = pipelineId;
			UpdatePipeline value = updatePipeline;
			bool flag = true;
			for (int i = 0; i < currentUpdates.Length; i++)
			{
				if (currentUpdates[i].stage == value.stage && currentUpdates[i].pipeline.Id == value.pipeline.Id && currentUpdates[i].connection == value.connection)
				{
					flag = false;
				}
			}
			if (flag)
			{
				currentUpdates.Add(in value);
			}
		}

		public void UpdateReceive(NetworkDriver driver, out int updateCount)
		{
			NativeArray<UpdatePipeline> nativeArray = new NativeArray<UpdatePipeline>(m_ReceiveStageNeedsUpdate.Length, Allocator.Temp);
			updateCount = 0;
			for (int i = 0; i < m_ReceiveStageNeedsUpdate.Length; i++)
			{
				if (driver.GetConnectionState(m_ReceiveStageNeedsUpdate[i].connection) == NetworkConnection.State.Connected)
				{
					nativeArray[updateCount++] = m_ReceiveStageNeedsUpdate[i];
				}
			}
			m_ReceiveStageNeedsUpdate.Clear();
			for (int j = 0; j < updateCount; j++)
			{
				UpdatePipeline updatePipeline = nativeArray[j];
				ProcessReceiveStagesFrom(driver, updatePipeline.stage, updatePipeline.pipeline, updatePipeline.connection, default(InboundRecvBuffer));
			}
		}

		public unsafe void Receive(NetworkDriver driver, NetworkConnection connection, NativeArray<byte> buffer)
		{
			byte b = buffer[0];
			if (b == 0 || b > m_Pipelines.Length)
			{
				UnityEngine.Debug.LogError("Received a packet with an invalid pipeline.");
				return;
			}
			int startStage = m_Pipelines[b - 1].NumStages - 1;
			InboundRecvBuffer buffer2 = default(InboundRecvBuffer);
			buffer2.buffer = (byte*)buffer.GetUnsafePtr() + 1;
			buffer2.bufferLength = buffer.Length - 1;
			ProcessReceiveStagesFrom(driver, startStage, new NetworkPipeline
			{
				Id = b
			}, connection, buffer2);
		}

		private unsafe void ProcessReceiveStagesFrom(NetworkDriver driver, int startStage, NetworkPipeline pipeline, NetworkConnection connection, InboundRecvBuffer buffer)
		{
			PipelineImpl pipelineImpl = m_Pipelines[pipeline.Id - 1];
			int networkId = connection.m_NetworkId;
			NativeList<int> resumeQ = new NativeList<int>(16, Allocator.Temp);
			int num = 0;
			int systemHeadersSize = driver.MaxProtocolHeaderSize();
			InboundRecvBuffer inboundBuffer = buffer;
			NetworkPipelineContext networkPipelineContext = default(NetworkPipelineContext);
			networkPipelineContext.timestamp = Timestamp;
			networkPipelineContext.header = default(DataStreamWriter);
			NetworkPipelineContext ctx = networkPipelineContext;
			while (true)
			{
				bool needsUpdate = false;
				bool needsSendUpdate = false;
				int num2 = pipelineImpl.receiveBufferOffset + sizePerConnection[1] * networkId;
				int num3 = pipelineImpl.sharedBufferOffset + sizePerConnection[2] * networkId;
				for (int i = 0; i < startStage; i++)
				{
					num2 += (m_StageCollection[m_StageList[pipelineImpl.FirstStageIndex + i]].ReceiveCapacity + 7) & -8;
					num3 += (m_StageCollection[m_StageList[pipelineImpl.FirstStageIndex + i]].SharedStateCapacity + 7) & -8;
				}
				for (int num4 = startStage; num4 >= 0; num4--)
				{
					ProcessReceiveStage(num4, pipeline, num2, num3, ref ctx, ref inboundBuffer, ref resumeQ, ref needsUpdate, ref needsSendUpdate, systemHeadersSize);
					if (needsUpdate)
					{
						UpdatePipeline updatePipeline = default(UpdatePipeline);
						updatePipeline.connection = connection;
						updatePipeline.stage = num4;
						updatePipeline.pipeline = pipeline;
						UpdatePipeline value = updatePipeline;
						bool flag = true;
						for (int j = 0; j < m_ReceiveStageNeedsUpdate.Length; j++)
						{
							if (m_ReceiveStageNeedsUpdate[j].stage == value.stage && m_ReceiveStageNeedsUpdate[j].pipeline.Id == value.pipeline.Id && m_ReceiveStageNeedsUpdate[j].connection == value.connection)
							{
								flag = false;
							}
						}
						if (flag)
						{
							m_ReceiveStageNeedsUpdate.Add(in value);
						}
					}
					if (needsSendUpdate)
					{
						AddSendUpdate(connection, num4, pipeline, m_SendStageNeedsUpdate);
					}
					if (inboundBuffer.bufferLength == 0)
					{
						break;
					}
					if (num4 > 0)
					{
						num2 -= (m_StageCollection[m_StageList[pipelineImpl.FirstStageIndex + num4 - 1]].ReceiveCapacity + 7) & -8;
						num3 -= (m_StageCollection[m_StageList[pipelineImpl.FirstStageIndex + num4 - 1]].SharedStateCapacity + 7) & -8;
					}
					needsUpdate = false;
				}
				if (inboundBuffer.bufferLength != 0)
				{
					driver.PushDataEvent(connection, pipeline.Id, inboundBuffer.buffer, inboundBuffer.bufferLength);
				}
				if (num >= resumeQ.Length)
				{
					break;
				}
				startStage = resumeQ[num++];
				inboundBuffer = default(InboundRecvBuffer);
			}
		}

		private unsafe void ProcessReceiveStage(int stage, NetworkPipeline pipeline, int internalBufferOffset, int internalSharedBufferOffset, ref NetworkPipelineContext ctx, ref InboundRecvBuffer inboundBuffer, ref NativeList<int> resumeQ, ref bool needsUpdate, ref bool needsSendUpdate, int systemHeadersSize)
		{
			PipelineImpl pipelineImpl = m_Pipelines[pipeline.Id - 1];
			int index = m_StageList[pipelineImpl.FirstStageIndex + stage];
			NetworkPipelineStage networkPipelineStage = m_StageCollection[index];
			ctx.staticInstanceBuffer = (byte*)m_StaticInstanceBuffer.GetUnsafePtr() + networkPipelineStage.StaticStateStart;
			ctx.staticInstanceBufferLength = networkPipelineStage.StaticStateCapcity;
			ctx.internalProcessBuffer = (byte*)m_ReceiveBuffer.GetUnsafePtr() + internalBufferOffset;
			ctx.internalProcessBufferLength = networkPipelineStage.ReceiveCapacity;
			ctx.internalSharedProcessBuffer = (byte*)m_SharedBuffer.GetUnsafePtr() + internalSharedBufferOffset;
			ctx.internalSharedProcessBufferLength = networkPipelineStage.SharedStateCapacity;
			NetworkPipelineStage.Requests requests = NetworkPipelineStage.Requests.None;
			networkPipelineStage.Receive.Ptr.Invoke(ref ctx, ref inboundBuffer, ref requests, systemHeadersSize);
			if ((requests & NetworkPipelineStage.Requests.Resume) != 0)
			{
				resumeQ.Add(in stage);
			}
			needsUpdate = (requests & NetworkPipelineStage.Requests.Update) != 0;
			needsSendUpdate = (requests & NetworkPipelineStage.Requests.SendUpdate) != 0;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		public static void ValidateSendHandle(NetworkInterfaceSendHandle handle)
		{
			if (handle.data == IntPtr.Zero)
			{
				throw new ArgumentException("Value for NetworkDataStreamParameter.size must be larger then zero.");
			}
		}
	}
}
