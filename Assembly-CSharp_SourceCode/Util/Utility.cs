using System.Collections.Generic;
using System.Diagnostics;
using Manager.MaiStudio;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
	public static class Utility
	{
		public static void destroyGameObject(ref GameObject gameObject)
		{
			if (null != gameObject)
			{
				Object.Destroy(gameObject);
				gameObject = null;
			}
		}

		public static void destroyGameObject<T>(ref T component) where T : Component
		{
			if ((Object)null != (Object)component)
			{
				Object.Destroy(component.gameObject);
				component = null;
			}
		}

		public static void destroyGameObject(ref GameObject gameObject, float delay)
		{
			if (null != gameObject)
			{
				Object.Destroy(gameObject, delay);
				gameObject = null;
			}
		}

		public static void destroyGameObject<T>(ref T component, float delay) where T : Component
		{
			if ((Object)null != (Object)component)
			{
				Object.Destroy(component.gameObject, delay);
				component = null;
			}
		}

		public static void destroyAllChildren(Transform root)
		{
			while (0 < root.childCount)
			{
				Transform child = root.GetChild(0);
				child.SetParent(null);
				Object.Destroy(child.gameObject);
			}
		}

		public static GameObject Instantiate(GameObject prefab, Transform parent, string name = null)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.name = ((name == null) ? prefab.name : name);
			if (null != parent)
			{
				Transform transform = gameObject.transform;
				transform.SetParent(parent, worldPositionStays: false);
				transform.SetAsLastSibling();
			}
			return gameObject;
		}

		public static GameObject InstantiateSwap(GameObject prefab, Transform dummyObject)
		{
			if (prefab == null)
			{
				return null;
			}
			GameObject gameObject = Instantiate(prefab, dummyObject.parent, dummyObject.name);
			gameObject.transform.localScale = dummyObject.localScale;
			gameObject.transform.localPosition = dummyObject.localPosition;
			destroyGameObject(ref dummyObject);
			return gameObject;
		}

		public static GameObject Instantiate2D(GameObject prefab, RectTransform parent, string name = null)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.name = ((name == null) ? prefab.name : name);
			if (null != parent)
			{
				RectTransform component = gameObject.GetComponent<RectTransform>();
				if (null == component)
				{
					Transform transform = gameObject.transform;
					transform.SetParent(parent, worldPositionStays: false);
					transform.SetAsLastSibling();
				}
				else
				{
					component.SetParent(parent, worldPositionStays: false);
					component.SetAsLastSibling();
				}
			}
			return gameObject;
		}

		public static void setParent2D(GameObject gameObject, RectTransform parent)
		{
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.SetParent(parent, worldPositionStays: false);
			component.SetAsLastSibling();
		}

		public static void setLocalIndentity(GameObject gameObject)
		{
			Transform transform = gameObject.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		public static void setLayerOfTree(Transform transform, int layer)
		{
			transform.gameObject.layer = layer;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				setLayerOfTree(transform.GetChild(i), layer);
			}
		}

		public static void setColor(int property, Color color, Transform root)
		{
			Renderer component = root.GetComponent<Renderer>();
			if (null != component && component.sharedMaterials != null)
			{
				Material[] materials = component.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					if (!(null == materials[i]) && materials[i].HasProperty(property))
					{
						materials[i].SetColor(property, color);
					}
				}
				component.materials = materials;
			}
			int childCount = root.childCount;
			for (int j = 0; j < childCount; j++)
			{
				setColor(property, color, root.GetChild(j));
			}
		}

		public static void swap<T>(ref T x0, ref T x1)
		{
			T val = x0;
			x0 = x1;
			x1 = val;
		}

		public static void swap<T>(T[] array, int i0, int i1)
		{
			T val = array[i0];
			array[i0] = array[i1];
			array[i1] = val;
		}

		public static void swap<T>(List<T> list, int i0, int i1)
		{
			T value = list[i0];
			list[i0] = list[i1];
			list[i1] = value;
		}

		public static Transform findChildRecursive(Transform t, string name)
		{
			Transform transform = t.Find(name);
			if (transform != null)
			{
				return transform;
			}
			for (int i = 0; i < t.childCount; i++)
			{
				transform = findChildRecursive(t.GetChild(i), name);
				if (transform != null)
				{
					return transform;
				}
			}
			return null;
		}

		public static T findChildRecursive<T>(Transform t, string name) where T : Component
		{
			T val = null;
			Transform transform = t.Find(name);
			val = ((transform != null) ? transform.GetComponent<T>() : null);
			if ((Object)val != (Object)null)
			{
				return val;
			}
			for (int i = 0; i < t.childCount; i++)
			{
				transform = findChildRecursive(t.GetChild(i), name);
				val = ((transform != null) ? transform.GetComponent<T>() : null);
				if ((Object)val != (Object)null)
				{
					return val;
				}
			}
			return null;
		}

		public static T findParentRecursive<T>(Transform t) where T : Component
		{
			if (t == null)
			{
				return null;
			}
			T component = t.GetComponent<T>();
			if ((Object)component != (Object)null)
			{
				return component;
			}
			if (t.parent != null)
			{
				return findParentRecursive<T>(t.parent);
			}
			return null;
		}

		public static void getMaterials(List<Material> materials, Transform root)
		{
			Renderer component = root.GetComponent<Renderer>();
			if (null != component)
			{
				Material[] materials2 = component.materials;
				for (int i = 0; i < materials2.Length; i++)
				{
					if (!(null == materials2[i]))
					{
						materials.Add(materials2[i]);
					}
				}
			}
			int childCount = root.childCount;
			for (int j = 0; j < childCount; j++)
			{
				getMaterials(materials, root.GetChild(j));
			}
		}

		public static void disableImageComponent(Transform trans)
		{
			Image component = trans.GetComponent<Image>();
			if (!(null == component))
			{
				component.enabled = false;
			}
		}

		public static bool isBlinkDisp(Stopwatch timer)
		{
			return ((int)(0.002f * (float)timer.ElapsedMilliseconds) & 1) == 0;
		}

		public static long createRandomUserId()
		{
			return 0L;
		}

		public static int safeAdd(int a, int b, int max = int.MaxValue)
		{
			if (a > max)
			{
				return max;
			}
			if (b > max - a)
			{
				return max;
			}
			return a + b;
		}

		public static uint safeAdd(uint a, uint b, uint max = 2147483647u)
		{
			if (a > max)
			{
				return max;
			}
			if (b > max - a)
			{
				return max;
			}
			return a + b;
		}

		public static long safeAdd(long a, long b, long max = long.MaxValue)
		{
			if (a > max)
			{
				return max;
			}
			if (b > max - a)
			{
				return max;
			}
			return a + b;
		}

		public static ulong safeAdd(ulong a, ulong b, ulong max = 9223372036854775807uL)
		{
			if (a > max)
			{
				return max;
			}
			if (b > max - a)
			{
				return max;
			}
			return a + b;
		}

		public static int safeSub(int a, int b, int min = 0)
		{
			if (a < min)
			{
				return min;
			}
			if (b > a - min)
			{
				return min;
			}
			return a - b;
		}

		public static uint safeSub(uint a, uint b, uint min = 0u)
		{
			if (a < min)
			{
				return min;
			}
			if (b > a - min)
			{
				return min;
			}
			return a - b;
		}

		public static long safeSub(long a, long b, long min = 0L)
		{
			if (a < min)
			{
				return min;
			}
			if (b > a - min)
			{
				return min;
			}
			return a - b;
		}

		public static ulong safeSub(ulong a, ulong b, ulong min = 0uL)
		{
			if (a < min)
			{
				return min;
			}
			if (b > a - min)
			{
				return min;
			}
			return a - b;
		}

		public static Color ConvertColor(uint color)
		{
			float num = 0.003921569f;
			return new Color(num * (float)((color >> 16) & 0xFFu), num * (float)((color >> 8) & 0xFFu), num * (float)(color & 0xFFu), 1f);
		}

		public static Color ConvertColor(Color24 color)
		{
			float num = 0.003921569f;
			return new Color(num * (float)(int)color.R, num * (float)(int)color.G, num * (float)(int)color.B, 1f);
		}
	}
}
