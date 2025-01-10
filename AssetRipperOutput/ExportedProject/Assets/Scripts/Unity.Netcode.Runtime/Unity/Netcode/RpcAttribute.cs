using System;

namespace Unity.Netcode
{
	public abstract class RpcAttribute : Attribute
	{
		public RpcDelivery Delivery;
	}
}
