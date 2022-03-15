using UnityEngine;
using UnityEngine.UI;

namespace Timeline
{
	public class GuageObject : TimeControlBaseObject
	{
		[SerializeField]
		private Image _guageImage;

		[SerializeField]
		[Range(0f, 1f)]
		private float _fromValue;

		[SerializeField]
		[Range(0f, 1f)]
		private float _toValue;

		private void OnValidate()
		{
		}

		public override void OnBehaviourPlay()
		{
			UpdateScore(0f);
		}

		public override void OnClipPlay()
		{
			UpdateScore(0f);
		}

		public override void OnClipTailEnd()
		{
			UpdateScore(1f);
		}

		public override void OnClipHeadEnd()
		{
			UpdateScore(0f);
		}

		public override void OnGraphStop()
		{
			UpdateScore(0f);
		}

		public override void PrepareFrame(double normalizeTime)
		{
			UpdateScore((float)normalizeTime);
		}

		public void SetCountData(float fromt, float to)
		{
			_fromValue = fromt;
			_toValue = to;
		}

		private void UpdateScore(float normalize)
		{
			if (_guageImage != null)
			{
				_guageImage.fillAmount = Mathf.Lerp(_fromValue, _toValue, normalize);
			}
		}
	}
}
