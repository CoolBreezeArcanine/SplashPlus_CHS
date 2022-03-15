using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.Achieve;
using Manager.MaiStudio;
using Monitor;
using Process.CourseResult;
using UnityEngine;
using Util;

namespace Process
{
	public class NextTrackProcess : FadeProcess
	{
		public enum NextTrackMode
		{
			NextTrack,
			CourseNextTrack,
			FreedomTimeup,
			NeedAwake,
			GotoEnd
		}

		private NextTrackMode _mode;

		private NextTrackMonitor[] _monitors;

		private float _nextTrackWaitTime = 2f;

		private float _freedomWaitTime = 4f;

		private float _timeCounter;

		private bool isAddAnim;

		private bool _isCallCheckAchieveCreditTotal;

		public NextTrackProcess(ProcessDataContainer dataContainer, ProcessBase from)
			: base(dataContainer, from)
		{
			fromProcess = from;
			toProcess = null;
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			if (GameManager.IsFreedomMode)
			{
				GameManager.PauseFreedomModeTimer(isPause: true);
			}
			if (GameManager.IsFreedomMode)
			{
				fadeType = ((!GameManager.IsFreedomTimeUp) ? FadeType.Type1 : FadeType.Type3);
			}
			else
			{
				fadeType = ((!GameManager.IsFinalTrack(GameManager.MusicTrackNumber + 1)) ? FadeType.Type1 : FadeType.Type3);
			}
			CheckAchieveTrack();
			GameManager.UpdateRandom();
			GameObject prefs = Resources.Load<GameObject>("Process/NextTrack/NextTrackProcess");
			_monitors = new NextTrackMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<NextTrackMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<NextTrackMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				fadeObject[i] = _monitors[i].gameObject;
			}
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			gameObject = Resources.Load<GameObject>("Process/ChangeScreen/Prefabs/ChangeScreen_0" + (int)fadeType);
			gameObject2 = Resources.Load<GameObject>("Process/ChangeScreen/Prefabs/Sub_ChangeScreen");
			for (int j = 0; j < 2; j++)
			{
				canvasObject[j, 0] = CreateInstanceAndSetParent(gameObject, _monitors[j].MainFadeRoot.transform);
				canvasObject[j, 1] = CreateInstanceAndSetParent(gameObject2, fadeObject[j].transform.Find("Canvas/Sub"));
				for (int k = 0; k < 2; k++)
				{
					animator[j, k] = canvasObject[j, k].GetComponent<Animator>();
					animator[j, k].Play("In");
				}
			}
			isFadeing = true;
			isAddAnim = false;
			for (int l = 0; l < _monitors.Length; l++)
			{
				_monitors[l].Initialize(l, Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry);
				SoundManager.StopBGM(l);
			}
			for (int m = 0; m < 2; m++)
			{
				GameManager.SelectGhostID[m] = GhostManager.GhostTarget.End;
			}
			for (int n = 2; n < 4; n++)
			{
				Singleton<UserDataManager>.Instance.GetUserData(n).Initialize();
				Singleton<UserDataManager>.Instance.SetDefault(n);
			}
			_mode = NextTrackMode.NextTrack;
			if (GameManager.IsFreedomMode)
			{
				if (GameManager.IsFreedomTimeUp)
				{
					_mode = NextTrackMode.FreedomTimeup;
					NextTrackMonitor[] monitors = _monitors;
					for (int num = 0; num < monitors.Length; num++)
					{
						monitors[num].SetFreedomTimeOut();
					}
					bool flag = false;
					for (int num2 = 0; num2 < _monitors.Length; num2++)
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(num2);
						if (userData.IsEntry && !userData.IsGuest() && Singleton<MapMaster>.Instance.IsCallAwake != null && Singleton<MapMaster>.Instance.IsNeedAwake != null && !Singleton<MapMaster>.Instance.IsCallAwake[num2] && !Singleton<MapMaster>.Instance.IsNeedAwake[num2])
						{
							Singleton<MapMaster>.Instance.IsNeedAwake[num2] = true;
							flag = true;
						}
					}
					if (flag)
					{
						_mode = NextTrackMode.NeedAwake;
					}
				}
			}
			else if (GameManager.IsCourseMode)
			{
				if (Singleton<CourseManager>.Instance.IsGameOver())
				{
					_mode = NextTrackMode.GotoEnd;
					NextTrackMonitor[] monitors = _monitors;
					for (int num = 0; num < monitors.Length; num++)
					{
						monitors[num].SetGoToEnd();
					}
				}
				else
				{
					_mode = NextTrackMode.CourseNextTrack;
					NextTrackMonitor[] monitors = _monitors;
					for (int num = 0; num < monitors.Length; num++)
					{
						monitors[num].SetGoToEnd();
					}
				}
			}
			else if (GameManager.IsGotoGameOver())
			{
				_mode = NextTrackMode.GotoEnd;
				NextTrackMonitor[] monitors = _monitors;
				for (int num = 0; num < monitors.Length; num++)
				{
					monitors[num].SetGoToEnd();
				}
			}
			if (_mode == NextTrackMode.NextTrack)
			{
				NextTrackMonitor[] monitors = _monitors;
				for (int num = 0; num < monitors.Length; num++)
				{
					monitors[num].SetNextTrack(GameManager.MusicTrackNumber + 1);
				}
				if (GameManager.NextMapSelect && !GameManager.IsFreedomMapSkip())
				{
					toProcess = new RegionalSelectProcess(container);
				}
				else if (GameManager.NextCharaSelect && !GameManager.IsFreedomMapSkip())
				{
					bool flag2 = true;
					for (int num3 = 0; num3 < _monitors.Length; num3++)
					{
						UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(num3);
						if (userData2.IsEntry)
						{
							flag2 = !userData2.Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCharaSelect);
							if (!flag2)
							{
								break;
							}
						}
					}
					toProcess = new CharacterSelectProces(container, flag2);
				}
				else
				{
					toProcess = new MusicSelectProcess(container);
				}
			}
			else if (_mode == NextTrackMode.CourseNextTrack)
			{
				toProcess = new MusicSelectProcess(container);
			}
			else if (_mode == NextTrackMode.NeedAwake)
			{
				toProcess = null;
			}
			else if (GameManager.IsCourseMode)
			{
				toProcess = new CourseResultProcess(container);
			}
			else if (Singleton<GamePlayManager>.Instance.IsPlayLog())
			{
				toProcess = new PhotoEditProcess(container);
			}
			else if (GameManager.IsEventMode)
			{
				toProcess = new DataSaveProcess(container);
			}
			else
			{
				toProcess = new UnlockMusicProcess(container);
			}
			GameManager.NextMapSelect = false;
			GameManager.NextCharaSelect = false;
		}

		public override void OnLateUpdate()
		{
			if (isFadeing && animator[0, 0].GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				if (state == FadeState.DoFadeOut)
				{
					ProcessingProcess();
					return;
				}
				container.processManager.ReleaseProcess(this);
				isFadeing = false;
			}
		}

		public override void ProcessingProcess()
		{
			if (isAddProcess)
			{
				return;
			}
			if (!isAddAnim)
			{
				switch (_mode)
				{
				case NextTrackMode.NextTrack:
				{
					_timeCounter = _nextTrackWaitTime;
					if (GameManager.IsFreedomMode && GameManager.IsFreedomMapSkip())
					{
						container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20003, true));
						if (!GameManager.IsFreedomTimeUp)
						{
							container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20002, true));
						}
					}
					for (int j = 0; j < _monitors.Length; j++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
						{
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_NEXT_TRACK, j);
							if (GameManager.IsFreedomMode)
							{
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000151, j);
							}
							else if (GameManager.IsFinalTrack(GameManager.MusicTrackNumber + 1))
							{
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000153, j);
							}
							else
							{
								SoundManager.PlayPartnerVoice(Singleton<UserDataManager>.Instance.IsSingleUser() ? Mai2.Voice_Partner_000001.Cue.VO_000151 : Mai2.Voice_Partner_000001.Cue.VO_000152, j);
							}
						}
					}
					break;
				}
				case NextTrackMode.CourseNextTrack:
					_timeCounter = 0f;
					break;
				case NextTrackMode.FreedomTimeup:
				{
					_timeCounter = _freedomWaitTime;
					int num = ((!Singleton<UserDataManager>.Instance.GetUserData(0L).IsEntry) ? 1 : 0);
					_monitors[num].PlayAnimIn();
					for (int k = 0; k < _monitors.Length; k++)
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_FREEDOM_TIMEUP, k);
					}
					container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20001));
					if (!_isCallCheckAchieveCreditTotal)
					{
						CheckAchieveCreditTotal();
						_isCallCheckAchieveCreditTotal = true;
					}
					break;
				}
				case NextTrackMode.NeedAwake:
					_timeCounter = 0f;
					break;
				case NextTrackMode.GotoEnd:
				{
					_timeCounter = 0f;
					if (!_isCallCheckAchieveCreditTotal)
					{
						CheckAchieveCreditTotal();
						_isCallCheckAchieveCreditTotal = true;
					}
					for (int i = 0; i < _monitors.Length; i++)
					{
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20020, i, false, false));
					}
					break;
				}
				}
				if (_mode == NextTrackMode.NextTrack || _mode == NextTrackMode.CourseNextTrack)
				{
					GameManager.MusicTrackNumber++;
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20031));
				}
				if (_mode == NextTrackMode.NeedAwake)
				{
					if (fromProcess != null)
					{
						container.processManager.ReleaseProcess(fromProcess);
					}
					toProcess = null;
					container.processManager.AddProcess(new MapResultProcess(container), 20);
				}
				else
				{
					NextTrackMonitor[] monitors = _monitors;
					for (int l = 0; l < monitors.Length; l++)
					{
						monitors[l].PlayAnimIn();
					}
				}
				isAddAnim = true;
				return;
			}
			_timeCounter -= Time.deltaTime;
			if (_timeCounter <= 0f)
			{
				if (fromProcess != null)
				{
					container.processManager.ReleaseProcess(fromProcess);
				}
				if (toProcess != null)
				{
					container.processManager.AddProcess(toProcess, 20);
				}
				isAddProcess = true;
				isFadeing = false;
			}
		}

		public override void StartFadeIn()
		{
			state = FadeState.DoFadeIn;
			isFadeing = true;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					if (animator[i, j] != null)
					{
						animator[i, j].Play("Out");
					}
				}
				_monitors[i].PlayAnimOut();
			}
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i]?.Release();
				if (_monitors[i] != null)
				{
					Object.Destroy(_monitors[i].gameObject);
				}
				if (fadeObject[i] != null)
				{
					Object.Destroy(fadeObject[i]);
				}
			}
		}

		private void CheckAchieveTrack()
		{
			CheckAchieve(ReleaseConditionCategory.Track);
		}

		private void CheckAchieveCreditTotal()
		{
			CheckAchieve(ReleaseConditionCategory.Credit);
			CheckAchieve(ReleaseConditionCategory.Total);
			CheckTicketGetData();
		}

		private void CheckAchieve(ReleaseConditionCategory type)
		{
			if (GameManager.IsEventMode)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (userData.IsEntry && !userData.IsGuest())
				{
					Singleton<CollectionAchieve>.Instance.BuildParameter(i, type);
					int[] array = Singleton<CollectionAchieve>.Instance.CheckNewPlate(type);
					int[] array2 = Singleton<CollectionAchieve>.Instance.CheckNewTitle(type);
					int[] array3 = Singleton<CollectionAchieve>.Instance.CheckNewIcon(type);
					int[] array4 = Singleton<CollectionAchieve>.Instance.CheckNewPartner(type);
					int[] array5 = Singleton<CollectionAchieve>.Instance.CheckNewFrame(type);
					int[] array6 = array;
					foreach (int id in array6)
					{
						userData.AddCollections(UserData.Collection.Plate, id, addNewGet: true);
					}
					array6 = array2;
					foreach (int id2 in array6)
					{
						userData.AddCollections(UserData.Collection.Title, id2, addNewGet: true);
					}
					array6 = array3;
					foreach (int id3 in array6)
					{
						userData.AddCollections(UserData.Collection.Icon, id3, addNewGet: true);
					}
					array6 = array4;
					foreach (int id4 in array6)
					{
						userData.AddCollections(UserData.Collection.Partner, id4, addNewGet: true);
					}
					array6 = array5;
					foreach (int id5 in array6)
					{
						userData.AddCollections(UserData.Collection.Frame, id5, addNewGet: true);
					}
				}
			}
		}

		private void CheckTicketGetData()
		{
			for (int i = 0; i < 2; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (userData.IsEntry && !userData.IsGuest())
				{
					if (GameManager.IsFreedomMode && Singleton<TicketManager>.Instance.GetFreedomPlayTicketId(i) != -1)
					{
						Singleton<TicketManager>.Instance.SaveFreedomTicketGet(i);
					}
					Singleton<TicketManager>.Instance.checkAndSaveEventTicketGet(i);
				}
			}
		}
	}
}
