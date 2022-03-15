using UnityEngine;
using UnityEngine.UI;

namespace FX
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	public class FX_ScreenPosWave : MonoBehaviour
	{
		[SerializeField]
		private Shader _shader;

		[SerializeField]
		[Range(-10f, 10f)]
		private float WaveSpeed_ = -2.5f;

		[SerializeField]
		[Range(0f, 2f)]
		private float WaveFillAmount_ = 0.46f;

		[SerializeField]
		[Range(0f, 360f)]
		private float WaveDirection_;

		[SerializeField]
		[Range(-200f, 200f)]
		private float WaveCount_ = 80f;

		[SerializeField]
		[Range(0f, 10f)]
		private float WaveHeight_ = 0.6f;

		[SerializeField]
		[Range(0f, 1f)]
		private float WaveEdgeLerp_ = 0.2f;

		private Material material_;

		private int id_WaveSpeed;

		private int id_WaveFillAmount;

		private int id_WaveDirection;

		private int id_WaveCount;

		private int id_WaveHeight;

		private int id_WaveEdgeLerp;

		private const HideFlags HideAndDontSave = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;

		private void Start()
		{
			if (null == material_)
			{
				material_ = new Material(_shader);
				material_.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild;
			}
			reset();
		}

		private void OnDidApplyAnimationProperties()
		{
			reset();
		}

		private void OnDestroy()
		{
			if (null != material_)
			{
				Object.DestroyImmediate(material_);
				material_ = null;
			}
		}

		private void reset()
		{
			Image component = base.transform.GetComponent<Image>();
			if (!(component == null))
			{
				initPropertyId();
				component.material = material_;
				component.material.SetFloat(id_WaveSpeed, WaveSpeed_);
				component.material.SetFloat(id_WaveFillAmount, WaveFillAmount_);
				component.material.SetFloat(id_WaveDirection, WaveDirection_);
				component.material.SetFloat(id_WaveCount, WaveCount_);
				component.material.SetFloat(id_WaveHeight, WaveHeight_);
				component.material.SetFloat(id_WaveEdgeLerp, WaveEdgeLerp_);
			}
		}

		private void initPropertyId()
		{
			if (id_WaveSpeed == 0)
			{
				id_WaveSpeed = Shader.PropertyToID("_WaveSpeed");
				id_WaveFillAmount = Shader.PropertyToID("_WaveFillAmount");
				id_WaveDirection = Shader.PropertyToID("_WaveDirection");
				id_WaveCount = Shader.PropertyToID("_WaveCount");
				id_WaveHeight = Shader.PropertyToID("_WaveHeight");
				id_WaveEdgeLerp = Shader.PropertyToID("_WaveEdgeLerp");
			}
		}
	}
}
