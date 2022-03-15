using UnityEngine;

namespace Monitor
{
	public class WarningMonitor : MonitorBase
	{
		private const string WarnAnimationName = "InOut";

		private Animator _warnAnimator;

		public override void Initialize(int monIndex, bool active)
		{
			_warnAnimator = Main.transform.Find("WarningImage").GetComponent<Animator>();
			base.Initialize(monIndex, active);
		}

		public override void ViewUpdate()
		{
		}

		public bool IsLogoAnimationEnd()
		{
			return _warnAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
		}

		public void SetLogoObjectActive(bool active)
		{
			_warnAnimator.gameObject.SetActive(active);
		}

		public void PlayLogo()
		{
			_warnAnimator.Play("InOut", 0, 0f);
		}
	}
}
