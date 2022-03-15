using System;
using UnityEngine;

public class EventWindowBase : MonoBehaviour
{
	protected int HashCode = Animator.StringToHash("Base Layer.Out");

	protected int HashCodeIn = Animator.StringToHash("In");

	protected int HashCodeOut = Animator.StringToHash("Out");

	protected int HashCodeIdle = Animator.StringToHash("Idle");

	protected readonly int HashSkip = Animator.StringToHash("Skip");

	[SerializeField]
	protected Animator _animator;

	protected StateAnimController _stateController;

	protected bool IsCanSkip;

	public bool _isCallSkip;

	public virtual void Play(Action onAction)
	{
		if (!_isCallSkip)
		{
			Idle();
		}
		_animator.Play("In", 0, 0f);
		IsCanSkip = true;
		if (_stateController == null && onAction != null)
		{
			if (!_isCallSkip)
			{
				_stateController = _animator.GetBehaviour<StateAnimController>();
				_stateController.SetExitParts(onAction, HashCode);
			}
			else
			{
				_isCallSkip = false;
			}
		}
	}

	public virtual bool Skip()
	{
		if (IsCanSkip)
		{
			_isCallSkip = true;
			_animator.Play("Out", 0, 0f);
			IsCanSkip = false;
			return true;
		}
		return false;
	}

	public virtual void Idle()
	{
		_animator.Play(HashCodeIdle);
	}
}
