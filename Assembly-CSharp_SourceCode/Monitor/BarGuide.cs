using MAI2.Util;
using Manager;
using Monitor.Game;
using UnityEngine;

namespace Monitor
{
	public class BarGuide : MonoBehaviour
	{
		public const float BarDispTiming = 5.33333349f;

		protected int MonitorIndex;

		private float _appearMsec;

		protected float DefaultMsec;

		protected float StartMsec;

		protected float StartPos;

		protected float EndPos;

		protected bool EndFlag;

		public int MonitorId
		{
			get
			{
				return MonitorIndex;
			}
			set
			{
				MonitorIndex = value;
			}
		}

		public Transform ParentTransform { get; set; }

		public bool IsEnd()
		{
			return EndFlag;
		}

		public void Initialize(float appearTime)
		{
			StartPos = 0f;
			EndPos = GameCtrl.NoteEndPos() * 2f;
			EndFlag = false;
			_appearMsec = appearTime;
			DefaultMsec = GameManager.GetNoteSpeedForBeat((int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetNoteSpeed) * 5.33333349f;
			StartMsec = _appearMsec - DefaultMsec;
			base.transform.localScale = new Vector3(0f, 0f, 1f);
		}

		public void Execute()
		{
			CheckEnd();
		}

		private void CheckEnd()
		{
			if (base.transform.localScale.y >= EndPos / GameManager.MainMonitorSize)
			{
				EndFlag = true;
				base.transform.localScale = new Vector3(0f, 0f, 1f);
			}
			if (!EndFlag)
			{
				float num = StartMsec + DefaultMsec;
				float num2 = StartPos / GameManager.MainMonitorSize;
				float num3 = EndPos / GameManager.MainMonitorSize;
				float num4 = ((num <= NotesManager.GetCurrentMsec()) ? num3 : (num2 + (num3 - num2) * (1f - (num - NotesManager.GetCurrentMsec()) / DefaultMsec)));
				base.transform.localScale = new Vector3(num4, num4, 0f);
			}
			else
			{
				base.gameObject.SetActive(value: false);
				base.transform.SetParent(ParentTransform, worldPositionStays: false);
			}
		}
	}
}
