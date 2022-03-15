using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ChallengeRelax : AccessorBase
	{
		public int Day { get; private set; }

		public int Life { get; private set; }

		public StringID ReleaseDiff { get; private set; }

		public ChallengeRelax()
		{
			Day = 0;
			Life = 0;
			ReleaseDiff = new StringID();
		}

		public void Init(Manager.MaiStudio.Serialize.ChallengeRelax sz)
		{
			Day = sz.Day;
			Life = sz.Life;
			ReleaseDiff = (StringID)sz.ReleaseDiff;
		}
	}
}
