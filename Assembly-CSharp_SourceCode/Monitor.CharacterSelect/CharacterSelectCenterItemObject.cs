using Monitor.MapResult.Parts;
using Process;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.CharacterSelect
{
	public class CharacterSelectCenterItemObject : MonoBehaviour
	{
		[SerializeField]
		private Image _charaImage;

		[SerializeField]
		private Image _baseImage;

		[SerializeField]
		private Image _frameImage;

		[SerializeField]
		private Image _levelBaseImage;

		[SerializeField]
		private OdoSpriteText _levelSpriteText;

		[SerializeField]
		private GameObject _setFrameObject;

		[SerializeField]
		[Header("CharaStateObject")]
		private InstantiateGenerator _charaStetGenerator;

		[SerializeField]
		[Header("StarObject")]
		private InstantiateGenerator _starObjectGenerator;

		[SerializeField]
		[Header("新規加入処理中央")]
		private InstantiateGenerator _characterGenerator;

		[SerializeField]
		[Header("新規加入モード時Animator")]
		private Animator _newcomerAinmator;

		private CharacterStateObject _characterState;

		private AwakeIconController _awakeIconController;

		private CharaParts _charaParts;

		private StateAnimController _stateController;

		public void Initialize()
		{
			_characterState = _charaStetGenerator.Instantiate<CharacterStateObject>();
			_awakeIconController = _starObjectGenerator.Instantiate<AwakeIconController>();
			_charaParts = _characterGenerator.Instantiate<CharaParts>();
			_characterState.Initialize();
			if (_newcomerAinmator != null)
			{
				_stateController = _newcomerAinmator.GetBehaviour<StateAnimController>();
			}
		}

		public void SetInformation(bool isParty, string charaName, int distance, WeakPoint weakPoint, CharacterSelectProces.ArrowDirection arrow)
		{
			if (_setFrameObject != null)
			{
				_setFrameObject.SetActive(isParty);
			}
			_characterState.SetData(charaName, distance, weakPoint, arrow);
		}

		public void SetData(int level, float amount, int awakeNum, Sprite backBase, Sprite face, Sprite frame, Sprite levelBase, Sprite starSmallBase, Sprite starLargeBase)
		{
			if (_levelSpriteText != null)
			{
				_levelSpriteText.SetOdoText(level);
			}
			if (_baseImage != null)
			{
				_baseImage.sprite = backBase;
			}
			if (_charaImage != null)
			{
				_charaImage.sprite = face;
			}
			if (_frameImage != null)
			{
				if (frame != null)
				{
					if (!_frameImage.gameObject.activeSelf)
					{
						_frameImage.gameObject.SetActive(value: true);
					}
					_frameImage.sprite = frame;
				}
				else
				{
					_frameImage.gameObject.SetActive(value: false);
				}
			}
			if (_levelBaseImage != null)
			{
				_levelBaseImage.sprite = levelBase;
			}
			_awakeIconController?.Prepare(starSmallBase, starLargeBase, amount, awakeNum);
			_charaParts?.SetParts(backBase, face, frame, levelBase, starSmallBase, starLargeBase, amount, level, awakeNum, Color.clear);
			_charaParts?.PlayJoinParty(OnPlayJoinPartyNext);
			if (_newcomerAinmator != null)
			{
				int num = Animator.StringToHash("Base Layer.In");
				_newcomerAinmator?.Play(num, 0, 0f);
				_stateController.SetExitParts(delegate
				{
					_newcomerAinmator?.Play(Animator.StringToHash("Loop"), 0, 0f);
				}, num);
			}
		}

		public void OnCenterIn()
		{
			_awakeIconController?.OnCenterIn();
		}

		public void OnCenterOut()
		{
			_awakeIconController?.OnCenterOut();
		}

		private void OnPlayJoinPartyNext()
		{
			_charaParts?.SetVisibleDecorate(isVisible: false);
		}

		public void SetVisibleDecorate(bool isVisible)
		{
			_charaParts?.SetVisibleDecorate(isVisible);
			_awakeIconController?.gameObject.SetActive(value: false);
		}
	}
}
