using Manager;
using Monitor.MusicSelect.UI;
using Process;
using UI;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor
{
	public class DifficultyContoroller : MonoBehaviour
	{
		[SerializeField]
		[Header("難易度名表示")]
		private Image _difficutlyNameImage;

		[SerializeField]
		[Header("コントロールボタンプレハブ")]
		private DifficultyButtonObject _controlBtnPrefab;

		[SerializeField]
		[Header("色変更背景")]
		private Image _background01Image;

		[SerializeField]
		[Header("ボタン画像リスト")]
		private Sprite[] _buttonTypeASprites;

		[SerializeField]
		[Header("ボタン文字画像リスト")]
		private Sprite _buttonPlusSymbolSprite;

		[SerializeField]
		private Sprite _buttonMinusSymbolSprite;

		[SerializeField]
		[Header("難易度名画像リスト")]
		private Sprite[] _difficultyNameSprites;

		[SerializeField]
		[Header("難易度の色設定")]
		private Color[] _difficultyColors;

		[SerializeField]
		[Header("ボタンのnullポインタ座標")]
		private Transform _leftButtonPositions;

		[SerializeField]
		private Transform _rightButtonPositions;

		[SerializeField]
		[Header("フリーダムモードインフォ")]
		private Image _freedomInfo;

		[SerializeField]
		private Animator _mainAnim;

		private int _playerIndex;

		private int _difficulty;

		private bool _isLeftActive;

		private bool _isRightActive;

		private bool _inAnimation = true;

		private bool _remasterEnable = true;

		protected float SyncTimer;

		private DifficultyButtonObject _leftButtonObject;

		private DifficultyButtonObject _rightButtonObject;

		public void Initialize(int playerIndex, IMusicSelectProcess process)
		{
			_playerIndex = playerIndex;
			_leftButtonObject = Object.Instantiate(_controlBtnPrefab, _leftButtonPositions);
			_leftButtonObject.Initialize(_playerIndex, InputManager.ButtonSetting.Button08, CommonButtonObject.LedColors.White);
			_rightButtonObject = Object.Instantiate(_controlBtnPrefab, _rightButtonPositions);
			_rightButtonObject.Initialize(_playerIndex, InputManager.ButtonSetting.Button01, CommonButtonObject.LedColors.White);
			_isLeftActive = (_isRightActive = false);
			_leftButtonObject.gameObject.SetActive(value: false);
			_rightButtonObject.gameObject.SetActive(value: false);
			_freedomInfo.gameObject.SetActive(GameManager.IsFreedomMode);
		}

		public void ViewUpdate()
		{
			SyncTimer += (float)GameManager.GetGameMSecAdd() / 1000f;
			_leftButtonObject?.ViewUpdate(SyncTimer);
			_rightButtonObject?.ViewUpdate(SyncTimer);
		}

		public void ChangeBackground()
		{
			_background01Image.color = _difficultyColors[_difficulty];
		}

		private void ChangeName()
		{
			_difficutlyNameImage.sprite = _difficultyNameSprites[_difficulty];
		}

		private void ChangeButton(bool isChange, bool isRightButton, bool isLedOn = true)
		{
			if (!ButtonActiveCheckLeft())
			{
				if (_isLeftActive && _leftButtonObject.gameObject.activeInHierarchy)
				{
					SetTrigger(Direction.Left, isActive: false);
					_isLeftActive = false;
				}
				if (isChange && !isRightButton)
				{
					_leftButtonObject.Pressed();
				}
			}
			else
			{
				int num = _difficulty - 1;
				_leftButtonObject.ChangeDifficulty(_buttonMinusSymbolSprite, _buttonTypeASprites[num], isFlip: false, num);
				if (isChange)
				{
					if (!isRightButton)
					{
						_leftButtonObject.Pressed();
					}
					if (isLedOn)
					{
						_leftButtonObject.ChangeColor(GetLedColorEnum(num), nowFlash: true);
					}
				}
				if (!_isLeftActive)
				{
					_leftButtonObject.gameObject.SetActive(value: true);
					if (_leftButtonObject.gameObject.activeInHierarchy)
					{
						SetTrigger(Direction.Left, isActive: true);
						if (isLedOn)
						{
							_leftButtonObject.ChangeColor(GetLedColorEnum(num), nowFlash: true);
						}
						_leftButtonObject.ChangeDifficulty(_buttonMinusSymbolSprite, _buttonTypeASprites[num], isFlip: false, num);
						_isLeftActive = true;
					}
				}
			}
			if (!ButtonActiveCheckRight())
			{
				if (_isRightActive && _rightButtonObject.gameObject.activeInHierarchy)
				{
					SetTrigger(Direction.Right, isActive: false);
					_isRightActive = false;
				}
				if (isChange && isRightButton)
				{
					_rightButtonObject.Pressed();
				}
				return;
			}
			int num2 = _difficulty + 1;
			_rightButtonObject.ChangeDifficulty(_buttonPlusSymbolSprite, _buttonTypeASprites[num2], isFlip: false, num2);
			if (isChange)
			{
				if (isRightButton)
				{
					_rightButtonObject.Pressed();
				}
				if (isLedOn)
				{
					_rightButtonObject.ChangeColor(GetLedColorEnum(num2), nowFlash: true);
				}
			}
			if (_isRightActive)
			{
				return;
			}
			_rightButtonObject.gameObject.SetActive(value: true);
			if (_rightButtonObject.gameObject.activeInHierarchy)
			{
				SetTrigger(Direction.Right, isActive: true);
				if (isLedOn)
				{
					_rightButtonObject.ChangeColor(GetLedColorEnum(num2), nowFlash: true);
				}
				_rightButtonObject.ChangeDifficulty(_buttonPlusSymbolSprite, _buttonTypeASprites[num2], isFlip: false, num2);
				_isRightActive = true;
			}
		}

		public void setForceDiffParam(MusicDifficultyID d)
		{
			_difficulty = (int)d;
		}

		public void AddDifficulty()
		{
			if (_difficulty + 1 <= 4)
			{
				_difficulty++;
				ChangeButton(isChange: true, isRightButton: true);
				ChangeName();
				ChangeBackground();
			}
		}

		public void SubDifficulty()
		{
			if (_difficulty - 1 >= 0)
			{
				_difficulty--;
				ChangeButton(isChange: true, isRightButton: false);
				ChangeName();
				ChangeBackground();
			}
		}

		public void InitDifficulty(MusicDifficultyID d)
		{
			_difficulty = (int)d;
			ChangeButton(isChange: false, isRightButton: false, isLedOn: false);
			ChangeName();
			ChangeBackground();
		}

		public void SetDifficulty(MusicDifficultyID d, bool isChange = false)
		{
			bool isRightButton = d - _difficulty > MusicDifficultyID.Basic;
			_difficulty = (int)d;
			ChangeButton(isChange, isRightButton);
			ChangeName();
			ChangeBackground();
			SetButtonColor(isActive: true);
		}

		public void SetDifficultyLevelSort(MusicDifficultyID d)
		{
			bool isChange = _difficulty != (int)d;
			bool isRightButton = d - _difficulty > MusicDifficultyID.Basic;
			_difficulty = (int)d;
			ChangeButton(isChange, isRightButton);
			ChangeName();
			ChangeBackground();
		}

		public void SetDifficultyExtra(MusicDifficultyID d, bool isButtonChange = true)
		{
			_difficulty = (int)d;
			ChangeName();
			ChangeBackground();
			_isLeftActive = (_isRightActive = false);
			_leftButtonObject.SetActiveButton(isActive: false);
			_rightButtonObject.SetActiveButton(isActive: false);
		}

		public void SetButtonVisible(bool isVisible, MusicDifficultyID d = MusicDifficultyID.Invalid)
		{
			if (d != MusicDifficultyID.Invalid)
			{
				_difficulty = (int)d;
			}
			if (isVisible)
			{
				ChangeButton(isChange: false, isRightButton: false, isLedOn: false);
			}
			else
			{
				_isLeftActive = (_isRightActive = false);
				_leftButtonObject.SetActiveButton(isActive: false);
				_rightButtonObject.SetActiveButton(isActive: false);
			}
			if (d != MusicDifficultyID.Invalid)
			{
				ChangeName();
				ChangeBackground();
			}
		}

		public void ForcedSetDifficulty(MusicDifficultyID d)
		{
			_isLeftActive = ButtonActiveCheckLeft();
			_isRightActive = ButtonActiveCheckRight();
			_leftButtonObject.gameObject.SetActive(ButtonActiveCheckLeft());
			_rightButtonObject.gameObject.SetActive(ButtonActiveCheckRight());
			_difficulty = (int)d;
			ChangeButton(isChange: false, isRightButton: false);
			ChangeName();
			ChangeBackground();
		}

		private void SetTrigger(Direction direction, bool isActive)
		{
			switch (direction)
			{
			case Direction.Left:
				_leftButtonObject.SetActiveButton(isActive);
				break;
			case Direction.Right:
				_rightButtonObject.SetActiveButton(isActive);
				break;
			}
		}

		public void DifficultyBaseIn()
		{
			if (!_inAnimation)
			{
				_mainAnim.Play("In");
				_inAnimation = true;
			}
		}

		public void DifficultyBaseOut()
		{
			if (_inAnimation)
			{
				_mainAnim.Play("Out");
				_inAnimation = false;
				SetButtonColor(isActive: false);
			}
		}

		private void SetButtonColor(bool isActive)
		{
			if (isActive)
			{
				_leftButtonObject.ChangeColor(GetLedColorEnum(_difficulty - 1), nowFlash: true);
				_rightButtonObject.ChangeColor(GetLedColorEnum(_difficulty + 1), nowFlash: true);
			}
			else
			{
				_leftButtonObject.ChangeColor(CommonButtonObject.LedColors.Black, nowFlash: true);
				_rightButtonObject.ChangeColor(CommonButtonObject.LedColors.Black, nowFlash: true);
			}
		}

		public void SetRemasterEnable(bool enable)
		{
			_remasterEnable = enable;
		}

		public static Color GetButtonLEDColor(int difficulty)
		{
			Color result = Color.white;
			switch (difficulty)
			{
			case 0:
				result = CommonScriptable.GetLedSetting().ButtonBasicColor;
				break;
			case 1:
				result = CommonScriptable.GetLedSetting().ButtonAdvancesColor;
				break;
			case 2:
				result = CommonScriptable.GetLedSetting().ButtonExpertColor;
				break;
			case 3:
				result = CommonScriptable.GetLedSetting().ButtonMasterColor;
				break;
			case 4:
				result = CommonScriptable.GetLedSetting().ButtonReMasterColor;
				break;
			}
			return result;
		}

		private static CommonButtonObject.LedColors GetLedColorEnum(int diff)
		{
			CommonButtonObject.LedColors result = CommonButtonObject.LedColors.Black;
			switch (diff)
			{
			case 0:
				result = CommonButtonObject.LedColors.Basic;
				break;
			case 1:
				result = CommonButtonObject.LedColors.Advanced;
				break;
			case 2:
				result = CommonButtonObject.LedColors.Expert;
				break;
			case 3:
				result = CommonButtonObject.LedColors.Master;
				break;
			case 4:
				result = CommonButtonObject.LedColors.Remaster;
				break;
			}
			return result;
		}

		private bool ButtonActiveCheckLeft()
		{
			return ButtonActiveCheck(isLeft: true);
		}

		private bool ButtonActiveCheckRight()
		{
			return ButtonActiveCheck(isLeft: false);
		}

		private bool ButtonActiveCheck(bool isLeft)
		{
			bool result = false;
			if (isLeft)
			{
				if (_difficulty != 0)
				{
					result = true;
				}
			}
			else if (_difficulty != 4 && (_remasterEnable || _difficulty + 1 != 4))
			{
				result = true;
			}
			return result;
		}
	}
}
