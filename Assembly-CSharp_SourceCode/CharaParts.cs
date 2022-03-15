using System;
using Monitor.MapResult.Parts;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class CharaParts : MonoBehaviour
{
	[SerializeField]
	private Image _baseImage;

	[SerializeField]
	private MultiImage _faceImage;

	[SerializeField]
	private MultiImage _effectImage;

	[SerializeField]
	private Image _frameImage;

	[SerializeField]
	private Image _levelImage;

	[SerializeField]
	private MultiImage _charaShadow;

	[SerializeField]
	[Header("デコレート")]
	private GameObject _decorateObject;

	[SerializeField]
	private AwakeIconController _awakeIconController;

	[SerializeField]
	private OdoSpriteText _levelOdoSprite;

	[SerializeField]
	private Animator _levelPopUpAnimator;

	[SerializeField]
	private GameObject _metreArrowObj;

	[SerializeField]
	[Header("本体のアニメーター")]
	private Animator _animator;

	[SerializeField]
	private Animator _levelUpAnimator;

	[SerializeField]
	[Header("リーダー")]
	private GameObject _leaderObject;

	private StateAnimController _stateController;

	private GameObject _releaseMarkObj;

	private Animator _effectAnim;

	private int _nextLevel;

	public void SetParts(Sprite bg, Sprite face, Sprite frame, Sprite levelSp, Sprite starSmallBase, Sprite starLargeBase, float gaugeAmount, int level, int awakeNum, Color shadowColor, bool isLeader = false)
	{
		SetCharacter(bg, face, frame, levelSp, shadowColor);
		_awakeIconController.Prepare(starSmallBase, starLargeBase, gaugeAmount, awakeNum);
		if (_levelPopUpAnimator != null)
		{
			_levelPopUpAnimator.Play("Idle");
		}
		_stateController = _animator.GetBehaviour<StateAnimController>();
		_leaderObject.SetActive(isLeader);
		SetLevel(level);
	}

	public void SetCharacter(Sprite bg, Sprite face, Sprite frame, Sprite levelSp, Color shadowColor)
	{
		_leaderObject.SetActive(value: false);
		_baseImage.gameObject.SetActive(bg != null);
		_baseImage.sprite = bg;
		_frameImage.gameObject.SetActive(frame != null);
		_frameImage.sprite = frame;
		_levelImage.sprite = levelSp;
		if (_faceImage != null)
		{
			Sprite sprite3 = (_faceImage.sprite = (_effectImage.sprite = face));
			if (_charaShadow != null)
			{
				_charaShadow.color = shadowColor;
				_charaShadow.Image2 = _faceImage.sprite;
			}
		}
	}

	public void SetVisivleMetreArrow(bool isActive)
	{
		if (_metreArrowObj != null)
		{
			_metreArrowObj.SetActive(isActive);
		}
	}

	public void SetReleaseMark(GameObject obj, Animator effect, int slotIndex)
	{
		_releaseMarkObj = obj;
		_releaseMarkObj.SetActive(value: false);
		_effectAnim = effect;
		if (_metreArrowObj != null)
		{
			_metreArrowObj.transform.localEulerAngles = Vector3.zero;
			_metreArrowObj.transform.Rotate(0f, 0f, 50f + (float)slotIndex * -50f * 0.5f);
		}
	}

	public void SetLevel(int level)
	{
		if (_levelOdoSprite != null)
		{
			_levelOdoSprite.SetOdoText(level);
		}
	}

	public void SetVisibleDecorate(bool isVisible)
	{
		if (_decorateObject != null)
		{
			_decorateObject.SetActive(isVisible);
		}
		if (isVisible)
		{
			_animator.Play(Animator.StringToHash("Chara_Active_Party_Loop"), 0, 0f);
		}
		else
		{
			_animator.Play(Animator.StringToHash("Chara_Active_Join_Loop"), 0, 0f);
		}
	}

	public void SetBlank()
	{
		_frameImage.gameObject.SetActive(value: false);
		if (_animator.gameObject.activeSelf && _animator.gameObject.activeInHierarchy)
		{
			_leaderObject.SetActive(value: false);
			_animator.Play("Chara_NoActive");
		}
	}

	public void PlayLevelUp()
	{
		_levelUpAnimator.Play("LevelUP");
		SetLevel(_nextLevel);
	}

	public void PlayJoinParty()
	{
		_animator.Play("Chara_Active_Join_In");
		_effectAnim?.Play("In");
	}

	public void PlayJoinParty(Action next)
	{
		_animator.Play("Chara_Active_Join_In");
		_effectAnim?.Play("In");
		int hashCode = Animator.StringToHash("Base Layer.Chara_Active_Join_In");
		SetExit(hashCode, next);
	}

	public void SetExit(int hashCode, Action action)
	{
		_stateController.SetExitParts(action, hashCode);
	}

	public void Pressed()
	{
		_animator?.SetTrigger("Pressed");
	}

	public void Loop()
	{
		_animator?.SetTrigger("Loop");
	}
}
