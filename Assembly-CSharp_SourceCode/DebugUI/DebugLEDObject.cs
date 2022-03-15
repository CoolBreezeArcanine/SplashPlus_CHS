using UnityEngine;
using UnityEngine.UI;

namespace DebugUI
{
	public class DebugLEDObject : MonoBehaviour
	{
		[SerializeField]
		private Image[] _ledImages;

		private float[] _durations;

		private float[] _timers;

		private bool[] _fades;

		private Color32[] _startColors;

		private Color32[] _endColors;

		private void Start()
		{
			_timers = new float[_ledImages.Length];
			_durations = new float[_ledImages.Length];
			_fades = new bool[_ledImages.Length];
			_startColors = new Color32[_ledImages.Length];
			_endColors = new Color32[_ledImages.Length];
			for (int i = 0; i < _ledImages.Length; i++)
			{
				_timers[i] = 0f;
				_fades[i] = false;
				_ledImages[i].color = Color.gray;
			}
		}

		private void Update()
		{
			for (int i = 0; i < 8; i++)
			{
				if (_fades[i])
				{
					_timers[i] += Time.deltaTime;
					_ledImages[i].color = Color32.Lerp(_startColors[i], _endColors[i], _timers[i] / _durations[i]);
					if (_timers[i] >= _durations[i])
					{
						_fades[i] = false;
						_durations[i] = 0f;
						_timers[i] = 0f;
					}
				}
			}
		}

		public void SetColorButton(byte ledPos, Color32 color)
		{
			_ledImages[ledPos].color = color;
		}

		public void SetColorMulti(Color32 color)
		{
			Image[] ledImages = _ledImages;
			for (int i = 0; i < ledImages.Length; i++)
			{
				ledImages[i].color = color;
			}
		}

		public void SetColorMultiFade(Color32 color, float speed)
		{
			for (int i = 0; i < _ledImages.Length; i++)
			{
				_durations[i] = speed / 1000f;
				_timers[i] = 0f;
				_startColors[i] = _ledImages[i].color;
				_endColors[i] = color;
				_fades[i] = true;
			}
		}
	}
}
