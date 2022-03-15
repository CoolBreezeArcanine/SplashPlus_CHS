using System;
using UnityEngine;

namespace SplitFlapDisplay
{
	[Serializable]
	public class DisplayElements<T> : ScriptableObject
	{
		[SerializeField]
		private T[] _elements;

		public int GetCount()
		{
			return _elements.Length;
		}

		public T GetElement(int index)
		{
			return _elements[index];
		}
	}
	[CreateAssetMenu(menuName = "DisplayElements", order = 1000)]
	public class DisplayElements : ScriptableObject
	{
		[SerializeField]
		private DisplayElement[] _elements;

		public int GetCount()
		{
			DisplayElement[] elements = _elements;
			if (elements == null)
			{
				return 0;
			}
			return elements.Length;
		}

		public DisplayElement GetElement(int index)
		{
			return _elements[index];
		}
	}
}
