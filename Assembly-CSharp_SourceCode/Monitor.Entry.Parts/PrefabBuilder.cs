using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class PrefabBuilder : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] _prefabs;

		public GameObject[] Entities { get; private set; }

		private void Awake()
		{
			Entities = new GameObject[_prefabs.Length];
			for (int i = 0; i < _prefabs.Length; i++)
			{
				Entities[i] = Object.Instantiate(_prefabs[i], base.transform, worldPositionStays: false);
			}
		}
	}
}
