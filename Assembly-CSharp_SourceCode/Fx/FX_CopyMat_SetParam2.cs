using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetParam2 : MonoBehaviour
	{
		[SerializeField]
		private string valueName1 = "_Exposure";

		[SerializeField]
		private float value1;

		[SerializeField]
		private string valueName2 = "_Opacity";

		[SerializeField]
		private float value2;

		private Material _material;

		private int _propertyID_ValueName1;

		private int _propertyID_ValueName2;

		private void Start()
		{
			Renderer component = GetComponent<Renderer>();
			_material = ((component != null) ? component.material : null);
			_propertyID_ValueName1 = Shader.PropertyToID(valueName1);
			_propertyID_ValueName2 = Shader.PropertyToID(valueName2);
		}

		private void Update()
		{
			if (_material != null)
			{
				_material.SetFloat(_propertyID_ValueName1, value1);
				_material.SetFloat(_propertyID_ValueName2, value2);
			}
		}
	}
}
