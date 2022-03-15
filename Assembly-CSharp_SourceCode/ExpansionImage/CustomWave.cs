using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ExpansionImage
{
	public class CustomWave : Image
	{
		private const string ShaderName = "Custom/CustomWave";

		private readonly int _edgeTexID = Shader.PropertyToID("_EdgeTex");

		private readonly int _timerID = Shader.PropertyToID("_Timer");

		private readonly int _fillAmountID = Shader.PropertyToID("_FillAmount");

		private readonly int _addColorID = Shader.PropertyToID("_AddColor");

		private readonly int _frequencyArrayID = Shader.PropertyToID("_frequencyArray");

		private readonly int _amplitudeArrayID = Shader.PropertyToID("_amplitudeArray");

		private readonly int _arrayLengthID = Shader.PropertyToID("_arrayLength");

		private readonly int _lerpID = Shader.PropertyToID("_Lerp");

		private readonly int _edgeColorID = Shader.PropertyToID("_EdgeColor");

		private readonly int _edgeWidthID = Shader.PropertyToID("_EdgeWidth");

		private readonly int _edgePowerID = Shader.PropertyToID("_EdgePower");

		private readonly int _addOnID = Shader.PropertyToID("_AddOn");

		private const string EdgeColor = "_EDGE_COLOR";

		private const string EdgeTex1 = "_EDGE_TEX1";

		private const string EdgeTex2 = "_EDGE_TEX2";

		[SerializeField]
		private WaveEdgeDrawType _drawType = WaveEdgeDrawType.Color;

		[SerializeField]
		private MultiImageBlendType _blendType;

		[SerializeField]
		private Sprite _edgeSprite;

		[SerializeField]
		private Color _edgeColor = Color.clear;

		[SerializeField]
		private Color _addColor = Color.clear;

		[SerializeField]
		[Range(-1f, 2f)]
		private float _fillAmount = 0.5f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _normalizeTime;

		[SerializeField]
		private float _lerp = 0.01f;

		[SerializeField]
		private float _edgeWidth = 0.01f;

		[SerializeField]
		private float _edgePower = 0.01f;

		[SerializeField]
		private bool _isTiles;

		[SerializeField]
		private List<Wave> _waveList = new List<Wave>
		{
			new Wave(10f, 10f)
		};

		[SerializeField]
		private WaveDirection _direction;

		private readonly List<float> _frequencyList = new List<float>();

		private readonly List<float> _amplitudeList = new List<float>();

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetMaterial();
			UpdateParameter();
		}

		private void SetMaterial()
		{
			if (material == null || material.shader.name != "Custom/CustomWave")
			{
				Shader shader = Shader.Find("Custom/CustomWave");
				material = new Material(shader);
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			base.OnDidApplyAnimationProperties();
			UpdateParameter();
		}

		public void UpdateParameter()
		{
			if (!(material.shader.name == "Custom/CustomWave"))
			{
				return;
			}
			material.SetTexture(_edgeTexID, (_edgeSprite == null) ? null : _edgeSprite.texture);
			float value = Mathf.Lerp(0f, (float)Math.PI * 2f, _normalizeTime);
			material.SetFloat(_timerID, value);
			material.SetFloat(_fillAmountID, _fillAmount);
			material.SetFloat(_lerpID, _lerp);
			material.SetColor(_edgeColorID, _edgeColor);
			material.SetFloat(_edgeWidthID, _edgeWidth);
			material.SetFloat(_edgePowerID, _edgePower);
			material.SetColor(_addColorID, _addColor);
			material.SetInt(_arrayLengthID, _waveList.Count);
			if (_waveList.Count > 0)
			{
				_frequencyList.Clear();
				_amplitudeList.Clear();
				foreach (Wave wave in _waveList)
				{
					_frequencyList.Add(wave.Frequency);
					_amplitudeList.Add(wave.Amplitude / 100f);
				}
				material.SetFloatArray(_frequencyArrayID, new float[10]);
				material.SetFloatArray(_amplitudeArrayID, new float[10]);
				material.SetFloatArray(_frequencyArrayID, _frequencyList);
				material.SetFloatArray(_amplitudeArrayID, _amplitudeList);
			}
			if (_isTiles)
			{
				material.EnableKeyword("_TILE_POS");
				material.DisableKeyword("_TILE_UV");
			}
			else
			{
				material.EnableKeyword("_TILE_UV");
				material.DisableKeyword("_TILE_POS");
			}
			switch (_drawType)
			{
			case WaveEdgeDrawType.Color:
				material.EnableKeyword("_EDGE_COLOR");
				material.DisableKeyword("_EDGE_TEX1");
				material.DisableKeyword("_EDGE_TEX2");
				break;
			case WaveEdgeDrawType.Texture1:
				material.DisableKeyword("_EDGE_COLOR");
				material.EnableKeyword("_EDGE_TEX1");
				material.DisableKeyword("_EDGE_TEX2");
				break;
			case WaveEdgeDrawType.Texture2:
				material.DisableKeyword("_EDGE_COLOR");
				material.DisableKeyword("_EDGE_TEX1");
				material.EnableKeyword("_EDGE_TEX2");
				break;
			default:
				material.DisableKeyword("_EDGE_COLOR");
				material.DisableKeyword("_EDGE_TEX1");
				material.DisableKeyword("_EDGE_TEX2");
				break;
			}
			switch (_blendType)
			{
			case MultiImageBlendType.Normal:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 10);
				material.SetInt("_BlendOp", 0);
				material.SetFloat(_addOnID, 0f);
				break;
			case MultiImageBlendType.Add:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 1);
				material.SetInt("_BlendOp", 0);
				material.SetFloat(_addOnID, 1f);
				break;
			case MultiImageBlendType.Subtract:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 1);
				material.SetInt("_BlendOp", 2);
				material.SetFloat(_addOnID, 1f);
				break;
			}
			switch (_direction)
			{
			case WaveDirection.Up:
				material.EnableKeyword("_WAVE_UP");
				material.DisableKeyword("_WAVE_BOTTOM");
				material.DisableKeyword("_WAVE_LEFT");
				material.DisableKeyword("_WAVE_RIGHT");
				break;
			case WaveDirection.Bottom:
				material.EnableKeyword("_WAVE_BOTTOM");
				material.DisableKeyword("_WAVE_UP");
				material.DisableKeyword("_WAVE_LEFT");
				material.DisableKeyword("_WAVE_RIGHT");
				break;
			case WaveDirection.Left:
				material.EnableKeyword("_WAVE_LEFT");
				material.DisableKeyword("_WAVE_BOTTOM");
				material.DisableKeyword("_WAVE_UP");
				material.DisableKeyword("_WAVE_RIGHT");
				break;
			case WaveDirection.Right:
				material.EnableKeyword("_WAVE_RIGHT");
				material.DisableKeyword("_WAVE_BOTTOM");
				material.DisableKeyword("_WAVE_LEFT");
				material.DisableKeyword("_WAVE_UP");
				break;
			}
		}
	}
}
