using System.Linq;
using DB;
using IO;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Monitor
{
	public class TutorialSelectMonitor : MonitorBase
	{
		public enum EntryStatus
		{
			None,
			BasicTutorialSelect,
			NewTutorialSelect,
			NotTutorialSelect,
			Max
		}

		[SerializeField]
		private TutorialSelectButtonController _buttonController;

		[SerializeField]
		private PlayableDirector _mainPlayableDirector;

		[SerializeField]
		private PlayableAsset _startPlayable;

		[SerializeField]
		private PlayableAsset _loopPlayable;

		[SerializeField]
		private PlayableAsset _selectPlayable;

		[SerializeField]
		private string _basicTrackName;

		[SerializeField]
		private string _newTrackName;

		[SerializeField]
		private string _effectTrackName;

		[SerializeField]
		private TextMeshProUGUI _newText;

		[SerializeField]
		private TextMeshProUGUI _basicText;

		private const string overrideSelectName = "_Select";

		private EntryStatus _entryStatus;

		private void Awake()
		{
			_newText.text = CommonMessageID.TutorialSelectNew.GetName();
			_basicText.text = CommonMessageID.TutorialSelectBasic.GetName();
		}

		public bool IsSelected()
		{
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
			_buttonController.Initialize(monIndex);
			_buttonController.SetVisible(false, 4);
			_mainPlayableDirector.playableAsset = _startPlayable;
			_mainPlayableDirector.Play();
			_mainPlayableDirector.Pause();
			_mainPlayableDirector.time = 0.0;
		}

		public override void ViewUpdate()
		{
			_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
		}

		public void Play()
		{
			_mainPlayableDirector.Play();
		}

		public void Loop()
		{
			_mainPlayableDirector.Stop();
			_mainPlayableDirector.playableAsset = _loopPlayable;
			_mainPlayableDirector.extrapolationMode = DirectorWrapMode.Loop;
			_mainPlayableDirector.Play();
			_buttonController.SetVisible(true, 4);
		}

		public void SelectBasicTutorialSelect()
		{
			_entryStatus = EntryStatus.BasicTutorialSelect;
			SetSelectTrack(basicBtn: true, newBtn: false);
			_buttonController.SetVisible(false, 4);
		}

		public void SelectNewTutorialSelect()
		{
			_entryStatus = EntryStatus.NewTutorialSelect;
			SetSelectTrack(basicBtn: false, newBtn: true);
			_buttonController.SetVisible(false, 4);
		}

		public void SelectNotTutorialSelect()
		{
			_entryStatus = EntryStatus.NotTutorialSelect;
			_buttonController.SetAnimationActive(4);
			SetSelectTrack(basicBtn: false, newBtn: false);
			_buttonController.SetVisible(false, 4);
		}

		private void SetSelectTrack(bool basicBtn, bool newBtn)
		{
			_mainPlayableDirector.Stop();
			_mainPlayableDirector.playableAsset = _selectPlayable;
			_mainPlayableDirector.extrapolationMode = DirectorWrapMode.Hold;
			((TimelineAsset)_mainPlayableDirector.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _basicTrackName).GetChildTracks()
				.First((TrackAsset c) => c.name == _basicTrackName + "_Select")
				.muted = !basicBtn;
			((TimelineAsset)_mainPlayableDirector.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _newTrackName).GetChildTracks()
				.First((TrackAsset c) => c.name == _newTrackName + "_Select")
				.muted = !newBtn;
			((TimelineAsset)_mainPlayableDirector.playableAsset).GetOutputTracks().First((TrackAsset c) => c.name == _effectTrackName).muted = !basicBtn && !newBtn;
			_mainPlayableDirector.Play();
		}

		public bool IsStartDispEnd()
		{
			if (!IsActive())
			{
				return true;
			}
			return _mainPlayableDirector.time >= _mainPlayableDirector.duration;
		}
	}
}
