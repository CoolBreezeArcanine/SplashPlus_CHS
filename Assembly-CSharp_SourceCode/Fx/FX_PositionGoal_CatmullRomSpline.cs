using UnityEngine;

namespace FX
{
	public class FX_PositionGoal_CatmullRomSpline : MonoBehaviour
	{
		public Transform[] controlPointsList;

		public bool isLooping = true;

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.white;
			for (int i = 0; i < controlPointsList.Length; i++)
			{
				if ((i != 0 && i != controlPointsList.Length - 2 && i != controlPointsList.Length - 1) || isLooping)
				{
					DisplayCatmullRomSpline(i);
				}
			}
		}

		private void DisplayCatmullRomSpline(int pos)
		{
			Vector3 position = controlPointsList[ClampListPos(pos - 1)].position;
			Vector3 position2 = controlPointsList[pos].position;
			Vector3 position3 = controlPointsList[ClampListPos(pos + 1)].position;
			Vector3 position4 = controlPointsList[ClampListPos(pos + 2)].position;
			Vector3 from = position2;
			float num = 0.2f;
			int num2 = Mathf.FloorToInt(1f / num);
			for (int i = 1; i <= num2; i++)
			{
				float t = (float)i * num;
				Vector3 catmullRomPosition = GetCatmullRomPosition(t, position, position2, position3, position4);
				Gizmos.DrawLine(from, catmullRomPosition);
				from = catmullRomPosition;
			}
		}

		private int ClampListPos(int pos)
		{
			if (pos < 0)
			{
				pos = controlPointsList.Length - 1;
			}
			if (pos > controlPointsList.Length)
			{
				pos = 1;
			}
			else if (pos > controlPointsList.Length - 1)
			{
				pos = 0;
			}
			return pos;
		}

		private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 vector = 2f * p1;
			Vector3 vector2 = p2 - p0;
			Vector3 vector3 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
			Vector3 vector4 = -p0 + 3f * p1 - 3f * p2 + p3;
			return 0.5f * (vector + vector2 * t + vector3 * t * t + vector4 * t * t * t);
		}
	}
}
