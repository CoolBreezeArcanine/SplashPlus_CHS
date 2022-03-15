using System.Collections.Generic;
using System.Diagnostics;
using DB;
using IO;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.UserDatas;
using UnityEngine;

namespace Monitor.CardMakerInfo
{
	public class CardMakerInfoMonitor : MonitorBase
	{
		public enum InfoStatus
		{
			None,
			Disp,
			Finish,
			Max
		}

		public struct WindowDataSt
		{
			public WindowMessageID windowId;

			public Cue cue;
		}

		private InfoStatus _entryStatus;

		[SerializeField]
		private CardMakerInfoButtonController _buttonController;

		private List<WindowDataSt> infoList = new List<WindowDataSt>();

		private const int DispWaitMsec = 10000;

		private Stopwatch timer = new Stopwatch();

		public bool TimeUp()
		{
			return timer.ElapsedMilliseconds > 10000;
		}

		public bool IsFinish()
		{
			return _entryStatus == InfoStatus.Finish;
		}

		public void SetStatus(InfoStatus stat)
		{
			_entryStatus = stat;
		}

		public WindowDataSt GetNowMessageId()
		{
			return infoList[0];
		}

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			if (IsActive())
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			if (!active)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
				SetStatus(InfoStatus.Finish);
				return;
			}
			_buttonController.Initialize(monIndex);
			_buttonController.SetVisibleImmediate(false, 3);
			infoList.Clear();
			if (!GameManager.IsEventMode && !Singleton<UserDataManager>.Instance.GetUserData(monIndex).IsGuest())
			{
				UserDetail detail = Singleton<UserDataManager>.Instance.GetUserData(monIndex).Detail;
				if (!detail.ContentBit.IsFlagOn(ContentBitID.FirstCollecitonEnd))
				{
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.DxPassFirst,
						cue = Cue.VO_000228
					});
					detail.ContentBit.SetFlag(ContentBitID.FirstCollecitonEnd, flag: true);
				}
			}
			if (infoList.Count == 0)
			{
				SetStatus(InfoStatus.Finish);
			}
			else
			{
				SetStatus(InfoStatus.Disp);
			}
		}

		public void Next()
		{
			timer.Restart();
			if (infoList.Count > 0)
			{
				infoList.RemoveAt(0);
			}
			if (infoList.Count == 0)
			{
				SetStatus(InfoStatus.Finish);
			}
		}

		public void Play()
		{
			timer.Restart();
			_buttonController.SetVisible(true, 3);
		}

		public void PushButton()
		{
			_buttonController.SetAnimationActive(3);
		}

		public void Stop()
		{
			_buttonController.SetAnimationActive(3);
			_buttonController.SetVisible(false, 3);
		}

		public override void ViewUpdate()
		{
		}
	}
}
