using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;

/*
 * https://partner.steamgames.com/doc/features/workshop/implementation
 */
namespace DSTEd.Core.Steam {
    public class WorkshopItem {
        private string title = null;
        private string description = null;
        private string url = null;
		ulong ownerID;

		/*public WorkshopItem(PublishedFileId_t FileID)
		{
			var handle = SteamRemoteStorage.GetPublishedFileDetails(FileID, 0);
			bool fail = false;
			CallResult<RemoteStorageGetPublishedFileDetailsResult_t> callResult = new CallResult<RemoteStorageGetPublishedFileDetailsResult_t>(call_result_fn);
			callResult.Set(handle);
			SteamAPI.RunCallbacks();
			while (SteamUtils.IsAPICallCompleted(handle, out fail))
				if (fail)
					return;
				else
					Thread.Sleep(50);
		}

		private void call_result_fn(RemoteStorageGetPublishedFileDetailsResult_t r, bool fail)
		{
			if (!fail && r.m_eResult == EResult.k_EResultOK)
			{
				title = r.m_rgchTitle;
				description = r.m_rgchDescription;
				url = r.m_rgchURL;
				ownerID = r.m_ulSteamIDOwner;
			}
		}*/

		public WorkshopItem(PublishedFileId_t FileID)
		{
			var result = SteamWorkshopHelper.RemoteStorageHelper.GetDetails(FileID);
			title = result.m_rgchTitle;
			url = result.m_rgchURL;
			description = result.m_rgchDescription;
			ownerID = result.m_ulSteamIDOwner;
		}

		public WorkshopItem(SteamUGCDetails_t details) {
			//details.m_bAcceptedForUse;
			//details.m_bBanned;
			//details.m_bTagsTruncated;
			//details.m_eFileType;
			//details.m_eResult;
			//details.m_eVisibility;
			//details.m_flScore;
			//details.m_hFile;
			//details.m_hPreviewFile;
			//details.m_nConsumerAppID;
			//details.m_nCreatorAppID;
			//details.m_nFileSize;
			//details.m_nPreviewFileSize;
			//details.m_nPublishedFileId;
			//details.m_pchFileName;
			this.description = details.m_rgchDescription;
			//details.m_rgchTags;
			this.title = details.m_rgchTitle;
			this.url = details.m_rgchURL;
			//details.m_rtimeAddedToUserList;
			//details.m_rtimeCreated;
			//details.m_rtimeUpdated;
			//details.m_ulSteamIDOwner;
			//details.m_unNumChildren;
			//details.m_unVotesDown;
			//details.m_unVotesUp;
		}
		
        public new string ToString() {
            return string.Format("[WorkshopItem Title=\"{0}\" URL=\"{1}\" Description=\"{2}\"]", this.title, this.url, this.description);
        }
    }

    public class Workshop {
        private List<Action> queue;
        private Boolean initalized = false;
        private Boolean running = false;

        public Workshop() {
            this.queue = new List<Action>();

            Task.Delay(2500);

            Task.Run(() => {
                while (true) {
                    this.Init();
                    Thread.Sleep(5000);
                }
            });
        }

        public AccountID_t GetAccountID() {
            return SteamUser.GetSteamID().GetAccountID();
        }

        public CSteamID GetSteamID() {
            return SteamUser.GetSteamID();
        }

        public string GetUsername() {
            return SteamFriends.GetPersonaName();
        }

        public AppId_t GetCreatorApp() {
            return new AppId_t((uint) (245850 & 0xFFFFFFul));
        }

        public void Init() {
            if (!SteamAPI.Init() && !this.initalized) {
                Logger.Info("Steam-INIT FAILED! Retry in 5 Seconds...");
                return;
            }

            this.initalized = true;

            if (SteamAPI.IsSteamRunning() && !this.running) {
                this.running = true;

                Task.Run(() => {
                    while (true) {
                        if (this.queue.Count >= 1) {
                            Logger.Info("Enqueue Call!");
                            Action callback = this.queue[0];
                            callback();
                            this.queue.RemoveAt(0);
                            Thread.Sleep(100);
                        }

                        SteamAPI.RunCallbacks();
                        Thread.Sleep(100);
                    }
                });

                Logger.Info("ID: " + this.GetSteamID() + ", Name: " + this.GetUsername() + ", Account: " + this.GetAccountID());
            }
        }

        public void Call(Action callback) {
            this.queue.Add(callback);
        }

        public void GetPublishedMods(int app_id, Action<WorkshopItem[]> callback) {
            this.Call(delegate () {
                EUserUGCList list = EUserUGCList.k_EUserUGCList_Published;
                EUGCMatchingUGCType type = EUGCMatchingUGCType.k_EUGCMatchingUGCType_All;
                EUserUGCListSortOrder order = EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderAsc;
                uint page = 1; //@ToDo iterate/count up! A single page has a maximum of 50 items. is the result-count under 50, it's the last page!
                UGCQueryHandle_t handle = SteamUGC.CreateQueryUserUGCRequest(this.GetAccountID(), list, type, order, this.GetCreatorApp(), new AppId_t((uint) ((uint) app_id & 0xFFFFFFul)), page);

                CallResult<SteamUGCQueryCompleted_t>.Create(delegate (SteamUGCQueryCompleted_t pCallback, bool bIOFailure) {
                    WorkshopItem[] result = new WorkshopItem[pCallback.m_unTotalMatchingResults];

                    for(uint index = 0; index < pCallback.m_unTotalMatchingResults; index++) {
                        SteamUGCDetails_t details;

                        if (SteamUGC.GetQueryUGCResult(handle, index, out details)) {
                            result[index] = new WorkshopItem(details);
                        }
                    }

                    callback(result);
                    SteamUGC.ReleaseQueryUGCRequest(handle);
                }).Set(SteamUGC.SendQueryUGCRequest(handle));
            });
        }
    }

	public class Workshop_RS
	{
		public static CSteamID CurrentUserSteamID { get => SteamUser.GetSteamID(); }
		public uint StartIndex = 0;
		protected CallResult<RemoteStorageEnumerateWorkshopFilesResult_t> querycallback;

		public Workshop_RS()
		{
			while (!SteamAPI.Init()) return;
			querycallback = new CallResult<RemoteStorageEnumerateWorkshopFilesResult_t>();
			Boot.Instance.DBGCLI.AddCommand("steam_workshoprs_query", dbgquery);
			Boot.Instance.DBGCLI.AddCommand("steam_workshoprs_getdetails", dbggetdetails);
		}
		private string dbggetdetails(params string[] args)
		{
			if (args[0] == null)
				return "Please input FileID";
			else
				return new WorkshopItem(new PublishedFileId_t(ulong.Parse(args[0]))).ToString();
		}
		private string dbgquery(params string[] args)
		{
			uint count;
			if (args.Length > 0)
				count = args[0] != null ? uint.Parse(args[0]) : 10;
			else
				count = 10;
			var result = QueryPublicMods(count, new List<string>(), new List<string>());
			System.Text.StringBuilder ret = new System.Text.StringBuilder("ID returned:\n");
			foreach (var id in result)
			{
				ret.AppendLine(id.ToString());
			}
			return ret.ToString();
		}

		public async Task<PublishedFileId_t[]> QueryPublicModsAsync(uint count,List<string> Tags,List<string> UserTags)
		{
			PublishedFileId_t[] ret = null;
			StartIndex += count;
			var handle = SteamRemoteStorage.EnumeratePublishedWorkshopFiles(
				EWorkshopEnumerationType.k_EWorkshopEnumerationTypeRecent,
				StartIndex,
				count,
				0,
				Tags,
				UserTags
				);
			querycallback = CallResult<RemoteStorageEnumerateWorkshopFilesResult_t>.Create(
				(RemoteStorageEnumerateWorkshopFilesResult_t r, bool fail) =>
				{
					if(!fail)
					{
						if (r.m_eResult == EResult.k_EResultOK)
						{
							ret = r.m_rgPublishedFileId;
						}
					}
				}
			);
			querycallback.Set(handle);
			await Task.Run(new Action(
				() =>
				{
					SteamAPI.RunCallbacks();
					while (ret == null)
						Thread.Sleep(300);
				})
				);
			return ret;
		}
		public PublishedFileId_t[] QueryPublicMods(uint count, List<string> Tags, List<string> UserTags)
		{
			PublishedFileId_t[] ret = null;
			StartIndex += count;
			var handle = SteamRemoteStorage.EnumeratePublishedWorkshopFiles(
				EWorkshopEnumerationType.k_EWorkshopEnumerationTypeRecent,
				StartIndex,
				count,
				0,
				Tags,
				UserTags
				);
			querycallback = CallResult<RemoteStorageEnumerateWorkshopFilesResult_t>.Create(
				(RemoteStorageEnumerateWorkshopFilesResult_t r, bool fail) =>
				{
					if (!fail)
					{
						if (r.m_eResult == EResult.k_EResultOK)
						{
							ret = r.m_rgPublishedFileId;
						}
					}
				}
			);
			querycallback.Set(handle);
			SteamAPI.RunCallbacks();
			while (ret == null)
				Thread.Sleep(300);
			return ret;
		}
	}
}
