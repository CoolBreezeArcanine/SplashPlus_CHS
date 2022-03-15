using UnityEngine;

namespace Monitor.MapCore.Component
{
	public class LerpVector3 : LerpComponent<Vector3>
	{
		protected override Vector3 Lerp(Vector3 from, Vector3 to, float weight)
		{
			return Vector3.Lerp(from, to, weight);
		}
	}
}
