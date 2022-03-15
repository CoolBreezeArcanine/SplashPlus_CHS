using UnityEngine;

namespace Timeline
{
	public class CounterObject : TimeControlBaseObject
	{
		[SerializeField]
		private SpriteCounter[] _targets;

		[SerializeField]
		private SpriteCounter[] _shadowCounters;

		[SerializeField]
		private Color _activeColor = Color.white;

		[SerializeField]
		private Color _shadowColor = Color.white;

		[SerializeField]
		private uint _fromValue;

		[SerializeField]
		private uint _toValue;

		[SerializeField]
		private bool _isComma = true;

		[SerializeField]
		private bool _isPrefixSign;

		private bool _isFixed;

		private void OnValidate()
		{
		}

		public override void OnBehaviourPlay()
		{
			_isFixed = false;
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

		public void Skip()
		{
			UpdateScore(1f);
			_isFixed = true;
		}

		public override void PrepareFrame(double normalizeTime)
		{
			UpdateScore((float)normalizeTime);
		}

		public void SetCountData(uint fromt, uint to)
		{
			_fromValue = fromt;
			_toValue = to;
		}

		private void UpdateScore(float normalize)
		{
			if (_isFixed)
			{
				return;
			}
			string text = ((uint)Mathf.Lerp(_fromValue, _toValue, normalize)).ToString(_isComma ? "N0" : "0");
			SpriteCounter[] targets = _targets;
			for (int i = 0; i < targets.Length; i++)
			{
				targets[i]?.SetColor(Color.clear);
			}
			if (_shadowCounters != null)
			{
				targets = _shadowCounters;
				for (int i = 0; i < targets.Length; i++)
				{
					targets[i]?.SetColor(Color.clear);
				}
			}
			int num = -1;
			switch (text.Length)
			{
			case 1:
				num = 0;
				break;
			case 2:
				num = 1;
				break;
			case 3:
				num = 2;
				break;
			case 4:
				num = 3;
				break;
			case 5:
				num = 4;
				break;
			}
			if (_isPrefixSign)
			{
				text = "+" + text;
			}
			if (num >= 0 && num < _targets.Length)
			{
				_targets[num]?.ChangeText(text);
				_targets[num]?.SetColor(_activeColor);
				if (_shadowCounters != null && num < _shadowCounters.Length)
				{
					_shadowCounters[num]?.ChangeText(text);
					_shadowCounters[num]?.SetColor(_shadowColor);
				}
			}
		}
	}
}
