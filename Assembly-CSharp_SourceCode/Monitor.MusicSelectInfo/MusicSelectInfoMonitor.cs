using System.Collections.Generic;
using System.Diagnostics;
using DB;
using IO;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.UserDatas;
using UnityEngine;

namespace Monitor.MusicSelectInfo
{
	public class MusicSelectInfoMonitor : MonitorBase
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
		private MusicSelectInfoButtonController _buttonController;

		private List<WindowDataSt> infoList = new List<WindowDataSt>();

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
			if (!GameManager.IsCourseMode && !GameManager.IsEventMode && 1 == GameManager.MusicTrackNumber)
			{
				UserDetail detail = Singleton<UserDataManager>.Instance.GetUserData(monIndex).Detail;
				switch (detail.PlayCount)
				{
				case 0:
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.CategorySelectFirst,
						cue = Cue.VO_000181
					});
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.MusicSelectFirst,
						cue = Cue.VO_000077
					});
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.PlayReadyFirst,
						cue = Cue.VO_000205
					});
					break;
				case 1:
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.DxStandardFirst,
						cue = Cue.VO_000183
					});
					break;
				case 2:
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.OtomodachiFirst,
						cue = Cue.VO_000228
					});
					break;
				}
				if (!detail.ContentBit.IsFlagOn(ContentBitID.FirstRateCategory) && GameManager.IsCardOpenRating[monIndex])
				{
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.RateCategoryFirst,
						cue = Cue.VO_000228
					});
					detail.ContentBit.SetFlag(ContentBitID.FirstRateCategory, flag: true);
				}
				if (GameManager.IsFreedomMode)
				{
					infoList.Add(new WindowDataSt
					{
						windowId = WindowMessageID.FreedomFirst,
						cue = Cue.VO_000228
					});
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
