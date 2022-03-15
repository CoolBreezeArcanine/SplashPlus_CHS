using Monitor.MapResult.Parts;
using Process;
using TMPro;
using UI;
using UnityEngine;

namespace Monitor.CharacterSelect
{
	public class CharacterStateObject : MonoBehaviour
	{
		private const string TypeB = "Loop_Color_B";

		[SerializeField]
		[Header("距離表示")]
		private OdoSpriteText _distanceCounter;

		[SerializeField]
		private TextMeshProUGUI _nameText;

		[SerializeField]
		[Header("得意苦手表示")]
		private MultipleImage _stateImage;

		[SerializeField]
		[Header("得意苦手表示")]
		private MultipleImage _textImage;

		[SerializeField]
		[Header("得意苦手表示")]
		private MultipleImage _arrowImage;

		[SerializeField]
		private Animator _changeArrowAnimator;

		[SerializeField]
		private GameObject _weakPointObject;

		[SerializeField]
		[Header("EmotionIcon")]
		private InstantiateGenerator _emotionIconGenerator;

		[SerializeField]
		[Header("NewObject")]
		private InstantiateGenerator _newObjectGenerator;

		[SerializeField]
		private InstantiateGenerator _characterGenerator;

		[SerializeField]
		[Header("数字色")]
		private ImageColorGroup _colorGroup;

		[SerializeField]
		[Header("ブランク表示")]
		private GameObject _blankObject;

		private CharaParts _charaParts;

		private Animator _instanceEmotionAnimator;

		public void Initialize()
		{
			_instanceEmotionAnimator = _emotionIconGenerator.Instantiate<Animator>();
			_charaParts = _characterGenerator.Instantiate<CharaParts>();
		}

		public void SetParts(Sprite backBase, Sprite face, Sprite frame, Sprite levelBase, Color shadowColor)
		{
			if (_charaParts != null)
			{
				_charaParts.SetCharacter(backBase, face, frame, levelBase, shadowColor);
				_charaParts.PlayJoinParty();
				if (_blankObject != null)
				{
					_blankObject.SetActive(value: false);
				}
			}
		}

		public void SetData(string charaName, int distance, WeakPoint weakPoint, CharacterSelectProces.ArrowDirection arrow)
		{
			_nameText.text = charaName;
			_stateImage.ChangeSprite((int)weakPoint);
			_textImage.ChangeSprite((int)weakPoint);
			_colorGroup.SetColor((int)weakPoint);
			if (_arrowImage != null && !_arrowImage.gameObject.activeSelf)
			{
				_arrowImage.gameObject.SetActive(value: true);
			}
			if (_distanceCounter != null && !_distanceCounter.gameObject.activeSelf)
			{
				_distanceCounter.gameObject.SetActive(value: true);
			}
			_distanceCounter?.SetOdoText(distance);
			SetArrowDirection(arrow);
			if (_weakPointObject != null && !_weakPointObject.gameObject.activeSelf)
			{
				_weakPointObject?.gameObject.SetActive(value: true);
			}
			if (_changeArrowAnimator != null)
			{
				_changeArrowAnimator.Play(Animator.StringToHash("Loop_Color_B"));
			}
			if (_instanceEmotionAnimator != null)
			{
				string text = ((weakPoint == WeakPoint.Weak) ? "Loop_DOWN" : "Loop_UP");
				_instanceEmotionAnimator.Play(Animator.StringToHash(text), 0, 0f);
			}
			if (_blankObject != null)
			{
				_blankObject.SetActive(value: false);
			}
		}

		public void SetArrowDirection(CharacterSelectProces.ArrowDirection arrow)
		{
			_arrowImage?.ChangeSprite((int)arrow);
		}

		public void SetBlank()
		{
			if (_charaParts != null)
			{
				_charaParts.SetBlank();
			}
			if (_blankObject != null)
			{
				_blankObject.SetActive(value: true);
			}
			_nameText.text = "‐‐‐";
			if (_arrowImage != null)
			{
				_arrowImage.gameObject.SetActive(value: false);
			}
			if (_weakPointObject != null)
			{
				_weakPointObject.gameObject.SetActive(value: false);
			}
			_distanceCounter?.SetOdoText(0);
			_distanceCounter?.gameObject.SetActive(value: false);
		}
	}
}
