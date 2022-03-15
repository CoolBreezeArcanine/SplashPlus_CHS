using UnityEngine;

namespace Monitor.Result
{
	public class ScoreBoardController : MonoBehaviour
	{
		[SerializeField]
		private ScoreBoardRowObject _tap;

		[SerializeField]
		private ScoreBoardRowObject _hold;

		[SerializeField]
		private ScoreBoardRowObject _slide;

		[SerializeField]
		private ScoreBoardRowObject _touch;

		[SerializeField]
		private ScoreBoardRowObject _break;

		public void SetScore(NoteScore.EScoreType type, uint critical, uint perfect, uint great, uint good, uint miss)
		{
			switch (type)
			{
			case NoteScore.EScoreType.Tap:
				_tap.SetScoreData(critical, perfect, great, good, miss);
				break;
			case NoteScore.EScoreType.Hold:
				_hold.SetScoreData(critical, perfect, great, good, miss);
				break;
			case NoteScore.EScoreType.Slide:
				_slide.SetScoreData(critical, perfect, great, good, miss);
				break;
			case NoteScore.EScoreType.Touch:
				_touch.SetScoreData(critical, perfect, great, good, miss);
				break;
			case NoteScore.EScoreType.Break:
				_break.SetScoreData(critical, perfect, great, good, miss);
				break;
			}
		}

		public void SetScore(NoteScore.EScoreType type, NoteJudge.JudgeBox notesType, uint score)
		{
			switch (type)
			{
			case NoteScore.EScoreType.Tap:
				_tap.SetScoreData(notesType, score);
				break;
			case NoteScore.EScoreType.Hold:
				_hold.SetScoreData(notesType, score);
				break;
			case NoteScore.EScoreType.Slide:
				_slide.SetScoreData(notesType, score);
				break;
			case NoteScore.EScoreType.Touch:
				_touch.SetScoreData(notesType, score);
				break;
			case NoteScore.EScoreType.Break:
				_break.SetScoreData(notesType, score);
				break;
			}
		}

		public void SetVisibleCloseBox(NoteScore.EScoreType type, NoteJudge.JudgeBox notesType, bool isVisible)
		{
			switch (type)
			{
			case NoteScore.EScoreType.Tap:
				_tap.SetVisibleCloseBox(notesType, isVisible);
				break;
			case NoteScore.EScoreType.Hold:
				_hold.SetVisibleCloseBox(notesType, isVisible);
				break;
			case NoteScore.EScoreType.Slide:
				_slide.SetVisibleCloseBox(notesType, isVisible);
				break;
			case NoteScore.EScoreType.Touch:
				_touch.SetVisibleCloseBox(notesType, isVisible);
				break;
			case NoteScore.EScoreType.Break:
				_break.SetVisibleCloseBox(notesType, isVisible);
				break;
			}
		}

		public void SetVisibleCloseBoxAll(NoteScore.EScoreType type, bool isVisible)
		{
			switch (type)
			{
			case NoteScore.EScoreType.Tap:
				_tap.SetVisibleCloseBox(isVisible);
				break;
			case NoteScore.EScoreType.Hold:
				_hold.SetVisibleCloseBox(isVisible);
				break;
			case NoteScore.EScoreType.Slide:
				_slide.SetVisibleCloseBox(isVisible);
				break;
			case NoteScore.EScoreType.Touch:
				_touch.SetVisibleCloseBox(isVisible);
				break;
			case NoteScore.EScoreType.Break:
				_break.SetVisibleCloseBox(isVisible);
				break;
			}
		}
	}
}
