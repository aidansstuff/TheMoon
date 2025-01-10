using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Unity.Netcode
{
	public class NetworkList<T> : NetworkVariableBase where T : unmanaged, IEquatable<T>
	{
		public delegate void OnListChangedDelegate(NetworkListEvent<T> changeEvent);

		private NativeList<T> m_List = new NativeList<T>(64, Allocator.Persistent);

		private NativeList<NetworkListEvent<T>> m_DirtyEvents = new NativeList<NetworkListEvent<T>>(64, Allocator.Persistent);

		public int Count => m_List.Length;

		public T this[int index]
		{
			get
			{
				return m_List[index];
			}
			set
			{
				if (!CanClientWrite(m_NetworkBehaviour.NetworkManager.LocalClientId))
				{
					throw new InvalidOperationException("Client is not allowed to write to this NetworkList");
				}
				T previousValue = m_List[index];
				m_List[index] = value;
				NetworkListEvent<T> networkListEvent = default(NetworkListEvent<T>);
				networkListEvent.Type = NetworkListEvent<T>.EventType.Value;
				networkListEvent.Index = index;
				networkListEvent.Value = value;
				networkListEvent.PreviousValue = previousValue;
				NetworkListEvent<T> listEvent = networkListEvent;
				HandleAddListEvent(listEvent);
			}
		}

		public int LastModifiedTick => int.MinValue;

		public event OnListChangedDelegate OnListChanged;

		public NetworkList()
		{
		}

		public NetworkList(IEnumerable<T> values = null, NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server)
			: base(readPerm, writePerm)
		{
			if (values == null)
			{
				return;
			}
			foreach (T value2 in values)
			{
				T value = value2;
				m_List.Add(in value);
			}
		}

		public override void ResetDirty()
		{
			base.ResetDirty();
			if (m_DirtyEvents.Length > 0)
			{
				m_DirtyEvents.Clear();
			}
		}

		public override bool IsDirty()
		{
			if (!base.IsDirty())
			{
				return m_DirtyEvents.Length > 0;
			}
			return true;
		}

		internal void MarkNetworkObjectDirty()
		{
			if (m_NetworkBehaviour == null)
			{
				Debug.LogWarning("NetworkList is written to, but doesn't know its NetworkBehaviour yet. Are you modifying a NetworkList before the NetworkObject is spawned?");
			}
			else
			{
				m_NetworkBehaviour.NetworkManager.BehaviourUpdater.AddForUpdate(m_NetworkBehaviour.NetworkObject);
			}
		}

		public override void WriteDelta(FastBufferWriter writer)
		{
			ushort value;
			if (base.IsDirty())
			{
				value = 1;
				writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				NetworkListEvent<T>.EventType value2 = NetworkListEvent<T>.EventType.Full;
				writer.WriteValueSafe(in value2, default(FastBufferWriter.ForEnums));
				WriteField(writer);
				return;
			}
			value = (ushort)m_DirtyEvents.Length;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < m_DirtyEvents.Length; i++)
			{
				NetworkListEvent<T> networkListEvent = m_DirtyEvents.ElementAt(i);
				writer.WriteValueSafe(in networkListEvent.Type, default(FastBufferWriter.ForEnums));
				switch (networkListEvent.Type)
				{
				case NetworkListEvent<T>.EventType.Add:
					NetworkVariableSerialization<T>.Write(writer, ref networkListEvent.Value);
					break;
				case NetworkListEvent<T>.EventType.Insert:
					writer.WriteValueSafe(in networkListEvent.Index, default(FastBufferWriter.ForPrimitives));
					NetworkVariableSerialization<T>.Write(writer, ref networkListEvent.Value);
					break;
				case NetworkListEvent<T>.EventType.Remove:
					NetworkVariableSerialization<T>.Write(writer, ref networkListEvent.Value);
					break;
				case NetworkListEvent<T>.EventType.RemoveAt:
					writer.WriteValueSafe(in networkListEvent.Index, default(FastBufferWriter.ForPrimitives));
					break;
				case NetworkListEvent<T>.EventType.Value:
					writer.WriteValueSafe(in networkListEvent.Index, default(FastBufferWriter.ForPrimitives));
					NetworkVariableSerialization<T>.Write(writer, ref networkListEvent.Value);
					break;
				}
			}
		}

		public override void WriteField(FastBufferWriter writer)
		{
			ushort value = (ushort)m_List.Length;
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < m_List.Length; i++)
			{
				NetworkVariableSerialization<T>.Write(writer, ref m_List.ElementAt(i));
			}
		}

		public override void ReadField(FastBufferReader reader)
		{
			m_List.Clear();
			reader.ReadValueSafe(out ushort value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < value; i++)
			{
				T value2 = new T();
				NetworkVariableSerialization<T>.Read(reader, ref value2);
				m_List.Add(in value2);
			}
		}

		public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
		{
			reader.ReadValueSafe(out ushort value, default(FastBufferWriter.ForPrimitives));
			for (int i = 0; i < value; i++)
			{
				reader.ReadValueSafe(out NetworkListEvent<T>.EventType value2, default(FastBufferWriter.ForEnums));
				switch (value2)
				{
				case NetworkListEvent<T>.EventType.Add:
				{
					T value7 = new T();
					NetworkVariableSerialization<T>.Read(reader, ref value7);
					m_List.Add(in value7);
					if (this.OnListChanged != null)
					{
						this.OnListChanged(new NetworkListEvent<T>
						{
							Type = value2,
							Index = m_List.Length - 1,
							Value = m_List[m_List.Length - 1]
						});
					}
					if (keepDirtyDelta)
					{
						ref NativeList<NetworkListEvent<T>> dirtyEvents4 = ref m_DirtyEvents;
						NetworkListEvent<T> value3 = new NetworkListEvent<T>
						{
							Type = value2,
							Index = m_List.Length - 1,
							Value = m_List[m_List.Length - 1]
						};
						dirtyEvents4.Add(in value3);
						MarkNetworkObjectDirty();
					}
					break;
				}
				case NetworkListEvent<T>.EventType.Insert:
				{
					reader.ReadValueSafe(out int value10, default(FastBufferWriter.ForPrimitives));
					T value11 = new T();
					NetworkVariableSerialization<T>.Read(reader, ref value11);
					if (value10 < m_List.Length)
					{
						m_List.InsertRangeWithBeginEnd(value10, value10 + 1);
						m_List[value10] = value11;
					}
					else
					{
						m_List.Add(in value11);
					}
					if (this.OnListChanged != null)
					{
						this.OnListChanged(new NetworkListEvent<T>
						{
							Type = value2,
							Index = value10,
							Value = m_List[value10]
						});
					}
					if (keepDirtyDelta)
					{
						ref NativeList<NetworkListEvent<T>> dirtyEvents6 = ref m_DirtyEvents;
						NetworkListEvent<T> value3 = new NetworkListEvent<T>
						{
							Type = value2,
							Index = value10,
							Value = m_List[value10]
						};
						dirtyEvents6.Add(in value3);
						MarkNetworkObjectDirty();
					}
					break;
				}
				case NetworkListEvent<T>.EventType.Remove:
				{
					T value4 = new T();
					NetworkVariableSerialization<T>.Read(reader, ref value4);
					int num = m_List.IndexOf(value4);
					if (num != -1)
					{
						m_List.RemoveAt(num);
						if (this.OnListChanged != null)
						{
							this.OnListChanged(new NetworkListEvent<T>
							{
								Type = value2,
								Index = num,
								Value = value4
							});
						}
						if (keepDirtyDelta)
						{
							ref NativeList<NetworkListEvent<T>> dirtyEvents2 = ref m_DirtyEvents;
							NetworkListEvent<T> value3 = new NetworkListEvent<T>
							{
								Type = value2,
								Index = num,
								Value = value4
							};
							dirtyEvents2.Add(in value3);
							MarkNetworkObjectDirty();
						}
					}
					break;
				}
				case NetworkListEvent<T>.EventType.RemoveAt:
				{
					reader.ReadValueSafe(out int value5, default(FastBufferWriter.ForPrimitives));
					T value6 = m_List[value5];
					m_List.RemoveAt(value5);
					if (this.OnListChanged != null)
					{
						this.OnListChanged(new NetworkListEvent<T>
						{
							Type = value2,
							Index = value5,
							Value = value6
						});
					}
					if (keepDirtyDelta)
					{
						ref NativeList<NetworkListEvent<T>> dirtyEvents3 = ref m_DirtyEvents;
						NetworkListEvent<T> value3 = new NetworkListEvent<T>
						{
							Type = value2,
							Index = value5,
							Value = value6
						};
						dirtyEvents3.Add(in value3);
						MarkNetworkObjectDirty();
					}
					break;
				}
				case NetworkListEvent<T>.EventType.Value:
				{
					reader.ReadValueSafe(out int value8, default(FastBufferWriter.ForPrimitives));
					T value9 = new T();
					NetworkVariableSerialization<T>.Read(reader, ref value9);
					if (value8 >= m_List.Length)
					{
						throw new Exception("Shouldn't be here, index is higher than list length");
					}
					T previousValue = m_List[value8];
					m_List[value8] = value9;
					if (this.OnListChanged != null)
					{
						this.OnListChanged(new NetworkListEvent<T>
						{
							Type = value2,
							Index = value8,
							Value = value9,
							PreviousValue = previousValue
						});
					}
					if (keepDirtyDelta)
					{
						ref NativeList<NetworkListEvent<T>> dirtyEvents5 = ref m_DirtyEvents;
						NetworkListEvent<T> value3 = new NetworkListEvent<T>
						{
							Type = value2,
							Index = value8,
							Value = value9,
							PreviousValue = previousValue
						};
						dirtyEvents5.Add(in value3);
						MarkNetworkObjectDirty();
					}
					break;
				}
				case NetworkListEvent<T>.EventType.Clear:
					m_List.Clear();
					if (this.OnListChanged != null)
					{
						this.OnListChanged(new NetworkListEvent<T>
						{
							Type = value2
						});
					}
					if (keepDirtyDelta)
					{
						ref NativeList<NetworkListEvent<T>> dirtyEvents = ref m_DirtyEvents;
						NetworkListEvent<T> value3 = new NetworkListEvent<T>
						{
							Type = value2
						};
						dirtyEvents.Add(in value3);
						MarkNetworkObjectDirty();
					}
					break;
				case NetworkListEvent<T>.EventType.Full:
					ReadField(reader);
					ResetDirty();
					break;
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return m_List.GetEnumerator();
		}

		public void Add(T item)
		{
			if (!CanClientWrite(m_NetworkBehaviour.NetworkManager.LocalClientId))
			{
				throw new InvalidOperationException("Client is not allowed to write to this NetworkList");
			}
			m_List.Add(in item);
			NetworkListEvent<T> networkListEvent = default(NetworkListEvent<T>);
			networkListEvent.Type = NetworkListEvent<T>.EventType.Add;
			networkListEvent.Value = item;
			networkListEvent.Index = m_List.Length - 1;
			NetworkListEvent<T> listEvent = networkListEvent;
			HandleAddListEvent(listEvent);
		}

		public void Clear()
		{
			if (!CanClientWrite(m_NetworkBehaviour.NetworkManager.LocalClientId))
			{
				throw new InvalidOperationException("Client is not allowed to write to this NetworkList");
			}
			m_List.Clear();
			NetworkListEvent<T> networkListEvent = default(NetworkListEvent<T>);
			networkListEvent.Type = NetworkListEvent<T>.EventType.Clear;
			NetworkListEvent<T> listEvent = networkListEvent;
			HandleAddListEvent(listEvent);
		}

		public bool Contains(T item)
		{
			return m_List.IndexOf(item) != -1;
		}

		public bool Remove(T item)
		{
			if (!CanClientWrite(m_NetworkBehaviour.NetworkManager.LocalClientId))
			{
				throw new InvalidOperationException("Client is not allowed to write to this NetworkList");
			}
			int num = m_List.IndexOf(item);
			if (num == -1)
			{
				return false;
			}
			m_List.RemoveAt(num);
			NetworkListEvent<T> networkListEvent = default(NetworkListEvent<T>);
			networkListEvent.Type = NetworkListEvent<T>.EventType.Remove;
			networkListEvent.Value = item;
			NetworkListEvent<T> listEvent = networkListEvent;
			HandleAddListEvent(listEvent);
			return true;
		}

		public int IndexOf(T item)
		{
			return m_List.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (!CanClientWrite(m_NetworkBehaviour.NetworkManager.LocalClientId))
			{
				throw new InvalidOperationException("Client is not allowed to write to this NetworkList");
			}
			if (index < m_List.Length)
			{
				m_List.InsertRangeWithBeginEnd(index, index + 1);
				m_List[index] = item;
			}
			else
			{
				m_List.Add(in item);
			}
			NetworkListEvent<T> networkListEvent = default(NetworkListEvent<T>);
			networkListEvent.Type = NetworkListEvent<T>.EventType.Insert;
			networkListEvent.Index = index;
			networkListEvent.Value = item;
			NetworkListEvent<T> listEvent = networkListEvent;
			HandleAddListEvent(listEvent);
		}

		public void RemoveAt(int index)
		{
			if (!CanClientWrite(m_NetworkBehaviour.NetworkManager.LocalClientId))
			{
				throw new InvalidOperationException("Client is not allowed to write to this NetworkList");
			}
			T value = m_List[index];
			m_List.RemoveAt(index);
			NetworkListEvent<T> networkListEvent = default(NetworkListEvent<T>);
			networkListEvent.Type = NetworkListEvent<T>.EventType.RemoveAt;
			networkListEvent.Index = index;
			networkListEvent.Value = value;
			NetworkListEvent<T> listEvent = networkListEvent;
			HandleAddListEvent(listEvent);
		}

		private void HandleAddListEvent(NetworkListEvent<T> listEvent)
		{
			m_DirtyEvents.Add(in listEvent);
			MarkNetworkObjectDirty();
			this.OnListChanged?.Invoke(listEvent);
		}

		public override void Dispose()
		{
			m_List.Dispose();
			m_DirtyEvents.Dispose();
		}
	}
}
