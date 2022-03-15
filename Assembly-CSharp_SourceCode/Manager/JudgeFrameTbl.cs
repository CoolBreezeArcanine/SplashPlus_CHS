namespace Manager
{
	public class JudgeFrameTbl
	{
		public JudgeFrame tap = new JudgeFrame();

		public JudgeFrame ext = new JudgeFrame();

		public JudgeFrame flk = new JudgeFrame();

		public JudgeFrame mne = new JudgeFrame();

		public JudgeFrameTbl()
		{
			init();
		}

		public void init()
		{
			tap.init();
			ext.init();
			flk.init();
			mne.init();
		}
	}
}
