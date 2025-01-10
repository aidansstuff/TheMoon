using System;
using UnityEngine;

namespace Unity.Netcode
{
	[Serializable]
	public class NetworkVariable<T> : NetworkVariableBase
	{
		public delegate void OnValueChangedDelegate(T previousValue, T newValue);

		public OnValueChangedDelegate OnValueChanged;

		[SerializeField]
		private protected T m_InternalValue;

		private protected T m_PreviousValue;

		private bool m_HasPreviousValue;

		private bool m_IsDisposed;

		public virtual T Value
		{
			get
			{
				return m_InternalValue;
			}
			set
			{
				if (!NetworkVariableSerialization<T>.AreEqual(ref m_InternalValue, ref value))
				{
					if ((bool)m_NetworkBehaviour && !CanClientWrite(m_NetworkBehaviour.NetworkManager.LocalClientId))
					{
						throw new InvalidOperationException("Client is not allowed to write to this NetworkVariable");
					}
					Set(value);
					m_IsDisposed = false;
				}
			}
		}

		public NetworkVariable(T value = default(T), NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server)
			: base(readPerm, writePerm)
		{
			m_InternalValue = value;
			m_PreviousValue = default(T);
		}

		internal ref T RefValue()
		{
			return ref m_InternalValue;
		}

		public override void Dispose()
		{
			if (!m_IsDisposed)
			{
				m_IsDisposed = true;
				if (m_InternalValue is IDisposable disposable)
				{
					disposable.Dispose();
				}
				m_InternalValue = default(T);
				if (m_HasPreviousValue && m_PreviousValue is IDisposable disposable2)
				{
					m_HasPreviousValue = false;
					disposable2.Dispose();
				}
				m_PreviousValue = default(T);
			}
		}

		~NetworkVariable()
		{
			Dispose();
		}

		public override bool IsDirty()
		{
			if (base.IsDirty())
			{
				return true;
			}
			bool flag = !NetworkVariableSerialization<T>.AreEqual(ref m_PreviousValue, ref m_InternalValue);
			SetDirty(flag);
			return flag;
		}

		public override void ResetDirty()
		{
			base.ResetDirty();
			m_HasPreviousValue = true;
			NetworkVariableSerialization<T>.Serializer.Duplicate(in m_InternalValue, ref m_PreviousValue);
		}

		private protected void Set(T value)
		{
			SetDirty(isDirty: true);
			T internalValue = m_InternalValue;
			m_InternalValue = value;
			OnValueChanged?.Invoke(internalValue, m_InternalValue);
		}

		public override void WriteDelta(FastBufferWriter writer)
		{
			WriteField(writer);
		}

		public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
		{
			T internalValue = m_InternalValue;
			NetworkVariableSerialization<T>.Read(reader, ref m_InternalValue);
			if (keepDirtyDelta)
			{
				SetDirty(isDirty: true);
			}
			OnValueChanged?.Invoke(internalValue, m_InternalValue);
		}

		public override void ReadField(FastBufferReader reader)
		{
			NetworkVariableSerialization<T>.Read(reader, ref m_InternalValue);
		}

		public override void WriteField(FastBufferWriter writer)
		{
			NetworkVariableSerialization<T>.Write(writer, ref m_InternalValue);
		}
	}
}
