using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ChallengeRelax : SerializeBase
	{
		public int Day;

		public int Life;

		public StringID ReleaseDiff;

		public ChallengeRelax()
		{
			Day = 0;
			Life = 0;
			ReleaseDiff = new StringID();
		}

		public static explicit operator Manager.MaiStudio.ChallengeRelax(ChallengeRelax sz)
		{
			Manager.MaiStudio.ChallengeRelax challengeRelax = new Manager.MaiStudio.ChallengeRelax();
			challengeRelax.Init(sz);
			return challengeRelax;
		}

		public override void AddPath(string parentPath)
		{
			ReleaseDiff.AddPath(parentPath);
		}
	}
}
