namespace Note
{
	public class JudgeArea
	{
		public bool enable;

		public FrameArea mis = new FrameArea();

		public FrameArea gd = new FrameArea();

		public FrameArea gr = new FrameArea();

		public FrameArea pf = new FrameArea();

		public FrameArea fix = new FrameArea();

		public float center;

		public JudgeArea()
		{
			enable = false;
			center = 0f;
		}

		public void set(float sM, float sGd, float sGr, float sP, float sF, float eM, float eGd, float eGr, float eP, float eF, float c = 0f)
		{
			mis.set(sM, eM);
			gd.set(sGd, eGd);
			gr.set(sGr, eGr);
			pf.set(sP, eP);
			fix.set(sF, eF);
			center = c;
			enable = true;
		}

		public void offset(float offset)
		{
			if (enable)
			{
				mis.st += offset;
				mis.ed += offset;
				gd.st += offset;
				gd.ed += offset;
				gr.st += offset;
				gr.ed += offset;
				pf.st += offset;
				pf.ed += offset;
				fix.st += offset;
				fix.ed += offset;
				center += offset;
			}
		}

		public void cutFast(float frame)
		{
			if (enable)
			{
				if (frame > fix.st)
				{
					frame = fix.st;
				}
				if (frame > center)
				{
					frame = center;
				}
				if (frame > pf.st)
				{
					pf.st = frame;
				}
				if (frame > gr.st)
				{
					gr.st = frame;
				}
				if (frame > gd.st)
				{
					gd.st = frame;
				}
				if (frame > mis.st)
				{
					mis.st = frame;
				}
			}
		}

		public void cutLate(float frame)
		{
			if (enable)
			{
				if (frame < fix.ed)
				{
					frame = fix.ed;
				}
				if (frame < center)
				{
					frame = center;
				}
				if (frame < pf.ed)
				{
					pf.ed = frame;
				}
				if (frame < gr.ed)
				{
					gr.ed = frame;
				}
				if (frame < gd.ed)
				{
					gd.ed = frame;
				}
				if (frame < mis.ed)
				{
					mis.ed = frame;
				}
			}
		}

		public bool isEnable()
		{
			return enable;
		}

		public bool isCheckStart(float frame)
		{
			if (!enable)
			{
				return false;
			}
			return mis.isStart(frame);
		}

		public bool isFast(float frame)
		{
			if (!enable)
			{
				return false;
			}
			return frame < center;
		}

		public bool isCheckEnd(float frame)
		{
			if (!enable)
			{
				return false;
			}
			return mis.isEnd(frame);
		}

		public JudgeTiming getResult(float frame)
		{
			JudgeTiming result = JudgeTiming.Begin;
			if (!enable)
			{
				return result;
			}
			if (pf.isIn(frame))
			{
				return JudgeTiming.Perfect;
			}
			bool flag = frame < center;
			if (gr.isIn(frame))
			{
				return flag ? JudgeTiming.FastGreat : JudgeTiming.LateGreat;
			}
			if (gd.isIn(frame))
			{
				return flag ? JudgeTiming.FastGood : JudgeTiming.LateGood;
			}
			if (mis.isIn(frame))
			{
				return flag ? JudgeTiming.FastMiss : JudgeTiming.LateMiss;
			}
			return (!flag) ? JudgeTiming.TooLate : JudgeTiming.Begin;
		}
	}
}
