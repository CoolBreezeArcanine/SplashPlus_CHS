using UI;
using UnityEngine;
using UnityEngine.UI;

public class AwakeCenterIcon : MonoBehaviour
{
	[SerializeField]
	[Header("Base")]
	private Image _baseImage;

	[SerializeField]
	[Header("覚醒ゲージ")]
	private Image _maskImage;

	[SerializeField]
	[Header("星")]
	private Image _starObj;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	[Header("星演出")]
	private InstantiateGenerator _starEffectGenerator;

	[SerializeField]
	[Header("覚醒時演出")]
	private InstantiateGenerator _awakeEffectGenerator;

	[SerializeField]
	private bool _isCharaSelectCenterChain;

	[SerializeField]
	[Header("覚醒星")]
	private Sprite _awakeingStarSprite;

	[SerializeField]
	[Header("カンスト星")]
	private Sprite _counterStopStarSprite;

	private bool _isStarActive;

	private Animator _starEffectAnimator;

	private Animator _awakeEffectAnimator;

	public void Prepare(Sprite baseSp, float gaugeAmount)
	{
		if (_starEffectGenerator != null && _starEffectAnimator == null)
		{
			_starEffectAnimator = _starEffectGenerator.Instantiate<Animator>();
			_starEffectAnimator?.gameObject.SetActive(value: false);
		}
		if (_awakeEffectGenerator != null && _awakeEffectAnimator == null)
		{
			_awakeEffectAnimator = _awakeEffectGenerator.Instantiate<Animator>();
			_awakeEffectAnimator?.gameObject.SetActive(value: false);
		}
		if (baseSp != null)
		{
			_baseImage.sprite = baseSp;
		}
		SetGaugeAmount(gaugeAmount);
		SetVisibleStar(isActive: false);
	}

	public void AwakePrepare(Sprite baseSp, float gaugeAmount)
	{
		_isCharaSelectCenterChain = false;
		if (_starEffectGenerator != null && _starEffectAnimator == null)
		{
			_starEffectAnimator = _starEffectGenerator.Instantiate<Animator>();
			_starEffectAnimator?.gameObject.SetActive(value: false);
		}
		if (_awakeEffectGenerator != null && _awakeEffectAnimator == null)
		{
			_awakeEffectAnimator = _awakeEffectGenerator.Instantiate<Animator>();
			_awakeEffectAnimator?.gameObject.SetActive(value: false);
		}
		_animator = base.gameObject.GetComponent<Animator>();
		if (baseSp != null)
		{
			_baseImage.sprite = baseSp;
		}
		SetGaugeAmount(gaugeAmount);
		SetVisibleStar(isActive: false);
		HideStar();
	}

	public void SetGaugeAmount(float currentValue)
	{
		_maskImage.fillAmount = currentValue;
	}

	public void SetVisibleStar(bool isActive)
	{
		_isStarActive = isActive;
		_starObj.gameObject.SetActive(value: true);
		if (_isCharaSelectCenterChain)
		{
			_baseImage.gameObject.SetActive(value: true);
		}
		if (!isActive && _animator != null)
		{
			_animator.Play("Idle");
			return;
		}
		_starObj.gameObject.SetActive(isActive);
		if (isActive)
		{
			if (_isCharaSelectCenterChain)
			{
				Animator starEffectAnimator = _starEffectAnimator;
				if ((object)starEffectAnimator != null && !starEffectAnimator.gameObject.activeSelf)
				{
					_starEffectAnimator.gameObject.SetActive(value: true);
				}
				_starEffectAnimator?.Play(Animator.StringToHash("GetStar_Eff"));
			}
		}
		else
		{
			_starEffectAnimator?.gameObject.SetActive(value: false);
		}
	}

	public void SetStarType(bool isCounterStop)
	{
		_starObj.sprite = (isCounterStop ? _counterStopStarSprite : _awakeingStarSprite);
	}

	public void AnimCenterSpark()
	{
		_awakeEffectAnimator?.gameObject.SetActive(value: true);
		_awakeEffectAnimator?.Play("SpartkStar_Eff");
	}

	public void OnCenterIn()
	{
		if (_isStarActive)
		{
			Animator starEffectAnimator = _starEffectAnimator;
			if ((object)starEffectAnimator != null && !starEffectAnimator.gameObject.activeSelf)
			{
				_starEffectAnimator.gameObject.SetActive(value: true);
			}
			_starEffectAnimator?.Play(Animator.StringToHash("GetStar_Eff"), 0, 0f);
			if (null != _animator)
			{
				_animator?.Play(Animator.StringToHash("GetStar"), 0, 0f);
			}
		}
	}

	public void OnCenterOut()
	{
	}

	public void HideStar()
	{
		_starObj?.gameObject.SetActive(value: false);
	}

	public void AwakeStar()
	{
		Animator starEffectAnimator = _starEffectAnimator;
		if ((object)starEffectAnimator != null && !starEffectAnimator.gameObject.activeSelf)
		{
			_starEffectAnimator.gameObject.SetActive(value: true);
		}
		_starObj?.gameObject.SetActive(value: true);
		_starEffectAnimator?.Play(Animator.StringToHash("GetStar_Eff"), 0, 0f);
		_animator?.Play(Animator.StringToHash("GetStar"), 0, 0f);
	}

	public void AwakeStarDisp(float gaugeAmount)
	{
		SetGaugeAmount(gaugeAmount);
	}
}
