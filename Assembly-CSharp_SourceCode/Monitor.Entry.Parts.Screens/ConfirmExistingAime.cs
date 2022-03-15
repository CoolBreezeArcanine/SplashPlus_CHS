using DB;
using MAI2.Util;
using MAI2System;
using Manager;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmExistingAime : ConfirmEntryAime
	{
		public override void Open(params object[] args)
		{
			WindowMessageID windowMessageID = WindowMessageID.Invalid;
			windowMessageID = (GameManager.IsEventMode ? WindowMessageID.EntryConfirmExistingAimeEvent : ((!IsOldVersion()) ? WindowMessageID.EntryConfirmExistingAimeOldUser : WindowMessageID.EntryConfirmExistingAime));
			base.Open(args[0], (object)windowMessageID, (object)PromotionType.Normal);
		}

		private bool IsOldVersion()
		{
			VersionNo versionNo = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo;
			VersionNo versionNo2 = Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo;
			VersionNo versionNo3 = default(VersionNo);
			VersionNo versionNo4 = default(VersionNo);
			versionNo3.tryParse((base.Monitor as EntryMonitor)?.RomDataVersion ?? "", setZeroIfFailed: true);
			versionNo4.tryParse((base.Monitor as EntryMonitor)?.TableDataVersion ?? "", setZeroIfFailed: true);
			uint versionCode = versionNo3.versionCode;
			uint versionCode2 = versionNo.versionCode;
			uint versionCode3 = versionNo4.versionCode;
			uint versionCode4 = versionNo2.versionCode;
			bool num = versionCode == versionCode2;
			bool result = versionCode3 == versionCode4;
			if (num)
			{
				return result;
			}
			return false;
		}
	}
}
