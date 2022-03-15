using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenreCardObject : MonoBehaviour
{
	public enum GenreCardPattern
	{
		FCAP,
		RANK
	}

	[SerializeField]
	[Header("表示パターンアニメ")]
	private Animation _showPatternAnim;

	[SerializeField]
	[Header("コンプアニメ")]
	private Animation _rainbowAnim;

	[SerializeField]
	private Animation _goldAnim;

	[SerializeField]
	private Animation _silverAnim;

	[SerializeField]
	[Header("フルコン系アイコン数")]
	private TextMeshProUGUI _fcNum;

	[SerializeField]
	private TextMeshProUGUI _apNum;

	[SerializeField]
	private TextMeshProUGUI _fsNum;

	[SerializeField]
	private TextMeshProUGUI _fspNum;

	[SerializeField]
	[Header("クリア、ランク数")]
	private TextMeshProUGUI _sNum;

	[SerializeField]
	private TextMeshProUGUI _ssNum;

	[SerializeField]
	private TextMeshProUGUI _sssNum;

	[SerializeField]
	private TextMeshProUGUI _clearNum;

	[SerializeField]
	[Header("曲セレのタブ画像")]
	private Image _selectTabImage;

	[SerializeField]
	[Header("各種メダル")]
	private Image _fcapIcon;

	[SerializeField]
	private Image _completeIcon;

	[SerializeField]
	private Image _rankIcon;

	[SerializeField]
	[Header("ミニカード")]
	private GenreCardObject _miniCard;

	[SerializeField]
	private GameObject _mainGenreObject;

	public int TotalMusicNum { get; set; }

	public void SetCardPattern(GenreCardPattern type)
	{
		switch (type)
		{
		case GenreCardPattern.FCAP:
			_showPatternAnim.Play("Type_01");
			break;
		case GenreCardPattern.RANK:
			_showPatternAnim.Play("Type_02");
			break;
		}
	}

	public void SetFCAPNum(int fcNum, int fcpNum, int apNum, int appNum, int fsNum, int fspNum)
	{
		if (TotalMusicNum != 0)
		{
			string text = fcNum.ToString("000");
			string text2 = apNum.ToString("000");
			string text3 = fsNum.ToString("000");
			string text4 = fspNum.ToString("000");
			_fcNum.text = text;
			_apNum.text = text2;
			_fsNum.text = text3;
			_fspNum.text = text4;
			if (_miniCard != null)
			{
				_miniCard.TotalMusicNum = TotalMusicNum;
				_miniCard.SetFCAPNum(fcNum, fcpNum, apNum, appNum, fsNum, fspNum);
			}
		}
	}

	public void SetRankNum(int clearNum, int sNum, int spNum, int ssNum, int sspNum, int sssNum, int ssspNum)
	{
		string text = TotalMusicNum.ToString("000");
		string text2 = clearNum.ToString("000");
		string text3 = sNum.ToString("000");
		string text4 = ssNum.ToString("000");
		string text5 = sssNum.ToString("000");
		_clearNum.text = text2 + "/" + text;
		_sNum.text = text3;
		_ssNum.text = text4;
		_sssNum.text = text5;
		bool flag = sNum == TotalMusicNum;
		bool flag2 = ssNum == TotalMusicNum;
		if (sssNum == TotalMusicNum)
		{
			_rainbowAnim.gameObject.SetActive(value: true);
			_goldAnim.gameObject.SetActive(value: false);
			_silverAnim.gameObject.SetActive(value: false);
		}
		else if (flag2)
		{
			_rainbowAnim.gameObject.SetActive(value: false);
			_goldAnim.gameObject.SetActive(value: true);
			_silverAnim.gameObject.SetActive(value: false);
		}
		else if (flag)
		{
			_rainbowAnim.gameObject.SetActive(value: false);
			_goldAnim.gameObject.SetActive(value: false);
			_silverAnim.gameObject.SetActive(value: true);
		}
		else
		{
			_rainbowAnim.gameObject.SetActive(value: false);
			_goldAnim.gameObject.SetActive(value: false);
			_silverAnim.gameObject.SetActive(value: false);
		}
		if (_miniCard != null)
		{
			_miniCard.SetRankNum(clearNum, sNum, spNum, ssNum, sspNum, sssNum, ssspNum);
		}
	}

	public void SetIconJacket(Sprite fcapIcon, Sprite compIcon, Sprite rankIcon)
	{
		_fcapIcon.sprite = fcapIcon;
		_completeIcon.sprite = compIcon;
		_rankIcon.sprite = rankIcon;
		if (_miniCard != null)
		{
			_miniCard.SetIconJacket(fcapIcon, compIcon, rankIcon);
		}
	}
}
