using IO;
using MAI2.Util;
using Manager;
using Manager.UserDatas;
using Mecha;
using Monitor.Game;
using UnityEngine;

namespace Monitor
{
	public class GameMonitor : MonitorBase
	{
		public enum GameModeEnum
		{
			Game,
			Advertise,
			Tutorial
		}

		private bool _debugPause;

		protected bool IsResultEffectEnd;

		protected bool IsAnyObjectActive;

		[SerializeField]
		[Header("ゲームコントローラ")]
		protected GameObject GameContrullerPrefub;

		[SerializeField]
		[Header("画面中央表示")]
		private GameObjectCtrl GameDispCtrl;

		protected GameCtrl GameController;

		protected NotesManager NoteMng;

		protected GameModeEnum GameMode;

		protected float _barTime;

		public GameMonitor()
		{
			GameMode = GameModeEnum.Game;
			NoteMng = null;
		}

		protected override void SetVisible(bool isActive)
		{
			Main.gameObject.SetActive(isActive);
			Sub.gameObject.SetActive(isActive);
		}

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			if (IsActive())
			{
				Bd15070_4IF bd15070_4IF = MechaManager.LedIf[monIndex];
				for (byte b = 0; b < 8; b = (byte)(b + 1))
				{
					bd15070_4IF.SetColor(b, Color.white);
				}
			}
			NoteMng = NotesManager.Instance(base.MonitorIndex);
			IsResultEffectEnd = false;
			IsAnyObjectActive = false;
			_barTime = 0f;
			GameController = Object.Instantiate(GameContrullerPrefub, base.transform).GetComponent<GameCtrl>();
			GameController.Initialize(monIndex, GameMode);
			GameController.SetNoteManager(NoteMng);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex);
			if (userData.IsEntry && null != GameDispCtrl)
			{
				int difficulty = Singleton<GamePlayManager>.Instance.GetGameScore(monIndex).SessionInfo.difficulty;
				int musicID = Singleton<GamePlayManager>.Instance.GetGameScore(monIndex).SessionInfo.musicId;
				int mybest = 0;
				UserScore userScore = userData.ScoreList[difficulty].Find((UserScore item) => item.id == musicID);
				if (userScore != null)
				{
					mybest = (int)userScore.achivement;
				}
				GameDispCtrl.Initialize(monitorIndex, mybest);
			}
		}

		public bool IsReady()
		{
			if (!IsActive())
			{
				return true;
			}
			return GameController.IsReady();
		}

		public void SetMovieSize(uint height, uint width)
		{
			GameController.SetMovieSize(height, width);
		}

		public NotesManager GetNotesManager()
		{
			return NoteMng;
		}

		public override void ViewUpdate()
		{
			GameController?.UpdateMovie();
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry || GameMode == GameModeEnum.Advertise)
			{
				IsAnyObjectActive = GameController.UpdateCtrl();
				if (null != GameDispCtrl)
				{
					GameDispCtrl.Execute();
				}
				if (NoteMng.isBeat())
				{
					BeatUpdate();
				}
			}
			else
			{
				IsAnyObjectActive = false;
			}
		}

		protected virtual void BeatUpdate()
		{
			BarData barNextData = NoteMng.getReader().getBarNextData(BarType.GAMEBEAT, _barTime);
			if (barNextData != null)
			{
				_barTime = barNextData.time.msec;
			}
		}

		public virtual void PlayStart()
		{
		}

		public virtual void PlayStop()
		{
		}

		public virtual bool PlayResult()
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry && null != GameDispCtrl)
			{
				return GameDispCtrl.GetResultEffectObj().Play();
			}
			return false;
		}

		public virtual void PlayPhoto()
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry && null != GameDispCtrl)
			{
				GameDispCtrl.GetPhotoKumaObj().Play();
			}
		}

		public void Seek(int setMsec)
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				GameController.ForceNoteCollect();
				GameController.SetDebugSkipFlame();
				_barTime = NotesManager.GetCurrentMsec();
			}
		}

		public bool IsPlayEnd()
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return true;
			}
			if (IsGameOver())
			{
				return IsResultEnd();
			}
			return false;
		}

		public bool IsGameOver()
		{
			if (!Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).IsTrackSkip)
			{
				return IsAllJudged();
			}
			return true;
		}

		public bool IsAllJudged()
		{
			if (Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).IsAllJudged())
			{
				return IsAllObjectEnd();
			}
			return false;
		}

		public bool IsAllObjectEnd()
		{
			return !IsAnyObjectActive;
		}

		public bool IsResultEnd()
		{
			if (!(GameDispCtrl == null))
			{
				return GameDispCtrl.GetResultEffectObj().IsEnd();
			}
			return true;
		}

		public void ForceAchivement(int achivement, int dxscore)
		{
			GameController.ForceAchivement(achivement, dxscore);
		}

		public void ForceAchivementLowAP()
		{
			GameController.ForceAchivementLowAP();
		}

		public int GetPushPhase()
		{
			return GameController.GetPushPhase();
		}

		public void SetForceShutter()
		{
			GameDispCtrl.SetForceShutter();
		}
	}
}
