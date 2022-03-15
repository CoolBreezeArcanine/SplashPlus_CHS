using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetParam : MonoBehaviour
	{
		[SerializeField]
		private string valueName = "_Opacity";

		[SerializeField]
		private float value;

		private Material _material;

		private int _propertyID_ValueName;

		private void Start()
		{
			Renderer component = GetComponent<Renderer>();
			_material = ((component != null) ? component.material : null);
			_propertyID_ValueName = Shader.PropertyToID(valueName);
		}

		private void Update()
		{
			if (_material != null)
			{
				_material.SetFloat(_propertyID_ValueName, value);
			}
		}
	}
}
