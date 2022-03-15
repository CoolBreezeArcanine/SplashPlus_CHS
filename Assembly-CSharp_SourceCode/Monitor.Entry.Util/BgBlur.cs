using Monitor.MapCore;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Entry.Util
{
	public class BgBlur : MapBehaviour
	{
		[SerializeField]
		private Image _blurImage;

		[SerializeField]
		private float _blurIntensity = 0.2f;

		private CanvasGroup _canvasGroup;

		private int _propertyId;

		public CanvasGroup ReferenceCanvasGroup { get; set; }

		private void Awake()
		{
			_propertyId = Shader.PropertyToID("_BlurIntensity");
			_blurImage.material = new Material(_blurImage.material);
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		protected override void OnLateUpdate(float deltaTime)
		{
			if (ReferenceCanvasGroup != null)
			{
				float alpha = ReferenceCanvasGroup.alpha;
				_canvasGroup.alpha = alpha;
				_blurImage.material.SetFloat(_propertyId, _blurIntensity * alpha);
			}
		}
	}
}
