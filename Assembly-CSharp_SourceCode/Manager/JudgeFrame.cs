namespace Manager
{
	public class JudgeFrame
	{
		public bool enable;

		public bool minimalize;

		public float areaBefore;

		public float justBefore;

		public float justAfter;

		public float areaAfter;

		public JudgeFrame()
		{
			init();
		}

		public void init(bool e = false, bool m = false, float ab = 0f, float jb = 0f, float ja = 0f, float aa = 0f)
		{
			enable = e;
			minimalize = m;
			areaBefore = ab;
			justBefore = jb;
			justAfter = ja;
			areaAfter = aa;
		}
	}
}
