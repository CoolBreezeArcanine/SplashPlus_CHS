using System.Collections;
using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindowController : MonoBehaviour
{
	[SerializeField]
	[Header("トグルスイッチ")]
	private Animator[] _toggleSwitchAnimators;

	[SerializeField]
	[Header("無効表示")]
	private CanvasRenderer[] _disableObjects;

	[SerializeField]
	[Header("次回予告")]
	private CanvasRenderer[] _nextTimeObjects;

	[SerializeField]
	[Header("ヘッドホン設定")]
	private Image _volumeGaugeImage;

	[SerializeField]
	private TextMeshProUGUI _volumeDigitText;

	[SerializeField]
	private Animator _plusButton;

	[SerializeField]
	private Animator _minusButton;

	private Animator _windowAnimator;

	private static readonly int Sync = Animator.StringToHash("Sync");

	private float syncTime;

	public void Initialize()
	{
		_windowAnimator = GetComponent<Animator>();
		_windowAnimator.Play("Out", 0, 1f);
		_plusButton.Play("Loop", 0, 0f);
		_minusButton.Play("Loop", 0, 0f);
	}

	public void ViewUpdate(float add)
	{
		syncTime += add / 1000f;
		_plusButton.SetFloat(Sync, syncTime);
		_minusButton.SetFloat(Sync, syncTime);
		if (1f < syncTime)
		{
			syncTime = 0f;
		}
	}

	public void SetActive(int index, bool active)
	{
		_disableObjects[index].SetAlpha((!active) ? 1 : 0);
	}

	public void SetVolume(OptionHeadphonevolumeID volume)
	{
		_volumeGaugeImage.fillAmount = volume.GetValue();
		_volumeDigitText.text = volume.GetName();
	}

	public void Open()
	{
		_windowAnimator.Play("In", 0, 0f);
	}

	public void Close()
	{
		_windowAnimator.Play("Out", 0, 0f);
	}

	public void SetSettingState(int index, bool state, bool nextTime)
	{
		_toggleSwitchAnimators[index].Play(state ? "On_Loop" : "Off_Loop", 0, 0f);
		_nextTimeObjects[index].SetAlpha(nextTime ? 1 : 0);
	}

	public void PressedToggle(int index, bool toState)
	{
		_toggleSwitchAnimators[index].Play(toState ? "Off_On" : "On_Off", 0, 0f);
	}

	public void PressedPlusButton()
	{
		_plusButton.Play("Press", 0, 0f);
		StartCoroutine(WaitForAnimation(_plusButton));
	}

	public void PressedMinusButton()
	{
		_minusButton.Play("Press", 0, 0f);
		StartCoroutine(WaitForAnimation(_minusButton));
	}

	public void HoldPlusButton()
	{
		_plusButton.Play("Hold", 0, 0f);
	}

	public void HoldMinusButton()
	{
		_minusButton.Play("Hold", 0, 0f);
	}

	private IEnumerator WaitForAnimation(Animator animator)
	{
		yield return null;
		yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
		animator.Play("Loop", 0);
	}

	public bool IsDone()
	{
		return 1f <= _windowAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}
}
