using System;
using UnityEngine;

namespace Monitor.MapCore.Component
{
	public abstract class LerpComponent<T> : MapBehaviour where T : struct
	{
		private float _duration;

		private float _elapsed;

		private T _from;

		private T _to;

		private T _now;

		private AnimationCurve _curve;

		public bool _request;

		public bool _pause;

		private Action<T> _onNext;

		private Action _onDone;

		public void StartLerp(T from, T to, AnimationCurve curve, float duration, Action<T> onNext, Action onDone)
		{
			_from = from;
			_to = to;
			_duration = duration;
			_curve = curve;
			_onNext = onNext;
			_onDone = onDone;
			_elapsed = 0f;
			_request = true;
			_onNext?.Invoke(_from);
		}

		public void StartLerp(T from, T to, float duration, Action<T> onNext, Action onDone)
		{
			StartLerp(from, to, null, duration, onNext, onDone);
		}

		private void StateUpdate(float deltaTime)
		{
			if (!_pause)
			{
				_elapsed += deltaTime;
				float num;
				float weight;
				if (_curve != null)
				{
					num = _elapsed / (_duration * _curve.keys[_curve.length - 1].time);
					weight = _curve.Evaluate(num);
				}
				else
				{
					num = _elapsed / _duration;
					weight = num;
				}
				_now = Lerp(_from, _to, weight);
				_onNext?.Invoke(_now);
				if (!(num < 1f))
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

		protected abstract T Lerp(T from, T to, float weight);
	}
}
