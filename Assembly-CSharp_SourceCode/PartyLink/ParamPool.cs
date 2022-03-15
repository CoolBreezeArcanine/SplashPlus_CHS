using System.Collections.Generic;

namespace PartyLink
{
	public class ParamPool<Type> where Type : class, new()
	{
		private List<Type> _useList;

		private List<Type> _poolList;

		public ParamPool(int maxCount)
		{
			initialize(maxCount);
		}

		public Type Get()
		{
			Type val = null;
			if (_poolList.Count > 0)
			{
				int index = _poolList.Count - 1;
				val = _poolList[index];
				_poolList.RemoveAt(index);
				_useList.Add(val);
			}
			return val;
		}

		public bool Return(Type t)
		{
			bool result = false;
			if (_useList.Contains(t))
			{
				_useList.Remove(t);
				_poolList.Add(t);
				result = true;
			}
			return result;
		}

		private void initialize(int maxCount)
		{
			_poolList = new List<Type>();
			_useList = new List<Type>();
			for (int i = 0; i < maxCount; i++)
			{
				_poolList.Add(new Type());
			}
		}
	}
}
