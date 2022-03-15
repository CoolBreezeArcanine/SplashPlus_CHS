using System;
using System.Collections.Generic;
using System.Diagnostics;
using DB;
using IO;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.Party.Party;
using Manager.UserDatas;
using Monitor;
using PartyLink;
using UnityEngine;

namespace Process
{
	public class GameProcess : ProcessBase
	{
		private enum GameSequence
		{
			Init,
			Sync,
			Start,
			StartWait,
			Play,
			PlayEnd,
			Result,
			ResultEnd,
			FinalWait,
			Release
		}

		private class DebugFeature
		{
			public bool _debugPause;

			public double _debugTimer;

			public bool _debugSlow;

			public int _debugSlowCounter;

			public readonly int[] _debugRepeatTimer;

			private GameProcess parent;

			public void SetSkip(bool isPause, float time)
			{
				NotesManager.StartPlay((int)time);
				NotesManager.Pause(_debugPause);
			}

			public void SetPause(bool isPause)
			{
				NotesManager.Pause(isPause);
			}

			public DebugFeature(GameProcess parent)
			{
				this.parent = parent;
			}

			public void DebugTimeSkip(int addMsec)
			{
				parent._gameMovie.Pause(pauseFlag: true);
				SetPause(isPause: true);
				if (addMsec >= 0)
				{
					_debugTimer += addMsec;
				}
				else
				{
					_debugTimer = ((_debugTimer + (double)addMsec >= 0.0) ? (_debugTimer + (double)addMsec) : 0.0);
				}
				parent._gameMovie.SetSeekFrame(_debugTimer);
				SoundManager.SeekMusic((int)_debugTimer);
				GameMonitor[] monitors = parent._monitors;
				for (int i = 0; i < monitors.Length; i++)
				{
					monitors[i].Seek((int)_debugTimer);
				}
				int num = 91;
				SetSkip(_debugPause, (int)_debugTimer + num);
				if (!_debugPause)
				{
					SoundManager.PauseMusic(pause: false);
					parent._gameMovie.Pause(pauseFlag: false);
					SetPause(isPause: false);
				}
				else
				{
					parent._gameMovie.Pause(pauseFlag: true);
				}
				parent.UpdateNotes();
			}
		}

		private GameSequence _sequence;

		private GameMonitor[] _monitors;

		private MessageGamePlayData[] _playDatas;

		private MessageGamePlayData[] _prevPlayData;

		private Message[] _message;

		private bool _startMusic;

		private const int GamePlayWaitTime = 1000;

		public const int SoundPlayWaitTime = 91;

		private const int GameEndWaitTime = 6000;

		private const int GameTrackSkipWaitTime = 4500;

		private long WaitLastShotTiming = 3500L;

		private readonly Stopwatch _startWait = new Stopwatch();

		private readonly Stopwatch _endWait = new Stopwatch();

		private readonly Stopwatch _trackSkipWait = new Stopwatch();

		private MovieController _gameMovie;

		private PhotoManager _photoManager = new PhotoManager();

		private ClientPlayInfo _clientInfo = new ClientPlayInfo();

		private int _clientSendCount;

		private int[] _lastCombo = new int[2];

		private int[] _lastMiss = new int[2];

		private int[] _skipPhase = new int[2] { -1, -1 };

		private DebugFeature debugFeature;

		public GameProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20050, true));
		}

		public override void OnStart()
		{
			debugFeature = new DebugFeature(this);
			Resources.UnloadUnusedAssets();
			GC.Collect();
			WebCamManager.ClearGamePhotoBuffer();
			GameObject prefs = Resources.Load<GameObject>("Process/Game/GameProcess");
			_monitors = new GameMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<GameMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<GameMonitor>()
			};
			_playDatas = new MessageGamePlayData[_monitors.Length];
			_prevPlayData = new MessageGamePlayData[_monitors.Length];
			_message = new Message[_monitors.Length];
			Singleton<GamePlayManager>.Instance.AddPlayLog();
			Singleton<GamePlayManager>.Instance.Initialize(IsPartyPlay());
			for (int i = 0; i < _monitors.Length; i++)
			{
				bool isEntry = Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry;
				_monitors[i].Initialize(i, isEntry);
				_playDatas[i] = new MessageGamePlayData(0u, 1u, 0u, 0);
				_prevPlayData[i] = new MessageGamePlayData(0u, 1u, 0u, 0);
				_message[i] = new Message(ProcessType.CommonProcess, 20003, i, _playDatas[i]);
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50004, i, false));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20020, i, false));
				SoundManager.StopBGM(i);
				if (isEntry)
				{
					UpdateSubbMonitorData(i);
					container.processManager.SendMessage(_message[i]);
				}
			}
			MusicData music = Singleton<DataManager>.Instance.GetMusic(GameManager.SelectMusicID[0]);
			SoundManager.MusicPrepare(music.cueName.id);
			_gameMovie = UnityEngine.Object.Instantiate(CommonPrefab.GetMovieCtrlObject());
			_gameMovie.SetMusicMovie(music.movieName.id);
			List<Jvs.LedPwmFadeParam> list = new List<Jvs.LedPwmFadeParam>();
			List<Color> list2 = new List<Color>
			{
				Color.red,
				new Color32(239, 129, 15, byte.MaxValue),
				Color.yellow,
				Color.green,
				Color.cyan,
				Color.blue,
				new Color32(167, 87, 168, byte.MaxValue)
			};
			for (int j = 0; j < list2.Count; j++)
			{
				list.Add(new Jvs.LedPwmFadeParam
				{
					StartFadeColor = list2[j],
					EndFadeColor = list2[(j + 1) % list2.Count],
					FadeTime = 1000L,
					NextIndex = (j + 1) % list2.Count
				});
			}
			List<long> photoTiming = new List<long>();
			SetPhotoTiming(ref photoTiming);
			_photoManager.Initialize(photoTiming);
			if (GameManager.IsPhotoAgree)
			{
				WebCamManager.PlayOnly();
			}
			if (IsPartyPlay())
			{
				_clientInfo.IpAddress = PartyLink.Util.MyIpAddress().ToNetworkByteOrderU32();
				_clientInfo.Count = 0;
				for (int k = 0; k < _monitors.Length; k++)
				{
					bool isEntry2 = Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry;
					_clientInfo.IsValids[k] = isEntry2;
					_clientInfo.Achieves[k] = 0;
					_clientInfo.Combos[k] = 0;
					_clientInfo.MissNums[k] = 0;
					_clientInfo.Miss[k] = false;
					_clientInfo.GameOvers[k] = false;
					_clientInfo.FullCombos[k] = 0;
					_lastCombo[k] = 0;
					_lastMiss[k] = 0;
				}
				_clientSendCount = 0;
			}
			GameManager.DebugAchievement = 0;
			GameManager.AutoPlay = GameManager.AutoPlayMode.None;
			if (GameManager.IsNoteCheckMode)
			{
				GameManager.AutoPlay = GameManager.AutoPlayMode.Critical;
			}
			GameManager.IsInGame = true;
		}

		public void UpdateNotes()
		{
			NotesManager.UpdateTimer();
			for (int i = 0; i < 4; i++)
			{
				NotesManager.Instance(i).updateNotes();
			}
		}

		public override void OnUpdate()
		{
			Singleton<GamePlayManager>.Instance.PlayHeadUpdate();
			switch (_sequence)
			{
			case GameSequence.Init:
			{
				if (!SoundManager.IsMusicPrepare() || !_gameMovie.IsMoviePrepare() || !_monitors[0].IsReady() || !_monitors[1].IsReady())
				{
					break;
				}
				container.processManager.NotificationFadeIn();
				_sequence = GameSequence.Sync;
				_trackSkipWait.Reset();
				_trackSkipWait.Stop();
				for (int num6 = 0; num6 < _monitors.Length; num6++)
				{
					bool isEntry = Singleton<UserDataManager>.Instance.GetUserData(num6).IsEntry;
					_monitors[num6].SetMovieSize(_gameMovie.GetMovieHeight(), _gameMovie.GetMovieWidth());
					if (isEntry)
					{
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20001, num6, isEntry, Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() == 1));
						UpdateSubbMonitorData(num6);
						container.processManager.SendMessage(_message[num6]);
					}
				}
				if (IsPartyPlay())
				{
					Manager.Party.Party.Party.Get().Ready();
				}
				break;
			}
			case GameSequence.Sync:
				if (!IsPartyPlay())
				{
					_sequence = GameSequence.Start;
				}
				else if (Manager.Party.Party.Party.Get().IsPlay())
				{
					_sequence = GameSequence.Start;
				}
				if (GameSequence.Start == _sequence)
				{
					_startWait.Reset();
					_startWait.Start();
				}
				break;
			case GameSequence.Start:
				if (_startWait.ElapsedMilliseconds > 1000)
				{
					_sequence = GameSequence.StartWait;
					NotesManager.StartPlay(0f);
					GameMonitor[] monitors = _monitors;
					for (int i = 0; i < monitors.Length; i++)
					{
						monitors[i].PlayStart();
					}
					_startWait.Reset();
					_startWait.Start();
				}
				break;
			case GameSequence.StartWait:
				UpdateNotes();
				if (_startWait.ElapsedMilliseconds > 91)
				{
					_sequence = GameSequence.Play;
					_gameMovie.Play();
					SoundManager.StartMusic();
					_startMusic = true;
				}
				break;
			case GameSequence.Play:
			{
				UpdateNotes();
				if (IsPartyPlay())
				{
					foreach (MemberPlayInfo item in Manager.Party.Party.Party.Get().GetPartyPlayInfo().GetJoinMembersWithoutMe())
					{
						for (int num7 = 0; num7 < 2; num7++)
						{
							int index2 = num7 + 2;
							int chain2 = 0;
							int num8 = 0;
							ChainHistory[] chainHistory2 = Manager.Party.Party.Party.Get().GetPartyPlayInfo().ChainHistory;
							for (int num9 = 0; num9 < chainHistory2.Length; num9++)
							{
								if (chainHistory2[num9].PacketNo != 0)
								{
									chain2 = chainHistory2[num9].Chain;
									if (num8 < chainHistory2[num9].Chain)
									{
										num8 = chainHistory2[num9].Chain;
									}
								}
							}
							Singleton<GamePlayManager>.Instance.GetGameScore(index2).OverridePartyScore(item.Achieves[num7], item.Rankings[num7], chain2, num8, item.FullCombos[num7], item.GameOvers[num7]);
						}
					}
				}
				for (int num10 = 0; num10 < _monitors.Length; num10++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(num10).IsEntry)
					{
						UpdateSubbMonitorData(num10);
						if (_playDatas[num10] != _prevPlayData[num10])
						{
							container.processManager.SendMessage(_message[num10]);
						}
						_prevPlayData[num10].SetData(_playDatas[num10]);
					}
				}
				GameMonitor[] monitors4;
				if (GameManager.IsCourseMode)
				{
					bool flag2 = true;
					for (int num11 = 0; num11 < _monitors.Length; num11++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(num11).IsEntry && Singleton<GamePlayManager>.Instance.GetGameScore(num11).Life != 0)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						monitors4 = _monitors;
						for (int num12 = 0; num12 < monitors4.Length; num12++)
						{
							monitors4[num12].SetForceShutter();
						}
					}
				}
				bool flag3 = true;
				monitors4 = _monitors;
				for (int num13 = 0; num13 < monitors4.Length; num13++)
				{
					if (!monitors4[num13].IsAllJudged())
					{
						flag3 = false;
					}
				}
				if (!flag3)
				{
					flag3 = IsTrackSkip();
				}
				if (IsPartyPlay())
				{
					int num14 = Mathf.FloorToInt(NotesManager.GetCurrentMsec()) / PartyLink.Party.c_clientSendInfoIntervalMSec;
					if (_clientSendCount < num14)
					{
						UpdateClientPlayInfo((uint)num14);
						Manager.Party.Party.Party.Get().SendClientPlayInfo(_clientInfo);
						_clientSendCount = num14;
					}
				}
				if (flag3)
				{
					if (IsPartyPlay())
					{
						int dataCount = Mathf.FloorToInt(NotesManager.GetCurrentMsec()) / PartyLink.Party.c_clientSendInfoIntervalMSec;
						UpdateClientPlayInfo((uint)dataCount);
						Manager.Party.Party.Party.Get().SendClientPlayInfo(_clientInfo);
						Manager.Party.Party.Party.Get().FinishPlay();
					}
					_sequence = GameSequence.PlayEnd;
				}
				break;
			}
			case GameSequence.PlayEnd:
			{
				UpdateNotes();
				if (!IsPartyPlay())
				{
					for (int j = 0; j < _monitors.Length; j++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
						{
							Singleton<GamePlayManager>.Instance.GetGameScore(j).CalcUpdate();
						}
					}
					for (int k = 0; k < _monitors.Length; k++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
						{
							Singleton<GamePlayManager>.Instance.SetSyncResult(k);
						}
					}
					_sequence = GameSequence.Result;
				}
				else
				{
					foreach (MemberPlayInfo item2 in Manager.Party.Party.Party.Get().GetPartyPlayInfo().GetJoinMembersWithoutMe())
					{
						for (int l = 0; l < 2; l++)
						{
							int index = l + 2;
							int chain = 0;
							int num = 0;
							ChainHistory[] chainHistory = Manager.Party.Party.Party.Get().GetPartyPlayInfo().ChainHistory;
							for (int m = 0; m < chainHistory.Length; m++)
							{
								if (chainHistory[m].PacketNo != 0)
								{
									chain = chainHistory[m].Chain;
									if (num < chainHistory[m].Chain)
									{
										num = chainHistory[m].Chain;
									}
								}
							}
							Singleton<GamePlayManager>.Instance.GetGameScore(index).OverridePartyScore(item2.Achieves[l], item2.Rankings[l], chain, num, item2.FullCombos[l], item2.GameOvers[l]);
						}
					}
					if (Manager.Party.Party.Party.Get().IsNews())
					{
						for (int n = 0; n < _monitors.Length; n++)
						{
							if (Singleton<UserDataManager>.Instance.GetUserData(n).IsEntry)
							{
								Singleton<GamePlayManager>.Instance.GetGameScore(n).CalcUpdate();
							}
						}
						for (int num2 = 0; num2 < _monitors.Length; num2++)
						{
							if (Singleton<UserDataManager>.Instance.GetUserData(num2).IsEntry)
							{
								Singleton<GamePlayManager>.Instance.SetSyncResult(num2);
							}
						}
						_sequence = GameSequence.Result;
					}
				}
				for (int num3 = 0; num3 < _monitors.Length; num3++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(num3).IsEntry)
					{
						UpdateSubbMonitorData(num3);
						if (_playDatas[num3] != _prevPlayData[num3])
						{
							container.processManager.SendMessage(_message[num3]);
						}
						_prevPlayData[num3].SetData(_playDatas[num3]);
					}
				}
				if (GameSequence.Result != _sequence)
				{
					break;
				}
				_endWait.Reset();
				_endWait.Start();
				bool flag = false;
				GameMonitor[] monitors2 = _monitors;
				foreach (GameMonitor gameMonitor in monitors2)
				{
					flag |= gameMonitor.PlayResult();
				}
				if (GameManager.IsPhotoAgree && !IsTrackSkip())
				{
					PhotoTiming value = _photoManager._pictureTimingList[0];
					value.TakeTime = GetNowNoteTime() + WaitLastShotTiming;
					_photoManager._pictureTimingList[0] = value;
					WebCamManager.Play();
					GameMonitor[] monitors3 = _monitors;
					for (int num5 = 0; num5 < monitors3.Length; num5++)
					{
						monitors3[num5].PlayPhoto();
					}
				}
				break;
			}
			case GameSequence.Result:
				UpdateNotes();
				if (_monitors[0].IsPlayEnd() && _monitors[1].IsPlayEnd())
				{
					if (!IsPartyPlay())
					{
						_sequence = GameSequence.ResultEnd;
						break;
					}
					_sequence = GameSequence.ResultEnd;
					Manager.Party.Party.Party.Get().FinishNews(gaugeClear0: true, 0u, gaugeClear1: true, 0u);
				}
				break;
			case GameSequence.ResultEnd:
				UpdateNotes();
				if (!IsPartyPlay())
				{
					_sequence = GameSequence.FinalWait;
				}
				else if (Manager.Party.Party.Party.Get().IsResult())
				{
					_sequence = GameSequence.FinalWait;
				}
				break;
			case GameSequence.FinalWait:
				UpdateNotes();
				break;
			}
			bool flag4 = false;
			if (((_startMusic && SoundManager.IsEndMusic() && _gameMovie.IsEnd() && _monitors[0].IsPlayEnd() && _monitors[1].IsPlayEnd() && _sequence != GameSequence.Release && _endWait.ElapsedMilliseconds > 6000) || IsTrackSkipWaitEnd()) && _sequence == GameSequence.FinalWait)
			{
				SetRelease();
				flag4 = true;
			}
			if (flag4)
			{
				bool[] array = new bool[_monitors.Length];
				uint[] array2 = new uint[_monitors.Length];
				for (int num15 = 0; num15 < _monitors.Length; num15++)
				{
					array[num15] = Singleton<GamePlayManager>.Instance.GetGameScore(num15).IsChallenge;
					array2[num15] = Singleton<GamePlayManager>.Instance.GetGameScore(num15).Life;
				}
				for (int num16 = 0; num16 < _monitors.Length; num16++)
				{
					if (!Singleton<UserDataManager>.Instance.GetUserData(num16).IsEntry)
					{
						continue;
					}
					Singleton<GamePlayManager>.Instance.GetGameScore(num16).IsChallenge = array[num16];
					Singleton<GamePlayManager>.Instance.GetGameScore(num16).SetLife(array2[num16]);
					int num17 = GameManager.SelectMusicID[num16];
					if (num17 > 0)
					{
						NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[num17];
						for (int num18 = 0; num18 < notesWrapper.ChallengeDetail.Length; num18++)
						{
							if (notesWrapper.ChallengeDetail[num18].isEnable && notesWrapper.ChallengeDetail[num18].startLife > 0)
							{
								array2[num16] = (uint)notesWrapper.ChallengeDetail[num18].startLife;
								break;
							}
						}
					}
					else
					{
						array2[num16] = 0u;
						Singleton<GamePlayManager>.Instance.GetGameScore(num16).SetLife(array2[num16]);
					}
					uint criticalNum = Singleton<GamePlayManager>.Instance.GetGameScore(num16).CriticalNum;
					uint perfectNum = Singleton<GamePlayManager>.Instance.GetGameScore(num16).PerfectNum;
					uint greatNum = Singleton<GamePlayManager>.Instance.GetGameScore(num16).GreatNum;
					uint goodNum = Singleton<GamePlayManager>.Instance.GetGameScore(num16).GoodNum;
					uint missNum = Singleton<GamePlayManager>.Instance.GetGameScore(num16).MissNum;
					uint num19 = 1u;
					uint num20 = (greatNum + goodNum + missNum) * num19;
					uint num21 = criticalNum + perfectNum + greatNum + goodNum + missNum;
					if (num20 != 0)
					{
						if (num20 > array2[num16])
						{
							array2[num16] = 0u;
						}
						else
						{
							array2[num16] -= num20;
						}
					}
					if (array2[num16] != 0 && num21 == 0)
					{
						array2[num16] = 0u;
						Singleton<GamePlayManager>.Instance.GetGameScore(num16).SetLife(array2[num16]);
					}
				}
			}
			if (_sequence >= GameSequence.Play && _sequence < GameSequence.Release && !GameManager.IsNoteCheckMode)
			{
				bool flag5 = false;
				if (DebugInput.GetKeyDown(KeyCode.Space))
				{
					flag5 = true;
				}
				else if (DebugInput.GetKeyDown(KeyCode.T) || DebugInput.GetKeyDown(KeyCode.Y) || DebugInput.GetKeyDown(KeyCode.U) || DebugInput.GetKeyDown(KeyCode.I))
				{
					flag5 = true;
					GameMonitor[] monitors5 = _monitors;
					for (int num22 = 0; num22 < monitors5.Length; num22++)
					{
						monitors5[num22].Seek(0);
					}
					debugFeature.SetSkip(debugFeature._debugPause, 0f);
					Singleton<GamePlayManager>.Instance.Initialize(IsPartyPlay());
					uint num23 = 0u;
					for (int num24 = 0; num24 < _monitors.Length; num24++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(num24).IsEntry)
						{
							float num25 = 0f;
							if (DebugInput.GetKey(KeyCode.LeftShift) || DebugInput.GetKey(KeyCode.RightShift))
							{
								num25 = -1f;
							}
							if (DebugInput.GetKey(KeyCode.LeftControl) || DebugInput.GetKey(KeyCode.RightControl))
							{
								num25 = 1f;
							}
							if (DebugInput.GetKeyDown(KeyCode.T))
							{
								_monitors[num24].ForceAchivement((int)(50f + num25), 0);
							}
							if (DebugInput.GetKeyDown(KeyCode.Y))
							{
								_monitors[num24].ForceAchivement((int)(80f + num25), 0);
							}
							if (DebugInput.GetKeyDown(KeyCode.U))
							{
								_monitors[num24].ForceAchivement((int)(97f + num25), 0);
							}
							if (DebugInput.GetKeyDown(KeyCode.I))
							{
								_monitors[num24].ForceAchivement((int)(100f + num25), 0);
							}
							num23 += Singleton<GamePlayManager>.Instance.GetGameScore(num24).MaxCombo;
						}
					}
					GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(2);
					if (gameScore.IsEnable && !gameScore.IsHuman())
					{
						for (int num26 = 0; num26 < 2; num26++)
						{
							if (Singleton<UserDataManager>.Instance.GetUserData(num26).IsEntry && GameManager.SelectGhostID[num26] != GhostManager.GhostTarget.End)
							{
								UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(GameManager.SelectGhostID[num26]);
								gameScore.SetForceAchivement_Battle((float)GameManager.ConvAchiveIntToDecimal(ghostToEnum.Achievement));
								break;
							}
						}
					}
					for (int num27 = 0; num27 < _monitors.Length; num27++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(num27).IsEntry)
						{
							Singleton<GamePlayManager>.Instance.GetGameScore(num27).SetChain(num23);
						}
					}
				}
				if (flag5)
				{
					for (int num28 = 0; num28 < _monitors.Length; num28++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(num28).IsEntry)
						{
							UpdateSubbMonitorData(num28);
							container.processManager.SendMessage(_message[num28]);
							Singleton<GamePlayManager>.Instance.SetSyncResult(num28);
						}
					}
					SetRelease();
				}
			}
			if (_sequence == GameSequence.Play)
			{
				if (!debugFeature._debugPause)
				{
					debugFeature._debugTimer += GameManager.GetGameMSecAddD();
				}
				if (DebugInput.GetKeyDown(KeyCode.Home))
				{
					GameManager.AutoPlay = (GameManager.AutoPlayMode)((int)(GameManager.AutoPlay + 1) % Enum.GetNames(typeof(GameManager.AutoPlayMode)).Length);
				}
				else if (DebugInput.GetKeyDown(KeyCode.Return))
				{
					if (_sequence == GameSequence.Play)
					{
						debugFeature._debugPause = !debugFeature._debugPause;
						if (debugFeature._debugPause)
						{
							SoundManager.PauseMusic(debugFeature._debugPause);
							_gameMovie.Pause(debugFeature._debugPause);
							debugFeature.SetPause(debugFeature._debugPause);
						}
						else
						{
							debugFeature.DebugTimeSkip(0);
						}
					}
				}
				else if (DebugInput.GetKeyDown(KeyCode.LeftArrow) || DebugInput.GetKeyDown(KeyCode.RightArrow))
				{
					int num29 = 0;
					if (DebugInput.GetKeyDown(KeyCode.LeftArrow))
					{
						num29 = -1000;
					}
					if (DebugInput.GetKeyDown(KeyCode.RightArrow))
					{
						num29 = 1000;
					}
					int addMsec = ((!DebugInput.GetKey(KeyCode.LeftShift) && !DebugInput.GetKey(KeyCode.RightShift)) ? ((!DebugInput.GetKey(KeyCode.LeftControl) && !DebugInput.GetKey(KeyCode.RightControl)) ? num29 : (num29 * 10)) : (num29 * 5));
					Singleton<GamePlayManager>.Instance.Initialize(IsPartyPlay());
					debugFeature.DebugTimeSkip(addMsec);
				}
				else if (DebugInput.GetKeyDown(KeyCode.Insert))
				{
					Singleton<GamePlayManager>.Instance.Initialize(IsPartyPlay());
					debugFeature.DebugTimeSkip(-2147483647);
				}
				else if (DebugInput.GetKeyDown(KeyCode.PageUp))
				{
					debugFeature._debugRepeatTimer[0] = (int)debugFeature._debugTimer;
				}
				else if (DebugInput.GetKeyDown(KeyCode.PageDown))
				{
					if (debugFeature._debugRepeatTimer[0] < (int)debugFeature._debugTimer)
					{
						debugFeature._debugRepeatTimer[1] = (int)debugFeature._debugTimer;
					}
				}
				else if (DebugInput.GetKeyDown(KeyCode.End))
				{
					debugFeature._debugRepeatTimer[0] = -1;
					debugFeature._debugRepeatTimer[1] = -1;
				}
				else if (DebugInput.GetKeyDown(KeyCode.Delete))
				{
					debugFeature._debugSlow = !debugFeature._debugSlow;
					if (debugFeature._debugSlow)
					{
						debugFeature._debugSlowCounter = 0;
					}
					else
					{
						debugFeature._debugPause = true;
						SoundManager.PauseMusic(debugFeature._debugPause);
						_gameMovie.Pause(debugFeature._debugPause);
						debugFeature.SetPause(debugFeature._debugPause);
					}
				}
				else if (debugFeature._debugPause)
				{
					if (DebugInput.GetKey(KeyCode.UpArrow))
					{
						debugFeature.DebugTimeSkip(-16);
					}
					else if (DebugInput.GetKey(KeyCode.DownArrow))
					{
						debugFeature.DebugTimeSkip(16);
					}
				}
			}
			for (int num30 = 0; num30 < _monitors.Length; num30++)
			{
				if (Singleton<GamePlayManager>.Instance.GetGameScore(num30).IsTrackSkip)
				{
					for (int num31 = 0; num31 < 35; num31++)
					{
						InputManager.SetUsedThisFrame(num30, (InputManager.TouchPanelArea)num31);
					}
				}
				_monitors[num30].ViewUpdate();
				if (Singleton<UserDataManager>.Instance.GetUserData(num30).IsEntry && _skipPhase[num30] != _monitors[num30].GetPushPhase())
				{
					_skipPhase[num30] = _monitors[num30].GetPushPhase();
					switch (_skipPhase[num30])
					{
					case -1:
						container.processManager.ForcedCloseWindow(num30);
						break;
					case 0:
						container.processManager.EnqueueMessage(num30, WindowMessageID.TrackSkip3Second, WindowPositionID.Middle);
						break;
					case 1:
						container.processManager.EnqueueMessage(num30, WindowMessageID.TrackSkip2Second, WindowPositionID.Middle);
						break;
					case 2:
						container.processManager.EnqueueMessage(num30, WindowMessageID.TrackSkip1Second, WindowPositionID.Middle);
						break;
					case 3:
						container.processManager.CloseWindow(num30);
						break;
					}
				}
			}
			_photoManager.Update(GetNowNoteTime());
			Singleton<GamePlayManager>.Instance.PlayLastUpdate();
		}

		private void SetRelease()
		{
			GameManager.IsInGame = false;
			container.processManager.AddProcess(new FadeProcess(container, this, new ResultProcess(container), FadeProcess.FadeType.Type3), 50);
			SoundManager.StopMusic();
			_sequence = GameSequence.Release;
			_gameMovie.Stop();
			NotesManager.StopPlay();
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20050, false));
			GameMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].PlayStop();
			}
			if (IsPartyPlay())
			{
				Manager.Party.Party.Party.Get().Wait();
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				Singleton<GamePlayManager>.Instance.GetGameScore(j).FinishPlay();
			}
			if (Singleton<GamePlayManager>.Instance.IsGhostFlag())
			{
				Singleton<GamePlayManager>.Instance.GetGhostScore().FinishPlayGhost();
			}
			for (int k = 0; k < _monitors.Length; k++)
			{
				Singleton<CourseManager>.Instance.SetLifeCalc(k, Singleton<GamePlayManager>.Instance.GetGameScore(k));
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			if (Singleton<SystemConfig>.Instance.config.IsMovieSupervision)
			{
				Singleton<GamePlayManager>.Instance.ClaerLog();
			}
			if (GameManager.IsPhotoAgree)
			{
				WebCamManager.Pause();
				WebCamManager.OutputPhotoData();
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20001, i, false, false));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50004, i, true));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50003, i, 0));
			}
			UnityEngine.Object.Destroy(_gameMovie?.gameObject);
			_gameMovie = null;
			GameMonitor[] monitors = _monitors;
			for (int j = 0; j < monitors.Length; j++)
			{
				UnityEngine.Object.Destroy(monitors[j].gameObject);
			}
			GC.Collect();
		}

		private long GetNowNoteTime()
		{
			long result = 0L;
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					float currentMsec = NotesManager.GetCurrentMsec();
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && currentMsec > 0f)
					{
						result = (long)currentMsec;
					}
				}
			}
			return result;
		}

		private void SetPhotoTiming(ref List<long> photoTiming)
		{
			photoTiming = new List<long> { 0L };
			photoTiming[0] = long.MaxValue;
		}

		private bool IsTrackSkip()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && !Singleton<GamePlayManager>.Instance.GetGameScore(i).IsTrackSkip)
				{
					return false;
				}
			}
			if (IsPartyPlay())
			{
				foreach (MemberPlayInfo item in Manager.Party.Party.Party.Get().GetPartyPlayInfo().GetJoinMembersWithoutMe())
				{
					_ = item;
					for (int j = 0; j < 2; j++)
					{
						int num = j + 2;
						if (Singleton<UserDataManager>.Instance.GetUserData(num).IsEntry && !Singleton<GamePlayManager>.Instance.GetGameScore(num).IsTrackSkip)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private bool IsTrackSkipWaitEnd()
		{
			if (!IsTrackSkip())
			{
				return false;
			}
			if (!_trackSkipWait.IsRunning)
			{
				_trackSkipWait.Restart();
				_trackSkipWait.Start();
			}
			if (_trackSkipWait.ElapsedMilliseconds > 4500)
			{
				return true;
			}
			return false;
		}

		private void UpdateSubbMonitorData(int i)
		{
			bool flag = Singleton<GamePlayManager>.Instance.GetGameScore(i).UserOption.SubmonitorAchive == OptionSubmonAchiveID.AchiveMinus || GameManager.IsEventMode;
			_playDatas[i].SetData((!flag) ? Singleton<GamePlayManager>.Instance.GetAchivement(i) : Singleton<GamePlayManager>.Instance.GetDecTheoryAchivement(i), Singleton<GamePlayManager>.Instance.GetVsRank(i), Singleton<GamePlayManager>.Instance.GetMaxChainCount(i), GameManager.IsEventMode ? ((int)(Singleton<GamePlayManager>.Instance.GetVsAchieve(i) * 10000m)) : 0);
		}

		private bool IsPartyPlay()
		{
			IManager manager = Manager.Party.Party.Party.Get();
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID != MachineGroupID.OFF && manager != null)
			{
				return manager.IsJoinAndActive();
			}
			return false;
		}

		private void UpdateClientPlayInfo(uint dataCount)
		{
			_clientInfo.Count = (int)dataCount;
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(i);
					_clientInfo.Achieves[i] = (int)Singleton<GamePlayManager>.Instance.GetAchivement(i);
					if (_lastMiss[i] == (int)gameScore.MissNum)
					{
						_clientInfo.Miss[i] = false;
						_clientInfo.Combos[i] = (int)Singleton<GamePlayManager>.Instance.GetCombo(i) - _lastCombo[i];
					}
					else
					{
						_clientInfo.Miss[i] = true;
						_clientInfo.Combos[i] = (int)Singleton<GamePlayManager>.Instance.GetCombo(i);
					}
					_lastCombo[i] = (int)Singleton<GamePlayManager>.Instance.GetCombo(i);
					_lastMiss[i] = (int)gameScore.MissNum;
					_clientInfo.MissNums[i] = (int)gameScore.MissNum;
					_clientInfo.FullCombos[i] = (int)Singleton<GamePlayManager>.Instance.GetComboType(i);
					_clientInfo.GameOvers[i] = gameScore.IsTrackSkip;
				}
			}
		}
	}
}
