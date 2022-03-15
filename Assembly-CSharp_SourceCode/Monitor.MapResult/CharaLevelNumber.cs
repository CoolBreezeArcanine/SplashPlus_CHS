using System.Linq;
using Monitor.MapResult.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MapResult
{
	public class CharaLevelNumber
	{
		public CommonNumber _texNumber = new CommonNumber();

		public Image[] digits = new Image[4];

		public CharaLevelNumber(GameObject obj)
		{
			Transform transform = obj.transform;
			digits[3] = transform.GetChild(0).gameObject.GetComponent<Image>();
			digits[2] = transform.GetChild(1).gameObject.GetComponent<Image>();
			digits[1] = transform.GetChild(2).gameObject.GetComponent<Image>();
			digits[0] = transform.GetChild(3).gameObject.GetComponent<Image>();
			string spritePath = _texNumber.GetSpritePath("UI_CMN_Num_26p");
			_texNumber.Load(spritePath);
		}

		public void SetDigits(int i, int num)
		{
			digits[i].sprite = _texNumber.texNumber[num];
			digits[i].gameObject.SetActive(value: true);
		}

		public void SetNumber(int d)
		{
			string text = Mathf.Abs(d).ToString();
			for (int i = text.Length; i < digits.Count(); i++)
			{
				SetDigits(i, 0);
				digits[i].gameObject.SetActive(value: false);
			}
			for (int j = 0; j < text.Length; j++)
			{
				int num = text.Length - 1 - j;
				if (num < 0)
				{
					num = 0;
				}
				string s = text.Substring(j, 1);
				SetDigits(num, int.Parse(s));
			}
		}
	}
}
