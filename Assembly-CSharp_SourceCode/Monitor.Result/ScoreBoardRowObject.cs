using UnityEngine;

namespace Monitor.Result
{
	public class ScoreBoardRowObject : MonoBehaviour
	{
		[SerializeField]
		[Header("クリティカルパーフェクト")]
		private ScoreBoardColumnObject _critical;

		[SerializeField]
		[Header("パーフェクト")]
		private ScoreBoardColumnObject _perfect;

		[SerializeField]
		[Header("グレート")]
		private ScoreBoardColumnObject _great;

		[SerializeField]
		[Header("グッド")]
		private ScoreBoardColumnObject _good;

		[SerializeField]
		[Header("ミス")]
		private ScoreBoardColumnObject _miss;

		public void SetScoreData(uint critical, uint perfect, uint great, uint good, uint miss)
		{
			_critical.SetScore(critical);
			_perfect.SetScore(perfect);
			_great.SetScore(great);
			_good.SetScore(good);
			_miss.SetScore(miss);
		}

		public void SetScoreData(NoteJudge.JudgeBox type, uint score)
		{
			switch (type)
			{
			case NoteJudge.JudgeBox.Critical:
				_critical.SetScore(score);
				break;
			case NoteJudge.JudgeBox.Perfect:
				_perfect.SetScore(score);
				break;
			case NoteJudge.JudgeBox.Great:
				_great.SetScore(score);
				break;
			case NoteJudge.JudgeBox.Good:
				_good.SetScore(score);
				break;
			case NoteJudge.JudgeBox.Miss:
				_miss.SetScore(score);
				break;
			}
		}

		public void SetVisibleCloseBox(NoteJudge.JudgeBox type, bool isVisible)
		{
			switch (type)
			{
			case NoteJudge.JudgeBox.Critical:
				_critical.SetVisibleCloseBox(isVisible);
				break;
			case NoteJudge.JudgeBox.Perfect:
				_perfect.SetVisibleCloseBox(isVisible);
				break;
			case NoteJudge.JudgeBox.Great:
				_great.SetVisibleCloseBox(isVisible);
				break;
			case NoteJudge.JudgeBox.Good:
				_good.SetVisibleCloseBox(isVisible);
				break;
			case NoteJudge.JudgeBox.Miss:
				_miss.SetVisibleCloseBox(isVisible);
				break;
			}
		}

		public void SetVisibleCloseBox(bool isVisible)
		{
			_critical.SetVisibleCloseBox(isVisible);
			_perfect.SetVisibleCloseBox(isVisible);
			_great.SetVisibleCloseBox(isVisible);
			_good.SetVisibleCloseBox(isVisible);
			_miss.SetVisibleCloseBox(isVisible);
		}
	}
}
