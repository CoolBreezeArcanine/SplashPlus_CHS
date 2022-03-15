using System;
using System.Collections.Generic;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor
{
	public class MonitorBackgroundController : MonoBehaviour
	{
		private enum State : byte
		{
			Morning,
			Noon,
			Evening,
			Night,
			Max
		}

		public int GenerateNum = 6;

		[SerializeField]
		[Header("空")]
		private CustomGradient[] _skys;

		[SerializeField]
		private RectTransform _mainRotationRectTransform;

		[SerializeField]
		private Gradient _highSky;

		[SerializeField]
		private Gradient _lowSky;

		[SerializeField]
		[Header("雲")]
		private Image _baseCloud01;

		[SerializeField]
		private Image _baseCloud02;

		[SerializeField]
		private Transform _cloudsParent;

		[SerializeField]
		private CanvasGroup _noonObjects;

		[SerializeField]
		private CanvasGroup _nightObjects;

		[SerializeField]
		[Header("時間")]
		[Range(0f, 1f)]
		private float _time;

		private readonly List<RectTransform> _cloudList = new List<RectTransform>();

		private Vector3 _amountMovement = new Vector3(-0.5f, 0f, 0f);

		private const float BaseTime = 200000f;

		private readonly int[] _times = new int[4] { 5, 25, 5, 25 };

		private bool _isAnimation;

		private float _masterTime;

		private float _rotationTimer;

		private float _dateTimer;

		private State _state = State.Noon;

		private State _toState = State.Evening;

		public virtual void Initialize()
		{
			UnityEngine.Random.InitState(DateTime.Now.Second);
			_state = State.Noon;
			GenerateNum = 4;
			_masterTime = 30f;
			for (int i = 0; i < GenerateNum; i++)
			{
				if (i < GenerateNum / 2)
				{
					RectTransform rectTransform = UnityEngine.Object.Instantiate(_baseCloud01.rectTransform, _cloudsParent);
					rectTransform.transform.localPosition = Vector3.zero;
					rectTransform.transform.localScale = Vector3.one;
					rectTransform.transform.localRotation = Quaternion.identity;
					rectTransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(-540, 540), UnityEngine.Random.Range(-105, 180));
					_cloudList.Add(rectTransform);
				}
				else
				{
					RectTransform rectTransform2 = UnityEngine.Object.Instantiate(_baseCloud02.rectTransform, _cloudsParent);
					rectTransform2.transform.localPosition = Vector3.zero;
					rectTransform2.transform.localScale = Vector3.one;
					rectTransform2.transform.localRotation = Quaternion.identity;
					rectTransform2.anchoredPosition = new Vector2(UnityEngine.Random.Range(-540, 540), UnityEngine.Random.Range(-105, 180));
					_cloudList.Add(rectTransform2);
				}
			}
			_baseCloud01.gameObject.SetActive(value: false);
			_baseCloud02.gameObject.SetActive(value: false);
			_noonObjects.alpha = 0f;
			_nightObjects.alpha = 0f;
		}

		public virtual void ViewUpdate()
		{
			_amountMovement = new Vector3((0f - (float)(1380 * GameManager.GetGameMSecAdd()) * 0.001f) * 0.075f, 0f, 0f);
			foreach (RectTransform cloud in _cloudList)
			{
				cloud.transform.Translate(_amountMovement);
				if (cloud.anchoredPosition.x < -690f)
				{
					cloud.anchoredPosition = new Vector2(690f, UnityEngine.Random.Range(-105, 180));
				}
			}
			float num = _masterTime / 60f;
			if (_skys != null)
			{
				CustomGradient[] skys = _skys;
				foreach (CustomGradient customGradient in skys)
				{
					if (customGradient != null)
					{
						customGradient.SetColor(_highSky.Evaluate(num), _lowSky.Evaluate(num));
					}
				}
			}
			if (num >= 0.25f && num <= 0.5f)
			{
				_noonObjects.alpha = Mathf.InverseLerp(0.25f, 0.5f, num);
			}
			else if (num >= 0.5f && num <= 0.75f)
			{
				_noonObjects.alpha = 1f - Mathf.InverseLerp(0.5f, 0.75f, num);
			}
			if (num >= 0f && num < 0.25f)
			{
				_nightObjects.alpha = 1f - Mathf.InverseLerp(0f, 0.25f, num);
			}
			else if (num >= 0.75f && num <= 1f)
			{
				_nightObjects.alpha = Mathf.InverseLerp(0.75f, 1f, num);
			}
			_rotationTimer += GameManager.GetGameMSecAdd();
			if (_rotationTimer >= 200000f)
			{
				_rotationTimer -= 200000f;
			}
		}

		private void UpdateBackground()
		{
			if ((DebugInput.GetKeyDown(KeyCode.Return) && !_isAnimation) || _masterTime >= (float)_times[(uint)_state])
			{
				_isAnimation = true;
				if ((int)(_state + 1) >= 4)
				{
					_toState = State.Morning;
				}
				else
				{
					_toState = _state + 1;
				}
				_masterTime = 0f;
			}
			if (_isAnimation)
			{
				_dateTimer += Time.deltaTime;
				if (_toState == State.Morning)
				{
					_nightObjects.alpha = 1f - _dateTimer / 3f;
				}
				else if (_toState == State.Noon)
				{
					_noonObjects.alpha = _dateTimer / 3f;
				}
				else if (_toState == State.Evening)
				{
					_noonObjects.alpha = 1f - _dateTimer / 3f;
				}
				else if (_toState == State.Night)
				{
					_nightObjects.alpha = _dateTimer / 3f;
				}
				if (_dateTimer >= 3f)
				{
					_state = _toState;
					_dateTimer = 0f;
					_isAnimation = false;
				}
			}
			_masterTime += Time.deltaTime;
		}
	}
}
