using UnityEngine;

namespace Monitor.MusicSelect
{
	public class StarRotationPreviewObject : NoteSpeedPreviewObject
	{
		[SerializeField]
		private Animator _starRotateAnimator;

		public override void AnimationReset()
		{
			base.AnimationReset();
			_starRotateAnimator.Rebind();
		}

		public void SetRotateSpeed(float speed)
		{
			float num = 1000f / (speed / 60f) * 4f;
			float value = 60f / (num / 1000f * 60f);
			if (_starRotateAnimator != null && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				_starRotateAnimator.SetFloat("Speed", value);
			}
		}
	}
}
