using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetParam3 : MonoBehaviour
	{
		[SerializeField]
		private string valueName1 = "_Exposure";

		[SerializeField]
		private float value1;

		[SerializeField]
		private string valueName2 = "_Opacity";

		[SerializeField]
		private float value2;

		[SerializeField]
		private string valueName3 = "";

		[SerializeField]
		private float value3;

		private Material _material;

		private int _propertyID_ValueName1;

		private int _propertyID_ValueName2;

		private int _propertyID_ValueName3;

		private void Start()
		{
			Renderer component = GetComponent<Renderer>();
			_material = ((component != null) ? component.material : null);
			_propertyID_ValueName1 = Shader.PropertyToID(valueName1);
			_propertyID_ValueName2 = Shader.PropertyToID(valueName2);
			_propertyID_ValueName3 = Shader.PropertyToID(valueName3);
		}

		private void Update()
		{
			if (_material != null)
			{
				_material.SetFloat(_propertyID_ValueName1, value1);
				_material.SetFloat(_propertyID_ValueName2, value2);
				_material.SetFloat(_propertyID_ValueName3, value3);
			}
		}
	}
}
