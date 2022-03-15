using System.Collections.Generic;
using UnityEngine;

public class ChildDistnationCheck : MonoBehaviour
{
	public List<double> DistList = new List<double>();

	public bool Check;

	private void Update()
	{
		if (!Check)
		{
			return;
		}
		DistList.Clear();
		if (base.gameObject.transform.childCount >= 2)
		{
			Vector3 a = base.gameObject.transform.GetChild(0).transform.position;
			for (int i = 1; i < base.gameObject.transform.childCount; i++)
			{
				Vector3 position = base.gameObject.transform.GetChild(i).transform.position;
				DistList.Add(Vector3.Distance(a, position));
				a = position;
			}
		}
		Check = false;
	}
}
