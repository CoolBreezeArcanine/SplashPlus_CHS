using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
	public class SoflanDataList : List<SoflanData>
	{
		public bool Empty()
		{
			return !this.Any();
		}

		public float getMsecDraw(float msecTarget, float msecCurrent)
		{
			if (msecTarget < msecCurrent)
			{
				return msecTarget;
			}
			float num = 0f;
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SoflanData current = enumerator.Current;
					if (current.compare(msecCurrent) != 1)
					{
						if (current.compare(msecTarget) == -1)
						{
							break;
						}
						float num2 = Mathf.Max(current.time.msec, msecCurrent);
						float num3 = Mathf.Min(current.end.msec, msecTarget);
						num += (num3 - num2) * (current.speed - 1f);
					}
				}
			}
			return msecTarget + num;
		}
	}
}
