using System;

namespace Monitor.MapCore.Component
{
	public class DelayComponent : MapBehaviour
	{
		private float _duration;

		private float _elapsed;

		private bool _request;

		private Action _onDone;

		private bool _pause;

		public void StartDelay(float duration, Action onDone)
		{
			_duration = duration;
			_onDone = onDone;
			_elapsed = 0f;
			_request = true;
		}

		private void StateUpdate(float deltaTime)
		{
			if (!_pause)
			{
				_elapsed += deltaTime;
				if (!(_elapsed < _duration))
				{
					_onDone?.Invoke();
					SetStateTerminate();
				}
			}
		}

		protected override void OnLateUpdate(float deltaTime)
		{
			if (_request)
			{
				State = StateUpdate;
				_request = false;
			}
		}

		public void Pause(bool flag)
		{
			_pause = flag;
		}

		public void Stop()
		{
			SetStateTerminate();
		}
	}
}
