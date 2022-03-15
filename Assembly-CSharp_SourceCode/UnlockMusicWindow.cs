using System;
using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockMusicWindow : EventWindowBase
{
	[SerializeField]
	private Image _jacket;

	[SerializeField]
	private TextMeshProUGUI _musicName;

	[SerializeField]
	private TextMeshProUGUI _infoText;

	private void Awake()
	{
		_infoText.text = CommonMessageID.GetWindowMusicUnlock.GetName();
	}

	public void Set(Sprite jacket, string musicName)
	{
		_jacket.sprite = jacket;
		_musicName.text = musicName;
	}

	public void SetInfoText(string infoText)
	{
		_infoText.text = infoText;
	}

	public override void Play(Action onAction)
	{
		if (!_isCallSkip)
		{
			Idle();
		}
		_animator.Play("In2", 0, 0f);
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

	public override bool Skip()
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
}
