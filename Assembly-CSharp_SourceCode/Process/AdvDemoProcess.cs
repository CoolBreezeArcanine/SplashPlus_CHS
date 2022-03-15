using System;
using System.Collections.Generic;
using System.Diagnostics;
using DB;
using IO;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Mecha;
using Monitor;
using Monitor.Common;
using UnityEngine;

namespace Process
{
	public class AdvDemoProcess : ProcessBase
	{
		private enum AdvDemoSequence
		{
			Init,
			Start,
			StartWait,
			Play,
			Release
		}

		private AdvDemoSequence _sequence;

		private AdvDemoMonitor[] _monitors;

		private bool _startMusic;

		private const int GamePlayWaitTime = 1000;

		private readonly Stopwatch _startWait = new Stopwatch();

		private readonly Stopwatch _pvTimer = new Stopwatch();

		private MovieController _gameMovie;

		private const int RankingDispMaxNum = 20;

		private const int AdvertiseTimes = 60000;

		private int pvStartMsec;

		private int pvEndMsec;

		private float _rebootTimer = -1f;

		public AdvDemoProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameManager.Initialize();
			GameManager.IsAdvDemo = true;
			SoundManager.SetMasterVolume(SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.AdvVol.GetVolume());
			GameObject prefs = Resources.Load<GameObject>("Process/AdvDemo/AdvDemoProcess");
			_monitors = new AdvDemoMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<AdvDemoMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<AdvDemoMonitor>()
			};
			MusicRankingManager instance = Singleton<MusicRankingManager>.Instance;
			List<MusicRankingManager.MusicRankingSt> rankings = instance.Rankings;
			for (int i = 0; i < _monitors.Length; i++)
			{
				GameManager.SelectMusicID[i] = rankings[instance.GetPlayDemoIndex()].MusicId;
				GameManager.SelectDifficultyID[i] = 0;
			}
			Singleton<GamePlayManager>.Instance.AddPlayLog();
			Singleton<GamePlayManager>.Instance.InitializeAdvertise();
			MusicData music = Singleton<DataManager>.Instance.GetMusic(rankings[instance.GetPlayDemoIndex()].MusicId);
			Texture2D jacketTexture2D = container.assetManager.GetJacketTexture2D(music.thumbnailName);
			for (int j = 0; j < _monitors.Length; j++)
			{
				UserOption userOption = Singleton<GamePlayManager>.Instance.GetGameScore(j).UserOption;
				userOption.Initialize();
				userOption.OptionKind = OptionKindID.Custom;
				userOption.NoteSpeed = OptionNotespeedID.Speed1_0;
				userOption.AnsVolume = OptionVolumeID.Mute;
				userOption.TapHoldVolume = OptionVolumeID.Mute;
				userOption.BreakVolume = OptionVolumeID.Mute;
				userOption.SlideVolume = OptionVolumeID.Mute;
				userOption.ExVolume = OptionVolumeID.Mute;
				userOption.TouchHoldVolume = OptionVolumeID.Mute;
				userOption.DamageSeVolume = OptionVolumeID.Mute;
				userOption.TouchEffect = OptionToucheffectID.On;
				_monitors[j].Initialize(j, isActive: true);
				_monitors[j].SetMusicInfo(jacketTexture2D, music.name.str, instance.GetPlayDemoIndex(), rankings[instance.GetPlayDemoIndex()].IsNew);
				SoundManager.StopBGM(j);
			}
			for (int k = 0; k < 20 && k < rankings.Count; k++)
			{
				MusicData music2 = Singleton<DataManager>.Instance.GetMusic(rankings[k].MusicId);
				if (music2 != null)
				{
					Texture2D jacketTexture2D2 = container.assetManager.GetJacketTexture2D(music2.thumbnailName);
					AdvDemoMonitor[] monitors = _monitors;
					for (int l = 0; l < monitors.Length; l++)
					{
						monitors[l].SetMusicRanking(jacketTexture2D2, music2.name.str, k, rankings[k].IsNew);
					}
				}
			}
			instance.AddDemoIndex();
			MusicData music3 = Singleton<DataManager>.Instance.GetMusic(GameManager.SelectMusicID[0]);
			SoundManager.MusicPrepare(music3.cueName.id);
			_gameMovie = UnityEngine.Object.Instantiate(CommonPrefab.GetMovieCtrlObject());
			_gameMovie.SetMusicMovie(music3.movieName.id);
			pvStartMsec = 0;
			pvEndMsec = 60000;
			GameManager.AutoPlay = GameManager.AutoPlayMode.Critical;
			List<Jvs.LedFetFadeParam> list = new List<Jvs.LedFetFadeParam>();
			list.Add(new Jvs.LedFetFadeParam
			{
				StartFadeColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
				EndFadeColor = new Color32(0, 0, byte.MaxValue, byte.MaxValue),
				FadeTime = 650L,
				NextIndex = 1
			});
			list.Add(new Jvs.LedFetFadeParam
			{
				StartFadeColor = new Color32(0, 0, byte.MaxValue, byte.MaxValue),
				EndFadeColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
				FadeTime = 650L,
				NextIndex = 0
			});
			for (int m = 0; m < 2; m++)
			{
				MechaManager.Jvs.SetPwmOutput((byte)m, CommonScriptable.GetLedSetting().BillboardMainColor);
				MechaManager.LedIf[m].SetColorFetAutoFade(list);
			}
			Bd15070_4IF[] ledIf = MechaManager.LedIf;
			for (int l = 0; l < ledIf.Length; l++)
			{
				ledIf[l].SetColorMulti(CommonScriptable.GetLedSetting().ButtonMainColor, 0);
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		public override void OnUpdate()
		{
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
				SetRelease(gotoEntry: true);
				return;
			}
			if (_startMusic && _pvTimer.ElapsedMilliseconds > pvEndMsec - pvStartMsec && _sequence != AdvDemoSequence.Release)
			{
				if (_sequence == AdvDemoSequence.Play)
				{
					SetRelease(gotoEntry: false);
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
			AdvDemoMonitor[] monitors;
			switch (_sequence)
			{
			case AdvDemoSequence.Init:
				if (SoundManager.IsMusicPrepare() && _gameMovie.IsMoviePrepare() && _monitors[0].IsReady() && _monitors[1].IsReady())
				{
					container.processManager.NotificationFadeIn();
					_sequence = AdvDemoSequence.Start;
					_startWait.Reset();
					_startWait.Start();
					for (int k = 0; k < _monitors.Length; k++)
					{
						_monitors[k].SetMovieSize(_gameMovie.GetMovieHeight(), _gameMovie.GetMovieWidth());
					}
				}
				break;
			case AdvDemoSequence.Start:
				if (_startWait.ElapsedMilliseconds > 1000)
				{
					_sequence = AdvDemoSequence.StartWait;
					NotesManager.StartPlay(0f);
					monitors = _monitors;
					for (int j = 0; j < monitors.Length; j++)
					{
						monitors[j].PlayStart();
					}
					_startWait.Reset();
					_startWait.Start();
				}
				break;
			case AdvDemoSequence.StartWait:
				UpdateNotes();
				if (_startWait.ElapsedMilliseconds > 91)
				{
					_sequence = AdvDemoSequence.Play;
					_gameMovie.Play();
					SoundManager.StartMusic();
					_startMusic = true;
					_pvTimer.Reset();
					_pvTimer.Start();
					for (int i = 0; i < _monitors.Length; i++)
					{
						SoundManager.PlayVoice(Cue.VO_000004, i);
					}
				}
				break;
			case AdvDemoSequence.Play:
				UpdateNotes();
				break;
			}
			UpdateOperationInfomation();
			monitors = _monitors;
			for (int j = 0; j < monitors.Length; j++)
			{
				monitors[j].ViewUpdate();
			}
		}

		private void UpdateOperationInfomation()
		{
			if (_sequence == AdvDemoSequence.Release)
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

		public void UpdateNotes()
		{
			NotesManager.UpdateTimer();
			for (int i = 0; i < 4; i++)
			{
				NotesManager.Instance(i).updateNotes();
			}
		}

		private void SetRelease(bool gotoEntry)
		{
			container.processManager.AddProcess(gotoEntry ? new FadeProcess(container, this, new EntryProcess(container)) : new FadeProcess(container, this, new AdvertiseProcess(container)), 50);
			if (gotoEntry)
			{
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 45001, false));
				for (int i = 0; i < _monitors.Length; i++)
				{
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 40000, i, OperationInformationController.InformationType.Hide));
				}
			}
			SoundManager.StopMusic();
			_sequence = AdvDemoSequence.Release;
			_gameMovie.Stop();
			NotesManager.StopPlay();
			AdvDemoMonitor[] monitors = _monitors;
			for (int j = 0; j < monitors.Length; j++)
			{
				monitors[j].PlayStop();
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			if (null != _gameMovie)
			{
				UnityEngine.Object.Destroy(_gameMovie.gameObject);
				_gameMovie = null;
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			for (int i = 0; i < _monitors?.Length; i++)
			{
				UnityEngine.Object.Destroy(_monitors[i]?.gameObject);
			}
			Singleton<GamePlayManager>.Instance.ClaerLog();
			GameManager.IsAdvDemo = false;
		}

		private bool CheckButtonOrAime()
		{
			if (AdvertiseProcess.IsGotoEntry() && _sequence != AdvDemoSequence.Release)
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
	}
}
