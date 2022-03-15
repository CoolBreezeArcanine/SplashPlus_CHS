using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CourseControllerBase : MonoBehaviour
{
	[SerializeField]
	protected Transform[] _leftTransforms;

	[SerializeField]
	protected Transform[] _rightTransforms;

	[SerializeField]
	protected AnimationParts _animation;

	[SerializeField]
	protected Transform[] _tabButtonTrans;

	[SerializeField]
	protected GameObject _tabButtonObj;

	[SerializeField]
	protected Image _leftIcon;

	[SerializeField]
	protected Image _rightIcon;

	protected TabButton[] _tabButtons;

	protected bool _isAnimation;

	protected int _monitorIndex;

	protected float _syncTimer;

	private void Awake()
	{
	}

	public virtual void Initialize(int monitorIndex)
	{
		_monitorIndex = monitorIndex;
		for (int i = 0; i < _tabButtons.Length; i++)
		{
			_tabButtons[i].Initialize(_monitorIndex);
		}
		_tabButtons[0].UseRightArrow();
		_tabButtons[1].UseLeftArrow();
		_animation.Play("Out");
		SetVisibleParts(isActive: false);
	}

	public void UpdateButtonView()
	{
		if (_isAnimation)
		{
			_syncTimer += (float)GameManager.GetGameMSecAdd() / 1000f;
			for (int i = 0; i < _tabButtons.Length; i++)
			{
				_tabButtons[i].ViewUpdate(_syncTimer);
			}
			if (1f < _syncTimer)
			{
				_syncTimer = 0f;
			}
		}
	}

	public void PlayInAnimation()
	{
		_animation.Play("In");
		_isAnimation = true;
		SetVisibleParts(isActive: true);
	}

	public void PlayOutAnimation()
	{
		if (_isAnimation)
		{
			_animation.Play("Out");
			_isAnimation = false;
			SetVisibleParts(isActive: false);
			_leftIcon?.gameObject.SetActive(value: false);
			_rightIcon?.gameObject.SetActive(value: false);
		}
	}

	public void PlayChangeAnimation()
	{
		_animation.Play("In_Change");
	}

	public void PlayMoveRightAnimation()
	{
		_animation.Play("Move_Right");
	}

	public void PlayMoveLeftAnimation()
	{
		_animation.Play("Move_Left");
	}

	public void PressedTabButton(bool isRight)
	{
		_tabButtons[(!isRight) ? 1 : 0].Pressed();
	}

	public virtual void SetVisibleParts(bool isActive)
	{
	}

	public void SetLeftIcon(Sprite sprite)
	{
		if (_leftIcon != null)
		{
			if (sprite != null)
			{
				_leftIcon.gameObject.SetActive(value: true);
				_leftIcon.sprite = sprite;
			}
			else
			{
				_leftIcon.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetRightIcon(Sprite sprite)
	{
		if (_rightIcon != null)
		{
			if (sprite != null)
			{
				_rightIcon.gameObject.SetActive(value: true);
				_rightIcon.sprite = sprite;
			}
			else
			{
				_rightIcon.gameObject.SetActive(value: false);
			}
		}
	}
}
