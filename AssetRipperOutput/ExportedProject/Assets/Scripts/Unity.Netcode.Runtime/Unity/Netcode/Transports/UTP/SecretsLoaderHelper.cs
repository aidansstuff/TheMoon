using System;
using System.IO;
using UnityEngine;

namespace Unity.Netcode.Transports.UTP
{
	public class SecretsLoaderHelper : MonoBehaviour
	{
		internal struct ServerSecrets
		{
			public string ServerPrivate;

			public string ServerCertificate;
		}

		internal struct ClientSecrets
		{
			public string ServerCommonName;

			public string ClientCertificate;
		}

		[Tooltip("Hostname")]
		[SerializeField]
		private string m_ServerCommonName = "localhost";

		[Tooltip("Client CA filepath. Useful with self-signed certificates")]
		[SerializeField]
		private string m_ClientCAFilePath = "";

		[Tooltip("Client CA Override. Only useful for development with self-signed certificates. Certificate content, for platforms that lack file access (WebGL)")]
		[SerializeField]
		private string m_ClientCAOverride = "";

		[Tooltip("Server Certificate filepath")]
		[SerializeField]
		private string m_ServerCertificateFilePath = "";

		[Tooltip("Server Private Key filepath")]
		[SerializeField]
		private string m_ServerPrivateFilePath = "";

		private string m_ClientCA;

		private string m_ServerCertificate;

		private string m_ServerPrivate;

		public string ServerCommonName
		{
			get
			{
				return m_ServerCommonName;
			}
			set
			{
				m_ServerCommonName = value;
			}
		}

		public string ClientCAFilePath
		{
			get
			{
				return m_ClientCAFilePath;
			}
			set
			{
				m_ClientCAFilePath = value;
			}
		}

		public string ClientCAOverride
		{
			get
			{
				return m_ClientCAOverride;
			}
			set
			{
				m_ClientCAOverride = value;
			}
		}

		public string ServerCertificateFilePath
		{
			get
			{
				return m_ServerCertificateFilePath;
			}
			set
			{
				m_ServerCertificateFilePath = value;
			}
		}

		public string ServerPrivateFilePath
		{
			get
			{
				return m_ServerPrivateFilePath;
			}
			set
			{
				m_ServerPrivate = value;
			}
		}

		public string ClientCA
		{
			get
			{
				if (m_ClientCAOverride != "")
				{
					return m_ClientCAOverride;
				}
				return ReadFile(m_ClientCAFilePath, "Client Certificate");
			}
			set
			{
				m_ClientCA = value;
			}
		}

		public string ServerCertificate
		{
			get
			{
				return ReadFile(m_ServerCertificateFilePath, "Server Certificate");
			}
			set
			{
				m_ServerCertificate = value;
			}
		}

		public string ServerPrivate
		{
			get
			{
				return ReadFile(m_ServerPrivateFilePath, "Server Key");
			}
			set
			{
				m_ServerPrivate = value;
			}
		}

		private void Awake()
		{
			ServerSecrets serverSecrets = default(ServerSecrets);
			try
			{
				serverSecrets.ServerCertificate = ServerCertificate;
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
			try
			{
				serverSecrets.ServerPrivate = ServerPrivate;
			}
			catch (Exception message2)
			{
				Debug.Log(message2);
			}
			ClientSecrets clientSecrets = default(ClientSecrets);
			try
			{
				clientSecrets.ClientCertificate = ClientCA;
			}
			catch (Exception message3)
			{
				Debug.Log(message3);
			}
			try
			{
				clientSecrets.ServerCommonName = ServerCommonName;
			}
			catch (Exception message4)
			{
				Debug.Log(message4);
			}
			UnityTransport component = GetComponent<UnityTransport>();
			if (component == null)
			{
				Debug.LogError("You need to select the UnityTransport protocol, in the NetworkManager, in order for the SecretsLoaderHelper component to be useful.");
				return;
			}
			component.SetServerSecrets(serverSecrets.ServerCertificate, serverSecrets.ServerPrivate);
			component.SetClientSecrets(clientSecrets.ServerCommonName, clientSecrets.ClientCertificate);
		}

		private static string ReadFile(string path, string label)
		{
			if (path == null || path == "")
			{
				return "";
			}
			string text = new StreamReader(path).ReadToEnd();
			Debug.Log((text.Length > 1) ? ("Successfully loaded " + text.Length + " byte(s) from " + label) : ("Could not read " + label + " file"));
			return text;
		}
	}
}
