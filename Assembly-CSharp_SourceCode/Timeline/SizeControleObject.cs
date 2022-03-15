using UnityEngine;

namespace Timeline
{
	public class SizeControleObject : TimeControlBaseObject
	{
		[SerializeField]
		private AnimationCurve _normalizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private Vector2 _from;

		[SerializeField]
		private Vector2 _to;

		private RectTransform _rect;

		public override void OnBehaviourPlay()
		{
			UpdateSize(0f);
		}

		public override void OnClipPlay()
		{
			UpdateSize(0f);
		}

		public override void OnClipTailEnd()
		{
			if (Application.isPlaying)
			{
				UpdateSize(1f);
			}
		}

		public override void OnClipHeadEnd()
		{
			UpdateSize(0f);
		}

		public override void OnGraphStop()
		{
			UpdateSize(Application.isPlaying ? 1f : 0f);
		}

		public override void PrepareFrame(double normalizeTime)
		{
			UpdateSize((float)normalizeTime);
		}

		public void SetSize(Vector2 from, Vector2 to)
		{
			_from = from;
			_to = to;
		}

		private void UpdateSize(float normalize)
		{
			float t = _normalizeCurve.Evaluate(normalize);
			Vector2 sizeDelta = Vector2.Lerp(_from, _to, t);
			if (_rect == null)
			{
				_rect = GetComponent<RectTransform>();
			}
			_rect.sizeDelta = sizeDelta;
		}
	}
}
