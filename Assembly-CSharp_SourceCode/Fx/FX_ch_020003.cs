using System.Collections.Generic;
using UnityEngine;

namespace FX
{
	public class FX_ch_020003 : MonoBehaviour
	{
		[SerializeField]
		private GameObject _fX;

		[SerializeField]
		private Transform _parent;

		[SerializeField]
		private Transform _controller;

		private int propertyID;

		private List<GameObject> list;

		private List<Renderer> renderers;

		private void Start()
		{
			GameObject obj = Object.Instantiate(_fX);
			obj.transform.position = _parent.position;
			obj.transform.parent = _parent;
			obj.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			propertyID = Shader.PropertyToID("opacity");
			list = base.gameObject.GetAll();
			foreach (GameObject item in list)
			{
				renderers.Add(item.GetComponent<Renderer>());
			}
		}

		private void Update()
		{
			if (renderers == null)
			{
				return;
			}
			foreach (Renderer renderer in renderers)
			{
				renderer.material.SetFloat(propertyID, _controller.transform.localScale.z);
			}
		}
	}
}
