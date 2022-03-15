using UnityEngine;
using UnityEngine.UI;

namespace DebugUI
{
	public class DebugLEDCheck : MonoBehaviour
	{
		[SerializeField]
		private Image _aimeReaderLED;

		[SerializeField]
		private DebugLEDObject[] _ledObjects;

		private float _ledOffTimer;

		private void Awake()
		{
			Object.Destroy(base.gameObject);
		}

		private void Update()
		{
			if (!(0f < _ledOffTimer))
			{
				return;
			}
			_ledOffTimer -= Time.deltaTime;
			if (_ledOffTimer <= 0f)
			{
				if (_aimeReaderLED != null)
				{
					_aimeReaderLED.color = Color.black;
				}
				_ledOffTimer = 0f;
			}
		}

		public void SetAimeReaderLed(Color color, float offTimer)
		{
			_ledOffTimer = offTimer;
			if (_aimeReaderLED != null)
			{
				_aimeReaderLED.color = color;
			}
		}

		public void SetColorButton(int index, byte ledPos, Color32 color)
		{
			DebugLEDObject[] ledObjects = _ledObjects;
			if (ledObjects != null)
			{
				ledObjects[index].SetColorButton(ledPos, color);
			}
		}

		public void SetColorMulti(int index, Color32 color)
		{
			DebugLEDObject[] ledObjects = _ledObjects;
			if (ledObjects != null)
			{
				ledObjects[index].SetColorMulti(color);
			}
		}

		public void SetColorMultiFade(int index, Color32 color, float speed)
		{
			DebugLEDObject[] ledObjects = _ledObjects;
			if (ledObjects != null)
			{
				ledObjects[index].SetColorMultiFade(color, speed);
			}
		}
	}
}
