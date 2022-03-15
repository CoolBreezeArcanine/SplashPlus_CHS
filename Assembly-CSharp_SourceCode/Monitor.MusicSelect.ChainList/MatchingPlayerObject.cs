using DB;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect.ChainList
{
	public class MatchingPlayerObject : MonoBehaviour
	{
		public enum PlayerState
		{
			Entry = 0,
			Setup = 1,
			Ready = 2,
			None = -1
		}

		[SerializeField]
		[Header("プレイヤー難易度")]
		private MultipleImage _playerDifficulty;

		[SerializeField]
		[Header("プレイヤー状態情報")]
		private MultipleImage _playerStateInfo;

		[SerializeField]
		private GameObject[] _playerStateInfoTextList;

		[SerializeField]
		[Header("プレイヤーアイコン画像")]
		private RawImage _playerIcon;

		[SerializeField]
		[Header("サテライト番号")]
		private MultipleImage _playerNum;

		[SerializeField]
		[Header("ユーザーネーム")]
		private TextMeshProUGUI _userName;

		private void Awake()
		{
			_playerStateInfoTextList[0].GetComponent<TextMeshProUGUI>().text = CommonMessageID.MusicSelectMatchEntry.GetName();
			_playerStateInfoTextList[1].GetComponent<TextMeshProUGUI>().text = CommonMessageID.MusicSelectMatchSetup.GetName();
			_playerStateInfoTextList[2].GetComponent<TextMeshProUGUI>().text = CommonMessageID.MusicSelectMatchSet.GetName();
		}

		public void SetPlayerInfo(int playerNum, int difficulty, PlayerState playerState, string userName, Texture2D texture)
		{
			_playerDifficulty.ChangeSprite(difficulty);
			ChangePlayerInfo(playerState);
			_playerNum.ChangeSprite(playerNum);
			if (texture != null)
			{
				_playerIcon.texture = texture;
			}
			if (_userName != null)
			{
				_userName.text = userName;
			}
		}

		public void ChangePlayerInfo(PlayerState playerState)
		{
			_playerStateInfo.ChangeSprite((int)playerState);
			for (int i = 0; i < _playerStateInfoTextList.Length; i++)
			{
				_playerStateInfoTextList[i].SetActive(i == (int)playerState);
			}
		}
	}
}
