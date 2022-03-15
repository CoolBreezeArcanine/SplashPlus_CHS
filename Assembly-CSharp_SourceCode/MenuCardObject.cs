using System.Collections.Generic;
using DB;
using Manager;
using Monitor.MusicSelect.ChainList;
using Monitor.MusicSelect.UI;
using Process;
using TMPro;
using UI;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

public class MenuCardObject : ChainObject
{
	private MusicSelectProcess.MenuType _type;

	[SerializeField]
	private List<GameObject> _menuObjectList;

	[SerializeField]
	private List<Animator> _cardAnimators;

	[SerializeField]
	[Header("キャラクターセレクト")]
	private TextMeshProUGUI _characterSelectMessage;

	[SerializeField]
	private Image _dotBackImage;

	[SerializeField]
	private Image _dotImage;

	[SerializeField]
	private Color _dotBackActiveColor = Color.white;

	[SerializeField]
	private Color _dotActiveColor = Color.white;

	[SerializeField]
	private Color _dotBackDeactivateColor = Color.white;

	[SerializeField]
	private Color _dotDeactivateColor = Color.white;

	[SerializeField]
	private Color _textActiveColor = Color.white;

	[SerializeField]
	private Color _textDeactivateColor = Color.white;

	[SerializeField]
	private Color _textOutlineDeactivateColor;

	[SerializeField]
	[Header("Volume")]
	private Image _volumeGauge;

	[SerializeField]
	private TextMeshProUGUI _volumeText;

	[SerializeField]
	private SpriteCounter[] _volumeCounter;

	[SerializeField]
	[Header("オプションカード")]
	private OptionCardGroupObject _optionCard;

	[SerializeField]
	private TextMeshProUGUI[] _speedText;

	[SerializeField]
	private TextMeshProUGUI _detilsText;

	[SerializeField]
	[Header("カードボタン")]
	private Animator[] _rightButtonAnimators;

	[SerializeField]
	private Animator[] _leftButtonAnimators;

	[SerializeField]
	[Header("どっとUVスクロール")]
	private MultiImage[] _dotBackImages;

	[SerializeField]
	private Transform _MusicChainCardParent;

	[SerializeField]
	private Transform _MatchingChainCardParent;

	[SerializeField]
	private MusicChainCardObejct _originalMusicChainCardObejct;

	[SerializeField]
	private MatchingChainCardObject _originalMatchingChainCardObejct;

	[SerializeField]
	private TextMeshProUGUI _volText;

	private float _duration;

	private float _normalizeTime;

	private float _lastTime;

	private bool _isHold;

	private Rect _rect;

	public MusicChainCardObejct MusicCardObject { get; private set; }

	public MatchingChainCardObject MatchingCardObject { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		MusicCardObject = Object.Instantiate(_originalMusicChainCardObejct, _MusicChainCardParent);
		MatchingCardObject = Object.Instantiate(_originalMatchingChainCardObejct, _MatchingChainCardParent);
		_volText.text = CommonMessageID.MusicSelectOptionVol.GetName();
	}

	public void Initialize(MusicSelectProcess.MenuType menuType)
	{
		_rect = new Rect(0f, 0f, 1f, 1f);
		for (int i = 0; i < _menuObjectList.Count; i++)
		{
			if (i != (int)menuType)
			{
				_menuObjectList[i].SetActive(value: false);
			}
		}
		_ = 1;
		for (int j = 0; j < _rightButtonAnimators.Length; j++)
		{
			if (_rightButtonAnimators[j].gameObject.activeInHierarchy)
			{
				_rightButtonAnimators[j].Rebind();
				_rightButtonAnimators[j].Play("Loop");
				_duration = _rightButtonAnimators[j].GetCurrentAnimatorStateInfo(0).length * 1000f;
			}
			if (_leftButtonAnimators[j].gameObject.activeInHierarchy)
			{
				_leftButtonAnimators[j].Rebind();
				_leftButtonAnimators[j].Play("Loop");
			}
		}
		_type = menuType;
	}

	public MusicSelectProcess.MenuType GetMenuType()
	{
		return _type;
	}

	public override void OnCenterIn()
	{
		switch (_type)
		{
		case MusicSelectProcess.MenuType.Option:
		case MusicSelectProcess.MenuType.Volume:
		{
			int type = (int)_type;
			if (_menuObjectList[type].activeSelf)
			{
				_cardAnimators[type].SetTrigger("OnCenterIn");
			}
			break;
		}
		}
		if (MusicCardObject.gameObject.activeSelf)
		{
			MusicCardObject.OnCenterIn();
		}
	}

	public override void OnCenterOut()
	{
		switch (_type)
		{
		case MusicSelectProcess.MenuType.Option:
		case MusicSelectProcess.MenuType.Volume:
		{
			int type = (int)_type;
			if (_menuObjectList[type].activeSelf)
			{
				_cardAnimators[type].SetTrigger("OnCenterOut");
			}
			break;
		}
		}
		if (MusicCardObject.gameObject.activeSelf)
		{
			MusicCardObject.OnCenterOut();
		}
		if (MatchingCardObject.gameObject.activeSelf)
		{
			MatchingCardObject.OnCenterOut();
		}
	}

	public override void ResetChain()
	{
		MusicCardObject?.ResetChain();
		MatchingCardObject?.ResetChain();
	}

	public void ChangeOptionCard(OptionKindID kind)
	{
		_optionCard.ChangeState(kind);
	}

	public void SetVolume(OptionHeadphonevolumeID volume, float volumeAmount)
	{
		int num = int.Parse(volume.GetName());
		if (num >= 10)
		{
			_volumeCounter[0].gameObject.SetActive(value: false);
			_volumeCounter[1].gameObject.SetActive(value: true);
			_volumeCounter[1].ChangeText(string.Concat(num));
		}
		else
		{
			_volumeCounter[0].gameObject.SetActive(value: true);
			_volumeCounter[1].gameObject.SetActive(value: false);
			_volumeCounter[0].ChangeText(string.Concat(num));
		}
		_volumeGauge.fillAmount = volumeAmount;
	}

	public override void ViewUpdate(float syncTimer)
	{
		_normalizeTime += GameManager.GetGameMSecAdd();
		float num = _normalizeTime / _duration;
		MultiImage[] dotBackImages = _dotBackImages;
		for (int i = 0; i < dotBackImages.Length; i++)
		{
			dotBackImages[i].UVScale = _rect;
		}
		_rect.x = syncTimer;
		for (int j = 0; j < _rightButtonAnimators.Length; j++)
		{
			if (_rightButtonAnimators[j].gameObject.activeInHierarchy)
			{
				_rightButtonAnimators[j].SetFloat("SyncTimer", num);
			}
			if (_leftButtonAnimators[j].gameObject.activeInHierarchy)
			{
				_leftButtonAnimators[j].SetFloat("SyncTimer", num);
			}
		}
		if (num > 1f)
		{
			_normalizeTime = 0f;
		}
		if (_isHold && (float)GameManager.GetGameMSec() - _lastTime > 200f)
		{
			_isHold = false;
			for (int k = 0; k < _rightButtonAnimators.Length; k++)
			{
				if (_rightButtonAnimators[k].gameObject.activeInHierarchy)
				{
					_rightButtonAnimators[k].SetTrigger("Loop");
				}
				if (_leftButtonAnimators[k].gameObject.activeInHierarchy)
				{
					_leftButtonAnimators[k].SetTrigger("Loop");
				}
			}
		}
		if (MusicCardObject.gameObject.activeSelf)
		{
			MusicCardObject.ViewUpdate(syncTimer);
		}
		base.ViewUpdate(syncTimer);
	}

	public void PressedButton(Direction direction)
	{
		_lastTime = GameManager.GetGameMSec();
		Animator[] rightButtonAnimators;
		if (direction == Direction.Right)
		{
			rightButtonAnimators = _rightButtonAnimators;
			foreach (Animator animator in rightButtonAnimators)
			{
				if (animator.gameObject.activeInHierarchy)
				{
					if (_isHold)
					{
						animator.SetTrigger(Animator.StringToHash("Hold"));
						continue;
					}
					_isHold = true;
					animator.SetTrigger("Pressed");
				}
			}
			return;
		}
		rightButtonAnimators = _leftButtonAnimators;
		foreach (Animator animator2 in rightButtonAnimators)
		{
			if (animator2.gameObject.activeInHierarchy)
			{
				if (_isHold)
				{
					animator2.SetTrigger("Hold");
					continue;
				}
				_isHold = true;
				animator2.SetTrigger("Pressed");
			}
		}
	}

	public void SetSpeed(string speed)
	{
		TextMeshProUGUI[] speedText = _speedText;
		for (int i = 0; i < speedText.Length; i++)
		{
			speedText[i].text = CommonMessageID.MusicSelectOptionMenuSpeed.GetName() + speed;
		}
	}

	public void SetCustomDetils(string mirror, string trackSkip)
	{
		_detilsText.text = CommonMessageID.MusicSelectOptionMenuMirror.GetName() + mirror + "\n" + CommonMessageID.MusicSelectOptionMenuTrackSkip.GetName() + trackSkip;
	}

	public void SetCharacterSelectMessage(bool isActive, string message)
	{
		_characterSelectMessage.text = message;
		_characterSelectMessage.color = (isActive ? _textActiveColor : _textDeactivateColor);
		_characterSelectMessage.outlineColor = (isActive ? Color.white : _textOutlineDeactivateColor);
		_dotBackImage.color = (isActive ? _dotBackActiveColor : _dotBackDeactivateColor);
		_dotImage.color = (isActive ? _dotActiveColor : _dotDeactivateColor);
	}
}
