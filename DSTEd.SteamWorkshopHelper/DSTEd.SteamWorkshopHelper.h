#pragma once
#include <steamapi/steam_api.h>
using namespace System;

namespace DSTEd
{
	namespace Core
	{
		namespace SteamWorkshopHelper
		{
			public ref class RemoteStorageHelper
			{
			public:
				static Steamworks::RemoteStorageGetPublishedFileDetailsResult_t GetDetails(Steamworks::PublishedFileId_t ManagedFileID);
			};
		}
	}
}
