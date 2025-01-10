using System;
using Unity.Collections;

namespace Unity.Networking.Transport.TLS
{
	public static class SecureParameterExtensions
	{
		public static ref NetworkSettings WithSecureClientParameters(this ref NetworkSettings settings, ref FixedString32Bytes serverName, uint readTimeout = 0u, uint handshakeTimeoutMax = 60000u, uint handshakeTimeoutMin = 1000u)
		{
			SecureNetworkProtocolParameter secureNetworkProtocolParameter = default(SecureNetworkProtocolParameter);
			secureNetworkProtocolParameter.Pem = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.Rsa = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.RsaKey = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.Hostname = serverName;
			secureNetworkProtocolParameter.Protocol = SecureTransportProtocol.DTLS;
			secureNetworkProtocolParameter.ClientAuthenticationPolicy = SecureClientAuthPolicy.None;
			secureNetworkProtocolParameter.SSLReadTimeoutMs = readTimeout;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = handshakeTimeoutMax;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = handshakeTimeoutMin;
			SecureNetworkProtocolParameter parameter = secureNetworkProtocolParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureClientParameters(this ref NetworkSettings settings, string serverName)
		{
			FixedString32Bytes serverName2 = new FixedString32Bytes(serverName);
			_ = ref settings.WithSecureClientParameters(ref serverName2);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureClientParameters(this ref NetworkSettings settings, ref FixedString4096Bytes caCertificate, ref FixedString32Bytes serverName, uint readTimeout = 0u, uint handshakeTimeoutMax = 60000u, uint handshakeTimeoutMin = 1000u)
		{
			SecureNetworkProtocolParameter secureNetworkProtocolParameter = default(SecureNetworkProtocolParameter);
			secureNetworkProtocolParameter.Pem = caCertificate;
			secureNetworkProtocolParameter.Rsa = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.RsaKey = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.Hostname = serverName;
			secureNetworkProtocolParameter.Protocol = SecureTransportProtocol.DTLS;
			secureNetworkProtocolParameter.ClientAuthenticationPolicy = SecureClientAuthPolicy.None;
			secureNetworkProtocolParameter.SSLReadTimeoutMs = readTimeout;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = handshakeTimeoutMax;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = handshakeTimeoutMin;
			SecureNetworkProtocolParameter parameter = secureNetworkProtocolParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureClientParameters(this ref NetworkSettings settings, string caCertificate, string serverName)
		{
			FixedString4096Bytes caCertificate2 = new FixedString4096Bytes(caCertificate);
			FixedString32Bytes serverName2 = new FixedString32Bytes(serverName);
			_ = ref settings.WithSecureClientParameters(ref caCertificate2, ref serverName2);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureClientParameters(this ref NetworkSettings settings, ref FixedString4096Bytes certificate, ref FixedString4096Bytes privateKey, ref FixedString4096Bytes caCertificate, ref FixedString32Bytes serverName, uint readTimeout = 0u, uint handshakeTimeoutMax = 60000u, uint handshakeTimeoutMin = 1000u)
		{
			SecureNetworkProtocolParameter secureNetworkProtocolParameter = default(SecureNetworkProtocolParameter);
			secureNetworkProtocolParameter.Pem = caCertificate;
			secureNetworkProtocolParameter.Rsa = certificate;
			secureNetworkProtocolParameter.RsaKey = privateKey;
			secureNetworkProtocolParameter.Hostname = serverName;
			secureNetworkProtocolParameter.Protocol = SecureTransportProtocol.DTLS;
			secureNetworkProtocolParameter.ClientAuthenticationPolicy = SecureClientAuthPolicy.None;
			secureNetworkProtocolParameter.SSLReadTimeoutMs = readTimeout;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = handshakeTimeoutMax;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = handshakeTimeoutMin;
			SecureNetworkProtocolParameter parameter = secureNetworkProtocolParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureClientParameters(this ref NetworkSettings settings, string certificate, string privateKey, string caCertificate, string serverName)
		{
			FixedString4096Bytes certificate2 = new FixedString4096Bytes(certificate);
			FixedString4096Bytes privateKey2 = new FixedString4096Bytes(privateKey);
			FixedString4096Bytes caCertificate2 = new FixedString4096Bytes(caCertificate);
			FixedString32Bytes serverName2 = new FixedString32Bytes(serverName);
			_ = ref settings.WithSecureClientParameters(ref certificate2, ref privateKey2, ref caCertificate2, ref serverName2);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureServerParameters(this ref NetworkSettings settings, ref FixedString4096Bytes certificate, ref FixedString4096Bytes privateKey, uint readTimeout = 0u, uint handshakeTimeoutMax = 60000u, uint handshakeTimeoutMin = 1000u)
		{
			SecureNetworkProtocolParameter secureNetworkProtocolParameter = default(SecureNetworkProtocolParameter);
			secureNetworkProtocolParameter.Pem = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.Rsa = certificate;
			secureNetworkProtocolParameter.RsaKey = privateKey;
			secureNetworkProtocolParameter.Hostname = default(FixedString32Bytes);
			secureNetworkProtocolParameter.Protocol = SecureTransportProtocol.DTLS;
			secureNetworkProtocolParameter.ClientAuthenticationPolicy = SecureClientAuthPolicy.None;
			secureNetworkProtocolParameter.SSLReadTimeoutMs = readTimeout;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = handshakeTimeoutMax;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = handshakeTimeoutMin;
			SecureNetworkProtocolParameter parameter = secureNetworkProtocolParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureServerParameters(this ref NetworkSettings settings, string certificate, string privateKey)
		{
			FixedString4096Bytes certificate2 = new FixedString4096Bytes(certificate);
			FixedString4096Bytes privateKey2 = new FixedString4096Bytes(privateKey);
			_ = ref settings.WithSecureServerParameters(ref certificate2, ref privateKey2);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureServerParameters(this ref NetworkSettings settings, ref FixedString4096Bytes certificate, ref FixedString4096Bytes privateKey, ref FixedString4096Bytes caCertificate, ref FixedString32Bytes clientName, SecureClientAuthPolicy clientAuthenticationPolicy = SecureClientAuthPolicy.Required, uint readTimeout = 0u, uint handshakeTimeoutMax = 60000u, uint handshakeTimeoutMin = 1000u)
		{
			SecureNetworkProtocolParameter secureNetworkProtocolParameter = default(SecureNetworkProtocolParameter);
			secureNetworkProtocolParameter.Pem = caCertificate;
			secureNetworkProtocolParameter.Rsa = certificate;
			secureNetworkProtocolParameter.RsaKey = privateKey;
			secureNetworkProtocolParameter.Hostname = clientName;
			secureNetworkProtocolParameter.Protocol = SecureTransportProtocol.DTLS;
			secureNetworkProtocolParameter.ClientAuthenticationPolicy = clientAuthenticationPolicy;
			secureNetworkProtocolParameter.SSLReadTimeoutMs = readTimeout;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = handshakeTimeoutMax;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = handshakeTimeoutMin;
			SecureNetworkProtocolParameter parameter = secureNetworkProtocolParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static ref NetworkSettings WithSecureServerParameters(this ref NetworkSettings settings, string certificate, string privateKey, string caCertificate, string clientName, SecureClientAuthPolicy clientAuthenticationPolicy = SecureClientAuthPolicy.Required)
		{
			FixedString4096Bytes certificate2 = new FixedString4096Bytes(certificate);
			FixedString4096Bytes privateKey2 = new FixedString4096Bytes(privateKey);
			FixedString4096Bytes caCertificate2 = new FixedString4096Bytes(caCertificate);
			FixedString32Bytes clientName2 = new FixedString32Bytes(clientName);
			_ = ref settings.WithSecureServerParameters(ref certificate2, ref privateKey2, ref caCertificate2, ref clientName2, clientAuthenticationPolicy);
			return ref settings;
		}

		[Obsolete("Use WithSecureClientParameters or WithSecureServerParameters instead.")]
		public static ref NetworkSettings WithSecureParameters(this ref NetworkSettings settings, ref FixedString4096Bytes pem, ref FixedString32Bytes hostname, SecureTransportProtocol protocol = SecureTransportProtocol.DTLS, SecureClientAuthPolicy clientAuthenticationPolicy = SecureClientAuthPolicy.Optional, uint sslReadTimeoutMs = 0u, uint sslHandshakeTimeoutMax = 60000u, uint sslHandshakeTimeoutMin = 1000u)
		{
			SecureNetworkProtocolParameter secureNetworkProtocolParameter = default(SecureNetworkProtocolParameter);
			secureNetworkProtocolParameter.Pem = pem;
			secureNetworkProtocolParameter.Rsa = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.RsaKey = default(FixedString4096Bytes);
			secureNetworkProtocolParameter.Hostname = hostname;
			secureNetworkProtocolParameter.Protocol = protocol;
			secureNetworkProtocolParameter.ClientAuthenticationPolicy = clientAuthenticationPolicy;
			secureNetworkProtocolParameter.SSLReadTimeoutMs = sslReadTimeoutMs;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = sslHandshakeTimeoutMax;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = sslHandshakeTimeoutMin;
			SecureNetworkProtocolParameter parameter = secureNetworkProtocolParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		[Obsolete("Use WithSecureClientParameters or WithSecureServerParameters instead.")]
		public static ref NetworkSettings WithSecureParameters(this ref NetworkSettings settings, ref FixedString4096Bytes pem, ref FixedString4096Bytes rsa, ref FixedString4096Bytes rsaKey, ref FixedString32Bytes hostname, SecureTransportProtocol protocol = SecureTransportProtocol.DTLS, SecureClientAuthPolicy clientAuthenticationPolicy = SecureClientAuthPolicy.Optional, uint sslReadTimeoutMs = 0u, uint sslHandshakeTimeoutMax = 60000u, uint sslHandshakeTimeoutMin = 1000u)
		{
			SecureNetworkProtocolParameter secureNetworkProtocolParameter = default(SecureNetworkProtocolParameter);
			secureNetworkProtocolParameter.Pem = pem;
			secureNetworkProtocolParameter.Rsa = rsa;
			secureNetworkProtocolParameter.RsaKey = rsaKey;
			secureNetworkProtocolParameter.Hostname = hostname;
			secureNetworkProtocolParameter.Protocol = protocol;
			secureNetworkProtocolParameter.ClientAuthenticationPolicy = clientAuthenticationPolicy;
			secureNetworkProtocolParameter.SSLReadTimeoutMs = sslReadTimeoutMs;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMax = sslHandshakeTimeoutMax;
			secureNetworkProtocolParameter.SSLHandshakeTimeoutMin = sslHandshakeTimeoutMin;
			SecureNetworkProtocolParameter parameter = secureNetworkProtocolParameter;
			settings.AddRawParameterStruct(ref parameter);
			return ref settings;
		}

		public static SecureNetworkProtocolParameter GetSecureParameters(this ref NetworkSettings settings)
		{
			if (!settings.TryGet<SecureNetworkProtocolParameter>(out var parameter))
			{
				throw new InvalidOperationException("Can't extract Secure parameters: SecureNetworkProtocolParameter must be provided to the NetworkSettings");
			}
			return parameter;
		}
	}
}
