using System;
using AMDaemon;
using AMDaemon.Allnet;
using IO;
using MAI2.Util;
using Manager.Operation;
using Net.Packet;
using Net.VO.Mai2;
using UnityEngine;

namespace Manager
{
	public class OperationManager : Singleton<OperationManager>
	{
		public enum NetStatus
		{
			Offline,
			LimitedOnline,
			Online
		}

		public enum State
		{
			CheckAuth,
			Download,
			Upload,
			Idle
		}

		private readonly Mode<OperationManager, State> _mode;

		private readonly OperationData _downloadData;

		private readonly OperationData _operationData;

		private readonly DataUploader _dataUploader;

		private readonly DataDownloaderMai2 _dataDownloader;

		private readonly MaintenanceTimer _maintenanceTimer;

		private readonly SegaBootTimer _segaBootTimer;

		private readonly ClosingTimer _closingTimer;

		private CoinBlocker.Mode _coinBlockerMode;

		private bool _coinBlockerFlag;

		private bool _isUnitTest;

		public bool IsAliveAimeReader = true;

		public bool AutomaticDownload
		{
			set
			{
				_dataDownloader.AutomaticDownload = value;
			}
		}

		public bool IsAuthGood { get; private set; }

		public bool IsAliveAime
		{
			get
			{
				if (IsAliveAimeReader)
				{
					return IsAliveAimeServer;
				}
				return false;
			}
		}

		public bool IsAliveAimeServer => Aime.IsDBAlive;

		public bool IsAliveServer => _dataDownloader.IsOnline;

		public bool WasDownloadSuccessOnce => _dataDownloader.WasDownloadSuccessOnce;

		public NetStatus NetIconStatus
		{
			get
			{
				if (!IsAliveServer || !IsAliveAime)
				{
					return NetStatus.Offline;
				}
				return NetStatus.Online;
			}
		}

		public ShopInfomation ShopData { get; }

		public OperationManager()
		{
			IsAuthGood = false;
			_mode = new Mode<OperationManager, State>(this);
			_downloadData = new OperationData();
			_operationData = new OperationData();
			_dataUploader = new DataUploader();
			_dataDownloader = new DataDownloaderMai2();
			ShopData = new ShopInfomation();
			_maintenanceTimer = new MaintenanceTimer();
			_closingTimer = new ClosingTimer();
			_segaBootTimer = new SegaBootTimer();
		}

		public void Initialize(bool isUnitTest = false)
		{
			_mode.set(State.CheckAuth);
			_isUnitTest = isUnitTest;
			if (!_isUnitTest)
			{
				_maintenanceTimer.Initialize();
				_closingTimer.Initialize();
			}
		}

		public void Terminate()
		{
			if (!_isUnitTest)
			{
				_maintenanceTimer.Terminate();
				_closingTimer.Terminate();
			}
		}

		public void Execute()
		{
			_mode.update();
			_dataUploader.Execute();
			_dataDownloader.Execute();
			if (!_isUnitTest)
			{
				_maintenanceTimer.Execute();
				_closingTimer.Execute();
				MonitorTimersState();
			}
			_segaBootTimer.Execute();
		}

		public void OnPacketFinish(PacketStatus status)
		{
			if (status == PacketStatus.ErrorDecodeResponse || status == PacketStatus.Ok)
			{
				_dataDownloader?.NotifyOnline();
			}
			else
			{
				_dataDownloader?.NotifyOffline();
			}
		}

		public void StartTest()
		{
			if (IsAuthGood)
			{
				_dataDownloader?.Start(isNetworkTest: true);
			}
		}

		public void UpdateGamePeriod()
		{
			if (_downloadData.IsUpdate)
			{
				_operationData.GameSetting = _downloadData.GameSetting;
				_operationData.GameRankings = _downloadData.GameRankings;
				_operationData.GameEvents = _downloadData.GameEvents;
				_operationData.GameTournamentInfos = _downloadData.GameTournamentInfos;
				_operationData.GameCharges = _downloadData.GameCharges;
				_downloadData.IsUpdate = false;
			}
		}

		private void CheckAuth_Proc()
		{
			if (Core.IsReady && Auth.IsGood)
			{
				IsAuthGood = true;
				_operationData.ServerUri = Auth.GameServerUri;
				ShopData.ShopName = Auth.LocationName;
				ShopData.ShopNickName = string.Join("", Auth.LocationNicknames);
				ShopData.RegionCode = Auth.RegionCode;
				ShopData.LocationId = Auth.LocationId;
				ShopData.CountryCode = Auth.CountryCode;
				_mode.set((!_isUnitTest) ? State.Download : State.Idle);
			}
		}

		private void Download_Init()
		{
			_dataDownloader.Start(isNetworkTest: false);
		}

		private void Download_Proc()
		{
			if (CheckUpdateDownloadData())
			{
				_mode.set(State.Upload);
			}
		}

		private void Upload_Init()
		{
			if (!_dataUploader.Start())
			{
				_mode.set(State.Idle);
			}
		}

		private void Upload_Proc()
		{
			if (_dataUploader.IsFinished)
			{
				_mode.set(State.Idle);
			}
		}

		private void Idle_Proc()
		{
			CheckUpdateDownloadData();
		}

		private bool CheckUpdateDownloadData()
		{
			if (!_dataDownloader.OperationData.IsUpdate)
			{
				return false;
			}
			_dataDownloader.OperationData.IsUpdate = false;
			_downloadData.IsDumpUpload = _dataDownloader.OperationData.IsDumpUpload;
			_downloadData.IsAou = _dataDownloader.OperationData.IsAou;
			_downloadData.GameSetting = _dataDownloader.OperationData.GameSetting;
			_downloadData.GameRankings = _dataDownloader.OperationData.GameRankings;
			_downloadData.GameEvents = _dataDownloader.OperationData.GameEvents;
			_downloadData.GameTournamentInfos = _dataDownloader.OperationData.GameTournamentInfos;
			_downloadData.GameCharges = _dataDownloader.OperationData.GameCharges;
			_downloadData.IsUpdate = true;
			_maintenanceTimer.SetMaintenanceTime(DateTime.Parse(_downloadData.GameSetting.rebootStartTime), DateTime.Parse(_downloadData.GameSetting.rebootEndTime));
			_operationData.IsDumpUpload = _downloadData.IsDumpUpload;
			_operationData.IsAou = _downloadData.IsAou;
			_operationData.RebootInterval = _downloadData.GameSetting.rebootInterval;
			_segaBootTimer.SetRebootInterval(_operationData.RebootInterval);
			return true;
		}

		private void MonitorTimersState()
		{
			if (MechaManager.CoinBlocker != null)
			{
				MechaManager.CoinBlocker.mode = _coinBlockerMode;
				MechaManager.CoinBlocker.isBlockedInTestMode = _coinBlockerFlag;
			}
		}

		public string GetBaseUri()
		{
			return _operationData.ServerUri;
		}

		public bool IsBusy()
		{
			return _dataDownloader?.IsBusy ?? false;
		}

		public int GetRebootInterval()
		{
			return _operationData.RebootInterval;
		}

		public GameRanking[] GetMusicRankingList()
		{
			return _operationData.GameRankings;
		}

		public GameEvent[] GetEventDataList()
		{
			return _operationData.GameEvents;
		}

		public GameTournamentInfo[] GetGameTournamentInfoDataList()
		{
			return _operationData.GameTournamentInfos;
		}

		public GameCharge[] GetGameChargeDataList()
		{
			return _operationData.GameCharges;
		}

		public bool IsUnderServerMaintenance()
		{
			return _maintenanceTimer?.IsUnderServerMaintenance() ?? false;
		}

		public int GetServerMaintenanceSec()
		{
			return _maintenanceTimer?.GetServerMaintenanceSec() ?? 86400;
		}

		public int GetAutoRebootSec()
		{
			return _maintenanceTimer?.GetAutoRebootSec() ?? 86400;
		}

		public bool IsAutoRebootNeeded()
		{
			return false;
		}

		public bool IsSegaBootNeeded()
		{
			return false;
		}

		public bool IsRebootNeeded()
		{
			return false;
		}

		public bool IsLoginDisable()
		{
			return SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.isReporting();
		}

		public bool IsAimeLoginDisable()
		{
			if (!IsLoginDisable() && IsAuthGood && IsAliveAime)
			{
				return !IsAliveServer;
			}
			return true;
		}

		public bool IsAimeOffline()
		{
			if (IsAuthGood && IsAliveAime)
			{
				return !IsAliveServer;
			}
			return true;
		}

		public int GetClosingRemainingMinutes()
		{
			return Mathf.Min(_maintenanceTimer.GetRemainingMinutes(), _closingTimer.GetRemainingMinutes());
		}

		public bool IsShowClosingRemainingMinutes()
		{
			return false;
		}

		public bool IsClosed()
		{
			return false;
		}

		public bool IsCoinAcceptable()
		{
			return true;
		}

		public int GetSegaBootRemainingMinutes()
		{
			return _segaBootTimer.GetRemainingMinutes();
		}

		public bool IsShowSegaBootRemainingMinutes()
		{
			return _segaBootTimer.IsShowRemainingMinutes();
		}

		public void SetCoinBlockerMode(CoinBlocker.Mode mode, bool flag = false)
		{
			_coinBlockerMode = mode;
			_coinBlockerFlag = flag;
		}
	}
}
