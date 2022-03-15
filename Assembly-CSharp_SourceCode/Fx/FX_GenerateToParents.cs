using UnityEngine;

namespace FX
{
	public class FX_GenerateToParents : MonoBehaviour
	{
		[SerializeField]
		private GameObject GenerateObject;

		[SerializeField]
		private GameObject[] _parent;

		private void Start()
		{
			if (_parent.Length != 0)
			{
				for (int num = _parent.Length - 1; num >= 0; num--)
				{
					GameObject obj = Object.Instantiate(GenerateObject);
					obj.transform.position = _parent[num].transform.position;
					obj.transform.parent = _parent[num].transform;
				}
			}
		}
	}
}
