using Fx;
using UnityEngine;

namespace FX
{
	public class FX_Dbg_PreviewLoop_Mai2Tap : MonoBehaviour
	{
		[SerializeField]
		[Range(0f, 5f)]
		private int _grade;

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

		private int alreadyLaunchedCount;

		private int loopCount = 500;

		private void Update()
		{
			if ((float)loopCount <= interval * 60f)
			{
				loopCount++;
				if (alreadyLaunchedCount < objRapidNumber - 1 && (float)loopCount >= objRapidInterval * (float)(alreadyLaunchedCount + 1))
				{
					for (int num = objNumber - 1; num >= 0; num--)
					{
						GenerateObj(base.transform, GenerateObject, _grade);
					}
					alreadyLaunchedCount++;
				}
			}
			else
			{
				loopCount = 0;
				foreach (Transform item in base.gameObject.transform)
				{
					Object.Destroy(item.gameObject);
				}
				for (int num2 = objNumber - 1; num2 >= 0; num2--)
				{
					GenerateObj(base.transform, GenerateObject, _grade);
				}
				alreadyLaunchedCount = 0;
			}
			if (loopCount == 1)
			{
				GameObject gameObject = base.transform.Find(GenerateObject.name.ToString() + "(Clone)/Root").gameObject;
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(value: true);
				}
			}
		}

		private static void GenerateObj(Transform parent, GameObject generateObj, int grade)
		{
			GameObject gameObject = Object.Instantiate(generateObj, parent);
			FX_Mai2_Note_Color.FxColor colorId = FX_Mai2_Note_Color.FxColor.NoEffect;
			switch (grade)
			{
			case 0:
				colorId = FX_Mai2_Note_Color.FxColor.NoEffect;
				break;
			case 1:
				colorId = FX_Mai2_Note_Color.FxColor.Good;
				break;
			case 2:
				colorId = FX_Mai2_Note_Color.FxColor.Great;
				break;
			case 3:
				colorId = FX_Mai2_Note_Color.FxColor.Perfect;
				break;
			case 4:
				colorId = FX_Mai2_Note_Color.FxColor.White;
				break;
			case 5:
				colorId = FX_Mai2_Note_Color.FxColor.Max;
				break;
			}
			gameObject.GetComponent<FX_Mai2_Note_Color>().ChangeColor(colorId);
		}
	}
}
