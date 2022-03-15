using UnityEngine;

namespace Timeline
{
	public class TranslateControleObject : TimeControlBaseObject
	{
		[SerializeField]
		private AnimationCurve _normalizeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private Vector3 _from;

		[SerializeField]
		private Vector3 _to;

		private RectTransform _rect;

		private void Reset()
		{
			_rect = GetComponent<RectTransform>();
		}

		public override void OnBehaviourPlay()
		{
			UpdatePosition(0f);
		}

		public override void OnClipPlay()
		{
			UpdatePosition(0f);
		}

		public override void OnClipTailEnd()
		{
			if (Application.isPlaying)
			{
				UpdatePosition(1f);
			}
		}

		public override void OnClipHeadEnd()
		{
			if (!Application.isPlaying)
			{
				UpdatePosition(0f);
			}
		}

		public override void OnGraphStop()
		{
			UpdatePosition(Application.isPlaying ? 1f : 0f);
		}

		public override void PrepareFrame(double normalizeTime)
		{
			UpdatePosition((float)normalizeTime);
		}

		public void SetPosition(Vector3 from, Vector3 to)
		{
			_from = from;
			_to = to;
		}

		private void UpdatePosition(float normalize)
		{
			float t = _normalizeCurve.Evaluate(normalize);
			Vector3 vector = Vector3.Lerp(_from, _to, t);
			if (_rect == null)
			{
				_rect = GetComponent<RectTransform>();
			}
			_rect.anchoredPosition = vector;
		}
	}
}
