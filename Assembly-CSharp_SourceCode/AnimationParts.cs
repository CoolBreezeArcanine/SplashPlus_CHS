using System;
using UnityEngine;

public class AnimationParts : MonoBehaviour
{
	[SerializeField]
	private Animator _animator;

	private StateAnimController _controller;

	private void Awake()
	{
		if (_animator != null)
		{
			_controller = _animator.GetBehaviour<StateAnimController>();
		}
	}

	public bool isNull()
	{
		bool flag = true;
		if ((object)_controller == null)
		{
			flag = ((_controller != null) ? true : false);
		}
		else if (_controller != null)
		{
			flag = true;
		}
		else
		{
			flag = false;
			_controller = null;
		}
		return !flag;
	}

	public void Play(string animName, Action next, bool isSameState = false)
	{
		if (isNull())
		{
			_controller = _animator.GetBehaviour<StateAnimController>();
		}
		if (_animator != null)
		{
			_animator.Play(animName, 0, 0f);
		}
		if (next != null)
		{
			int hash = Animator.StringToHash("Base Layer." + animName);
			if (!isNull())
			{
				_controller.SetExitParts(next, hash, isSameState);
			}
		}
	}

	public void Play(string animName)
	{
		if (_animator != null)
		{
			_animator.Play(animName, 0, 0f);
		}
	}

	public void ResetExitParts()
	{
		if (!isNull())
		{
			_controller.ResetExitParts();
		}
	}
}
