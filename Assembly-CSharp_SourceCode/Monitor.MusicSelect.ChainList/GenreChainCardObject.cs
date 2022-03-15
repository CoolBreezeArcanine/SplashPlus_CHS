using TMPro;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect.ChainList
{
	public class GenreChainCardObject : ChainObject
	{
		public enum GenreCardPattern
		{
			Default,
			Extend
		}

		[SerializeField]
		[Header("表示パターンアニメ")]
		private Animator _showPatternAnim;

		[SerializeField]
		[Header("コンプアニメ")]
		private Animator _rainbowAnim;

		[SerializeField]
		private Animator _goldAnim;

		[SerializeField]
		private Animator _silverAnim;

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
		[Header("トータル楽曲数(スクリプト)")]
		private CustomNum _totalNum;

		[SerializeField]
		[Header("ミニカード")]
		private GenreChainCardObject _miniCard;

		[SerializeField]
		private GameObject _mainGenreObject;

		[SerializeField]
		private CanvasGroup _miniCardGroup;

		[SerializeField]
		private CanvasGroup _mainCardGroup;

		public int categoryID { get; set; }

		public int TotalMusicNum { get; set; }

		public void SetCardPattern(GenreCardPattern type)
		{
			switch (type)
			{
			case GenreCardPattern.Default:
				_showPatternAnim.Play("Type_01");
				break;
			case GenreCardPattern.Extend:
				_showPatternAnim.Play("Type_02");
				break;
			}
		}

		public void SetFCAPNum(int fcNum, int fcpNum, int apNum, int appNum, int fsNum, int fsdNum)
		{
			if (TotalMusicNum != 0)
			{
				string text = fcNum.ToString();
				string text2 = apNum.ToString();
				string text3 = fsNum.ToString();
				string text4 = fsdNum.ToString();
				_fcNum.text = text;
				_apNum.text = text2;
				_fsNum.text = text3;
				_fspNum.text = text4;
			}
		}

		public void SetRankNum(int clearNum, int sNum, int spNum, int ssNum, int sspNum, int sssNum, int ssspNum, bool isExtra)
		{
			string text = clearNum.ToString();
			string text2 = sNum.ToString();
			string text3 = ssNum.ToString();
			string text4 = sssNum.ToString();
			_totalNum.SetNumerator(TotalMusicNum);
			if (_clearNum != null)
			{
				_clearNum.text = text;
			}
			if (_sNum != null)
			{
				_sNum.text = text2;
			}
			if (_ssNum != null)
			{
				_ssNum.text = text3;
			}
			if (_sssNum != null)
			{
				_sssNum.text = text4;
			}
			bool flag = sNum == TotalMusicNum;
			bool flag2 = ssNum == TotalMusicNum;
			bool flag3 = sssNum == TotalMusicNum;
			if (isExtra)
			{
				_rainbowAnim.gameObject.SetActive(value: false);
				_goldAnim.gameObject.SetActive(value: false);
				_silverAnim.gameObject.SetActive(value: false);
			}
			else if (flag3)
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
				_miniCard.TotalMusicNum = TotalMusicNum;
				_miniCard.SetRankNum(clearNum, sNum, spNum, ssNum, sspNum, sssNum, ssspNum, isExtra);
			}
		}

		public void SetIconJacket(Sprite fcapIcon, Sprite compIcon, Sprite rankIcon)
		{
			SetSpriteImage(_fcapIcon, fcapIcon);
			SetSpriteImage(_completeIcon, compIcon);
			SetSpriteImage(_rankIcon, rankIcon);
			if (_miniCard != null)
			{
				_miniCard.SetIconJacket(fcapIcon, compIcon, rankIcon);
			}
		}

		public void SetJacket(Sprite jacket)
		{
			_selectTabImage.sprite = jacket;
			if (_miniCard != null)
			{
				_miniCard.SetJacket(jacket);
			}
		}

		public void IsVisible(bool isVisible)
		{
			base.gameObject.SetActive(isVisible);
		}

		private void SetSpriteImage(Image image, Sprite sprite)
		{
			if (sprite != null)
			{
				image.gameObject.SetActive(value: true);
				image.sprite = sprite;
			}
			else
			{
				image.gameObject.SetActive(value: false);
			}
		}

		public override void ViewUpdate(float syncTimer)
		{
			base.ViewUpdate(syncTimer);
		}

		public void ChangeSize(bool isMainActive)
		{
			if (_miniCard != null)
			{
				if (_miniCardGroup != null)
				{
					_miniCardGroup.alpha = ((!isMainActive) ? 1 : 0);
				}
				if (_mainCardGroup != null)
				{
					_mainCardGroup.alpha = (isMainActive ? 1 : 0);
				}
			}
		}

		public override void OnCenterIn()
		{
			ChangeSize(isMainActive: true);
		}

		public override void OnCenterOut()
		{
			ChangeSize(isMainActive: false);
		}

		public override void ResetChain()
		{
			_rainbowAnim.gameObject.SetActive(value: false);
			_goldAnim.gameObject.SetActive(value: false);
			_silverAnim.gameObject.SetActive(value: false);
			if (_mainCardGroup != null)
			{
				_mainCardGroup.alpha = 0f;
			}
			if (_miniCard != null)
			{
				_miniCard.ResetChain();
			}
		}
	}
}
