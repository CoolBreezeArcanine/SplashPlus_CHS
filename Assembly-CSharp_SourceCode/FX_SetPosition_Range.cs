using UnityEngine;

public class FX_SetPosition_Range : MonoBehaviour
{
	private float randomRot = Random.Range(0f, 1f);

	private float randomDst = Random.Range(0f, 1f);

	[SerializeField]
	private float middleRadius;

	[SerializeField]
	[Range(0f, 1f)]
	private float middleRadiusThickness = 1f;

	private void Start()
	{
		randomRot = Random.Range(0f, 1f);
		randomDst = Random.Range(0f, 1f);
		base.transform.position = Quaternion.Euler(0f, 0f, 360f * randomRot) * (Vector3.up * middleRadius - Vector3.up * middleRadius * randomDst * middleRadiusThickness);
	}

	private void Update()
	{
	}
}
