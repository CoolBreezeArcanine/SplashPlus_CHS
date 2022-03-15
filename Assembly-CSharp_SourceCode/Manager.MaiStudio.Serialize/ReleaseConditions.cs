using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class ReleaseConditions : SerializeBase
	{
		public ReleaseConditionCategory category;

		public ReleaseItemConditions condition0;

		public ReleaseItemConditions condition1;

		public ReleaseItemConditions condition2;

		public ReleaseItemConditions condition3;

		public ReleaseItemConditions condition4;

		public ReleaseConditions()
		{
			category = ReleaseConditionCategory.Track;
			condition0 = new ReleaseItemConditions();
			condition1 = new ReleaseItemConditions();
			condition2 = new ReleaseItemConditions();
			condition3 = new ReleaseItemConditions();
			condition4 = new ReleaseItemConditions();
		}

		public static explicit operator Manager.MaiStudio.ReleaseConditions(ReleaseConditions sz)
		{
			Manager.MaiStudio.ReleaseConditions releaseConditions = new Manager.MaiStudio.ReleaseConditions();
			releaseConditions.Init(sz);
			return releaseConditions;
		}

		public override void AddPath(string parentPath)
		{
			condition0.AddPath(parentPath);
			condition1.AddPath(parentPath);
			condition2.AddPath(parentPath);
			condition3.AddPath(parentPath);
			condition4.AddPath(parentPath);
		}
	}
}
