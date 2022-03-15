using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourseCard : MonoBehaviour
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
	private Material[] _materials;

	[SerializeField]
	private TextMeshProUGUI _levelText;

	[SerializeField]
	private SpriteCounter _lifeText;

	[SerializeField]
	private TextMeshProUGUI _recoverText;

	[SerializeField]
	private TextMeshProUGUI _greatDamegeText;

	[SerializeField]
	private TextMeshProUGUI _goodDamegeText;

	[SerializeField]
	private TextMeshProUGUI _missDamegeText;

	[SerializeField]
	private TextMeshProUGUI _achieveText;

	[SerializeField]
	private Animator _anim;

	[SerializeField]
	private Transform _clearPlateTransform;

	[SerializeField]
	private GameObject _originalClearPlateObj;

	private CourseClearPlate _clearPlate;

	public void Initialize()
	{
		if (_originalClearPlateObj != null && _clearPlateTransform != null)
		{
			_clearPlate = Object.Instantiate(_originalClearPlateObj, _clearPlateTransform).GetComponent<CourseClearPlate>();
			_clearPlate.gameObject.SetActive(value: false);
		}
	}

	public void Prepare(Sprite sprite, int mode, MusicLevelID level, int life, int recover, int greatDamage, int goodDamage, int missDamage, int achieve, int restLife, bool isPlay, bool isMain)
	{
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
		if (_image != null)
		{
			_image.sprite = sprite;
		}
		if (isMain)
		{
			if (_levelText != null)
			{
				_levelText.text = "最大レベル Lv" + level.GetLevelNum();
				if (mode - 1 >= 0 && mode - 1 < _materials.Length)
				{
					_levelText.fontMaterial = _materials[mode - 1];
				}
			}
		}
		else if (_levelText != null)
		{
			_levelText.text = "Lv" + level.GetLevelNum();
			if (mode - 1 >= 0 && mode - 1 < _materials.Length)
			{
				_levelText.fontMaterial = _materials[mode - 1];
			}
		}
		if (_lifeText != null)
		{
			_lifeText.ChangeText(text);
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
		if (_achieveText != null)
		{
			_achieveText.text = ((float)achieve / 10000f).ToString("0.0000") + "%";
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
		bool isClear = restLife != 0;
		SetClearPlate(isPlay, isClear);
	}

	public void SetPlayAnim(string animName)
	{
		if (_anim != null)
		{
			_anim.Play(animName);
		}
	}

	public void SetClearPlate(bool isActive, bool isClear)
	{
		_clearPlate?.gameObject.SetActive(isActive);
		if (isActive)
		{
			_clearPlate?.ViewClear(isClear);
			_clearPlate?.SetAnim(CourseClearPlate.BGAnim.Loop);
		}
	}

	public void Prepare(CourseCardData data, bool isMain)
	{
		Prepare(data._sprite, data._courseMode, data._level, data._life, data._recover, data._greatDamage, data._goodDamage, data._missDamage, data._achievement, data._restLife, data._isPlay, isMain);
	}
}
