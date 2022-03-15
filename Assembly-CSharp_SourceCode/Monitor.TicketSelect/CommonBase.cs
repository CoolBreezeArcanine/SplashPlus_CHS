using System.Collections.Generic;
using UnityEngine;

namespace Monitor.TicketSelect
{
	public class CommonBase
	{
		public void SetEmptyPrefab(GameObject parent, GameObject prefab, GameObject child)
		{
			if (parent != null && prefab != null && child != null)
			{
				GameObject gameObject = new GameObject("Empty0");
				gameObject.transform.SetParent(parent.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				gameObject.transform.localScale = Vector3.one;
				child.transform.SetParent(gameObject.transform);
				child.transform.localPosition = prefab.transform.localPosition;
				child.transform.localRotation = prefab.transform.localRotation;
				child.transform.localScale = prefab.transform.localScale;
			}
		}

		public void SetEmpty2Prefab(GameObject parent, GameObject prefab, GameObject child)
		{
			if (parent != null && prefab != null && child != null)
			{
				GameObject gameObject = new GameObject("Empty1");
				gameObject.transform.SetParent(parent.transform);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				gameObject.transform.localScale = Vector3.one;
				GameObject gameObject2 = new GameObject("Empty2");
				gameObject2.transform.SetParent(gameObject.transform);
				gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject2.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				gameObject2.transform.localScale = Vector3.one;
				child.transform.SetParent(gameObject2.transform);
				child.transform.localPosition = prefab.transform.localPosition;
				child.transform.localRotation = prefab.transform.localRotation;
				child.transform.localScale = prefab.transform.localScale;
			}
		}

		public void SetEmptyWithPrefab(Transform _base, List<GameObject> _nullList, List<GameObject> _objList, GameObject _prefab)
		{
			int count = _nullList.Count;
			GameObject gameObject = new GameObject("Empty");
			gameObject.transform.SetParent(_base.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			gameObject.transform.localScale = Vector3.one;
			_nullList.Add(gameObject);
			_objList.Add(Object.Instantiate(_prefab));
			_objList[count].transform.SetParent(_nullList[count].transform);
			_objList[count].transform.localPosition = _prefab.transform.localPosition;
			_objList[count].transform.localRotation = _prefab.transform.localRotation;
			_objList[count].transform.localScale = _prefab.transform.localScale;
			int childCount = _objList[count].transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				_objList[count].transform.GetChild(i).gameObject.SetActive(value: false);
			}
		}

		public bool IsEndAnim(Animator anim)
		{
			if (anim == null || anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				return true;
			}
			return false;
		}
	}
}
