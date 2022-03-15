using System.Collections;
using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class CourseTrackStart : MonoBehaviour
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
	[Header("背景色関連")]
	private ImageColorGroup _backgroundColorGroup;

	[SerializeField]
	[Header("背景")]
	private GameObject _courseBGObj;

	[SerializeField]
	private Transform _courseBGTransform;

	private CourseBGController _courseBg;

	[SerializeField]
	[Header("ライフ")]
	private GameObject _courseLifeObj;

	[SerializeField]
	private Transform _courseLifeTransform;

	private CourseLife _courseLife;

	[SerializeField]
	private GameObject _lifeUpObj;

	[SerializeField]
	private MultipleImage _trackImage;

	[SerializeField]
	private SpriteCounter _recoverLife;

	private int _beforeLife;

	private int _afterLife;

	private int _courseMode;

	public bool _isEnd;

	public void Initialize()
	{
		_courseBg = Object.Instantiate(_courseBGObj, _courseBGTransform).GetComponent<CourseBGController>();
		_courseLife = Object.Instantiate(_courseLifeObj, _courseLifeTransform).GetComponent<CourseLife>();
		_anim.Play("Idle");
	}

	public void Prepare(Sprite sprite, int mode, int beforeLife, int afterLife, int track)
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
		if (_courseBg != null)
		{
			switch (mode)
			{
			case 1:
			case 3:
				_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Track_Loop);
				break;
			case 2:
				_courseBg.SetAnim(CourseBGController.BGAnim.SinDani_Track_Loop);
				break;
			}
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
		_courseLife.SetLife((uint)beforeLife);
		int num = afterLife - beforeLife;
		string text = "+" + num;
		int num2 = num;
		int num3 = 0;
		while (num2 < 100)
		{
			num2 *= 10;
			text += " ";
			num3++;
			if (num3 >= 2)
			{
				break;
			}
		}
		if (_lifeUpObj != null)
		{
			_lifeUpObj.SetActive(value: false);
		}
		if (track >= 0 && track <= 4)
		{
			_trackImage.ChangeSprite(track - 1);
		}
		if (_recoverLife != null)
		{
			_recoverLife.ChangeText(text);
		}
		_beforeLife = beforeLife;
		_afterLife = afterLife;
		_courseMode = mode;
	}

	public void SetFadeInAnimation(int monitorIndex)
	{
		StartCoroutine(PlayAnimCoroutine(monitorIndex));
	}

	private IEnumerator PlayAnimCoroutine(int monitorIndex)
	{
		SoundManager.PlaySE(Cue.SE_TRACK_START_DANI, monitorIndex);
		switch (_courseMode)
		{
		case 1:
			_anim.Play("TrackStart_Dani");
			break;
		case 2:
			_anim.Play("TrackStart_ShinDani");
			break;
		case 3:
			_anim.Play("TrackStart_Random");
			break;
		}
		yield return new WaitForSeconds(2f);
		int upLife = _afterLife - _beforeLife;
		if (upLife != 0)
		{
			SoundManager.PlaySE(Cue.SE_DANI_LIFE_RECOVER_01, monitorIndex);
			_lifeUpObj.SetActive(value: true);
			_anim.Play("ClearBonus");
		}
		for (int i = 0; i < 10; i++)
		{
			if (i != 0)
			{
				yield return new WaitForSeconds(0.04f);
			}
			int num = upLife * (i + 1) / 10;
			int life = _beforeLife + num;
			_courseLife.SetLife((uint)life);
		}
		yield return new WaitForSeconds(2f);
		_isEnd = true;
	}
}
