using UnityEngine;

namespace FX
{
	public class FX_Dbg_PreviewLoop : MonoBehaviour
	{
		[SerializeField]
		private GameObject GenerateObject;

		[SerializeField]
		private float interval = 2f;

		[SerializeField]
		private int objNumber = 1;

		[SerializeField]
		private int objRapidNumber = 1;

		[SerializeField]
		private float objRapidInterval = 0.1f;

		private int alreadyCount;

		private float loopCount = 500f;

		[SerializeField]
		private float childSpeed;

		[SerializeField]
		private float childRotSpeed;

		[SerializeField]
		private bool _thisEffectIsBullet;

		[SerializeField]
		private GameObject MotionChangeObject;

		private void Update()
		{
			if (loopCount <= interval)
			{
				loopCount += Time.deltaTime;
				if (alreadyCount < objRapidNumber - 1 && loopCount >= objRapidInterval * (float)(alreadyCount + 1))
				{
					for (int num = objNumber - 1; num >= 0; num--)
					{
						GenerateObj(base.transform, GenerateObject, _thisEffectIsBullet);
					}
					alreadyCount++;
				}
				{
					foreach (Transform item in base.gameObject.transform)
					{
						item.transform.Translate(0f, 0f, childSpeed * 60f * Time.deltaTime);
						item.transform.Rotate(0f, childRotSpeed * 60f * Time.deltaTime, 0f);
					}
					return;
				}
			}
			loopCount = 0f;
			foreach (Transform item2 in base.gameObject.transform)
			{
				Object.Destroy(item2.gameObject);
			}
			for (int num2 = objNumber - 1; num2 >= 0; num2--)
			{
				GenerateObj(base.transform, GenerateObject, _thisEffectIsBullet);
			}
			alreadyCount = 0;
		}

		private static void GenerateObj(Transform parent, GameObject generateObj, bool thisEffectIsBullet)
		{
			GameObject gameObject = Object.Instantiate(generateObj, parent);
			if (thisEffectIsBullet)
			{
				gameObject.AddComponent<FX_Dbg_BulletMove>();
			}
		}
	}
}
