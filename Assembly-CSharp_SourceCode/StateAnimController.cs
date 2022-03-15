using System;
using UnityEngine;

public class StateAnimController : StateMachineBehaviour
{
	private int _hashCodeStateEnter;

	private int _hashCodeStateExit;

	private Action _enter;

	private Action _exit;

	private bool _isSkip;

	public void SetEnterParts(Action enter, int hash)
	{
		_enter = enter;
		_hashCodeStateEnter = hash;
	}

	public void ResetEnterParts()
	{
		_enter = null;
		_hashCodeStateEnter = 0;
	}

	public void ResetExitParts()
	{
		_exit = null;
		_hashCodeStateExit = 0;
	}

	public void SetExitParts(Action exit, int hash, bool isSameState = false)
	{
		_exit = exit;
		_hashCodeStateExit = hash;
		_isSkip = isSameState;
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (animatorStateInfo.fullPathHash == _hashCodeStateEnter)
		{
			_enter?.Invoke();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (_isSkip)
		{
			_isSkip = false;
		}
		else if (animatorStateInfo.fullPathHash == _hashCodeStateExit)
		{
			_exit?.Invoke();
		}
	}
}
