using System.Collections.Generic;
using UnityEngine.Events;

namespace ExpansionImage
{
	public class CustomObjectPool<T> where T : new()
	{
		private readonly Stack<T> stack = new Stack<T>();

		private readonly UnityAction<T> actionOnGet;

		private readonly UnityAction<T> actionOnRelease;

		public int CountAll { get; private set; }

		public int CountActive => CountAll - CountInactive;

		public int CountInactive => stack.Count;

		public CustomObjectPool(UnityAction<T> onGet, UnityAction<T> onRelease)
		{
			actionOnGet = onGet;
			actionOnRelease = onRelease;
		}

		public T Get()
		{
			T val;
			if (stack.Count == 0)
			{
				val = new T();
				CountAll++;
			}
			else
			{
				val = stack.Pop();
			}
			if (actionOnGet != null)
			{
				actionOnGet(val);
			}
			return val;
		}

		public void Release(T element)
		{
			if (stack.Count > 0)
			{
				_ = (object)stack.Peek();
				_ = (object)element;
			}
			if (actionOnRelease != null)
			{
				actionOnRelease(element);
			}
			stack.Push(element);
		}
	}
}
