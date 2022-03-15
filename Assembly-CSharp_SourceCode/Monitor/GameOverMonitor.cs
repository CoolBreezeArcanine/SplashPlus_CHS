using UnityEngine.Playables;

namespace Monitor
{
	public class GameOverMonitor : MonitorBase
	{
		private PlayableDirector _director;

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (!isActive)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
				return;
			}
			_director = Main.GetComponent<PlayableDirector>();
			_director.time = 0.0;
			_director.Play();
			_director.Pause();
		}

		public void Play()
		{
			_director.Resume();
		}

		public bool IsPlayEnd()
		{
			if (!isPlayerActive)
			{
				return true;
			}
			return _director.time >= _director.duration;
		}

		public override void ViewUpdate()
		{
		}
	}
}
