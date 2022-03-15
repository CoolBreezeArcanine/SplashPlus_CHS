using System;
using System.Collections.Generic;
using System.Linq;
using AMDaemon;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Monitor;
using Monitor.Entry.Parts.Screens;
using UnityEngine;

namespace Process
{
	public class DataSaveProcess : ProcessBase
	{
		public enum DataSaveSequence
		{
			Wait,
			Disp,
			Error,
			GotoEnd,
			Release
		}

		private DataSaveSequence _state;

		private DataSaveMonitor[] _monitors;

		private float _timeCounter;

		private UserDataULProcess _uploader;

		private bool _anyError;

		public DataSaveProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/DataSave/DataSaveProcess");
			_monitors = new DataSaveMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<DataSaveMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<DataSaveMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
				SoundManager.StopBGM(i);
				_monitors[i].AnimStartIn();
			}
			container.processManager.NotificationFadeIn();
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20000));
			SaveDataFix();
			for (int j = 0; j < _monitors.Length; j++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(j);
				if (!userData.IsEntry)
				{
					continue;
				}
				if (userData.UserType != UserData.UserIDType.New)
				{
					if (!GameManager.IsFreedomMode)
					{
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.EndPlay(j, Accounting.KindCoe.Credit, Accounting.StatusCode.PayToPlay_End, 1);
					}
					else
					{
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.EndPlay(j, Accounting.KindCoe.Freedom, Accounting.StatusCode.PayToPlay_End, 2);
					}
				}
				else
				{
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.EndPlay(j, Accounting.KindCoe.Credit, Accounting.StatusCode.FreeToPlay_End, 0);
				}
			}
			if (Singleton<OperationManager>.Instance.NetIconStatus != 0)
			{
				_anyError = false;
				_uploader = new UserDataULProcess(container);
				container.processManager.AddProcess(_uploader, 50);
				_state = DataSaveSequence.Wait;
			}
			else
			{
				_state = DataSaveSequence.GotoEnd;
			}
		}

		public override void OnUpdate()
		{
			switch (_state)
			{
			case DataSaveSequence.GotoEnd:
				_state = DataSaveSequence.Release;
				container.processManager.AddProcess(new GameOverProcess(container), 50);
				container.processManager.ReleaseProcess(this);
				break;
			case DataSaveSequence.Wait:
			{
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				_timeCounter = 0f;
				_state = DataSaveSequence.Disp;
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
					{
						SoundManager.PlaySE(Cue.SE_DATA_SAVED, k);
						container.processManager.EnqueueMessage(k, WindowMessageID.DataSaveStart);
					}
				}
				break;
			}
			case DataSaveSequence.Disp:
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 2f))
				{
					break;
				}
				if (_uploader.IsDone)
				{
					_timeCounter = 0f;
					for (int i = 0; i < _monitors.Length; i++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && _uploader.GetUpsertError(i))
						{
							container.processManager.EnqueueMessage(i, WindowMessageID.DataSaveError);
							_anyError = true;
						}
					}
					if (_anyError)
					{
						_state = DataSaveSequence.Error;
						break;
					}
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.localParameter.AddPlayCount();
					_state = DataSaveSequence.Release;
					AnimationOut();
					GotoNextProcess();
				}
				else
				{
					if (!_uploader.IsError)
					{
						break;
					}
					_timeCounter = 0f;
					for (int j = 0; j < _monitors.Length; j++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
						{
							container.processManager.EnqueueMessage(j, WindowMessageID.DataSaveError);
						}
					}
					_anyError = true;
					_state = DataSaveSequence.Error;
				}
				break;
			case DataSaveSequence.Error:
				_timeCounter += Time.deltaTime;
				if (_timeCounter >= 4f)
				{
					_timeCounter = 0f;
					_state = DataSaveSequence.Release;
					AnimationOut();
					GotoNextProcess();
				}
				break;
			case DataSaveSequence.Release:
				return;
			}
			for (int l = 0; l < _monitors.Length; l++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry)
				{
					_monitors[l].ViewUpdate();
				}
			}
		}

		private void GotoNextProcess()
		{
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsContinue && !GameManager.IsEventMode && !Singleton<OperationManager>.Instance.IsLoginDisable() && !_anyError)
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new ContinueProcess(container), FadeProcess.FadeType.Type3), 50);
			}
			else
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new GameOverProcess(container), FadeProcess.FadeType.Type3), 50);
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				UnityEngine.Object.Destroy(_monitors[i].gameObject);
			}
			_uploader?.Kill();
		}

		private void SaveDataFix()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (!userData.IsEntry)
				{
					continue;
				}
				UserDetail detail = userData.Detail;
				_ = userData.ScoreList;
				userData.AddPlayCount();
				detail.EventWatchedDate = TimeManager.GetDateString(TimeManager.PlayBaseTime);
				userData.CalcTotalValue();
				float num = 0f;
				try
				{
					if (userData.RatingList.RatingList.Any())
					{
						num = userData.RatingList.RatingList.Last().SingleRate;
					}
				}
				catch
				{
				}
				num = (float)Math.Ceiling((double)((num + 1f) / GameManager.TheoryRateBorderNum) * 10.0);
				float num2 = 0f;
				try
				{
					if (userData.RatingList.NextRatingList.Any())
					{
						num2 = userData.RatingList.NewRatingList.Last().SingleRate;
					}
				}
				catch
				{
				}
				num2 = (float)Math.Ceiling((double)((num2 + 1f) / GameManager.TheoryRateBorderNum) * 10.0);
				string logDateString = TimeManager.GetLogDateString(TimeManager.PlayBaseTime);
				string timeJp = (string.IsNullOrEmpty(userData.Detail.DailyBonusDate) ? TimeManager.GetLogDateString(0L) : userData.Detail.DailyBonusDate);
				if (userData.IsEntry && userData.Detail.IsNetMember >= 2 && !GameManager.IsEventMode && TimeManager.GetUnixTime(logDateString) > TimeManager.GetUnixTime(timeJp) && Singleton<UserDataManager>.Instance.IsSingleUser() && !GameManager.IsFreedomMode && !GameManager.IsCourseMode && !DoneEntry.IsWeekdayBonus(userData))
				{
					userData.Detail.DailyBonusDate = logDateString;
				}
				List<UserRate> list = new List<UserRate>();
				List<UserRate> list2 = new List<UserRate>();
				List<UserScore>[] scoreList = userData.ScoreList;
				List<UserRate> ratingList = userData.RatingList.RatingList;
				List<UserRate> newRatingList = userData.RatingList.NewRatingList;
				int achive = RatingTableID.Rate_22.GetAchive();
				for (int j = 0; j < scoreList.Length; j++)
				{
					if (scoreList[j] == null)
					{
						continue;
					}
					foreach (UserScore item2 in scoreList[j])
					{
						if (achive <= item2.achivement)
						{
							continue;
						}
						MusicData music = Singleton<DataManager>.Instance.GetMusic(item2.id);
						if (music == null)
						{
							continue;
						}
						UserRate item = new UserRate(item2.id, j, item2.achivement, (uint)music.version);
						if (item.OldFlag)
						{
							if (num <= (float)item.Level && !ratingList.Contains(item))
							{
								list.Add(item);
							}
						}
						else if (num2 <= (float)item.Level && !newRatingList.Contains(item))
						{
							list2.Add(item);
						}
					}
				}
				list.Sort();
				list.Reverse();
				if (list.Count > 10)
				{
					list.RemoveRange(10, list.Count - 10);
				}
				userData.RatingList.NextRatingList = list;
				list2.Sort();
				list2.Reverse();
				if (list2.Count > 10)
				{
					list2.RemoveRange(10, list2.Count - 10);
				}
				userData.RatingList.NextNewRatingList = list2;
				int num3 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.GameCostEnoughPlay();
				if (GameManager.IsFreedomMode)
				{
					num3 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.GameCostEnoughFreedom();
				}
				int boughtTicketId = Singleton<TicketManager>.Instance.GetBoughtTicketId(i);
				int ticketCredit = Singleton<TicketManager>.Instance.GetTicketCredit(boughtTicketId);
				int lastPlayCredit = num3 + ticketCredit;
				userData.Detail.LastPlayCredit = lastPlayCredit;
				userData.Detail.LastPlayMode = 0;
				if (GameManager.IsFreedomMode)
				{
					userData.Detail.LastPlayMode = 1;
				}
				if (GameManager.IsCourseMode)
				{
					userData.Detail.LastPlayMode = 2;
				}
				userData.Detail.LastGameId = AMDaemon.System.GameId;
				userData.Detail.LastRomVersion = Singleton<SystemConfig>.Instance.config.romVersionInfo.versionNo.versionString;
				userData.Detail.LastDataVersion = Singleton<SystemConfig>.Instance.config.dataVersionInfo.versionNo.versionString;
			}
		}

		private void AnimationOut()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					_monitors[i].AnimStartOut();
				}
			}
		}
	}
}
