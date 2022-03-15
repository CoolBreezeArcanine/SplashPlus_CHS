using System.Collections;
using System.Collections.Generic;

namespace MAI2
{
	public class ListList<T> : IEnumerable<T>, IEnumerable
	{
		private List<List<T>> _listList = new List<List<T>>();

		public T this[int index]
		{
			get
			{
				foreach (List<T> list in _listList)
				{
					if (index < list.Count)
					{
						return list[index];
					}
					index -= list.Count;
				}
				return default(T);
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				foreach (List<T> list in _listList)
				{
					num += list.Count;
				}
				return num;
			}
		}

		public int ListCount => _listList.Count;

		public IEnumerator<T> GetEnumerator()
		{
			foreach (List<T> list in _listList)
			{
				foreach (T item in list)
				{
					yield return item;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void AddList(List<T> list)
		{
			_listList.Add(list);
		}

		public void RemoveList(List<T> list)
		{
			_listList.Remove(list);
		}

		public List<T> GetList(int index)
		{
			return _listList[index];
		}
	}
}
