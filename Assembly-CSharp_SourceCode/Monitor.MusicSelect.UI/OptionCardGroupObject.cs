using System.Collections.Generic;
using DB;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect.UI
{
	public class OptionCardGroupObject : MonoBehaviour
	{
		[SerializeField]
		private OptionKindID _settingKind;

		[SerializeField]
		private TextMeshProUGUI _titleText;

		[SerializeField]
		private TextMeshProUGUI _messageText;

		private readonly List<string> _titleList = new List<string>(4)
		{
			CommonMessageID.MusicSelectOptionBasic.GetName(),
			CommonMessageID.MusicSelectOptionAdvanced.GetName(),
			CommonMessageID.MusicSelectOptionExpert.GetName(),
			CommonMessageID.MusicSelectOptionCustom.GetName()
		};

		private readonly List<string> _messageList = new List<string>(4)
		{
			CommonMessageID.MusicSelectOptionBasicInfo.GetName(),
			CommonMessageID.MusicSelectOptionAdvancedInfo.GetName(),
			CommonMessageID.MusicSelectOptionExpertInfo.GetName(),
			CommonMessageID.MusicSelectOptionCustomInfo.GetName()
		};

		[SerializeField]
		private List<MultipleImage> _targetList;

		[SerializeField]
		private List<ColorTarget> _colorTargetList;

		[SerializeField]
		private List<ActiveTarget> _disableObjectList;

		[SerializeField]
		private List<ActiveTarget> _activeObjectList;

		public void ChangeState(OptionKindID settingKind)
		{
			if (settingKind < OptionKindID.Basic || settingKind >= OptionKindID.End)
			{
				return;
			}
			foreach (MultipleImage target in _targetList)
			{
				target.ChangeSprite((int)settingKind);
			}
			if (_colorTargetList.Count > 0)
			{
				foreach (ColorTarget colorTarget in _colorTargetList)
				{
					if (colorTarget.ColorList.Count <= 0)
					{
						continue;
					}
					foreach (Graphic targetGraphic in colorTarget.TargetGraphicList)
					{
						targetGraphic.color = colorTarget.ColorList[(int)settingKind];
					}
					foreach (Shadow shadow in colorTarget.ShadowList)
					{
						shadow.effectColor = colorTarget.ColorList[(int)settingKind];
					}
				}
			}
			if (_messageText != null && _messageList.Count > 0)
			{
				_messageText.text = _messageList[(int)settingKind];
			}
			if (_titleText != null && _titleList.Count > 0)
			{
				_titleText.text = _titleList[(int)settingKind];
			}
			for (int i = 0; i < _disableObjectList.Count; i++)
			{
				if (settingKind != (OptionKindID)i)
				{
					continue;
				}
				ActiveTarget activeTarget = _disableObjectList[i];
				if (activeTarget.TargetGameObjectList.Count <= 0)
				{
					continue;
				}
				foreach (GameObject targetGameObject in activeTarget.TargetGameObjectList)
				{
					targetGameObject?.SetActive(value: false);
				}
			}
			for (int j = 0; j < _activeObjectList.Count; j++)
			{
				if (settingKind != (OptionKindID)j)
				{
					continue;
				}
				ActiveTarget activeTarget2 = _activeObjectList[j];
				if (activeTarget2.TargetGameObjectList.Count <= 0)
				{
					continue;
				}
				foreach (GameObject targetGameObject2 in activeTarget2.TargetGameObjectList)
				{
					targetGameObject2?.SetActive(value: true);
				}
			}
		}
	}
}
