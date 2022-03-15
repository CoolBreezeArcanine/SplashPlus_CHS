using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

public class AdjustButtonController : ButtonControllerBase
{
	[SerializeField]
	private FrameAdjustButton[] _frameAdjustButtons;

	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_frameAdjustButtons.Length];
		for (int i = 0; i < CommonButtons.Length; i++)
		{
			if (_frameAdjustButtons[i] != null)
			{
				CommonButtons[i] = _frameAdjustButtons[i];
				CommonButtons[i].Initialize(monitorIndex, (InputManager.ButtonSetting)i, CommonButtonObject.LedColors.None);
				CommonButtons[i].SetSE(Cue.SE_SYS_CURSOR);
			}
		}
	}

	public void SetTopImage(bool isNormal, params int[] ids)
	{
		for (int i = 0; i < ids.Length; i++)
		{
			if (CommonButtons[ids[i]] is FrameAdjustButton)
			{
				((FrameAdjustButton)CommonButtons[ids[i]]).SetTopSprite(isNormal);
			}
		}
	}

	public void SetLoopButton()
	{
		CommonButtonObject[] commonButtons = CommonButtons;
		foreach (CommonButtonObject commonButtonObject in commonButtons)
		{
			if (commonButtonObject != null && commonButtonObject is FrameAdjustButton && commonButtonObject.gameObject.activeSelf && commonButtonObject.gameObject.activeInHierarchy)
			{
				commonButtonObject.SetLoop();
			}
		}
	}
}
