using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks.Ugc
{
	public struct Editor
	{
		private PublishedFileId fileId;

		private bool creatingNew;

		private WorkshopFileType creatingType;

		private AppId consumerAppId;

		private string Title;

		private string Description;

		private string MetaData;

		private string ChangeLog;

		private string Language;

		private string PreviewFile;

		private DirectoryInfo ContentFolder;

		private RemoteStoragePublishedFileVisibility? Visibility;

		private List<string> Tags;

		private Dictionary<string, string> KeyValueTags;

		public static Editor NewCommunityFile => new Editor(WorkshopFileType.First);

		public static Editor NewMicrotransactionFile => new Editor(WorkshopFileType.Microtransaction);

		internal Editor(WorkshopFileType filetype)
		{
			this = default(Editor);
			creatingNew = true;
			creatingType = filetype;
		}

		public Editor(PublishedFileId fileId)
		{
			this = default(Editor);
			this.fileId = fileId;
		}

		public Editor ForAppId(AppId id)
		{
			consumerAppId = id;
			return this;
		}

		public Editor WithTitle(string t)
		{
			Title = t;
			return this;
		}

		public Editor WithDescription(string t)
		{
			Description = t;
			return this;
		}

		public Editor WithMetaData(string t)
		{
			MetaData = t;
			return this;
		}

		public Editor WithChangeLog(string t)
		{
			ChangeLog = t;
			return this;
		}

		public Editor InLanguage(string t)
		{
			Language = t;
			return this;
		}

		public Editor WithPreviewFile(string t)
		{
			PreviewFile = t;
			return this;
		}

		public Editor WithContent(DirectoryInfo t)
		{
			ContentFolder = t;
			return this;
		}

		public Editor WithContent(string folderName)
		{
			return WithContent(new DirectoryInfo(folderName));
		}

		public Editor WithPublicVisibility()
		{
			Visibility = RemoteStoragePublishedFileVisibility.Public;
			return this;
		}

		public Editor WithFriendsOnlyVisibility()
		{
			Visibility = RemoteStoragePublishedFileVisibility.FriendsOnly;
			return this;
		}

		public Editor WithPrivateVisibility()
		{
			Visibility = RemoteStoragePublishedFileVisibility.Private;
			return this;
		}

		public Editor WithTag(string tag)
		{
			if (Tags == null)
			{
				Tags = new List<string>();
			}
			Tags.Add(tag);
			return this;
		}

		public Editor AddKeyValueTag(string key, string value)
		{
			if (KeyValueTags == null)
			{
				KeyValueTags = new Dictionary<string, string>();
			}
			KeyValueTags.Add(key, value);
			return this;
		}

		public async Task<PublishResult> SubmitAsync(IProgress<float> progress = null)
		{
			PublishResult result = default(PublishResult);
			progress?.Report(0f);
			if ((uint)consumerAppId == 0)
			{
				consumerAppId = SteamClient.AppId;
			}
			if (creatingNew)
			{
				result.Result = Result.Fail;
				CreateItemResult_t? created = await SteamUGC.Internal.CreateItem(consumerAppId, creatingType);
				if (!created.HasValue)
				{
					return result;
				}
				result.Result = created.Value.Result;
				if (result.Result != Result.OK)
				{
					return result;
				}
				fileId = created.Value.PublishedFileId;
				result.NeedsWorkshopAgreement = created.Value.UserNeedsToAcceptWorkshopLegalAgreement;
				result.FileId = fileId;
			}
			result.FileId = fileId;
			UGCUpdateHandle_t handle = SteamUGC.Internal.StartItemUpdate(consumerAppId, fileId);
			if (handle == ulong.MaxValue)
			{
				return result;
			}
			if (Title != null)
			{
				SteamUGC.Internal.SetItemTitle(handle, Title);
			}
			if (Description != null)
			{
				SteamUGC.Internal.SetItemDescription(handle, Description);
			}
			if (MetaData != null)
			{
				SteamUGC.Internal.SetItemMetadata(handle, MetaData);
			}
			if (Language != null)
			{
				SteamUGC.Internal.SetItemUpdateLanguage(handle, Language);
			}
			if (ContentFolder != null)
			{
				SteamUGC.Internal.SetItemContent(handle, ContentFolder.FullName);
			}
			if (PreviewFile != null)
			{
				SteamUGC.Internal.SetItemPreview(handle, PreviewFile);
			}
			if (Visibility.HasValue)
			{
				SteamUGC.Internal.SetItemVisibility(handle, Visibility.Value);
			}
			if (Tags != null && Tags.Count > 0)
			{
				SteamParamStringArray a = SteamParamStringArray.From(Tags.ToArray());
				try
				{
					SteamParamStringArray_t val = a.Value;
					SteamUGC.Internal.SetItemTags(handle, ref val);
				}
				finally
				{
					((IDisposable)a).Dispose();
				}
			}
			if (KeyValueTags != null && KeyValueTags.Count > 0)
			{
				foreach (KeyValuePair<string, string> keyValueTag in KeyValueTags)
				{
					SteamUGC.Internal.AddItemKeyValueTag(handle, keyValueTag.Key, keyValueTag.Value);
				}
			}
			result.Result = Result.Fail;
			if (ChangeLog == null)
			{
				ChangeLog = "";
			}
			CallResult<SubmitItemUpdateResult_t> updating = SteamUGC.Internal.SubmitItemUpdate(handle, ChangeLog);
			while (!updating.IsCompleted)
			{
				if (progress != null)
				{
					ulong total = 0uL;
					ulong processed = 0uL;
					switch (SteamUGC.Internal.GetItemUpdateProgress(handle, ref processed, ref total))
					{
					case ItemUpdateStatus.PreparingConfig:
						progress?.Report(0.1f);
						break;
					case ItemUpdateStatus.PreparingContent:
						progress?.Report(0.2f);
						break;
					case ItemUpdateStatus.UploadingContent:
					{
						float uploaded = ((total != 0) ? ((float)processed / (float)total) : 0f);
						progress?.Report(0.2f + uploaded * 0.7f);
						break;
					}
					case ItemUpdateStatus.UploadingPreviewFile:
						progress?.Report(0.8f);
						break;
					case ItemUpdateStatus.CommittingChanges:
						progress?.Report(1f);
						break;
					}
				}
				await Task.Delay(16);
			}
			progress?.Report(1f);
			SubmitItemUpdateResult_t? updated = updating.GetResult();
			if (!updated.HasValue)
			{
				return result;
			}
			result.Result = updated.Value.Result;
			if (result.Result != Result.OK)
			{
				return result;
			}
			result.NeedsWorkshopAgreement = updated.Value.UserNeedsToAcceptWorkshopLegalAgreement;
			result.FileId = fileId;
			return result;
		}
	}
}
