using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetParam_ByScale : MonoBehaviour
	{
		[SerializeField]
		private string valueName = "_Opacity";

		private Material _material;

		private Material[] _materials;

		private int _propertyID_ValueName;

		private void Start()
		{
			Renderer component = GetComponent<Renderer>();
			_materials = ((component != null) ? component.materials : null);
			_propertyID_ValueName = Shader.PropertyToID(valueName);
		}

		private void Update()
		{
			if (_materials.Length != 0)
			{
				for (int num = _materials.Length - 1; num >= 0; num--)
				{
					_materials[num].SetFloat(_propertyID_ValueName, base.transform.lossyScale.x);
				}
			}
		}
	}
}
