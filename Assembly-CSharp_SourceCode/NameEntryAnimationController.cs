using System;
using UnityEngine;

public class NameEntryAnimationController : MonoBehaviour
{
	[SerializeField]
	private AnimationParts _bg;

	[SerializeField]
	private AnimationParts _title;

	[SerializeField]
	private AnimationParts _nameField;

	[SerializeField]
	private AnimationParts _list;

	[SerializeField]
	private AnimationParts _message;

	[SerializeField]
	private CanvasGroup _listCanvasGroup;

	private StateAnimController _controller;

	public void PrepareAnimation()
	{
		_listCanvasGroup.alpha = 0f;
		_bg.Play("Disabled");
	}

	public void StartAnimation(Action next)
	{
		_bg.Play("In", delegate
		{
			InSecond(next);
		});
	}

	private void InSecond(Action next)
	{
		_title.Play("In");
		_nameField.Play("In");
		_list.Play("In", delegate
		{
			InEnd(next);
		});
	}

	private void InEnd(Action next)
	{
		_listCanvasGroup.alpha = 1f;
		_message.Play("In", next);
	}

	public void Out(Action next)
	{
		string animName = "Out";
		if (!_title.isNull())
		{
			_title.Play(animName);
		}
		if (!_nameField.isNull())
		{
			_nameField.Play(animName);
		}
		if (!_list.isNull())
		{
			_list.Play(animName);
		}
		if (!_message.isNull())
		{
			_message.Play(animName);
		}
		if (!_bg.isNull())
		{
			_bg.Play(animName, next);
		}
	}
}
