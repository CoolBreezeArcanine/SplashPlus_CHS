using System;
using System.Collections.Generic;
using UnityEngine;

namespace FX
{
	public static class GetAllChildren
	{
		public static List<GameObject> GetAll(this GameObject obj)
		{
			List<GameObject> allChildren = new List<GameObject>();
			GetChildren(obj, ref allChildren);
			return allChildren;
		}

		public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
		{
			Transform componentInChildren = obj.GetComponentInChildren<Transform>();
			if (componentInChildren.childCount == 0)
			{
				return;
			}
			foreach (Transform item in componentInChildren)
			{
				allChildren.Add(item.gameObject);
				GetChildren(item.gameObject, ref allChildren);
			}
		}

		public static void GetComponentsInChildren<T>(Transform trans, List<T> components) where T : class
		{
			for (int i = 0; i < trans.childCount; i++)
			{
				Transform child = trans.GetChild(i);
				T component = child.GetComponent<T>();
				if (component != null)
				{
					components.Add(component);
				}
				GetComponentsInChildren(child, components);
			}
		}

		public static void MapComponentsInChildren<T>(Transform trans, Action<T> func) where T : class
		{
			for (int i = 0; i < trans.childCount; i++)
			{
				Transform child = trans.GetChild(i);
				T component = child.GetComponent<T>();
				if (component != null)
				{
					func(component);
				}
				MapComponentsInChildren(child, func);
			}
		}
	}
}
