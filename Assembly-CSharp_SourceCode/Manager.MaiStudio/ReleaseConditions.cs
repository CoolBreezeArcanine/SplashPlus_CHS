using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class ReleaseConditions : AccessorBase
	{
		public ReleaseConditionCategory category { get; private set; }

		public ReleaseItemConditions condition0 { get; private set; }

		public ReleaseItemConditions condition1 { get; private set; }

		public ReleaseItemConditions condition2 { get; private set; }

		public ReleaseItemConditions condition3 { get; private set; }

		public ReleaseItemConditions condition4 { get; private set; }

		public ReleaseConditions()
		{
			category = ReleaseConditionCategory.Track;
			condition0 = new ReleaseItemConditions();
			condition1 = new ReleaseItemConditions();
			condition2 = new ReleaseItemConditions();
			condition3 = new ReleaseItemConditions();
			condition4 = new ReleaseItemConditions();
		}

		public void Init(Manager.MaiStudio.Serialize.ReleaseConditions sz)
		{
			category = sz.category;
			condition0 = (ReleaseItemConditions)sz.condition0;
			condition1 = (ReleaseItemConditions)sz.condition1;
			condition2 = (ReleaseItemConditions)sz.condition2;
			condition3 = (ReleaseItemConditions)sz.condition3;
			condition4 = (ReleaseItemConditions)sz.condition4;
		}
	}
}
