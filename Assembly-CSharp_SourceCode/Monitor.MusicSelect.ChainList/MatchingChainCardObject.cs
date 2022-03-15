using DB;
using TMPro;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.MusicSelect.ChainList
{
	public class MatchingChainCardObject : ChainObject
	{
		public enum MatchingPattern
		{
			Matching,
			Enable,
			Disable
		}

		[SerializeField]
		[Header("表示ベース")]
		private GameObject _matchingWaitObj;

		[SerializeField]
		private GameObject _matchingRecruit;

		[SerializeField]
		[Header("人数テキスト")]
		private TextMeshProUGUI _matchingNum;

		[SerializeField]
		[Header("非表示テキスト")]
		private TextMeshProUGUI _NgReason;

		[SerializeField]
		[Header("プレイヤー情報のnullポインタ座標")]
		private Transform[] _playerInfoPosition;

		[SerializeField]
		[Header("マッチング表示ベース")]
		private GameObject _matchingDisable;

		[SerializeField]
		private GameObject _matchingEnable;

		[SerializeField]
		[Header("オリジナルのmatcingPlayerInfo")]
		private MatchingPlayerObject _originalMatchingPlayerInfo;

		[SerializeField]
		private TextMeshProUGUI _matchEnableText;

		[SerializeField]
		private TextMeshProUGUI _matchDisableText;

		private bool[] _matchingList;

		private bool[] _settingOkList;

		private MatchingPlayerObject[] _matchingPlayerInfo;

		protected override void Awake()
		{
			base.Awake();
			_matchEnableText.text = CommonMessageID.MusicSelectEnableMatch.GetName();
			_matchDisableText.text = CommonMessageID.MusicSelectDisableMatch.GetName();
		}

		public void SetMatchingInfo(MatchingPattern patter)
		{
			switch (patter)
			{
			case MatchingPattern.Matching:
				_matchingWaitObj.SetActive(value: false);
				_matchingRecruit.SetActive(value: true);
				break;
			case MatchingPattern.Enable:
				_matchingWaitObj.SetActive(value: true);
				_matchingRecruit.SetActive(value: false);
				_matchingEnable.SetActive(value: true);
				_matchingDisable.SetActive(value: false);
				break;
			case MatchingPattern.Disable:
				_matchingWaitObj.SetActive(value: true);
				_matchingRecruit.SetActive(value: false);
				_matchingEnable.SetActive(value: false);
				_matchingDisable.SetActive(value: true);
				break;
			}
		}

		public void SetNgReason(string text)
		{
			if (_NgReason != null)
			{
				_NgReason.text = text;
			}
		}

		public void SetMatchingNum(int preparedNum, int entryNum)
		{
			string text = preparedNum.ToString("0") + "/" + entryNum.ToString("0");
			_matchingNum.text = text;
		}

		public void SetPlayerInfo(int playerID, int difficulty, MatchingPlayerObject.PlayerState playerState, string userName, Texture2D texture)
		{
			if (playerID >= _matchingPlayerInfo.Length)
			{
				return;
			}
			if (playerState != MatchingPlayerObject.PlayerState.None)
			{
				_matchingPlayerInfo[playerID].gameObject.SetActive(value: true);
				_matchingPlayerInfo[playerID].SetPlayerInfo(playerID, difficulty, playerState, userName, texture);
				_matchingList[playerID] = true;
				_settingOkList[playerID] = playerState == MatchingPlayerObject.PlayerState.Ready;
			}
			else
			{
				_matchingPlayerInfo[playerID].gameObject.SetActive(value: false);
				_matchingList[playerID] = false;
				_settingOkList[playerID] = false;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < _playerInfoPosition.Length; i++)
			{
				if (_matchingList[i])
				{
					num2++;
				}
				if (_settingOkList[i])
				{
					num++;
				}
			}
			SetMatchingNum(num, num2);
		}

		public void SetRecruitBaseSize(bool isBig)
		{
			float num = (isBig ? 0.8f : 0.65f);
			_matchingRecruit.transform.localScale = new Vector3(num, num);
		}

		private void Start()
		{
			_matchingPlayerInfo = new MatchingPlayerObject[_playerInfoPosition.Length];
			_matchingList = new bool[_playerInfoPosition.Length];
			_settingOkList = new bool[_playerInfoPosition.Length];
			for (int i = 0; i < _playerInfoPosition.Length; i++)
			{
				_matchingPlayerInfo[i] = Object.Instantiate(_originalMatchingPlayerInfo, _playerInfoPosition[i]);
				_matchingPlayerInfo[i].gameObject.SetActive(value: false);
				_matchingList[i] = false;
				_settingOkList[i] = false;
			}
		}
	}
}
