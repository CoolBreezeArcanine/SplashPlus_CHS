using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : CommonButtonObject
{
	[SerializeField]
	private Image _leftArrow;

	[SerializeField]
	private Image _rightArrow;

	public void Initialize(int monitorIndex)
	{
		MonitorIndex = monitorIndex;
		IsButtonVisible = true;
		if (base.gameObject.activeInHierarchy && IsButtonVisible)
		{
			ButtonAnimator.SetTrigger("In");
		}
	}

	public void UseRightArrow()
	{
		_leftArrow?.gameObject.SetActive(value: false);
		_rightArrow?.gameObject.SetActive(value: true);
	}

	public void UseLeftArrow()
	{
		_leftArrow?.gameObject.SetActive(value: true);
		_rightArrow?.gameObject.SetActive(value: false);
	}

	public override void Pressed()
	{
		if (base.gameObject.activeInHierarchy)
		{
			ButtonAnimator.SetTrigger("Pressed");
			SoundManager.PlaySE(Cue.SE_SYS_TAB, MonitorIndex);
			ParticleControler?.SetTransform(EffectTransform.position);
			ParticleControler?.Play();
		}
	}
}
