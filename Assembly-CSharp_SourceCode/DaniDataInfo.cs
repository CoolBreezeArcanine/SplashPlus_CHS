using MAI2.Util;
using Manager;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class DaniDataInfo : MonoBehaviour
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
	private SpriteCounter[] _lifeText;

	[SerializeField]
	private TextMeshProUGUI _recoverText;

	[SerializeField]
	private TextMeshProUGUI _greatDamegeText;

	[SerializeField]
	private TextMeshProUGUI _goodDamegeText;

	[SerializeField]
	private TextMeshProUGUI _missDamegeText;

	[SerializeField]
	private MultipleImage _lifeHeartObj;

	[SerializeField]
	private GameObject _lifeRecoverInfo;

	[SerializeField]
	private Animator _anim;

	public void Prepare(Sprite sprite, int mode, MusicLevelID level, int life, int recover, int greatDamage, int goodDamage, int missDamage, int achieve, int restLife)
	{
		CourseManager.LifeColor lifeColor = Singleton<CourseManager>.Instance.GetLifeColor((uint)life);
		string text = life.ToString();
		int num = life;
		int num2 = 0;
		while (num < 100)
		{
			num *= 10;
			text += " ";
			num2++;
			if (num2 >= 2)
			{
				break;
			}
		}
		for (int i = 0; i < _lifeText.Length; i++)
		{
			_lifeText[i].ChangeText(text);
			if (i == (int)lifeColor)
			{
				_lifeText[i].gameObject.SetActive(value: true);
			}
			else
			{
				_lifeText[i].gameObject.SetActive(value: false);
			}
		}
		_lifeHeartObj.ChangeSprite((int)lifeColor);
		if (_image != null)
		{
			_image.sprite = sprite;
		}
		if (_recoverText != null)
		{
			_recoverText.text = "";
			if (recover > 0)
			{
				_recoverText.text = "+";
			}
			_recoverText.text += recover;
		}
		if (_greatDamegeText != null)
		{
			_greatDamegeText.text = "";
			if (greatDamage > 0)
			{
				_greatDamegeText.text = "-";
			}
			_greatDamegeText.text += greatDamage;
		}
		if (_goodDamegeText != null)
		{
			_goodDamegeText.text = "";
			if (goodDamage > 0)
			{
				_goodDamegeText.text = "-";
			}
			_goodDamegeText.text += goodDamage;
		}
		if (_missDamegeText != null)
		{
			_missDamegeText.text = "";
			if (missDamage > 0)
			{
				_missDamegeText.text = "-";
			}
			_missDamegeText.text += missDamage;
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
		if (life == 0 || recover == 0)
		{
			_lifeRecoverInfo.SetActive(value: false);
		}
		else
		{
			_lifeRecoverInfo.SetActive(value: true);
		}
	}

	public void SetPlayAnim(string animName)
	{
		if (_anim != null)
		{
			_anim.Play(animName);
		}
	}

	public void Prepare(CourseCardData data)
	{
		Prepare(data._sprite, data._courseMode, data._level, data._life, data._recover, data._greatDamage, data._goodDamage, data._missDamage, data._achievement, data._restLife);
	}
}
