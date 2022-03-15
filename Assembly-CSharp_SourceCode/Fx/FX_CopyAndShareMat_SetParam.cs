using UnityEngine;

namespace FX
{
	public class FX_CopyAndShareMat_SetParam : MonoBehaviour
	{
		[SerializeField]
		private string valueName = "_Opacity";

		[SerializeField]
		private float value;

		[SerializeField]
		private Renderer[] _renderers;

		private Material _sharedMaterial;

		private int _propertyID_ValueName;

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
			_propertyID_ValueName = Shader.PropertyToID(valueName);
		}

		private void Update()
		{
			if (_sharedMaterial != null)
			{
				_sharedMaterial.SetFloat(_propertyID_ValueName, value);
			}
		}
	}
}
