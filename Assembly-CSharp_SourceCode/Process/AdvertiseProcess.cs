using System;
using System.Collections.Generic;
using AMDaemon;
using DB;
using IO;
using MAI2.Util;
using Manager;
using Mecha;
using Monitor;
using Monitor.Common;
using PartyLink;
using UnityEngine;

namespace Process
{
	public class AdvertiseProcess : ProcessBase
	{
		public enum AdvertiseSequence
		{
			Wait,
			Logo,
			Title,
			SyncTitle,
			Release
		}

		private AdvertiseSequence _state;

		private float _timeCounter;

		private float _syncTimer;

		private float _rebootTimer = -1f;

		private MovieController _titleMovie;

		private AdvertiseMonitor[] _monitors;

		private int _frameCount;

		public AdvertiseProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameManager.Initialize();
			UpdateGamePeriod();
			Singleton<MusicRankingManager>.Instance.Initialize();
			Singleton<ScoreRankingManager>.Instance.UpdateData();
			SoundManager.SetMasterVolume(SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.AdvVol.GetVolume());
			GameObject prefs = Resources.Load<GameObject>("Process/Advertise/AdvertiseProcess");
			_monitors = new AdvertiseMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<AdvertiseMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<AdvertiseMonitor>()
			};
			for (int i = 0; i < 4; i++)
			{
				Singleton<UserDataManager>.Instance.GetUserData(i).Initialize();
				Singleton<UserDataManager>.Instance.SetDefault(i);
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				_monitors[j].Initialize(j, active: true);
				SoundManager.SetHeadPhoneVolume(j, Singleton<UserDataManager>.Instance.GetUserData(j).Option.HeadPhoneVolume.GetValue());
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30000));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, j, 1));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50002, CommonMonitor.SkyDaylight.MorningNow));
			}
			container.processManager.NotificationFadeIn();
			bool flag = Singleton<OperationManager>.Instance.IsAimeLoginDisable();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(!flag);
			for (int k = 0; k < _monitors.Length; k++)
			{
				_monitors[k].PlayLogo();
				SoundManager.StopBGM(k);
			}
			GC.Collect();
			List<IO.Jvs.LedPwmFadeParam> list = new List<IO.Jvs.LedPwmFadeParam>();
			List<Color> list2 = new List<Color>
			{
				CommonScriptable.GetLedSetting().ButtonMainColor,
				CommonScriptable.GetLedSetting().ButtonSubColor
			};
			for (int l = 0; l < list2.Count; l++)
			{
				list.Add(new IO.Jvs.LedPwmFadeParam
				{
					StartFadeColor = list2[l],
					EndFadeColor = list2[(l + 1) % list2.Count],
					FadeTime = 4000L,
					NextIndex = (l + 1) % list2.Count
				});
			}
			for (int m = 0; m < 2; m++)
			{
				MechaManager.Jvs.SetPwmOutput((byte)m, CommonScriptable.GetLedSetting().BillboardMainColor);
				MechaManager.LedIf[m].SetColorMultiFet(Bd15070_4IF.BodyBrightOutGame);
			}
			Bd15070_4IF[] ledIf = MechaManager.LedIf;
			for (int n = 0; n < ledIf.Length; n++)
			{
				ledIf[n].SetColorMultiAutoFade(list);
			}
			_titleMovie = UnityEngine.Object.Instantiate(CommonPrefab.GetMovieCtrlObject());
			_titleMovie.SetMovieFile("DX_title");
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SetCoinCostInit();
			Singleton<OperationManager>.Instance.AutomaticDownload = true;
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 45001, true));
		}

		public override void OnUpdate()
		{
			_syncTimer -= Time.deltaTime;
			bool num = UpdateCheckReboot();
			bool flag = false;
			bool flag2 = CheckButtonOrAime();
			if (num)
			{
				GameManager.IsGotoReboot = true;
				return;
			}
			if (!flag && flag2)
			{
				_ = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsEventMode;
				container.processManager.AddProcess(new FadeProcess(container, this, new EntryProcess(container)), 50);
				LeaveAdvertise();
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 45001, false));
				for (int i = 0; i < _monitors.Length; i++)
				{
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 40000, i, OperationInformationController.InformationType.Hide));
				}
				return;
			}
			if (!SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableFlag && !Singleton<OperationManager>.Instance.IsAimeLoginDisable())
			{
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: true);
			}
			else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableFlag && Singleton<OperationManager>.Instance.IsAimeLoginDisable())
			{
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.EnableRead(flag: false);
			}
			switch (_state)
			{
			case AdvertiseSequence.Wait:
				_timeCounter += Time.deltaTime;
				if (_timeCounter >= 1f && _titleMovie.IsMoviePrepare())
				{
					_timeCounter = 0f;
					_state = AdvertiseSequence.Logo;
					for (int j = 0; j < _monitors.Length; j++)
					{
						_monitors[j].PlayLogo();
					}
				}
				break;
			case AdvertiseSequence.Logo:
				if (_monitors[0].IsLogoAnimationEnd())
				{
					_state = AdvertiseSequence.Title;
					Advertise.get()?.terminate();
					_syncTimer = 30f;
					for (int k = 0; k < _monitors.Length; k++)
					{
						_monitors[k].PlayTitleLogo();
					}
					_titleMovie.Play();
				}
				break;
			case AdvertiseSequence.Title:
				if (_monitors[0].IsTitleAnimationEnd())
				{
					_state = AdvertiseSequence.SyncTitle;
				}
				break;
			case AdvertiseSequence.SyncTitle:
				if (GameManager.IsEventMode || Singleton<MusicRankingManager>.Instance.Rankings.Count == 0)
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new AdvertiseProcess(container)), 50);
				}
				else
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new AdvDemoProcess(container)), 50);
				}
				LeaveAdvertise();
				break;
			case AdvertiseSequence.Release:
				return;
			}
			UpdateOperationInfomation();
			CheckNetworkDelivery();
			UpdateSetting();
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			if (null != _titleMovie)
			{
				UnityEngine.Object.Destroy(_titleMovie.gameObject);
			}
			_titleMovie = null;
			Resources.UnloadUnusedAssets();
			GC.Collect();
			if (_monitors == null)
			{
				return;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (null != _monitors[i])
				{
					UnityEngine.Object.Destroy(_monitors[i].gameObject);
				}
			}
		}

		public static bool IsGotoEntry()
		{
			if ((SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnough() && InputManager.IsButtonDown()) || SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsAddCoinNow())
			{
				_ = 1;
			}
			else
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.AnyRead();
			bool flag = InputManager.GetButtonDown(0, InputManager.ButtonSetting.Select) || InputManager.GetButtonLongPush(0, InputManager.ButtonSetting.Select, 1000L) || InputManager.GetButtonDown(1, InputManager.ButtonSetting.Select) || InputManager.GetButtonLongPush(1, InputManager.ButtonSetting.Select, 1000L);
			bool num = (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnough() && InputManager.IsButtonDown()) || SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsAddCoinNow() || SingletonStateMachine<AmManager, AmManager.EState>.Instance.AimeReader.AnyRead() || flag;
			bool flag2 = !Singleton<OperationManager>.Instance.IsLoginDisable();
			if (num && flag2)
			{
				return true;
			}
			return false;
		}

		private bool CheckButtonOrAime()
		{
			if (IsGotoEntry() && _state != AdvertiseSequence.Release)
			{
				return true;
			}
			return false;
		}

		private bool UpdateCheckReboot()
		{
			bool result = false;
			if (_rebootTimer < 0f)
			{
				if (Singleton<OperationManager>.Instance.IsRebootNeeded())
				{
					_rebootTimer = 5f;
				}
			}
			else if (Singleton<OperationManager>.Instance.IsRebootNeeded())
			{
				_rebootTimer -= Time.deltaTime;
				if (_rebootTimer < 0f)
				{
					result = true;
				}
			}
			else
			{
				_rebootTimer = -1f;
			}
			return result;
		}

		private static void UpdateGamePeriod()
		{
			Singleton<OperationManager>.Instance.UpdateGamePeriod();
		}

		private void UpdateOperationInfomation()
		{
			if (_state == AdvertiseSequence.Release)
			{
				return;
			}
			if (Singleton<OperationManager>.Instance.IsCoinAcceptable())
			{
				bool flag = Singleton<OperationManager>.Instance.IsUnderServerMaintenance();
				bool flag2 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnough();
				bool flag3 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain != 0 || SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit != 0;
				bool flag4 = Singleton<OperationManager>.Instance.IsAimeOffline();
				for (int i = 0; i < 2; i++)
				{
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 40001, i, flag2, flag3, false, flag4, flag));
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 40000, j, OperationInformationController.InformationType.Hide));
				}
			}
		}

		private void CheckNetworkDelivery()
		{
			DeliveryChecker.IManager manager = DeliveryChecker.get();
			if (manager != null && manager.isError())
			{
				manager.getErrorID();
				_ = 2;
			}
		}

		private void UpdateSetting()
		{
			Setting.IManager manager = Setting.get();
			if (manager == null)
			{
				return;
			}
			if (manager.isStandardSettingMachine)
			{
				if (manager.isDuplicate())
				{
					manager.terminate();
					AMDaemon.Error.Set(3201);
				}
			}
			else if (!manager.isBusy() && manager.isDataValid())
			{
				Setting.Data data = manager.getData();
				BackupGameSetting gameSetting = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting;
				if (data.isEventModeSettingAvailable != gameSetting.IsEventMode || data.eventModeMusicCount != (int)gameSetting.EventModeTrack)
				{
					gameSetting.IsEventMode = data.isEventModeSettingAvailable;
					gameSetting.EventModeTrack = (EventModeMusicCountID)data.eventModeMusicCount;
				}
			}
		}

		private void LeaveAdvertise()
		{
			Advertise.get()?.exit();
			_state = AdvertiseSequence.Release;
			SoundManager.StopBGM(2);
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].AllStop();
			}
		}
	}
}
