using UnityEngine;

namespace Monitor.MusicSelect
{
	public class SlideSpeedPreviewObject : NoteSpeedPreviewObject
	{
		private enum SlidArrowState
		{
			Introduction,
			Staging,
			Idle
		}

		[SerializeField]
		private Animator _arrowAnimator;

		[SerializeField]
		private Animator _starRotateAnimator;

		[SerializeField]
		private int _slideSpeed;

		private SlidArrowState _state;

		private bool _isCheckStageing;

		private float _showRate;

		private float _starRotateSpeed;

		private void Update()
		{
			AnimationUpdate();
		}

		public void AnimationUpdate()
		{
			if (!base.gameObject.activeSelf || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (currentAnimatorStateInfo.IsName("Base Layer.Introduction"))
			{
				if (_state == SlidArrowState.Idle)
				{
					_arrowAnimator.Play(Animator.StringToHash("Idle"), 0, 0f);
					_state = SlidArrowState.Introduction;
				}
				if (!_isCheckStageing && _state == SlidArrowState.Introduction && currentAnimatorStateInfo.normalizedTime >= _showRate)
				{
					_arrowAnimator.Play(Animator.StringToHash("Staging"), 0, 0f);
					_state = SlidArrowState.Staging;
				}
			}
			else if (currentAnimatorStateInfo.IsName("Base Layer.Staging"))
			{
				if (_isCheckStageing && _state == SlidArrowState.Introduction)
				{
					_arrowAnimator.Play(Animator.StringToHash("Staging"), 0, 0f);
				}
				if (_state == SlidArrowState.Staging && currentAnimatorStateInfo.normalizedTime >= 0.95f)
				{
					_arrowAnimator.Play(Animator.StringToHash("Interval"), 0, 0f);
					_state = SlidArrowState.Idle;
				}
			}
			else if (currentAnimatorStateInfo.IsName("Base Layer.Interval"))
			{
				if (_state == SlidArrowState.Staging)
				{
					_arrowAnimator.Play(Animator.StringToHash("Interval"), 0, 0f);
					_state = SlidArrowState.Idle;
				}
				if (_state == SlidArrowState.Idle && currentAnimatorStateInfo.normalizedTime >= 0.95f)
				{
					_arrowAnimator.Play(Animator.StringToHash("Idle"), 0, 0f);
					_state = SlidArrowState.Introduction;
				}
			}
		}

		public void SetOptionValue(int speed)
		{
			_slideSpeed = speed;
		}

		public void SetRotateSpeed(float speed)
		{
			float num = 1000f / (speed / 60f) * 4f;
			_starRotateSpeed = 60f / (num / 1000f * 60f);
			if (_starRotateAnimator != null && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				_starRotateAnimator.SetFloat("Speed", _starRotateSpeed);
			}
		}

		public override void AnimationReset()
		{
			base.AnimationReset();
			_starRotateAnimator.Rebind();
			_starRotateAnimator.SetFloat("Speed", _starRotateSpeed);
		}

		public override void SetSpeed(float speed)
		{
			base.SetSpeed(speed);
			_state = SlidArrowState.Introduction;
			float num = 1000f / (speed / 60f) * 4f;
			float num2 = num * 2f / 21f * (float)_slideSpeed;
			if (num2 < num)
			{
				_showRate = num2 / num;
				_isCheckStageing = false;
			}
			else
			{
				_showRate = num2 / num - 1f;
				_isCheckStageing = true;
			}
		}
	}
}
