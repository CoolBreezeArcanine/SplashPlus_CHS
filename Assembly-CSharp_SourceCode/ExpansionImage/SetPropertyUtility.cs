using System;
using UnityEngine;

namespace ExpansionImage
{
	public static class SetPropertyUtility
	{
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if ((double)currentValue.r == (double)newValue.r && (double)currentValue.g == (double)newValue.g && (double)currentValue.b == (double)newValue.b && (double)currentValue.a == (double)newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue, Action<T> onSuccess) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			onSuccess(currentValue);
			return true;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			return SetStruct(ref currentValue, newValue, delegate
			{
			});
		}

		public static bool SetClass<T>(ref T currentValue, T newValue, Action<T> onSuccess) where T : class
		{
			if (currentValue == null && newValue == null)
			{
				return false;
			}
			if (currentValue != null && currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			onSuccess(currentValue);
			return true;
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			return SetClass(ref currentValue, newValue, delegate
			{
			});
		}
	}
}
