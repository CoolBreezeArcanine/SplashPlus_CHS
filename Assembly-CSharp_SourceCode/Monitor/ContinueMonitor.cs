using IO;
using MAI2.Util;
using Manager;
using UnityEngine;
using UnityEngine.Playables;

namespace Monitor
{
	public class ContinueMonitor : MonitorBase
	{
		public enum EntryStatus
		{
			None,
			Continue,
			NotContinue,
			End
		}

		[SerializeField]
		private ContinueButtonController _buttonController;

		[SerializeField]
		[Header("再生タイムライン(カウントダウン)")]
		private PlayableAsset _selectPlayable;

		[SerializeField]
		private GameObject _selectObjectRoot;

		[SerializeField]
		[Header("再生タイムライン（コンティニュー）")]
		private PlayableAsset _continuePlayable;

		[SerializeField]
		private GameObject _continueObjectRoot;

		private PlayableDirector _director;

		private EntryStatus _entryStatus;

		public bool IsSelected()
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return true;
			}
			return _entryStatus != EntryStatus.None;
		}

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (IsActive())
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			if (!isActive)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
				return;
			}
			_buttonController.Initialize(monitorIndex);
			_buttonController.SetVisible(true, 3, 4);
			_director = Main.GetComponent<PlayableDirector>();
			_director.playableAsset = _selectPlayable;
			_selectObjectRoot.SetActive(value: true);
			_continueObjectRoot.SetActive(value: false);
			_director.Play();
			_director.Pause();
			_director.time = 0.0;
		}

		public override void ViewUpdate()
		{
		}

		public void WaitStart()
		{
			_director.Play();
		}

		public void SelectContinue()
		{
			_entryStatus = EntryStatus.Continue;
			_buttonController.SetAnimationActive(3);
			_buttonController.SetVisible(false, 3, 4);
		}

		public void SelectNotContinue()
		{
			_entryStatus = EntryStatus.NotContinue;
			_buttonController.SetAnimationActive(4);
			_buttonController.SetVisible(false, 3, 4);
		}

		public void PlayContinue()
		{
			_selectObjectRoot.SetActive(value: false);
			_continueObjectRoot.SetActive(value: true);
			_director.Stop();
			_director.playableAsset = _continuePlayable;
			_director.Play();
		}

		public void PlayNotContinue()
		{
		}
	}
}
