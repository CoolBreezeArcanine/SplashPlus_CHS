using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetParamCurve : MonoBehaviour
	{
		[SerializeField]
		private string _valueName = "_Opacity";

		[SerializeField]
		private float _keyValueMagnification = 1f;

		[SerializeField]
		private float _keyTimeMagnification = 1f;

		private float duration;

		public AnimationCurve CurveX = new AnimationCurve(new Keyframe(0f, 0f, 3f, 3f), new Keyframe(1f, 1f));

		private Material _material;

		private int _propertyID_ValueName;

		private void Start()
		{
			Renderer component = GetComponent<Renderer>();
			_material = ((component != null) ? component.material : null);
			_propertyID_ValueName = Shader.PropertyToID(_valueName);
		}

		private void Update()
		{
			if (_material != null)
			{
				float value = _keyValueMagnification * CurveX.Evaluate(duration);
				_material.SetFloat(_propertyID_ValueName, value);
			}
			duration += Time.deltaTime / _keyTimeMagnification;
		}
	}
}
