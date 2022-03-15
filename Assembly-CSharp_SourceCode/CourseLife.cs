using MAI2.Util;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CourseLife : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup _mainGroup;

	[SerializeField]
	private Image _lifeGreenObj;

	[SerializeField]
	private Image _lifeOrangeObj;

	[SerializeField]
	private Image _lifeRedObj;

	[SerializeField]
	private Image _lifeGrayObj;

	[SerializeField]
	private SpriteCounter[] _lifeText;

	[SerializeField]
	private SpriteCounter _lifeZeroText;

	public void SetLife(uint life, bool isGame = false)
	{
		for (int i = 0; i < _lifeText.Length; i++)
		{
			string text = life.ToString("D000");
			if (life > 999)
			{
				text = "999";
			}
			_lifeText[i].ChangeText(text);
			if (life < 100)
			{
				_lifeText[i].FrameList[2].Scale = 0f;
				_lifeText[i].FrameList[0].RelativePosition.x = 47f;
				_lifeText[i].FrameList[1].RelativePosition.x = 29f;
				if (life < 10)
				{
					_lifeText[i].FrameList[1].Scale = 0f;
					_lifeText[i].FrameList[0].RelativePosition.x = 74f;
				}
				else
				{
					_lifeText[i].FrameList[1].Scale = 1f;
				}
			}
			else
			{
				_lifeText[i].FrameList[2].Scale = 1f;
				_lifeText[i].FrameList[0].RelativePosition.x = 18f;
				_lifeText[i].FrameList[1].RelativePosition.x = 0f;
				_lifeText[i].FrameList[2].RelativePosition.x = -18f;
			}
		}
		_lifeZeroText.ChangeText("0");
		switch (Singleton<CourseManager>.Instance.GetLifeColor(life))
		{
		case CourseManager.LifeColor.Green:
			_lifeGreenObj.gameObject.SetActive(value: true);
			_lifeOrangeObj.gameObject.SetActive(value: false);
			_lifeRedObj.gameObject.SetActive(value: false);
			_lifeGrayObj.gameObject.SetActive(value: false);
			break;
		case CourseManager.LifeColor.Orange:
			_lifeGreenObj.gameObject.SetActive(value: false);
			_lifeOrangeObj.gameObject.SetActive(value: true);
			_lifeRedObj.gameObject.SetActive(value: false);
			_lifeGrayObj.gameObject.SetActive(value: false);
			break;
		case CourseManager.LifeColor.Red:
			_lifeGreenObj.gameObject.SetActive(value: false);
			_lifeOrangeObj.gameObject.SetActive(value: false);
			_lifeRedObj.gameObject.SetActive(value: true);
			_lifeGrayObj.gameObject.SetActive(value: false);
			break;
		case CourseManager.LifeColor.Gray:
			_lifeGreenObj.gameObject.SetActive(value: false);
			_lifeOrangeObj.gameObject.SetActive(value: false);
			_lifeRedObj.gameObject.SetActive(value: false);
			_lifeGrayObj.gameObject.SetActive(value: true);
			break;
		}
		if (!isGame)
		{
			_mainGroup.alpha = 1f;
			_lifeGreenObj.color = Color.white;
			_lifeOrangeObj.color = Color.white;
			_lifeRedObj.color = Color.white;
			_lifeGrayObj.color = Color.white;
			for (int j = 0; j < _lifeText.Length; j++)
			{
				_lifeText[j].SetColor(Color.white);
				_lifeZeroText.SetColor(Color.white);
			}
		}
	}
}
