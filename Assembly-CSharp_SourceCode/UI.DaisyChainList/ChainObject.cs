using UnityEngine;

namespace UI.DaisyChainList
{
	[RequireComponent(typeof(CanvasGroup))]
	public class ChainObject : MonoBehaviour
	{
		[SerializeField]
		[Header("Basic")]
		private Vector3 _activeScale = Vector3.one;

		[SerializeField]
		private Vector3 _inactiveScale = Vector3.one;

		[SerializeField]
		[Header("独自スケール指定を行う場合に設定する")]
		private bool _isUseCustom;

		[SerializeField]
		private Vector2 _activeSize;

		[SerializeField]
		private Vector2 _inactiveSize;

		private CanvasGroup _canvasGroup;

		protected Vector3 ToPosition;

		private Vector3 _fromScale;

		private Vector3 _toScale;

		private bool _isExistSyncTimer;

		private bool _isExistScroll;

		private RectTransform _rectTransform;

		protected Animator Animator;

		public Vector3 ActiveScale => _activeScale;

		public Vector3 InactiveScale => _inactiveScale;

		public bool IsSetSpot { get; set; }

		public bool IsUsed { get; private set; }

		public bool IsAnimation { get; set; }

		public Vector3 FromPosition { get; private set; }

		public CanvasGroup CanvasGroup => _canvasGroup ?? GetComponent<CanvasGroup>();

		public RectTransform RectTransform => _rectTransform ?? GetComponent<RectTransform>();

		protected virtual void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			Animator = GetComponent<Animator>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_isExistSyncTimer = false;
			_isExistScroll = false;
			if (!(Animator != null))
			{
				return;
			}
			AnimatorControllerParameter[] parameters = Animator.parameters;
			foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
			{
				if (animatorControllerParameter.name == "SyncTimer")
				{
					_isExistSyncTimer = true;
					break;
				}
				if (animatorControllerParameter.name == "Scroll")
				{
					_isExistScroll = true;
					break;
				}
			}
		}

		public Vector2 GetSize(bool isCenter = false)
		{
			if (_isUseCustom)
			{
				if (!isCenter)
				{
					return _inactiveSize;
				}
				return _activeSize;
			}
			return RectTransform.sizeDelta;
		}

		public virtual void SetChainActive(bool isActive)
		{
			CanvasGroup.alpha = (isActive ? 1 : 0);
			IsUsed = isActive;
		}

		public virtual void ResetChain()
		{
		}

		public virtual void OnCenterIn()
		{
		}

		public virtual void OnCenter()
		{
		}

		public virtual void OnCenterOut()
		{
		}

		public virtual void OnCenterOutEnd()
		{
		}

		public void Immediate(Vector3 position, Vector3 scale)
		{
			if (!IsAnimation)
			{
				base.transform.localPosition = position;
			}
			base.transform.localScale = scale;
			FromPosition = position;
			_fromScale = scale;
			ToPosition = position;
			_toScale = scale;
			IsSetSpot = false;
		}

		public void BeginScroll(Vector3 toPosition, Vector3 toScale)
		{
			ToPosition = toPosition;
			_toScale = toScale;
			_fromScale = base.transform.localScale;
			if (Animator != null && base.gameObject.activeSelf && base.gameObject.activeInHierarchy && _isExistScroll)
			{
				Animator.SetTrigger("Scroll");
			}
		}

		public void BeginScroll(Vector3 fromPosition, Vector3 toPosition, Vector3 toScale)
		{
			ToPosition = toPosition;
			_toScale = toScale;
			FromPosition = fromPosition;
			_fromScale = base.transform.localScale;
			if (Animator != null && base.gameObject.activeSelf && base.gameObject.activeInHierarchy && _isExistScroll)
			{
				Animator.SetTrigger("Scroll");
			}
		}

		public virtual void ViewUpdate(float syncTimer)
		{
			if (Animator != null && base.gameObject.activeSelf && base.gameObject.activeInHierarchy && _isExistSyncTimer)
			{
				Animator.SetFloat("SyncTimer", syncTimer);
			}
		}

		public virtual void ScrollUpdate(float progress)
		{
			base.transform.localPosition = Vector3.Lerp(FromPosition, ToPosition, progress);
			base.transform.localScale = Vector3.Lerp(_fromScale, _toScale, progress);
			if (progress >= 1f)
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: true);
				}
				base.transform.localPosition = Vector3.Lerp(FromPosition, ToPosition, progress);
				base.transform.localScale = _toScale;
				FromPosition = ToPosition;
			}
		}
	}
}
