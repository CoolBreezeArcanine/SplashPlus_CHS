using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class FavoriteButton : CommonButtonObject
{
	[SerializeField]
	private Image _baseImage;

	[SerializeField]
	private Image _mainImage;

	[SerializeField]
	private Shadow _shadow;

	[SerializeField]
	private Image _bShadowImage;

	[SerializeField]
	private Image _aBaseShadowImage;

	[SerializeField]
	[Header("シンボルスプライト")]
	private Sprite[] _symbolSprites;

	[SerializeField]
	[Header("AType")]
	private Sprite[] _typeASprites;

	[SerializeField]
	[Header("BType")]
	private Sprite[] _typeBSprites;

	[SerializeField]
	[Header("ShadowEffectColor")]
	private Color[] _effectColors;

	public void Initialize(int monitorIndex)
	{
		MonitorIndex = monitorIndex;
	}

	public void ChangeString(bool isFavorite)
	{
		if (isFavorite)
		{
			SetSymbol(_symbolSprites[0], isFlip: false);
			Sprite sprite3 = (_baseImage.sprite = (_bShadowImage.sprite = _typeBSprites[0]));
			sprite3 = (_mainImage.sprite = (_aBaseShadowImage.sprite = _typeASprites[0]));
			_shadow.effectColor = _effectColors[0];
		}
		else
		{
			SetSymbol(_symbolSprites[1], isFlip: false);
			Sprite sprite3 = (_baseImage.sprite = (_bShadowImage.sprite = _typeBSprites[1]));
			sprite3 = (_mainImage.sprite = (_aBaseShadowImage.sprite = _typeASprites[1]));
			_shadow.effectColor = _effectColors[1];
		}
	}

	public void ImmediateChangeString(bool isFavorite)
	{
		if (isFavorite)
		{
			SetSymbolSprite(_symbolSprites[0], isFlip: false);
			Sprite sprite3 = (_baseImage.sprite = (_bShadowImage.sprite = _typeBSprites[0]));
			sprite3 = (_mainImage.sprite = (_aBaseShadowImage.sprite = _typeASprites[0]));
			_shadow.effectColor = _effectColors[0];
		}
		else
		{
			SetSymbolSprite(_symbolSprites[1], isFlip: false);
			Sprite sprite3 = (_baseImage.sprite = (_bShadowImage.sprite = _typeBSprites[1]));
			sprite3 = (_mainImage.sprite = (_aBaseShadowImage.sprite = _typeASprites[1]));
			_shadow.effectColor = _effectColors[1];
		}
	}

	public override void SetSymbol(Sprite synbolSprite, bool isFlip)
	{
		SetSymbolSprite(synbolSprite, isFlip);
		IsButtonVisible = true;
		if (base.gameObject.activeInHierarchy && IsButtonVisible)
		{
			ButtonAnimator.SetTrigger("In");
		}
	}

	public override void Pressed()
	{
		if (base.gameObject.activeInHierarchy)
		{
			ButtonAnimator.SetTrigger("Pressed");
			SoundManager.PlaySE(Cue.SE_SYS_CURSOR, MonitorIndex);
			ParticleControler?.SetTransform(EffectTransform.position);
			ParticleControler?.Play();
		}
	}

	public StateAnimController GetBehaviour()
	{
		return ButtonAnimator?.GetBehaviour<StateAnimController>();
	}
}
