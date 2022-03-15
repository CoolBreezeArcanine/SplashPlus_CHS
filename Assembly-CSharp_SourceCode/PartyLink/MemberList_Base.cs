using System;
using System.Collections.Generic;

namespace PartyLink
{
	public abstract class MemberList_Base<T> : List<T> where T : class, IDisposable
	{
		public void ClearWithDispose()
		{
			Util.SafeDispose(this);
			Clear();
		}

		public void erase_if(Predicate<T> match)
		{
			for (int i = 0; i < base.Count; i++)
			{
				if (match(base[i]) && base[i] != null)
				{
					base[i].Dispose();
					base[i] = null;
				}
			}
			RemoveAll((T m) => m == null);
		}
	}
}
