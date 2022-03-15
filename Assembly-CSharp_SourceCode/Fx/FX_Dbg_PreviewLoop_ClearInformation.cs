using UnityEngine;

namespace FX
{
	public class FX_Dbg_PreviewLoop_ClearInformation : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] _disableObjects;

		[SerializeField]
		private GameObject[] _generateObjects;

		[SerializeField]
		private float interval = 3f;

		private float loopCount = 500f;

		private void Start()
		{
			for (int i = 0; i < _disableObjects.Length; i++)
			{
				_disableObjects[i].SetActive(value: false);
			}
		}

		private void Update()
		{
			if (loopCount <= interval)
			{
				loopCount += Time.deltaTime;
				return;
			}
			loopCount = 0f;
			foreach (Transform item in base.gameObject.transform)
			{
				Object.Destroy(item.gameObject);
			}
			for (int i = 0; i < _generateObjects.Length; i++)
			{
				GameObject obj = Object.Instantiate(_generateObjects[i]);
				obj.transform.position += base.transform.position;
				obj.transform.parent = base.transform;
			}
		}
	}
}
