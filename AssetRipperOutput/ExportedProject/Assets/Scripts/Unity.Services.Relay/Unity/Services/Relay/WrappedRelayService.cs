using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Qos.Internal;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Services.Relay.RelayAllocations;
using UnityEngine;

namespace Unity.Services.Relay
{
	internal class WrappedRelayService : IRelayService, IRelayServiceSDK, IRelayServiceSDKConfiguration
	{
		private const string QosRelayServiceName = "relay";

		internal IRelayServiceSdk m_RelayService { get; set; }

		internal WrappedRelayService(IRelayServiceSdk relayService)
		{
			m_RelayService = relayService;
		}

		public async Task<Allocation> CreateAllocationAsync(int maxConnections, string region = null)
		{
			EnsureSignedIn();
			if (maxConnections <= 0)
			{
				throw new ArgumentException("Maximum number of connections for an allocation must be greater than 0!");
			}
			if (m_RelayService.QosResults == null)
			{
				throw new Exception("Qos component should not be null, check that is is properly initialized.");
			}
			if (string.IsNullOrEmpty(region))
			{
				try
				{
					List<string> regions = (await ListRegionsAsync()).Select((Region r) => r.Id).ToList();
					IList<QosResult> list = await m_RelayService.QosResults.GetSortedQosResultsAsync("relay", regions);
					if (list.Any())
					{
						region = list[0].Region;
						Debug.Log("best region is " + region);
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning("Could not do Qos region selection. Will use default." + Environment.NewLine + "QoS failed due to [" + ex.GetType().Name + "]. Reason: " + ex.Message);
				}
			}
			try
			{
				return (await m_RelayService.AllocationsApi.CreateAllocationAsync(new CreateAllocationRequest(new AllocationRequest(maxConnections, region)), m_RelayService.Configuration)).Result.Data.Allocation;
			}
			catch (HttpException<ErrorResponseBody> ex2)
			{
				throw new RelayServiceException(ex2.ActualError.GetExceptionReason(), ex2.ActualError.GetExceptionMessage(), ex2);
			}
			catch (HttpException ex3)
			{
				if (ex3.Response.IsHttpError)
				{
					throw new RelayServiceException(ex3.Response.GetExceptionReason(), ex3.Response.ErrorMessage, ex3);
				}
				if (ex3.Response.IsNetworkError)
				{
					throw new RelayServiceException(RelayExceptionReason.NetworkError, ex3.Response.ErrorMessage);
				}
				throw new RequestFailedException(15999, "Something went wrong.", ex3);
			}
		}

		public async Task<string> GetJoinCodeAsync(Guid allocationId)
		{
			EnsureSignedIn();
			if (allocationId == Guid.Empty)
			{
				throw new ArgumentNullException("AllocationId cannot be null or empty!");
			}
			try
			{
				return (await m_RelayService.AllocationsApi.CreateJoincodeAsync(new CreateJoincodeRequest(new JoinCodeRequest(allocationId)), m_RelayService.Configuration)).Result.Data.JoinCode;
			}
			catch (HttpException<ErrorResponseBody> ex)
			{
				throw new RelayServiceException(ex.ActualError.GetExceptionReason(), ex.ActualError.GetExceptionMessage(), ex);
			}
			catch (HttpException ex2)
			{
				if (ex2.Response.IsHttpError)
				{
					throw new RelayServiceException(ex2.Response.GetExceptionReason(), ex2.Response.ErrorMessage, ex2);
				}
				if (ex2.Response.IsNetworkError)
				{
					throw new RelayServiceException(RelayExceptionReason.NetworkError, ex2.Response.ErrorMessage);
				}
				throw new RequestFailedException(15999, "Something went wrong.", ex2);
			}
		}

		public async Task<JoinAllocation> JoinAllocationAsync(string joinCode)
		{
			EnsureSignedIn();
			if (string.IsNullOrWhiteSpace(joinCode))
			{
				throw new ArgumentNullException("JoinCode must be non-null, non-empty, and cannot contain only whitespace!");
			}
			try
			{
				return (await m_RelayService.AllocationsApi.JoinRelayAsync(new JoinRelayRequest(new JoinRequest(joinCode)), m_RelayService.Configuration)).Result.Data.Allocation;
			}
			catch (HttpException<ErrorResponseBody> ex)
			{
				throw new RelayServiceException(ex.ActualError.GetExceptionReason(), ex.ActualError.GetExceptionMessage(), ex);
			}
			catch (HttpException ex2)
			{
				if (ex2.Response.IsHttpError)
				{
					throw new RelayServiceException(ex2.Response.GetExceptionReason(), ex2.Response.ErrorMessage, ex2);
				}
				if (ex2.Response.IsNetworkError)
				{
					throw new RelayServiceException(RelayExceptionReason.NetworkError, ex2.Response.ErrorMessage);
				}
				throw new RequestFailedException(15999, "Something went wrong.", ex2);
			}
		}

		public async Task<List<Region>> ListRegionsAsync()
		{
			EnsureSignedIn();
			try
			{
				return (await m_RelayService.AllocationsApi.ListRegionsAsync(new ListRegionsRequest(), m_RelayService.Configuration)).Result.Data.Regions;
			}
			catch (HttpException<ErrorResponseBody> ex)
			{
				throw new RelayServiceException(ex.ActualError.GetExceptionReason(), ex.ActualError.GetExceptionMessage(), ex);
			}
			catch (HttpException ex2)
			{
				if (ex2.Response.IsHttpError)
				{
					throw new RelayServiceException(ex2.Response.GetExceptionReason(), ex2.Response.ErrorMessage, ex2);
				}
				if (ex2.Response.IsNetworkError)
				{
					throw new RelayServiceException(RelayExceptionReason.NetworkError, ex2.Response.ErrorMessage);
				}
				throw new RequestFailedException(15999, "Something went wrong.", ex2);
			}
		}

		public void SetAllocationsServiceBasePath(string allocationsBasePath)
		{
			m_RelayService.Configuration.BasePath = allocationsBasePath;
		}

		private void EnsureSignedIn()
		{
			if (m_RelayService.AccessToken.AccessToken == null)
			{
				throw new RelayServiceException(RelayExceptionReason.Unauthorized, "You are not signed in to the Authentication Service. Please sign in.");
			}
		}
	}
}
