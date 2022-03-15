using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class CourseModeCard : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private GameObject _daniBG;

	[SerializeField]
	private GameObject _sinDaniBG;

	[SerializeField]
	private GameObject _randomBG;

	[SerializeField]
	private Animator _anim;

	[SerializeField]
	private TextMeshProUGUI[] _modeText;

	[SerializeField]
	[Header("背景色関連")]
	private ImageColorGroup _backgroundColorGroup;

	public void Prepare(Sprite sprite, int mode)
	{
		if (_image != null)
		{
			_image.sprite = sprite;
		}
		if (_daniBG != null && _sinDaniBG != null && _sinDaniBG != null)
		{
			switch (mode)
			{
			case 1:
				_daniBG.SetActive(value: true);
				_sinDaniBG.SetActive(value: false);
				_randomBG.SetActive(value: false);
				break;
			case 2:
				_daniBG.SetActive(value: false);
				_sinDaniBG.SetActive(value: true);
				_randomBG.SetActive(value: false);
				break;
			case 3:
				_daniBG.SetActive(value: false);
				_sinDaniBG.SetActive(value: false);
				_randomBG.SetActive(value: true);
				break;
			}
		}
		if (_modeText != null && _modeText.Length == 3)
		{
			_modeText[0].text = "LIFEを残したまま4曲クリアできたら段位獲得！";
			_modeText[1].text = "真のmaimai力を追求しよう！";
			_modeText[2].text = "運も実力のうち！運試しの段位認定！";
		}
		int color = 0;
		switch (mode)
		{
		case 1:
			color = 0;
			break;
		case 2:
			color = 1;
			break;
		case 3:
			color = 2;
			break;
		}
		if (_backgroundColorGroup != null)
		{
			_backgroundColorGroup.SetColor(color);
		}
	}

	public void SetPlayAnim(string animName)
	{
		if (_anim != null)
		{
			_anim.Play(animName);
		}
	}

	public void Prepare(CourseModeCardData data)
	{
		Prepare(data._sprite, data._courseMode);
	}
}
