#include "pch.h"
#define _CRT_SECURE_NO_WARNINGS
#include "DSTEd.SteamWorkshopHelper.h"

namespace DSTEd
{
	namespace Core
	{
		namespace SteamWorkshopHelper
		{
			namespace Utilitity
			{
				template<typename TargetStruct, int ID = TargetStruct::k_iCallback>
				class STCallResult
				{
				public:
					STCallResult(SteamAPICall_t handle)
					{
						Result = new TargetStruct;
						Callresult.Set(handle, this, &STCallResult::Function);
						bool fail = false;
						SteamUtils()->GetAPICallResult(handle, Result, sizeof(TargetStruct), ID, &fail);
					}
					void Function(TargetStruct* r, bool iofail)
					{
						if (!iofail)
							Result = r;
					}
					TargetStruct* operator->()
					{
						return Result;
					}
					TargetStruct* Result;
				private:
					CCallResult<STCallResult<TargetStruct>, TargetStruct>Callresult;
				};
			}
		}
	}
}
using namespace std;
using namespace DSTEd::Core::SteamWorkshopHelper::Utilitity;
Steamworks::RemoteStorageGetPublishedFileDetailsResult_t DSTEd::Core::SteamWorkshopHelper::RemoteStorageHelper::GetDetails(Steamworks::PublishedFileId_t ManagedFileID)
{
	if (!SteamAPI_Init())
		return Steamworks::RemoteStorageGetPublishedFileDetailsResult_t();
	auto rs = SteamRemoteStorage();
	auto util = SteamUtils();
	auto handle = rs->GetPublishedFileDetails(ManagedFileID.m_PublishedFileId, 0);
	char temp[10] = { 0 };
	STCallResult<RemoteStorageGetPublishedFileDetailsResult_t> Detail(handle);
	auto returnvalue = Steamworks::RemoteStorageGetPublishedFileDetailsResult_t();
	returnvalue.m_bAcceptedForUse = Detail->m_bAcceptedForUse;
	returnvalue.m_bBanned = Detail->m_bBanned;
	returnvalue.m_bTagsTruncated = Detail->m_bTagsTruncated;
	System::Enum::TryParse<Steamworks::EWorkshopFileType>(System::Int32(Detail->m_eFileType).ToString(), returnvalue.m_eFileType);
	System::Enum::TryParse<Steamworks::EResult>(System::Int32(Detail->m_eResult).ToString(), returnvalue.m_eResult);
	System::Enum::TryParse<Steamworks::ERemoteStoragePublishedFileVisibility>(System::Int32(Detail->m_eVisibility).ToString(), returnvalue.m_eVisibility);
	returnvalue.m_hFile = Steamworks::UGCHandle_t(Detail->m_hFile);
	returnvalue.m_hPreviewFile		= Steamworks::UGCHandle_t(Detail->m_hPreviewFile);
	returnvalue.m_nConsumerAppID	= Steamworks::AppId_t(Detail->m_nConsumerAppID);
	returnvalue.m_nCreatorAppID	= Steamworks::AppId_t(Detail->m_nCreatorAppID);
	returnvalue.m_nFileSize		= Detail->m_nFileSize;
	returnvalue.m_nPreviewFileSize = Detail->m_nPreviewFileSize;
	returnvalue.m_nPublishedFileId = Steamworks::PublishedFileId_t(Detail->m_nPublishedFileId);
	returnvalue.m_rgchDescription	= gcnew String(Detail->m_rgchDescription);
	returnvalue.m_rgchTags			= gcnew String(Detail->m_rgchTags);
	returnvalue.m_rgchTitle		= gcnew String(Detail->m_rgchTitle);
	returnvalue.m_rgchURL			= gcnew String(Detail->m_rgchURL);
	returnvalue.m_rtimeCreated		= Detail->m_rtimeCreated;
	returnvalue.m_rtimeUpdated		= Detail->m_rtimeUpdated;
	returnvalue.m_ulSteamIDOwner	= Detail->m_ulSteamIDOwner;
	return returnvalue;
}
