using UnityEngine;

namespace Note
{
	public class JudgeTapChecker
	{
		public JudgeArea _judgeArea = new JudgeArea();

		public float _checkStart;

		public float _checkEnd;

		public JudgeTiming _tempResult;

		public JudgeTiming _finalResult;

		public JudgeTapChecker()
		{
			clear();
		}

		public void init(JudgeArea judgeArea)
		{
			clear();
			_judgeArea.set(judgeArea.mis.st, judgeArea.gd.st, judgeArea.gr.st, judgeArea.pf.st, judgeArea.fix.st, judgeArea.mis.ed, judgeArea.gd.ed, judgeArea.gr.ed, judgeArea.pf.ed, judgeArea.fix.ed);
			updateArea();
		}

		private void clear()
		{
			_checkStart = 0f;
			_checkEnd = 0f;
			_tempResult = JudgeTiming.Begin;
			_finalResult = JudgeTiming.Begin;
		}

		public void offset(float frame)
		{
			_judgeArea.offset(frame);
			_checkStart += frame;
			_checkEnd += frame;
		}

		public void cutFastFrame(float frame)
		{
			_judgeArea.cutFast(frame);
			updateArea();
		}

		public void cutLateFrame(float frame)
		{
			_judgeArea.cutLate(frame);
			updateArea();
		}

		private void updateArea()
		{
			if (_judgeArea.enable)
			{
				_checkStart = Mathf.Min(_checkStart, _judgeArea.mis.st);
				_checkEnd = Mathf.Max(_checkEnd, _judgeArea.mis.ed);
			}
			else
			{
				_checkStart = 0f;
				_checkEnd = 0f;
			}
		}

		public bool getJudgeArea(out JudgeArea judgeArea)
		{
			judgeArea = null;
			if (!_judgeArea.isEnable())
			{
				return false;
			}
			judgeArea = _judgeArea;
			return true;
		}

		public float getStartFrame()
		{
			return _checkStart;
		}

		public bool isIn(float frame)
		{
			return frame > _checkStart;
		}

		public float getStartFramePos()
		{
			if (!_judgeArea.enable)
			{
				return 0f;
			}
			return _judgeArea.mis.st;
		}

		public float getCenterFrame()
		{
			if (!_judgeArea.enable)
			{
				return 0f;
			}
			return _judgeArea.center;
		}

		public bool isLater(float frame)
		{
			if (!_judgeArea.enable)
			{
				return false;
			}
			return frame >= _judgeArea.center;
		}

		public float getEndFrame()
		{
			return _checkEnd;
		}

		public bool isOut(float frame)
		{
			return frame > _checkEnd;
		}

		public float getEndFramePos()
		{
			if (!_judgeArea.enable)
			{
				return 0f;
			}
			return _judgeArea.mis.ed;
		}

		public JudgeTiming check(float curFrame, out Judge judge, out Timing timing)
		{
			JudgeTiming result = _judgeArea.getResult(curFrame);
			NoteUtil.tempResult2Result(result, out judge, out timing);
			return result;
		}

		public void setTempResult(JudgeTiming result)
		{
			_tempResult = result;
		}

		public bool isTempResult()
		{
			return _tempResult >= JudgeTiming.FastMiss;
		}

		public JudgeTiming getTempResult()
		{
			return _tempResult;
		}

		public Judge getTempResultID()
		{
			Judge judge = Judge.Invalid;
			Timing timing = Timing.End;
			NoteUtil.tempResult2Result(_tempResult, out judge, out timing);
			return judge;
		}

		public Timing getTempTimingID()
		{
			Judge judge = Judge.Invalid;
			Timing timing = Timing.End;
			NoteUtil.tempResult2Result(_tempResult, out judge, out timing);
			return timing;
		}

		public void setFinalResult(JudgeTiming result)
		{
			_finalResult = result;
		}

		public bool isFinalResult()
		{
			return _finalResult >= JudgeTiming.FastMiss;
		}

		public JudgeTiming getFinalResult()
		{
			return _finalResult;
		}

		public Judge getFinalJudge()
		{
			Judge judge = Judge.Invalid;
			Timing timing = Timing.End;
			NoteUtil.tempResult2Result(_finalResult, out judge, out timing);
			return judge;
		}

		public Timing getFinalTiming()
		{
			Judge judge = Judge.Invalid;
			Timing timing = Timing.End;
			NoteUtil.tempResult2Result(_finalResult, out judge, out timing);
			return timing;
		}
	}
}
