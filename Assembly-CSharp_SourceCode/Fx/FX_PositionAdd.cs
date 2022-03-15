using UnityEngine;

namespace FX
{
	public class FX_PositionAdd : MonoBehaviour
	{
		[SerializeField]
		private Vector3 posStart = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 posStartRandom = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 posAdd = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private Vector3 posAddRandom = new Vector3(0f, 0f, 0f);

		private Vector3 posStartRandom_Rate;

		private Vector3 posAddRandom_Rate;

		private Transform transform_;

		private Vector3 localPosition_;

		private void Start()
		{
			localPosition_ = transform_.localPosition;
		}

		private void OnEnable()
		{
			transform_ = base.transform;
			transform_.localPosition = localPosition_;
			posStartRandom_Rate = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			transform_.Translate(posStart[0] + posStartRandom[0] * posStartRandom_Rate[0], posStart[1] + posStartRandom[1] * posStartRandom_Rate[1], posStart[2] + posStartRandom[2] * posStartRandom_Rate[2]);
			posAddRandom_Rate = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		}

		private void Update()
		{
			transform_.Translate(posAdd[0] + posAddRandom[0] * posAddRandom_Rate[0] * 60f * Time.deltaTime, posAdd[1] + posAddRandom[1] * posAddRandom_Rate[1] * 60f * Time.deltaTime, posAdd[2] + posAddRandom[2] * posAddRandom_Rate[2] * 60f * Time.deltaTime);
		}
	}
}
