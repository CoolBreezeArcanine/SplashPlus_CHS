using UnityEngine;

namespace FX
{
	public class FX_RotationAdd : MonoBehaviour
	{
		[SerializeField]
		private Vector3 rotStart = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 rotStartRandom = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 rotAdd = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 rotAddRandom = new Vector3(0f, 0f, 0f);

		private Vector3 rotStartRandom_Rate;

		private Vector3 rotAddRandom_Rate;

		[SerializeField]
		private bool _individualXYZ;

		private float _timeCount;

		private void Start()
		{
			rotStartRandom_Rate = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			base.transform.Rotate(rotStart[0] + rotStartRandom[0] * rotStartRandom_Rate[0], rotStart[1] + rotStartRandom[1] * rotStartRandom_Rate[1], rotStart[2] + rotStartRandom[2] * rotStartRandom_Rate[2]);
			rotAddRandom_Rate = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}

		private void Update()
		{
			float num = Time.deltaTime * 60f;
			if (!_individualXYZ)
			{
				base.transform.Rotate(rotAdd[0] * num + rotAddRandom[0] * rotAddRandom_Rate[0] * num, rotAdd[1] * num + rotAddRandom[1] * rotAddRandom_Rate[1] * num, rotAdd[2] * num + rotAddRandom[2] * rotAddRandom_Rate[2] * num);
				return;
			}
			float x = rotStart[0] + rotStartRandom[0] * rotStartRandom_Rate[0] + (rotAdd[0] * _timeCount + rotAddRandom[0] * rotAddRandom_Rate[0] * _timeCount);
			float y = rotStart[1] + rotStartRandom[1] * rotStartRandom_Rate[1] + (rotAdd[1] * _timeCount + rotAddRandom[1] * rotAddRandom_Rate[1] * _timeCount);
			float z = rotStart[2] + rotStartRandom[2] * rotStartRandom_Rate[2] + (rotAdd[2] * _timeCount + rotAddRandom[2] * rotAddRandom_Rate[2] * _timeCount);
			base.transform.localEulerAngles = new Vector3(x, y, z);
			_timeCount += num;
		}
	}
}
