using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using UnityEngine;

namespace Datas.DebugData
{
	[CreateAssetMenu(menuName = "Mai2Data/DebugGameScoreList", fileName = "DebugGameScoreList")]
	public class DebugGameScoreList : ScriptableObject
	{
		[SerializeField]
		public bool IsDetaile;

		[SerializeField]
		private bool _isParty;

		[SerializeField]
		public DebugGameScoreListData[] GameScoreData;

		public GameScoreList GetGameScoreList(int playerIndex, int trackNumber)
		{
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(playerIndex, trackNumber);
			gameScore.DebugSetData(playerIndex, trackNumber, this, _isParty);
			return gameScore;
		}

		public Notes GetNotesData(int index, int track)
		{
			return new Notes();
		}
	}
}
