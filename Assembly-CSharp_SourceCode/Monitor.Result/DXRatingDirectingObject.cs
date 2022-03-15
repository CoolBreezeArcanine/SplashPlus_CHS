using Timeline;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Result
{
	public class DXRatingDirectingObject : TimeControlBaseObject
	{
		[SerializeField]
		private Image _plateImage01;

		[SerializeField]
		private DXRatingCounterDisplay _counter01;

		[SerializeField]
		private Image _plateImage02;

		[SerializeField]
		private SpriteCounter _counter02;

		private CanvasGroup _canvasGroup;

		private Animator _directingAnimator;

		private double _fadeInDuration;

		private int _targetRate;

		public void Initialize()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_directingAnimator = GetComponent<Animator>();
			_counter01.Initialize(0);
		}

		public void SetData(int prevRate, int nextRate, Sprite plate, Sprite nextPlate)
		{
			_counter01.Initialize(prevRate);
			_counter02.ChangeText(string.Format("{0,5}", nextRate));
			_targetRate = nextRate;
			_plateImage01.sprite = plate;
			_plateImage02.sprite = nextPlate;
		}

		public void Play()
		{
			_counter01.Play(_targetRate);
		}

		public void SetVisible(bool isVisible)
		{
			_canvasGroup.alpha = (isVisible ? 1f : 0f);
		}

		public void ViewUpdate(float deltaTime)
		{
			_counter01.DisplayUpdate();
		}

		public override void OnClipPlay()
		{
			Play();
			base.OnClipPlay();
		}

		public override void OnClipTailEnd()
		{
			_counter01.Stop();
			base.OnClipTailEnd();
		}
	}
}
