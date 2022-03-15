using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CustomMaskUtilities
{
	public static int GetStencilDepth<T>(Transform transform, Transform stopAfter) where T : Mask
	{
		int num = 0;
		if (transform == stopAfter)
		{
			return num;
		}
		Transform parent = transform.parent;
		List<T> list = CustomListPool<T>.Get();
		while (parent != null)
		{
			parent.GetComponents(list);
			for (int i = 0; i < list.Count; i++)
			{
				if ((Object)list[i] != (Object)null && list[i].MaskEnabled())
				{
					num++;
					break;
				}
			}
			if (parent == stopAfter)
			{
				break;
			}
			parent = parent.parent;
		}
		CustomListPool<T>.Release(list);
		return num;
	}

	public static T[] GetMaskParentComponents<T>(Transform transform, Transform stopAfter) where T : Mask
	{
		List<T> list = CustomListPool<T>.Get();
		Transform parent = transform.parent;
		List<T> list2 = CustomListPool<T>.Get();
		while (parent != null)
		{
			parent.GetComponents(list2);
			for (int i = 0; i < list2.Count; i++)
			{
				if ((Object)list2[i] != (Object)null && list2[i].MaskEnabled())
				{
					list.Add(list2[i]);
					break;
				}
			}
			if (parent == stopAfter)
			{
				break;
			}
			parent = parent.parent;
		}
		T[] result = list.ToArray();
		CustomListPool<T>.Release(list2);
		CustomListPool<T>.Release(list);
		return result;
	}
}
