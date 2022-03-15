using System;
using UnityEngine;

namespace UI
{
	[Serializable]
	public class InstantiateGenerator
	{
		[SerializeField]
		[Header("生成元")]
		private GameObject _original;

		[SerializeField]
		[Header("生成先")]
		private Transform _parent;

		public T Instantiate<T>() where T : UnityEngine.Object
		{
			if (_original == null || _parent == null)
			{
				return null;
			}
			return UnityEngine.Object.Instantiate(_original, _parent).GetComponent<T>();
		}

		public GameObject Instantiate()
		{
			if (_original == null || _parent == null)
			{
				return null;
			}
			return UnityEngine.Object.Instantiate(_original, _parent);
		}
	}
}
