using System.Collections.Generic;
using Manager.MaiStudio;

namespace Manager.Achieve
{
	public static class ReleaseConditionsExtention
	{
		public static IEnumerable<ReleaseItemConditions> GetItemConditions(this ReleaseConditions src)
		{
			yield return src.condition0;
			yield return src.condition1;
			yield return src.condition2;
			yield return src.condition3;
			yield return src.condition4;
		}
	}
}
