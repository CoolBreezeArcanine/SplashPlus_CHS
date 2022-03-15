using TMPro;
using UnityEngine;

namespace Monitor.MusicSelect.OtherParts
{
	public class ScoreAttackRankingInfo : MonoBehaviour
	{
		[SerializeField]
		private GameObject _musicInfo;

		[SerializeField]
		private GameObject _rankingInfo;

		[SerializeField]
		private TextMeshProUGUI _textScore;

		[SerializeField]
		private TextMeshProUGUI _textRanking;

		private string _scoreText;

		private string _rankingTextBef;

		private string _rankingTextAft;

		public void Initialize()
		{
			_scoreText = "現在のスコア\u3000";
			_rankingTextBef = "現在の順位:";
			_rankingTextAft = "位";
			SetText(0, 999999);
		}

		public void SetRankingInfo(bool isRanking, int score = 0, int ranking = -1)
		{
			if (isRanking)
			{
				_musicInfo.SetActive(value: false);
				_rankingInfo.SetActive(value: true);
				SetText(score, ranking);
			}
			else
			{
				_musicInfo.SetActive(value: true);
				_rankingInfo.SetActive(value: false);
			}
		}

		private void SetText(int score, int ranking)
		{
			int num = score / 10000;
			int num2 = score % 10000;
			_textScore.SetText(_scoreText + num + "." + num2.ToString("0000") + "%");
			if (ranking > 0)
			{
				_textRanking.SetText(_rankingTextBef + ranking + _rankingTextAft);
			}
			else
			{
				_textRanking.SetText(_rankingTextBef + "------" + _rankingTextAft);
			}
		}
	}
}
