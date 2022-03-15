using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainTabPanel : MonoBehaviour
{
	[SerializeField]
	private Image _bgImage;

	[SerializeField]
	private Image _titleImage;

	[SerializeField]
	private Image _subTitleBgImage;

	[SerializeField]
	private TextMeshProUGUI _subTitleText;

	public void ResetSubTitle()
	{
		_subTitleText.text = "";
	}

	public void Prepare(Color bgColor, Sprite title, string subTitle)
	{
		_bgImage.color = bgColor;
		if (subTitle == "")
		{
			_subTitleBgImage.color = Color.clear;
			_subTitleText.alpha = 0f;
		}
		else
		{
			_subTitleBgImage.color = Color.white;
			_subTitleText.alpha = 1f;
			_subTitleText.text = subTitle;
		}
		_titleImage.sprite = title;
		_titleImage.SetNativeSize();
	}

	public void Prepare(TabDataBase data)
	{
		Prepare(data.BaseColor, data.TitleSprite, data.SubTitle);
	}
}
