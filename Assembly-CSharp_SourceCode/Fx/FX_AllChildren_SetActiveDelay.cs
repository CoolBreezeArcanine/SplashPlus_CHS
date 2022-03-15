using System.Collections.Generic;
using UnityEngine;

namespace FX
{
	public class FX_AllChildren_SetActiveDelay : MonoBehaviour
	{
		[SerializeField]
		private float _delayFrame = 30f;

		private float count;

		private List<GameObject> gameObject_list;

		private void Start()
		{
			gameObject_list = base.gameObject.GetAll();
			foreach (GameObject item in gameObject_list)
			{
				item.SetActive(value: false);
			}
		}

		private void Update()
		{
			if (count >= _delayFrame)
			{
				foreach (GameObject item in gameObject_list)
				{
					item.SetActive(value: true);
				}
				return;
			}
			count += Time.deltaTime * 60f;
		}
	}
}
