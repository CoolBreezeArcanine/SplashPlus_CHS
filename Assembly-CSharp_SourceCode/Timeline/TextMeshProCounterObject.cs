using TMPro;
using UnityEngine;

namespace Timeline
{
	public class TextMeshProCounterObject : TimeControlBaseObject
	{
		[SerializeField]
		private TextMeshProUGUI _counterText;

		[SerializeField]
		private bool _isShowSign;

		[SerializeField]
		private bool _isShowComma;

		[SerializeField]
		private int _fromValue;

		[SerializeField]
		private int _toValue;

		private bool _isZeroPlus;

		private bool _isSkip;

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

		public void Skip()
		{
			UpdateScore(1f);
			_isSkip = true;
		}

		public void SetCountData(int fromt, int to, bool isZeroPlus = false)
		{
			_fromValue = fromt;
			_toValue = to;
			_isZeroPlus = isZeroPlus;
		}

		public void SetColor(Color color)
		{
			_counterText.color = color;
		}

		private void UpdateScore(float normalize)
		{
			if (!_isSkip && _counterText != null)
			{
				float num = Mathf.Lerp(_fromValue, _toValue, normalize);
				string arg = ((!_isZeroPlus) ? ((!_isShowSign) ? "" : ((num > 0f) ? "+" : "")) : ((!_isShowSign) ? "" : ((num >= 0f) ? "+" : "")));
				_counterText.text = (_isShowComma ? $"{arg}{num:#,0}" : $"{arg}{num:0}");
			}
		}
	}
}
