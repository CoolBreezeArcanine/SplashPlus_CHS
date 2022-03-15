using System;
using IO;
using Mai2.Voice_Partner_000001;
using Manager;
using TMPro;
using UnityEngine;

public class CollectionGetMonitor : MonitorBase
{
	[SerializeField]
	private CollectionGetButtonController _buttonController;

	[SerializeField]
	private AnimationParts _bgAnimator;

	[SerializeField]
	private CollectionGetList _list;

	[SerializeField]
	private AnimationParts _messageAnimator;

	[SerializeField]
	private TextMeshProUGUI _message;

	[SerializeField]
	private CanvasGroup _blur;

	public override void Initialize(int monIndex, bool active)
	{
		base.Initialize(monIndex, active);
		if (IsActive())
		{
			MechaManager.LedIf[monIndex].ButtonLedReset();
		}
		if (active)
		{
			_buttonController.Initialize(monIndex);
			SetVisibleBlur(!active);
		}
		SetVisible(active);
	}

	public void SetFixedWindowMessage(string message)
	{
		_message.text = message;
	}

	protected override void SetVisible(bool isActive)
	{
		Main.alpha = (isActive ? 1 : 0);
		Sub.alpha = (isActive ? 1 : 0);
	}

	public bool StartAnimation(Action next, AssetManager manager)
	{
		bool num = _list.Prepare(monitorIndex, manager);
		if (num)
		{
			SoundManager.PlayPartnerVoice(Cue.VO_000155, base.MonitorIndex);
			_bgAnimator.Play("In", delegate
			{
				_messageAnimator.Play("In", delegate
				{
					_list.PrepareAddAnimation(next);
				});
			});
		}
		return num;
	}

	public void SetActiveButton()
	{
		_buttonController.SetVisible(true, default(int));
	}

	public void SetButtonAnimationActive()
	{
		_buttonController.SetAnimationActive(0);
	}

	public override void ViewUpdate()
	{
		_list.PlayAddAnimation((float)GameManager.GetGameMSecAdd() / 1000f);
	}

	public void SetVisibleBlur(bool isActive)
	{
		_blur.alpha = (isActive ? 1 : 0);
	}
}
