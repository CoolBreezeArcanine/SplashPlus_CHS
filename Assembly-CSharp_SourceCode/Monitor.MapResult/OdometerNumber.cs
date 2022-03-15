using System.Linq;
using Monitor.MapResult.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MapResult
{
	public class OdometerNumber
	{
		public GameObject sign;

		public GameObject comma;

		public GameObject period;

		public GameObject km;

		public GameObject k;

		public GameObject m;

		public CommonNumber _texNumber = new CommonNumber();

		public Image[] digits = new Image[9];

		public bool isAnother;

		public bool _upper;

		public uint _disp;

		public OdometerNumber(GameObject obj, bool another, int tex_type)
		{
			Transform transform = obj.transform;
			sign = transform.GetChild(0).gameObject;
			digits[8] = transform.GetChild(1).gameObject.GetComponent<Image>();
			digits[7] = transform.GetChild(2).gameObject.GetComponent<Image>();
			digits[6] = transform.GetChild(3).gameObject.GetComponent<Image>();
			comma = transform.GetChild(4).gameObject;
			digits[5] = transform.GetChild(5).gameObject.GetComponent<Image>();
			digits[4] = transform.GetChild(6).gameObject.GetComponent<Image>();
			digits[3] = transform.GetChild(7).gameObject.GetComponent<Image>();
			period = transform.GetChild(8).gameObject;
			digits[2] = transform.GetChild(9).gameObject.GetComponent<Image>();
			digits[1] = transform.GetChild(10).gameObject.GetComponent<Image>();
			digits[0] = transform.GetChild(11).gameObject.GetComponent<Image>();
			Transform transform2 = transform.GetChild(12).gameObject.transform;
			isAnother = false;
			if (another)
			{
				isAnother = true;
			}
			if (isAnother)
			{
				km = transform2.GetChild(0).gameObject;
				m = transform2.GetChild(1).gameObject;
			}
			else
			{
				km = transform2.GetChild(0).gameObject;
				k = transform2.GetChild(1).gameObject;
				m = transform2.GetChild(2).gameObject;
			}
			string spritePath = _texNumber.GetSpritePath("UI_CHR_Num_Distance_02");
			switch (tex_type)
			{
			case 0:
				spritePath = _texNumber.GetSpritePath("UI_CHR_Num_Distance_00");
				break;
			case 1:
				spritePath = _texNumber.GetSpritePath("UI_CHR_Num_Distance_01");
				break;
			case 2:
				spritePath = _texNumber.GetSpritePath("UI_CHR_Num_Distance_02");
				break;
			case 211:
				spritePath = _texNumber.GetSpritePath("UI_CMN_Num_70p");
				break;
			case 212:
				spritePath = _texNumber.GetSpritePath("UI_CMN_Num_70p");
				break;
			case 300:
				spritePath = _texNumber.GetSpritePath("UI_CMN_Num_70p");
				break;
			}
			_texNumber.Load(spritePath);
			_upper = true;
			_disp = 0u;
		}

		public void SetDigits(int i, int num)
		{
			digits[i].sprite = _texNumber.texNumber[num];
			digits[i].gameObject.SetActive(value: true);
		}

		public void SetNumber(uint d)
		{
			uint num = d;
			string text = num.ToString();
			GameObject gameObject = null;
			int num2 = 0;
			if (isAnother)
			{
				gameObject = km;
			}
			else
			{
				gameObject = km;
				this.k.SetActive(value: false);
			}
			uint num3 = 10000u;
			if (d < num3 && !_upper)
			{
				gameObject.SetActive(value: false);
				this.m.SetActive(value: true);
				if (text.Length >= 4)
				{
					comma.SetActive(value: true);
					text = (num * 1000).ToString();
					num2 = 3;
				}
				else
				{
					comma.SetActive(value: false);
				}
				period.SetActive(value: false);
			}
			else
			{
				if (num >= 999999999)
				{
					text = 999999999u.ToString();
				}
				gameObject.SetActive(value: true);
				this.m.SetActive(value: false);
				if (text.Length >= 4)
				{
					period.SetActive(value: true);
				}
				else
				{
					period.SetActive(value: false);
					if (_upper && text.Length < 4 && _disp != 0)
					{
						period.SetActive(value: true);
					}
				}
				if (text.Length >= 7)
				{
					comma.SetActive(value: true);
				}
				else
				{
					comma.SetActive(value: false);
				}
			}
			for (int i = text.Length; i < digits.Count(); i++)
			{
				SetDigits(i, 0);
				digits[i].gameObject.SetActive(value: false);
			}
			for (int j = 0; j < text.Length; j++)
			{
				int num4 = text.Length - 1 - j;
				if (num4 < 0)
				{
					num4 = 0;
				}
				string s = text.Substring(j, 1);
				SetDigits(num4, int.Parse(s));
			}
			if (_upper && text.Length < 4)
			{
				int num5 = 4;
				for (int k = text.Length; k < num5; k++)
				{
					SetDigits(k, 0);
					digits[k].gameObject.SetActive(value: true);
				}
			}
			for (int l = 0; l < num2; l++)
			{
				SetDigits(l, 0);
				digits[l].gameObject.SetActive(value: false);
			}
			if (!_upper)
			{
				return;
			}
			if (_disp == 0)
			{
				period.SetActive(value: false);
			}
			if (_disp < 3)
			{
				for (int m = 0; m <= 2 - _disp; m++)
				{
					digits[m].gameObject.SetActive(value: false);
				}
			}
		}
	}
}
