using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect.PartyConnect
{
	public class ConnectInfoObject : MonoBehaviour
	{
		private enum InfoKind
		{
			Music = 0,
			Player = 1,
			End = 2,
			None = -1
		}

		private enum EntryKind
		{
			Entry,
			Cancel,
			Ecd
		}

		[SerializeField]
		[Header("マッチング募集中オブジェクト")]
		private GameObject _recruitBase;

		[SerializeField]
		[Header("参加するよオブジェクト")]
		private GameObject _connectingBase;

		[SerializeField]
		[Header("募集中のジャケット")]
		private RawImage _recruitTexture;

		[SerializeField]
		[Header("参加orキャンセル画像")]
		private MultipleImage _entryImage;

		[SerializeField]
		private MultipleImage _entryImageShadow;

		[SerializeField]
		[Header("プレイヤーアイコン画像")]
		private RawImage[] _playerIcon;

		[SerializeField]
		[Header("サテライト番号")]
		private TextMeshProUGUI[] _playerNum;

		[SerializeField]
		[Header("ユーザーネーム")]
		private TextMeshProUGUI[] _userName;

		[SerializeField]
		[Header("土台アニメーション")]
		private Animator _baseAnim;

		private int _loopTimer;

		private const int LOOP_TIME = 180;

		private InfoKind _infoType = InfoKind.None;

		public void Init()
		{
			_recruitBase.SetActive(value: false);
			_connectingBase.SetActive(value: false);
		}

		public void SetRecritInfo(Texture2D musicTexture)
		{
			_recruitBase.SetActive(value: true);
			_connectingBase.SetActive(value: false);
			_recruitTexture.texture = musicTexture;
			_baseAnim?.Play("In_Music");
			_infoType = InfoKind.Music;
			_loopTimer = 180;
		}

		public void SetConnectingInfo(Texture2D[] textureList, string[] playerNumList, string[] playerNameList)
		{
			_recruitBase.SetActive(value: false);
			_connectingBase.SetActive(value: true);
			_entryImage.ChangeSprite(0);
			_entryImageShadow.ChangeSprite(0);
			for (int i = 0; i < _playerIcon.Length; i++)
			{
				if (textureList[i] != null)
				{
					_playerIcon[i].gameObject.SetActive(value: true);
					_playerIcon[i].texture = textureList[i];
				}
				else
				{
					_playerIcon[i].gameObject.SetActive(value: false);
				}
			}
			for (int j = 0; j < playerNumList.Length; j++)
			{
				if (playerNumList[j] != null)
				{
					_playerNum[j].text = playerNumList[j].ToString();
				}
			}
			for (int k = 0; k < playerNameList.Length; k++)
			{
				if (playerNameList[k] != null)
				{
					_userName[k].text = playerNameList[k];
				}
			}
			_baseAnim?.Play("In_Player");
			_infoType = InfoKind.Player;
			_loopTimer = 180;
		}

		public void SetCancelInfo()
		{
			_recruitBase.SetActive(value: false);
			_connectingBase.SetActive(value: true);
			_entryImage.ChangeSprite(1);
			_entryImageShadow.ChangeSprite(1);
			_baseAnim?.Play("In_Player");
			_infoType = InfoKind.Player;
			_loopTimer = 180;
		}

		public void UpdateTIme()
		{
			if (_loopTimer == 0)
			{
				return;
			}
			_loopTimer--;
			if (_loopTimer == 0)
			{
				switch (_infoType)
				{
				case InfoKind.Music:
					_baseAnim?.Play("Out_Music");
					break;
				case InfoKind.Player:
					_baseAnim?.Play("Out_Player");
					break;
				}
			}
		}
	}
}
