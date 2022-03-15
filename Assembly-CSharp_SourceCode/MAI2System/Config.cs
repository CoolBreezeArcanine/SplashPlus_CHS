using System.Text;
using AMDaemon;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Net;
using UnityEngine;

namespace MAI2System
{
	public class Config
	{
		public const int CameraType_First = 0;

		public const int CameraType_Second = 1;

		private VersionInfo _romVersionInfo = new VersionInfo();

		private VersionInfo _dataVersionInfo = new VersionInfo();

		private VersionInfo _cardMakerVersionInfo = new VersionInfo();

		private string _displayVersionString = "";

		public bool IsDebug;

		public VersionInfo romVersionInfo => _romVersionInfo;

		public VersionInfo dataVersionInfo => _dataVersionInfo;

		public VersionInfo cardMakerVersionInfo => _cardMakerVersionInfo;

		public string displayVersionString => _displayVersionString;

		public bool IsDebugDisp { get; private set; }

		public bool IsAllOpen { get; private set; }

		public bool IsAllCollection { get; private set; }

		public bool IsAllCharactor { get; private set; }

		public int ForceRate { get; private set; }

		public int ForceDxScoreT { get; private set; }

		public int ForceDxScoreY { get; private set; }

		public int ForceDxScoreU { get; private set; }

		public int ForceDxScoreI { get; private set; }

		public int ForceAchiveT { get; private set; }

		public int ForceAchiveY { get; private set; }

		public int ForceAchiveU { get; private set; }

		public int ForceAchiveI { get; private set; }

		public int MapMeter { get; private set; }

		public int GhostAchive { get; private set; }

		public int GhostClassValue { get; private set; }

		public bool IsAutoPlay { get; private set; }

		public int MaxTrack { get; private set; }

		public bool IsMovieSupervision { get; private set; }

		public bool IsFumenCheck { get; private set; }

		public bool IsEventOverride { get; private set; }

		public bool FunenEditor { get; private set; }

		public int NotesSpeed { get; private set; }

		public int TouchSpeed { get; private set; }

		public int NotesDesign { get; private set; }

		public int HoldDesign { get; private set; }

		public int SlideDesign { get; private set; }

		public bool IsTarget { get; private set; }

		public bool IsDummyTouchPanel { get; private set; }

		public bool IsDummyLed { get; private set; }

		public bool IsDummyQrCamera { get; private set; }

		public bool IsDummyPhotoCamera { get; private set; }

		public bool IsIgnoreError { get; private set; }

		public bool IsUseNetwork { get; private set; }

		public bool Is8Ch { get; private set; }

		public int DebugMajorVersion { get; private set; }

		public int DebugMinorVersion { get; private set; }

		public int DebugReleaseVersion { get; private set; }

		public void initialize()
		{
			using IniFile iniFile = new IniFile("mai2.ini");
			bool flag = false;
			IsDebugDisp = iniFile.getValue("Debug", "DebugDisp", flag);
			IsAllOpen = iniFile.getValue("Debug", "AllOpen", defaultParam: false);
			IsAutoPlay = iniFile.getValue("Debug", "AutoPlay", defaultParam: false);
			MaxTrack = iniFile.getValue("Debug", "MaxTrack", 0);
			IsMovieSupervision = iniFile.getValue("Debug", "MovieCheck", defaultParam: false);
			IsFumenCheck = iniFile.getValue("Debug", "FumenCheck", defaultParam: false);
			IsEventOverride = iniFile.getValue("Debug", "EventOverride", defaultParam: false);
			IsAllCollection = iniFile.getValue("Debug", "AllCollection", defaultParam: false);
			IsAllCharactor = iniFile.getValue("Debug", "AllChara", defaultParam: false);
			ForceRate = iniFile.getValue("Debug", "ForceRate", 0);
			ForceDxScoreT = iniFile.getValue("Debug", "ForceDxScoreT", 5000);
			ForceDxScoreY = iniFile.getValue("Debug", "ForceDxScoreY", 8000);
			ForceDxScoreU = iniFile.getValue("Debug", "ForceDxScoreU", 9700);
			ForceDxScoreI = iniFile.getValue("Debug", "ForceDxScoreI", 10000);
			ForceAchiveT = iniFile.getValue("Debug", "ForceAchieveT", 5000);
			ForceAchiveY = iniFile.getValue("Debug", "ForceAchieveY", 8000);
			ForceAchiveU = iniFile.getValue("Debug", "ForceAchieveU", 9700);
			ForceAchiveI = iniFile.getValue("Debug", "ForceAchieveI", 10000);
			MapMeter = iniFile.getValue("Debug", "MapMeter", 0);
			GhostAchive = iniFile.getValue("Debug", "GhostAchive", 0);
			GhostClassValue = iniFile.getValue("Debug", "GhostClassValue", 0);
			IsDebug = iniFile.getValue("Debug", "Debug", defaultParam: false);
			FunenEditor = iniFile.getValue("FumenEditor", "FunenEditor", defaultParam: false);
			NotesSpeed = iniFile.getValue("FumenEditor", "NotesSpeed", 9);
			TouchSpeed = iniFile.getValue("FumenEditor", "TouchSpeed", 10);
			NotesDesign = iniFile.getValue("FumenEditor", "NotesDesign", 0);
			HoldDesign = iniFile.getValue("FumenEditor", "HoldDesign", 1);
			SlideDesign = iniFile.getValue("FumenEditor", "SlideDesign", 1);
			IsTarget = iniFile.getValue("AM", "Target", !flag);
			IsDummyTouchPanel = iniFile.getValue("AM", "DummyTouchPanel", flag);
			IsDummyLed = iniFile.getValue("AM", "DummyLED", flag);
			IsDummyQrCamera = iniFile.getValue("AM", "DummyCodeCamera", flag);
			IsDummyPhotoCamera = iniFile.getValue("AM", "DummyPhotoCamera", flag);
			IsIgnoreError = iniFile.getValue("AM", "IgnoreError", defaultParam: false);
			IsUseNetwork = iniFile.getValue("Network", "UseNetwork", defaultParam: true);
			Is8Ch = iniFile.getValue("Sound", "Sound8Ch", defaultParam: true);
			DebugMajorVersion = iniFile.getValue("DebugVersion", "DebugMajorVersion", -1);
			DebugMinorVersion = iniFile.getValue("DebugVersion", "DebugMinorVersion", -1);
			DebugReleaseVersion = iniFile.getValue("DebugVersion", "DebugReleaseVersion", -1);
		}

		public void initializeAfterAMDaemonReady()
		{
			Singleton<DataManager>.Instance.SetDirs(Application.streamingAssetsPath, AppImage.OptionMountRootPath);
			Singleton<DataManager>.Instance.SetupConfig();
			int majorNo = ((DebugMajorVersion >= 0) ? DebugMajorVersion : SingletonStateMachine<AmManager, AmManager.EState>.Instance.VersionNo.majorNo);
			int minorNo = ((DebugMinorVersion >= 0) ? DebugMinorVersion : SingletonStateMachine<AmManager, AmManager.EState>.Instance.VersionNo.minorNo);
			int releaseNo = ((DebugReleaseVersion >= 0) ? DebugReleaseVersion : SingletonStateMachine<AmManager, AmManager.EState>.Instance.VersionNo.releaseNo);
			VersionNo versionNo = new VersionNo((uint)majorNo, (uint)minorNo, (uint)releaseNo);
			Manager.MaiStudio.Config config = Singleton<DataManager>.Instance.GetConfig();
			_romVersionInfo.set("SINMAI", versionNo);
			Manager.MaiStudio.Version dataVersion = config.GetDataVersion();
			_dataVersionInfo.set("SINMAI", (byte)dataVersion.major, (byte)dataVersion.minor, (byte)dataVersion.release);
			Manager.MaiStudio.Version cardMakerVersion = config.GetCardMakerVersion();
			_cardMakerVersionInfo.set("SINMAI", (byte)cardMakerVersion.major, (byte)cardMakerVersion.minor, (byte)cardMakerVersion.release);
			string releaseNoAlphabet = _dataVersionInfo.versionNo.releaseNoAlphabet;
			StringBuilder stringBuilder = Singleton<SystemConfig>.Instance.getStringBuilder();
			int minorNo2 = romVersionInfo.versionNo.minorNo;
			stringBuilder.AppendFormat("Ver.DX{0}.{1:00}", romVersionInfo.versionNo.majorNo, (byte)minorNo2);
			if (!string.IsNullOrEmpty(releaseNoAlphabet))
			{
				stringBuilder.AppendFormat("-{0}", releaseNoAlphabet);
			}
			_displayVersionString = stringBuilder.ToString();
			NetConfig.ClientId = AMDaemon.System.KeychipId.ShortValue;
		}
	}
}
