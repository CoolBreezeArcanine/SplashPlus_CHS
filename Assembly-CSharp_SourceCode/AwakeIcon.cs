using UI;
using UnityEngine;
using UnityEngine.UI;

public class AwakeIcon : MonoBehaviour
{
	[SerializeField]
	private Image _baseImage;

	[SerializeField]
	[Header("星")]
	private Image _starObject;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	[Header("キャラセット用")]
	private InstantiateGenerator _starEffectGenerator;

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

	public void Prepare(Sprite sp, bool isActive)
	{
		if (_starEffectGenerator != null && _starEffectAnimator == null)
		{
			_starEffectAnimator = _starEffectGenerator.Instantiate<Animator>();
			_starEffectAnimator?.gameObject.SetActive(value: false);
		}
		_starEffectAnimator?.gameObject.SetActive(value: false);
		SetStarType(isCounterStop: false);
		SetBase(sp);
		SetVisible(isActive);
	}

	public void AwakePrepare(Sprite sp, bool isActive)
	{
		_isCharaSelectCenterChain = false;
		if (_starEffectGenerator != null && _starEffectAnimator == null)
		{
			_starEffectAnimator = _starEffectGenerator.Instantiate<Animator>();
			_starEffectAnimator?.gameObject.SetActive(value: false);
		}
		_animator = base.gameObject.GetComponent<Animator>();
		_starEffectAnimator?.gameObject.SetActive(value: false);
		SetStarType(isCounterStop: false);
		SetBase(sp);
		SetVisible(isActive);
		if (!isActive)
		{
			HideStar();
		}
	}

	public void SetVisible(bool isActive)
	{
		_isStarActive = isActive;
		base.gameObject.SetActive(value: true);
		if (_starObject != null)
		{
			_starObject.gameObject.SetActive(value: true);
		}
		if (_isCharaSelectCenterChain)
		{
			_baseImage.gameObject.SetActive(value: true);
		}
		if (!isActive && _animator != null)
		{
			_animator.Play("Idle");
			return;
		}
		if (!isActive)
		{
			base.gameObject.SetActive(isActive);
			return;
		}
		if (_starObject != null)
		{
			_starObject.gameObject.SetActive(isActive);
		}
		if (_isCharaSelectCenterChain)
		{
			_baseImage.gameObject.SetActive(isActive);
		}
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
		_starObject.sprite = (isCounterStop ? _counterStopStarSprite : _awakeingStarSprite);
	}

	private void SetBase(Sprite sp)
	{
		if (sp != null)
		{
			_baseImage.sprite = sp;
		}
	}

	public void OnCenterIn()
	{
		if (_isStarActive && !(_starObject == null) && _starObject.gameObject.activeSelf)
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
		_starObject?.gameObject.SetActive(value: false);
	}

	public void AwakeStar()
	{
		_starEffectAnimator?.gameObject.SetActive(value: true);
		_starEffectAnimator?.Play(Animator.StringToHash("GetStar_Eff"), 0, 0f);
		_starObject?.gameObject.SetActive(value: true);
		_animator?.Play(Animator.StringToHash("GetStar"), 0, 0f);
	}
}
