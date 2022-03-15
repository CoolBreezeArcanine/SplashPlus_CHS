using UnityEngine;

namespace FX
{
	public class FX_CopyAndShareMat_SetParam2 : MonoBehaviour
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
		private Renderer[] _renderers;

		private Material _sharedMaterial;

		private int _propertyID_ValueName1;

		private int _propertyID_ValueName2;

		private void Start()
		{
			Renderer[] renderers = _renderers;
			foreach (Renderer renderer in renderers)
			{
				if (renderer != null && renderer.material != null)
				{
					if (_sharedMaterial == null)
					{
						_sharedMaterial = new Material(renderer.material);
					}
					renderer.material = _sharedMaterial;
				}
			}
			_propertyID_ValueName1 = Shader.PropertyToID(valueName1);
			_propertyID_ValueName2 = Shader.PropertyToID(valueName2);
		}

		private void Update()
		{
			if (_sharedMaterial != null)
			{
				_sharedMaterial.SetFloat(_propertyID_ValueName1, value1);
				_sharedMaterial.SetFloat(_propertyID_ValueName2, value2);
			}
		}
	}
}
