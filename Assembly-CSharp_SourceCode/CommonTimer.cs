using Mai2.Mai2Cue;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CommonTimer : MonoBehaviour
{
	private SpriteCounter _normaiTime;

	private SpriteCounter _limitTime;

	private Image _infinityImage;

	private Animator _animator;

	private bool _isPlaySound;

	private uint _preDispTime;

	public const int TimeMax = 99;

	public int MonitorIndex { get; set; }

	private void Awake()
	{
		_normaiTime = base.transform.Find("NormalNum").gameObject.GetComponent<SpriteCounter>();
		_limitTime = base.transform.Find("LimitNum").gameObject.GetComponent<SpriteCounter>();
		_infinityImage = base.transform.Find("Infinity").gameObject.GetComponent<Image>();
		_animator = base.gameObject.GetComponent<Animator>();
		_preDispTime = 0u;
	}

	public void SetVisible(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
		_isPlaySound = isVisible;
	}

	private void SetActiveNormalTime(bool isActive)
	{
		if (_normaiTime.gameObject.activeSelf != isActive)
		{
			_normaiTime.gameObject.SetActive(isActive);
		}
	}

	private void SetActiveLimitTime(bool isActive)
	{
		if (_limitTime.gameObject.activeSelf != isActive)
		{
			_limitTime.gameObject.SetActive(isActive);
		}
	}

	public void SetTimerCount(uint dispTime, bool isInfinity = false)
	{
		if (isInfinity)
		{
			SetActiveNormalTime(isActive: false);
			SetActiveLimitTime(isActive: false);
			if (!GameManager.IsFreedomMode)
			{
				_infinityImage.gameObject.SetActive(value: true);
			}
		}
		else if (dispTime >= 10)
		{
			if (dispTime > 99)
			{
				dispTime = 99u;
			}
			SetActiveNormalTime(isActive: true);
			SetActiveLimitTime(isActive: false);
			_infinityImage.gameObject.SetActive(value: false);
			if (_isPlaySound)
			{
				_normaiTime.ChangeText(dispTime.ToString());
			}
		}
		else
		{
			SetActiveNormalTime(isActive: false);
			SetActiveLimitTime(isActive: true);
			_infinityImage.gameObject.SetActive(value: false);
			if (_isPlaySound)
			{
				_limitTime.ChangeText(dispTime.ToString());
			}
		}
		if (_preDispTime != dispTime)
		{
			switch (dispTime)
			{
			case 10u:
			case 20u:
			case 30u:
				if (_isPlaySound)
				{
					SoundManager.PlaySE(Cue.SE_SYS_COUNT_30_20_10, MonitorIndex);
				}
				break;
			case 0u:
			case 1u:
			case 2u:
			case 3u:
			case 4u:
			case 5u:
			case 6u:
			case 7u:
			case 8u:
			case 9u:
				if (dispTime >= 0)
				{
					if (_isPlaySound)
					{
						SoundManager.PlaySE(Cue.SE_SYS_COUNT, MonitorIndex);
					}
					if (_animator.gameObject.activeSelf && _animator.gameObject.activeInHierarchy)
					{
						_animator.Play("LimitAction", 0, 0f);
					}
				}
				break;
			}
		}
		_preDispTime = dispTime;
	}
}
