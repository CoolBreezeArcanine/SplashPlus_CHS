using UnityEngine;

namespace FX
{
	public class FX_we_001012 : MonoBehaviour
	{
		public GameObject _fX_we_001012;

		public GameObject _fX_we_001012_1;

		public Transform _sw_1_weapon_b;

		public Transform _sw_2_weapon_b;

		public Transform _sw_3_weapon_b;

		public Transform _sw_4_weapon_b;

		public Transform _sw_5_weapon_b;

		private float count;

		private Animator animator_1;

		private Animator animator_2;

		private Animator animator_3;

		private Animator animator_4;

		private Animator animator_5;

		private void Start()
		{
			GameObject gameObject = Object.Instantiate(_fX_we_001012_1);
			gameObject.transform.position = _sw_1_weapon_b.position;
			animator_1 = gameObject.GetComponent<Animator>();
			gameObject.transform.parent = _sw_1_weapon_b;
			GameObject gameObject2 = Object.Instantiate(_fX_we_001012);
			gameObject2.transform.position = _sw_2_weapon_b.position;
			animator_2 = gameObject2.GetComponent<Animator>();
			gameObject2.transform.parent = _sw_2_weapon_b;
			GameObject gameObject3 = Object.Instantiate(_fX_we_001012);
			gameObject3.transform.position = _sw_3_weapon_b.position;
			animator_3 = gameObject3.GetComponent<Animator>();
			gameObject3.transform.parent = _sw_3_weapon_b;
			GameObject gameObject4 = Object.Instantiate(_fX_we_001012);
			gameObject4.transform.position = _sw_4_weapon_b.position;
			animator_4 = gameObject4.GetComponent<Animator>();
			gameObject4.transform.parent = _sw_4_weapon_b;
			GameObject gameObject5 = Object.Instantiate(_fX_we_001012);
			gameObject5.transform.position = _sw_5_weapon_b.position;
			animator_5 = gameObject5.GetComponent<Animator>();
			gameObject5.transform.parent = _sw_5_weapon_b;
		}

		private void Update()
		{
			if (count >= 120f)
			{
				animator_1.SetInteger("Form_Number", (int)Random.Range(0f, 2.99f));
				animator_2.SetInteger("Form_Number", (int)Random.Range(0f, 2.99f));
				animator_3.SetInteger("Form_Number", (int)Random.Range(0f, 2.99f));
				animator_4.SetInteger("Form_Number", (int)Random.Range(0f, 2.99f));
				animator_5.SetInteger("Form_Number", (int)Random.Range(0f, 2.99f));
				count = 0f;
			}
			count += Time.deltaTime * 60f;
		}
	}
}
