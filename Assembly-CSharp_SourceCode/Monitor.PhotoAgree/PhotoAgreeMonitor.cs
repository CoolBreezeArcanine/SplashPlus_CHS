using IO;
using UnityEngine;

namespace Monitor.PhotoAgree
{
	public class PhotoAgreeMonitor : MonitorBase
	{
		public enum EntryStatus
		{
			None,
			Agree,
			Disagree,
			Max
		}

		private EntryStatus _entryStatus;

		[SerializeField]
		private PhotoAgreeButtonController _buttonController;

		public bool IsSelected()
		{
			return _entryStatus != EntryStatus.None;
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
				return;
			}
			_buttonController.Initialize(monIndex);
			_buttonController.SetVisibleImmediate(false, 3, 4);
		}

		public void Play()
		{
			_buttonController.SetVisible(true, 3, 4);
		}

		public void Stop()
		{
			_buttonController.SetVisible(false, 3, 4);
		}

		public void Agree()
		{
			_buttonController.SetAnimationActive(3);
		}

		public void Disagree()
		{
			_buttonController.SetAnimationActive(4);
		}

		public override void ViewUpdate()
		{
		}
	}
}
