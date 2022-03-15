using Manager.MaiStudio.CardTypeName;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MapResult
{
	public class PassBonusIconColor
	{
		public enum PassBonusIconColorType
		{
			Bronze,
			Gold,
			Omake,
			Platinum,
			Silver,
			Common
		}

		public Color circle01 = new Color(1f, 0.82353f, 0.43922f);

		public Color circle02 = new Color(1f, 1f, 1f);

		public Color circle03 = new Color(1f, 0.9451f, 0.55686f);

		public Color star01 = new Color(1f, 0.8784314f, 0f);

		public Color star02 = new Color(1f, 57f / 85f, 0f);

		public PassBonusIconColorType colorType = PassBonusIconColorType.Common;

		public PassBonusIconColor()
		{
			circle01.r = 1f;
			circle01.g = 0.82353f;
			circle01.b = 0.43922f;
			circle02.r = 1f;
			circle02.g = 1f;
			circle02.b = 1f;
			circle03.r = 1f;
			circle03.g = 0.9451f;
			circle03.b = 0.55686f;
			star01.r = 1f;
			star01.g = 0.8784314f;
			star01.b = 0f;
			star02.r = 1f;
			star02.g = 0.8784314f;
			star02.b = 0f;
			colorType = PassBonusIconColorType.Common;
		}

		public void SetIconColor(GameObject obj, Table cardPassType)
		{
			PassBonusIconColorType passBonusIconColorType = PassBonusIconColorType.Common;
			switch (cardPassType)
			{
			case Table.OutOfEffect:
				passBonusIconColorType = PassBonusIconColorType.Omake;
				break;
			case Table.BronzePass:
				passBonusIconColorType = PassBonusIconColorType.Bronze;
				break;
			case Table.SilverPass:
				passBonusIconColorType = PassBonusIconColorType.Silver;
				break;
			case Table.GoldPass:
				passBonusIconColorType = PassBonusIconColorType.Gold;
				break;
			case Table.PlatinumPass:
				passBonusIconColorType = PassBonusIconColorType.Platinum;
				break;
			}
			switch (passBonusIconColorType)
			{
			case PassBonusIconColorType.Bronze:
				circle01.r = 1f;
				circle01.g = 0.81961f;
				circle01.b = 0.65882f;
				circle02.r = 1f;
				circle02.g = 1f;
				circle02.b = 1f;
				circle03.r = 0.89623f;
				circle03.g = 0.61421f;
				circle03.b = 0.40161f;
				star01.r = 1f;
				star01.g = 0.81961f;
				star01.b = 0.65882f;
				star02.r = 1f;
				star02.g = 0.79245f;
				star02.b = 0.63679f;
				colorType = PassBonusIconColorType.Bronze;
				break;
			case PassBonusIconColorType.Gold:
				circle01.r = 1f;
				circle01.g = 0.99309f;
				circle01.b = 0.91981f;
				circle02.r = 1f;
				circle02.g = 0.91379f;
				circle02.b = 0f;
				circle03.r = 1f;
				circle03.g = 0.58258f;
				circle03.b = 0.29412f;
				star01.r = 1f;
				star01.g = 0.58039f;
				star01.b = 0.29412f;
				star02.r = 1f;
				star02.g = 0.91373f;
				star02.b = 0f;
				colorType = PassBonusIconColorType.Gold;
				break;
			case PassBonusIconColorType.Omake:
				circle01.r = 0.68627f;
				circle01.g = 0.88235f;
				circle01.b = 0.21569f;
				circle02.r = 1f;
				circle02.g = 1f;
				circle02.b = 1f;
				circle03.r = 0.55229f;
				circle03.g = 1f;
				circle03.b = 0.33491f;
				star01.r = 0.84305f;
				star01.g = 0.91509f;
				star01.b = 0.3928f;
				star02.r = 0.67843f;
				star02.g = 0.86667f;
				star02.b = 0.22745f;
				colorType = PassBonusIconColorType.Omake;
				break;
			case PassBonusIconColorType.Platinum:
				circle01.r = 1f;
				circle01.g = 0.41176f;
				circle01.b = 0.6902f;
				circle02.r = 1f;
				circle02.g = 1f;
				circle02.b = 1f;
				circle03.r = 0.74462f;
				circle03.g = 0.50472f;
				circle03.b = 1f;
				star01.r = 0.95283f;
				star01.g = 0.59906f;
				star01.b = 1f;
				star02.r = 0.46752f;
				star02.g = 0.96226f;
				star02.b = 0.93068f;
				colorType = PassBonusIconColorType.Platinum;
				break;
			case PassBonusIconColorType.Silver:
				circle01.r = 0.60849f;
				circle01.g = 0.8695f;
				circle01.b = 1f;
				circle02.r = 1f;
				circle02.g = 1f;
				circle02.b = 1f;
				circle03.r = 0.18396f;
				circle03.g = 0.81613f;
				circle03.b = 1f;
				star01.r = 0.47642f;
				star01.g = 0.6838f;
				star01.b = 1f;
				star02.r = 0.18039f;
				star02.g = 0.81569f;
				star02.b = 1f;
				colorType = PassBonusIconColorType.Silver;
				break;
			case PassBonusIconColorType.Common:
				circle01.r = 1f;
				circle01.g = 0.82353f;
				circle01.b = 0.43922f;
				circle02.r = 1f;
				circle02.g = 1f;
				circle02.b = 1f;
				circle03.r = 1f;
				circle03.g = 0.9451f;
				circle03.b = 0.55686f;
				star01.r = 1f;
				star01.g = 0.8784314f;
				star01.b = 0f;
				star02.r = 1f;
				star02.g = 0.8784314f;
				star02.b = 0f;
				colorType = PassBonusIconColorType.Common;
				break;
			}
			GameObject gameObject = obj.transform.GetChild(1).gameObject;
			int num = 0;
			num = 0;
			gameObject.transform.GetChild(0).gameObject.transform.GetChild(num).gameObject.GetComponent<MultipleImage>().color = new Color(circle01.r, circle01.g, circle01.b);
			num = 1;
			gameObject.transform.GetChild(0).gameObject.transform.GetChild(num).gameObject.GetComponent<MultipleImage>().color = new Color(circle02.r, circle02.g, circle02.b);
			num = 2;
			gameObject.transform.GetChild(0).gameObject.transform.GetChild(num).gameObject.GetComponent<MultipleImage>().color = new Color(circle03.r, circle03.g, circle03.b);
			num = 0;
			for (int i = 0; i < 4; i++)
			{
				gameObject.transform.GetChild(1).gameObject.transform.GetChild(num).gameObject.transform.GetChild(i).GetComponent<Image>().color = new Color(star01.r, star01.g, star01.b);
			}
			num = 1;
			for (int j = 0; j < 4; j++)
			{
				gameObject.transform.GetChild(1).gameObject.transform.GetChild(num).gameObject.transform.GetChild(j).GetComponent<Image>().color = new Color(star02.r, star02.g, star02.b);
			}
			GameObject gameObject2 = obj.transform.GetChild(0).gameObject.transform.GetChild(3).transform.GetChild(2).gameObject;
			switch (passBonusIconColorType)
			{
			case PassBonusIconColorType.Bronze:
			case PassBonusIconColorType.Gold:
			case PassBonusIconColorType.Platinum:
			case PassBonusIconColorType.Silver:
				gameObject2.transform.GetChild(0).gameObject.SetActive(value: false);
				gameObject2.transform.GetChild(1).gameObject.SetActive(value: true);
				break;
			case PassBonusIconColorType.Omake:
			case PassBonusIconColorType.Common:
				gameObject2.transform.GetChild(0).gameObject.SetActive(value: true);
				gameObject2.transform.GetChild(1).gameObject.SetActive(value: false);
				break;
			}
		}
	}
}
