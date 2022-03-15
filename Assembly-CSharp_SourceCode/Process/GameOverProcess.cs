using AMDaemon;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using Monitor;
using Net;
using UnityEngine;

namespace Process
{
	public class GameOverProcess : ProcessBase
	{
		public enum GameOverSequence
		{
			Wait,
			SkyChange,
			Disp,
			Release
		}

		private GameOverSequence _state;

		private GameOverMonitor[] _monitors;

		private float _timeCounter;

		public GameOverProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/GameOver/GameOverProcess");
			_monitors = new GameOverMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<GameOverMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<GameOverMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20000));
				SoundManager.StopBGM(i);
			}
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			switch (_state)
			{
			case GameOverSequence.Wait:
			{
				_timeCounter = 0f;
				_state = GameOverSequence.SkyChange;
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
					{
						if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit != 0 && !SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay())
						{
							SoundManager.PlayPartnerVoice(Cue.VO_000166, k);
						}
						else
						{
							SoundManager.PlayPartnerVoice(Cue.VO_000167, k);
						}
						_monitors[k].Play();
					}
				}
				break;
			}
			case GameOverSequence.SkyChange:
				_timeCounter += Time.deltaTime;
				if (_timeCounter >= 3f)
				{
					_timeCounter = 0f;
					_state = GameOverSequence.Disp;
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 50002, CommonMonitor.SkyDaylight.EveningToNight));
				}
				break;
			case GameOverSequence.Disp:
			{
				if (!_monitors[0].IsPlayEnd() || !_monitors[1].IsPlayEnd())
				{
					break;
				}
				_timeCounter = 0f;
				_state = GameOverSequence.Release;
				container.processManager.AddProcess(new FadeProcess(container, this, new AdvertiseProcess(container)), 50);
				BackupBookkeep.EntryState[] array = new BackupBookkeep.EntryState[_monitors.Length];
				bool[] array2 = new bool[_monitors.Length];
				for (int i = 0; i < _monitors.Length; i++)
				{
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
					array[i].entry = false;
					array2[i] = false;
					if (userData.IsEntry)
					{
						array[i].entry = true;
						array[i].type = (UserID.IsGuest(userData.Detail.UserID) ? BackupBookkeep.LoginType.Guest : BackupBookkeep.LoginType.Aime);
						array2[i] = Singleton<UserDataManager>.Instance.GetUserData(i).UserType == UserData.UserIDType.New;
						if (!userData.IsGuest())
						{
							SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.SendAimeLog(userData.AimeId, AimeLogStatus.Leave);
						}
					}
				}
				SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.endPlayTime(array, array2);
				for (int j = 0; j < _monitors.Length; j++)
				{
					Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry = false;
				}
				container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20000));
				break;
			}
			case GameOverSequence.Release:
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

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}
	}
}
