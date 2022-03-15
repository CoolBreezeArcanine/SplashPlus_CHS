using System.Collections.Generic;
using System.Globalization;
using TMPro;

namespace Monitor.TestMode.SubSequence
{
	public class ItemText
	{
		public TextMeshProUGUI LabelText;

		public List<TextMeshProUGUI> ValueTextList = new List<TextMeshProUGUI>();

		public TextMeshProUGUI ValueText
		{
			get
			{
				if (ValueTextList.Count > 0)
				{
					return ValueTextList[0];
				}
				return null;
			}
		}

		public void SetLabel(string text)
		{
			if (LabelText != null)
			{
				LabelText.text = text;
			}
		}

		public void SetValueOnOff(bool flag, string onText, string offText)
		{
			if (ValueText != null)
			{
				ValueText.text = (flag ? onText : offText);
			}
		}

		public void SetValueOnOff(bool flag)
		{
			SetValueOnOff(flag, "ON", "OFF");
		}

		public void SetValueInt(int value)
		{
			if (ValueText != null)
			{
				ValueText.text = value.ToString();
			}
		}

		public void SetValueFloat(float value)
		{
			if (ValueText != null)
			{
				ValueText.text = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public void SetValueString(string text)
		{
			if (ValueText != null)
			{
				ValueText.text = text;
			}
		}

		public void SetValueOnOff(int index, bool flag, string onText, string offText)
		{
			TextMeshProUGUI textMeshProUGUI = ((index < ValueTextList.Count) ? ValueTextList[index] : null);
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.text = (flag ? onText : offText);
			}
		}

		public void SetValueOnOff(int index, bool flag)
		{
			SetValueOnOff(index, flag, "ON", "OFF");
		}

		public void SetValueInt(int index, int value)
		{
			TextMeshProUGUI textMeshProUGUI = ((index < ValueTextList.Count) ? ValueTextList[index] : null);
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.text = value.ToString();
			}
		}

		public void SetValueFloat(int index, float value)
		{
			TextMeshProUGUI textMeshProUGUI = ((index < ValueTextList.Count) ? ValueTextList[index] : null);
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.text = value.ToString();
			}
		}

		public void SetValueString(int index, string text)
		{
			TextMeshProUGUI textMeshProUGUI = ((index < ValueTextList.Count) ? ValueTextList[index] : null);
			if (textMeshProUGUI != null)
			{
				textMeshProUGUI.text = text;
			}
		}
	}
}
