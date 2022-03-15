using System.Collections.Generic;

namespace ExpansionImage
{
	public static class CustomListPool<T>
	{
		private static readonly CustomObjectPool<List<T>> listPool = new CustomObjectPool<List<T>>(null, delegate(List<T> l)
		{
			l.Clear();
		});

		public static List<T> Get()
		{
			return listPool.Get();
		}

		public static void Release(List<T> toRelease)
		{
			listPool.Release(toRelease);
		}
	}
}
