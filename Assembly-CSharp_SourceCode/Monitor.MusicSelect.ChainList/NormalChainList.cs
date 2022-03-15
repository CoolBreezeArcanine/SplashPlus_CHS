using System;
using DB;
using Manager.UserDatas;
using Process;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.MusicSelect.ChainList
{
	public class NormalChainList : MusicSelectDaisyChainList
	{
		private enum ChainMode
		{
			Sort,
			Option
		}

		[SerializeField]
		[Header("オプションプレビューオリジナル")]
		private NoteSpeedPreviewObject _originalTapNotePreviewObject;

		[SerializeField]
		private NoteSpeedPreviewObject _originalTouchPreviewObject;

		[SerializeField]
		private StarRotationPreviewObject _originalStarRotationPreviewObject;

		[SerializeField]
		private SlideSpeedPreviewObject _originalSlideSpeecPreviewObject;

		private NoteSpeedPreviewObject _tapSpeedPreview;

		private NoteSpeedPreviewObject _touchSpeedPreview;

		private StarRotationPreviewObject _starRotationPreview;

		private SlideSpeedPreviewObject _slideSpeedPreview;

		private ChainMode _mode;

		public override void Initialize()
		{
			base.Initialize();
			_tapSpeedPreview = UnityEngine.Object.Instantiate(_originalTapNotePreviewObject, _viewTransform);
			_touchSpeedPreview = UnityEngine.Object.Instantiate(_originalTouchPreviewObject, _viewTransform);
			_starRotationPreview = UnityEngine.Object.Instantiate(_originalStarRotationPreviewObject, _viewTransform);
			_slideSpeedPreview = UnityEngine.Object.Instantiate(_originalSlideSpeecPreviewObject, _viewTransform);
			_tapSpeedPreview.gameObject.SetActive(value: false);
			_touchSpeedPreview.gameObject.SetActive(value: false);
			_starRotationPreview.gameObject.SetActive(value: false);
			_slideSpeedPreview.gameObject.SetActive(value: false);
		}

		public void SetPlayerIndex(int playerIndex)
		{
			_tapSpeedPreview.Initialize(playerIndex);
			_touchSpeedPreview.Initialize(playerIndex);
			_starRotationPreview.Initialize(playerIndex);
			_slideSpeedPreview.Initialize(playerIndex);
		}

		public override void SetScrollCard(bool isVisible)
		{
		}

		public override void ViewUpdate()
		{
			base.ViewUpdate();
			_slideSpeedPreview?.AnimationUpdate();
		}

		public void OptionDeploy()
		{
			base.RemoveAll();
			int num = 0;
			for (int i = 0; i < 5; i++)
			{
				num += Enum.GetValues(UserOption.OptionTypes[i]).Length - 1;
			}
			int num2 = ((9 <= num) ? 4 : 0);
			for (int j = 0; j < 9; j++)
			{
				if (j < 4 - num2)
				{
					continue;
				}
				int diffIndex = j - 4;
				OptionCategoryID category;
				string value;
				string detail;
				string valueDetails;
				string spriteKey;
				bool isLeftButtonActive;
				bool isRightButtonActive;
				string optionName = SelectProcess.GetOptionName(MonitorIndex, diffIndex, out category, out value, out detail, out valueDetails, out spriteKey, out isLeftButtonActive, out isRightButtonActive);
				if (SelectProcess.IsOptionBoundary(MonitorIndex, diffIndex, out var overCount))
				{
					string right = category.GetName();
					string optionCategoryName = SelectProcess.GetOptionCategoryName(MonitorIndex, overCount);
					SeparateBaseChainObject separate = GetSeparate(optionCategoryName, right);
					SetSpot(j, separate);
					continue;
				}
				OptionCardObject chain = GetChain<OptionCardObject>();
				Sprite optionValueSprite = SelectProcess.GetOptionValueSprite(spriteKey);
				chain.SetData(optionName, value, detail, valueDetails, optionValueSprite, isLeftButtonActive, isRightButtonActive);
				chain.Initialize(250f);
				if (j == 4)
				{
					chain.OnCenterIn();
				}
				else
				{
					chain.ResetChain();
				}
				SetSpot(j, chain);
			}
			base.IsListEnable = true;
			Positioning(isImmediate: true, isAnimation: true);
			_mode = ChainMode.Option;
		}

		public void SortDeploy()
		{
			base.RemoveAll();
			int end = SortRootID.End.GetEnd();
			int num;
			switch (end)
			{
			case 1:
			case 2:
				num = 4;
				break;
			case 3:
			case 4:
				num = 3;
				break;
			case 5:
			case 6:
				num = 2;
				break;
			case 7:
			case 8:
				num = 1;
				break;
			default:
				num = 0;
				break;
			}
			int num2 = ((9 > end) ? (num + end) : 9);
			for (int i = num; i < num2; i++)
			{
				OptionCardObject chain = GetChain<OptionCardObject>();
				SortRootID root = (SortRootID)(i - num);
				SortData sortData = SelectProcess.GetSortData(root);
				chain.SetData(sortData.TargetName, sortData.SortTypeName, "", "", SelectProcess.GetSortValueSprite(root), isLeftButtonActive: false, isRightButtonActive: false);
				chain.Initialize(250f);
				if (i == 4)
				{
					chain.OnCenterIn();
				}
				SetSpot(i, chain);
			}
			base.IsListEnable = true;
			Positioning(isImmediate: true, isAnimation: true);
			_mode = ChainMode.Sort;
		}

		public override void RemoveAll()
		{
			ChainObject[] spotArray = SpotArray;
			for (int i = 0; i < spotArray.Length; i++)
			{
				spotArray[i]?.OnCenterOut();
			}
			base.RemoveAll();
		}

		public void ValueChange(string value, string valueDetails, Sprite sprite)
		{
			OptionCardObject optionCardObject = SpotArray[4] as OptionCardObject;
			if (optionCardObject != null)
			{
				optionCardObject.SetValue(value, valueDetails, sprite);
			}
		}

		public void ValueChange(string targetName, string value, string valueDetails, string detailName)
		{
			OptionCardObject optionCardObject = SpotArray[4] as OptionCardObject;
			if (optionCardObject != null)
			{
				optionCardObject.SetData(targetName, value, detailName, valueDetails, null, isLeftButtonActive: false, isRightButtonActive: false);
			}
		}

		public void SetSpeed(OptionCategoryID category, int optionIndex, float speed)
		{
			_tapSpeedPreview.gameObject.SetActive(value: false);
			_touchSpeedPreview.gameObject.SetActive(value: false);
			_starRotationPreview.gameObject.SetActive(value: false);
			_slideSpeedPreview.gameObject.SetActive(value: false);
			if (!(speed >= 0f))
			{
				return;
			}
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (optionIndex)
				{
				case 0:
					SetPreview(_tapSpeedPreview);
					_tapSpeedPreview.SetSpeed(speed);
					_starRotationPreview.SetSpeed(speed);
					break;
				case 1:
					SetPreview(_touchSpeedPreview);
					_touchSpeedPreview.SetSpeed(speed);
					break;
				case 2:
					SetPreview(_slideSpeedPreview);
					_slideSpeedPreview.SetSpeed(speed);
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				if (optionIndex == 2)
				{
					SetPreview(_starRotationPreview);
					_starRotationPreview.SetSpeedRateAnimation();
					_starRotationPreview.SetRotateSpeed(speed);
					_slideSpeedPreview.SetRotateSpeed(speed);
				}
				break;
			}
		}

		public void Hide()
		{
			_tapSpeedPreview?.gameObject.SetActive(value: false);
			_touchSpeedPreview?.gameObject.SetActive(value: false);
			_starRotationPreview?.gameObject.SetActive(value: false);
			_slideSpeedPreview?.gameObject.SetActive(value: false);
		}

		public void SetSlideSpeedOptionValue(int value)
		{
			_slideSpeedPreview.SetOptionValue(value);
		}

		protected override void Next(int targetIndex, Direction direction)
		{
			int index = ((direction != Direction.Right) ? 8 : 0);
			if (_mode == ChainMode.Option)
			{
				OptionCategoryID category;
				string value;
				string detail;
				string valueDetails;
				string spriteKey;
				bool isLeftButtonActive;
				bool isRightButtonActive;
				string optionName = SelectProcess.GetOptionName(MonitorIndex, targetIndex, out category, out value, out detail, out valueDetails, out spriteKey, out isLeftButtonActive, out isRightButtonActive);
				ChainObject chainObject;
				if (SelectProcess.IsOptionBoundary(MonitorIndex, targetIndex, out var overCount))
				{
					string right = category.GetName();
					string optionCategoryName = SelectProcess.GetOptionCategoryName(MonitorIndex, overCount);
					chainObject = GetSeparate(optionCategoryName, right);
				}
				else
				{
					OptionCardObject chain = GetChain<OptionCardObject>();
					chain.SetData(optionSprite: SelectProcess.GetOptionValueSprite(spriteKey), title: optionName, value: value, details: detail, valueDetails: valueDetails, isLeftButtonActive: isLeftButtonActive, isRightButtonActive: isRightButtonActive);
					chainObject = chain;
				}
				chainObject.ResetChain();
				SetSpot(index, chainObject);
			}
			else
			{
				_ = _mode;
			}
		}

		private void SetPreview(NoteSpeedPreviewObject previewObject)
		{
			((OptionCardObject)SpotArray[4]).SetPreviewPrefab(previewObject.gameObject);
			previewObject.gameObject.SetActive(value: true);
			previewObject.AnimationReset();
		}

		public void PressedPreview(OptionCategoryID category, int optionIndex)
		{
			if (category == OptionCategoryID.SpeedSetting && optionIndex != 0 && optionIndex == 1)
			{
				_touchSpeedPreview.Pressed();
			}
		}

		public void Preseedbutton(Direction direction, bool isLongTouch, bool toOut)
		{
			OptionCardObject optionCardObject = (OptionCardObject)SpotArray[4];
			if (optionCardObject != null)
			{
				optionCardObject.PressedButton(direction, isLongTouch, toOut);
			}
		}

		public void SetVisibleButton(bool isVisible, Direction direction)
		{
			OptionCardObject optionCardObject = (OptionCardObject)SpotArray[4];
			if (optionCardObject != null)
			{
				optionCardObject.SetVisibleButton(isVisible, direction);
			}
		}
	}
}
