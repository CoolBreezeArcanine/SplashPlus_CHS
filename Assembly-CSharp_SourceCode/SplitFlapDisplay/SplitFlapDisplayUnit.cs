using UnityEngine;
using UnityEngine.UI;

namespace SplitFlapDisplay
{
	public class SplitFlapDisplayUnit : MonoBehaviour
	{
		protected enum State
		{
			None,
			Init,
			Scroll,
			Next
		}

		protected int CurrentIndex;

		protected int TargetIndex;

		[SerializeField]
		private Color _backgroundColor;

		[SerializeField]
		private bool _isTotalColor;

		[SerializeField]
		private Image[] _backgroundImage;

		private State _state;

		private Animator _flapAnimator;

		public void UnitUpdate()
		{
			switch (_state)
			{
			case State.Next:
				Next();
				break;
			case State.None:
			case State.Init:
			case State.Scroll:
				break;
			}
		}

		public virtual void Initialize(int initialIndex)
		{
			_flapAnimator = GetComponent<Animator>();
			_state = State.None;
			CurrentIndex = initialIndex;
			SetCurrentElement(CurrentIndex);
			TargetIndex = 0;
			_flapAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
		}

		public void Play(int targetIndex)
		{
			if (_state != 0 || targetIndex != CurrentIndex)
			{
				TargetIndex = targetIndex;
				if (_state == State.None)
				{
					PlaySplitFlap();
				}
			}
		}

		public void Stop(int targetIndex)
		{
			if (_state != 0)
			{
				TargetIndex = targetIndex;
				CurrentIndex = targetIndex;
				Next();
			}
		}

		[ContextMenu("Split-Flap")]
		public virtual void PlaySplitFlap()
		{
			if (CurrentIndex + 1 < GetElementCount())
			{
				CurrentIndex++;
			}
			else
			{
				CurrentIndex = 0;
			}
			_flapAnimator.Play(Animator.StringToHash("Scroll"), 0, 0f);
			_state = State.Scroll;
		}

		public virtual int GetElementCount()
		{
			return 0;
		}

		protected virtual void SetCurrentElement(int index)
		{
		}

		protected virtual void SetNextElement(int index)
		{
		}

		protected virtual void Next()
		{
			int nextElement = ((CurrentIndex + 1 < GetElementCount()) ? (CurrentIndex + 1) : 0);
			SetCurrentElement(CurrentIndex);
			SetNextElement(nextElement);
			if (CurrentIndex == TargetIndex)
			{
				_state = State.None;
			}
			else
			{
				PlaySplitFlap();
			}
		}

		public void OnTriggerAnimationEnd()
		{
			_flapAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
			_state = State.Next;
		}
	}
}
