using System;
using UnityEngine;

namespace FX
{
	public class FX_GenerateFan : MonoBehaviour
	{
		public GameObject GenerateObject;

		public int ObjNumber = 6;

		public int ObjRadius = 1;

		public bool SaveRotate = true;

		public Vector3 RotateFanEnd = new Vector3(0f, 0f, 0f);

		private void Start()
		{
			for (int num = ObjNumber; num >= 1; num--)
			{
				Vector3 vector = new Vector3(base.gameObject.transform.eulerAngles.x + RotateFanEnd[0] * (float)(num - 1) / (float)(ObjNumber - 1), base.gameObject.transform.eulerAngles.y + RotateFanEnd[1] * (float)(num - 1) / (float)(ObjNumber - 1), base.gameObject.transform.eulerAngles.z + RotateFanEnd[2] * (float)(num - 1) / (float)(ObjNumber - 1));
				GameObject obj = UnityEngine.Object.Instantiate(GenerateObject);
				obj.transform.position = new Vector3(base.gameObject.transform.position.x + Mathf.Sin(vector[1] * ((float)Math.PI / 180f)) * (float)ObjRadius, base.gameObject.transform.position.y + 0f, base.gameObject.transform.position.z + Mathf.Cos(vector[1] * ((float)Math.PI / 180f)) * (float)ObjRadius);
				obj.transform.eulerAngles = new Vector3(vector[0], vector[1], vector[2]);
				obj.transform.parent = base.transform;
			}
		}
	}
}
