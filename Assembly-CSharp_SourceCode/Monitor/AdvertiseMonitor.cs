using Manager;
using UnityEngine;
using UnityEngine.Playables;

namespace Monitor
{
	public class AdvertiseMonitor : MonitorBase
	{
		private CanvasGroup _titleObject;

		[SerializeField]
		private PlayableDirector _advertiseDirector;

		[SerializeField]
		private PlayableDirector _titleLogoDirector;

		[SerializeField]
		private GameObject _eventModeObject;

		[SerializeField]
		private TournamentInfo _tournamentInfo;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			_titleLogoDirector.Play();
			_titleLogoDirector.Pause();
			_titleLogoDirector.time = 0.0;
			_titleObject = _titleLogoDirector.gameObject.GetComponent<CanvasGroup>();
			_titleObject.alpha = 0f;
			_tournamentInfo.SetInit();
		}

		public override void ViewUpdate()
		{
		}

		public bool IsLogoAnimationEnd()
		{
			return _advertiseDirector.time >= _advertiseDirector.duration;
		}

		public bool IsTitleAnimationEnd()
		{
			return _titleLogoDirector.time >= _titleLogoDirector.duration;
		}

		public void PlayTitleLogo()
		{
			_eventModeObject.SetActive(GameManager.IsEventMode);
			_titleLogoDirector.Play();
		}

		public void PlayLogo()
		{
			_eventModeObject.SetActive(value: false);
			_advertiseDirector.Play();
		}

		public void AllStop()
		{
			_advertiseDirector.time = _advertiseDirector.duration;
			_titleLogoDirector.time = _titleLogoDirector.duration;
			_advertiseDirector.Evaluate();
			_titleLogoDirector.Evaluate();
		}
	}
}
