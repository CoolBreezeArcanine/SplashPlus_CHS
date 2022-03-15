using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
	public static class Safe
	{
		public class ReadonlySortedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
		{
			private SortedDictionary<TKey, TValue> _list;

			public int Count => _list.Count;

			public TValue this[TKey key]
			{
				get
				{
					if (_list.ContainsKey(key))
					{
						return _list[key];
					}
					throw new ArgumentException();
				}
			}

			public ReadonlySortedDictionary(SortedDictionary<TKey, TValue> list)
			{
				if (list != null)
				{
					_list = list;
				}
				else
				{
					_list = new SortedDictionary<TKey, TValue>();
				}
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				foreach (KeyValuePair<TKey, TValue> item in _list)
				{
					yield return item;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public bool ContainsKey(TKey key)
			{
				return _list.ContainsKey(key);
			}

			public bool ContainsValue(TValue value)
			{
				return _list.ContainsValue(value);
			}
		}

		public static bool IsNull<T>(T obj)
		{
			return obj == null;
		}

		public static void Destroy<T>(ref T obj) where T : UnityEngine.Object
		{
			UnityEngine.Object.Destroy(obj);
			obj = null;
		}

		public static void DestroyImmediate<T>(ref T obj) where T : UnityEngine.Object
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = null;
		}

		public static ulong AddNumUlong(ulong a, ulong b, ulong stop = ulong.MaxValue)
		{
			ulong num = 0uL;
			try
			{
				num = checked(a + b);
			}
			catch
			{
				num = stop;
			}
			if (num > stop)
			{
				num = stop;
			}
			return num;
		}

		public static uint AddNumUint(uint a, uint b, uint stop = uint.MaxValue)
		{
			uint num = 0u;
			try
			{
				num = checked(a + b);
			}
			catch
			{
				num = stop;
			}
			if (num > stop)
			{
				num = stop;
			}
			return num;
		}

		public static int SubNumInt(int a, int b)
		{
			int result = 0;
			if (a > b)
			{
				result = a - b;
			}
			return result;
		}

		public static void Normalize(ref Vector2 value)
		{
			Normalize(ref value.x);
			Normalize(ref value.y);
		}

		public static void Normalize(ref float value)
		{
			if (0f > value)
			{
				value = 0f;
			}
			if (value > 1f)
			{
				value = 1f;
			}
		}

		public static double ClampDouble(double value, double min, double max)
		{
			double num = value;
			if (min > num)
			{
				num = min;
			}
			if (num > max)
			{
				num = max;
			}
			return num;
		}

		public static bool IsRangeOk(int value, int overValue, int startValue = 0)
		{
			if (value >= startValue && overValue > value)
			{
				return true;
			}
			return false;
		}
	}
}
