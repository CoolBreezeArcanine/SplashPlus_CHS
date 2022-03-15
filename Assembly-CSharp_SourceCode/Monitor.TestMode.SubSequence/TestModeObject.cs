using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	[RequireComponent(typeof(RectTransform))]
	public class TestModeObject : MonoBehaviour
	{
		protected const float DefaultTopMargin = 0.3f;

		protected const float DefaultBottomMargin = 0.3f;

		protected const int DefaultItemTopLine = 2;

		protected const int DefaultFontSize = 20;

		protected const int DefaultLineStep = 24;

		[SerializeField]
		protected TMP_FontAsset BaseFont;

		[SerializeField]
		protected float TopMargin = 0.3f;

		[SerializeField]
		protected float BottomMargin = 0.3f;

		[SerializeField]
		protected int ItemTopLine = 2;

		[SerializeField]
		protected float LeftMargin = 0.4f;

		[SerializeField]
		protected float LabelWidth = 0.4f;

		[SerializeField]
		protected float RightMargin;

		[SerializeField]
		protected string[] Titles;

		[SerializeField]
		protected int ItemNum;

		protected static TMP_FontAsset DefaultFont;

		protected Vector2 CanvasSize;

		protected float TopPositionY;

		protected float LabelPositionX;

		protected float LabelSizeX;

		protected float LabelFullSizeX;

		protected float ValuePositionX;

		protected float ValueSizeX;

		private List<TextMeshProUGUI> _titleList;

		private List<ItemText> _itemList;

		protected TextMeshProUGUI InstructionText;

		protected int TitleListCount => _titleList.Count;

		protected int ItemListCount => _itemList.Count;

		public void SetTitleText(int index, string text)
		{
			if (_titleList != null && index >= 0 && index < _titleList.Count)
			{
				_titleList[index].text = text;
			}
		}

		public void SetLabelText(int index, string text)
		{
			if (_itemList != null && index >= 0 && index < _itemList.Count)
			{
				_itemList[index].SetLabel(text);
			}
		}

		public void SetValueOnOff(int index, bool flag)
		{
			if (_itemList != null && index >= 0 && index < _itemList.Count)
			{
				_itemList[index].SetValueOnOff(flag);
			}
		}

		public void SetValueInt(int index, int value)
		{
			if (_itemList != null && index >= 0 && index < _itemList.Count)
			{
				_itemList[index].SetValueInt(value);
			}
		}

		public void SetValueFloat(int index, float value)
		{
			if (_itemList != null && index >= 0 && index < _itemList.Count)
			{
				_itemList[index].SetValueFloat(value);
			}
		}

		public void SetValueString(int index, string text)
		{
			if (_itemList != null && index >= 0 && index < _itemList.Count)
			{
				_itemList[index].SetValueString(text);
			}
		}

		public void ChangeTextLayoutHorizontal(TextMeshProUGUI text, float left, float width)
		{
			RectTransform obj = text.transform as RectTransform;
			Vector3 position = obj.transform.position;
			Vector2 sizeDelta = obj.sizeDelta;
			position.x = CanvasSize.x * left;
			sizeDelta.x = CanvasSize.x * width;
			obj.transform.position = position;
			obj.sizeDelta = sizeDelta;
		}

		public void SetLabelAlignCenter(int index)
		{
			if (_itemList != null && index >= 0 && index < _itemList.Count)
			{
				ItemText itemText = _itemList[index];
				ChangeTextLayoutHorizontal(itemText.LabelText, 0.02f, 0.98f);
				itemText.LabelText.alignment = TextAlignmentOptions.Top;
				itemText.LabelText.autoSizeTextContainer = true;
			}
		}

		public void ClearAll()
		{
			foreach (TextMeshProUGUI title in _titleList)
			{
				title.text = "";
			}
			foreach (ItemText item in _itemList)
			{
				item.SetLabel("");
				item.SetValueString("");
			}
			InstructionText.text = "";
		}

		protected void Awake()
		{
			InitializeParamater();
			CreateTitle();
			CreateItems();
			CreateInstruction();
		}

		protected virtual void InitializeParamater()
		{
			if (BaseFont != null)
			{
				DefaultFont = BaseFont;
			}
			CanvasSize = new Vector2(1080f, 1080f);
			TopPositionY = CanvasSize.y / 2f * (1f - TopMargin);
			LabelPositionX = CanvasSize.x * LeftMargin - CanvasSize.x / 2f;
			LabelSizeX = CanvasSize.x * LabelWidth;
			LabelFullSizeX = CanvasSize.x * (1f - LeftMargin - RightMargin);
			ValuePositionX = CanvasSize.x * LabelWidth;
			ValueSizeX = CanvasSize.x * (1f - LeftMargin - RightMargin - LabelWidth);
		}

		protected virtual void CreateTitle()
		{
			_titleList = new List<TextMeshProUGUI>();
			float num = CanvasSize.y / 2f * (1f - TopMargin);
			string[] titles = Titles;
			foreach (string text in titles)
			{
				TextMeshProUGUI textMeshProUGUI = MakeTitleText();
				textMeshProUGUI.text = text;
				textMeshProUGUI.transform.SetParent(base.transform, worldPositionStays: false);
				Vector3 position = textMeshProUGUI.transform.position;
				position.y = num;
				position.x = 0f;
				textMeshProUGUI.transform.localPosition = position;
				textMeshProUGUI.name = "Title";
				num -= 24f;
				_titleList.Add(textMeshProUGUI);
			}
		}

		protected virtual void CreateInstruction()
		{
			float y = 0f - CanvasSize.y / 2f * (1f - BottomMargin);
			TextMeshProUGUI textMeshProUGUI = MakeTitleText();
			textMeshProUGUI.text = "";
			textMeshProUGUI.transform.SetParent(base.transform, worldPositionStays: false);
			Vector3 position = textMeshProUGUI.transform.position;
			position.y = y;
			position.x = 0f;
			textMeshProUGUI.transform.localPosition = position;
			textMeshProUGUI.name = "Instruction";
			InstructionText = textMeshProUGUI;
		}

		protected virtual void CreateItems()
		{
			_itemList = new List<ItemText>();
			for (int i = 0; i < ItemNum; i++)
			{
				ItemText itemText = new ItemText();
				float y = TopPositionY - (float)((i + ItemTopLine) * 24);
				TextMeshProUGUI textMeshProUGUI = MakeItemLabelText(hasValue: true);
				textMeshProUGUI.text = "";
				textMeshProUGUI.transform.SetParent(base.transform, worldPositionStays: false);
				textMeshProUGUI.transform.localPosition = new Vector3(LabelPositionX, y, 0f);
				itemText.LabelText = textMeshProUGUI;
				TextMeshProUGUI textMeshProUGUI2 = MakeItemValueText();
				textMeshProUGUI2.text = "";
				textMeshProUGUI2.transform.SetParent(textMeshProUGUI.transform, worldPositionStays: false);
				textMeshProUGUI2.transform.localPosition = new Vector3(ValuePositionX, 0f, 0f);
				itemText.ValueTextList.Add(textMeshProUGUI2);
				_itemList.Add(itemText);
			}
		}

		protected TextMeshProUGUI MakeTitleText()
		{
			TextMeshProUGUI textMeshProUGUI = MakeText();
			if (textMeshProUGUI != null)
			{
				RectTransform rectTransform = textMeshProUGUI.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.transform.position = Vector3.zero;
					rectTransform.sizeDelta = new Vector2(CanvasSize.x, 20f);
					textMeshProUGUI.alignment = TextAlignmentOptions.Top;
					rectTransform.pivot = new Vector2(0.5f, 0f);
				}
			}
			return textMeshProUGUI;
		}

		protected TextMeshProUGUI MakeItemLabelText(bool hasValue)
		{
			TextMeshProUGUI textMeshProUGUI = MakeText();
			if (textMeshProUGUI != null)
			{
				RectTransform rectTransform = textMeshProUGUI.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.transform.position = Vector3.zero;
					float x = (hasValue ? LabelSizeX : LabelFullSizeX);
					rectTransform.sizeDelta = new Vector2(x, 20f);
					textMeshProUGUI.alignment = TextAlignmentOptions.TopLeft;
					rectTransform.pivot = new Vector2(0f, 0f);
					textMeshProUGUI.name = "Label";
				}
			}
			return textMeshProUGUI;
		}

		protected TextMeshProUGUI MakeItemValueText()
		{
			TextMeshProUGUI textMeshProUGUI = MakeText();
			if (textMeshProUGUI != null)
			{
				RectTransform rectTransform = textMeshProUGUI.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.transform.position = Vector3.zero;
					float valueSizeX = ValueSizeX;
					rectTransform.sizeDelta = new Vector2(valueSizeX, 20f);
					textMeshProUGUI.alignment = TextAlignmentOptions.TopLeft;
					rectTransform.pivot = new Vector2(0f, 0f);
					textMeshProUGUI.name = "Value";
				}
			}
			return textMeshProUGUI;
		}

		protected TextMeshProUGUI MakeText()
		{
			TextMeshProUGUI textMeshProUGUI = new GameObject().AddComponent<TextMeshProUGUI>();
			textMeshProUGUI.font = BaseFont;
			textMeshProUGUI.fontSize = 20f;
			return textMeshProUGUI;
		}
	}
}
