using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageGetPublishedFileDetailsResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		internal AppId CreatorAppID;

		internal AppId ConsumerAppID;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 129)]
		internal byte[] Title;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8000)]
		internal byte[] Description;

		internal ulong File;

		internal ulong PreviewFile;

		internal ulong SteamIDOwner;

		internal uint TimeCreated;

		internal uint TimeUpdated;

		internal RemoteStoragePublishedFileVisibility Visibility;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Banned;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025)]
		internal byte[] Tags;

		[MarshalAs(UnmanagedType.I1)]
		internal bool TagsTruncated;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
		internal byte[] PchFileName;

		internal int FileSize;

		internal int PreviewFileSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		internal byte[] URL;

		internal WorkshopFileType FileType;

		[MarshalAs(UnmanagedType.I1)]
		internal bool AcceptedForUse;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageGetPublishedFileDetailsResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageGetPublishedFileDetailsResult;

		internal string TitleUTF8()
		{
			return Encoding.UTF8.GetString(Title, 0, Array.IndexOf(Title, (byte)0));
		}

		internal string DescriptionUTF8()
		{
			return Encoding.UTF8.GetString(Description, 0, Array.IndexOf(Description, (byte)0));
		}

		internal string TagsUTF8()
		{
			return Encoding.UTF8.GetString(Tags, 0, Array.IndexOf(Tags, (byte)0));
		}

		internal string PchFileNameUTF8()
		{
			return Encoding.UTF8.GetString(PchFileName, 0, Array.IndexOf(PchFileName, (byte)0));
		}

		internal string URLUTF8()
		{
			return Encoding.UTF8.GetString(URL, 0, Array.IndexOf(URL, (byte)0));
		}
	}
}
