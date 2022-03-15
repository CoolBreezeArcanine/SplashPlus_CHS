using UnityEngine;

namespace FX
{
	public class FX_GenerateLine : MonoBehaviour
	{
		public GameObject GenerateObject;

		public int ObjNumber = 1;

		public Vector3 CoordinateDistance = new Vector3(0f, 0f, 0f);

		private void Start()
		{
			for (int num = ObjNumber - 1; num >= 0; num--)
			{
				GameObject obj = Object.Instantiate(GenerateObject);
				obj.transform.position = new Vector3(base.gameObject.transform.position.x + CoordinateDistance[0] * (float)num, base.gameObject.transform.position.y + CoordinateDistance[1] * (float)num, base.gameObject.transform.position.z + CoordinateDistance[2] * (float)num);
				obj.transform.parent = base.transform;
			}
		}
	}
}
