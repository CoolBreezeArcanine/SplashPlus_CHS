using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

public class NameEntryButtonController : ButtonControllerBase
{
	[SerializeField]
	private GameObject _categoryButtonObj;

	[SerializeField]
	private Sprite[] _categoryButtonSprites;

	private readonly int[] CategoryButtonIndex = new int[4] { 0, 1, 6, 7 };

	public override void Initialize(int monitorIndex)
	{
		base.Initialize(monitorIndex);
		CommonButtons = new CommonButtonObject[_positions.Length];
		CommonButtons[0] = Object.Instantiate(_categoryButtonObj.GetComponent<NameEntryTextButtonObject>(), _positions[0]);
		CommonButtons[0].Initialize(MonitorIndex, InputManager.ButtonSetting.Button01, CommonButtonObject.LedColors.White);
		CommonButtons[0].SetSymbolSprite(_categoryButtonSprites[0], isFlip: false);
		CommonButtons[0].SetSE(Cue.SE_SYS_TAB);
		CommonButtons[1] = Object.Instantiate(_categoryButtonObj.GetComponent<NameEntryTextButtonObject>(), _positions[1]);
		CommonButtons[1].Initialize(MonitorIndex, InputManager.ButtonSetting.Button02, CommonButtonObject.LedColors.White);
		CommonButtons[1].SetSymbolSprite(_categoryButtonSprites[1], isFlip: false);
		CommonButtons[1].SetSE(Cue.SE_SYS_TAB);
		CommonButtons[2] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[2]);
		CommonButtons[2].Initialize(MonitorIndex, InputManager.ButtonSetting.Button03, CommonButtonObject.LedColors.White);
		CommonButtons[2].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: false);
		CommonButtons[2].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[3] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[3]);
		CommonButtons[3].Initialize(MonitorIndex, InputManager.ButtonSetting.Button04, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).LedColor);
		CommonButtons[3].SetSymbolStartDisable(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Image, isFlip: false);
		CommonButtons[3].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.Ok).Cue);
		CommonButtons[4] = Object.Instantiate(CommonPrefab.GetFlatButtonObject(), _positions[4]);
		CommonButtons[4].Initialize(MonitorIndex, InputManager.ButtonSetting.Button05, ButtonControllerBase.GetFlatButtonParam(FlatButtonType.NameDelete).LedColor);
		CommonButtons[4].SetSymbolStartDisable(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.NameDelete).Image, isFlip: false);
		CommonButtons[4].SetSE(ButtonControllerBase.GetFlatButtonParam(FlatButtonType.NameDelete).Cue);
		CommonButtons[5] = Object.Instantiate(CommonPrefab.GetCirclebuButtonObject(), _positions[5]);
		CommonButtons[5].Initialize(MonitorIndex, InputManager.ButtonSetting.Button06, CommonButtonObject.LedColors.White);
		CommonButtons[5].SetSymbolStartDisable(ButtonControllerBase.ArrowSprite, isFlip: true);
		CommonButtons[5].SetSE(Cue.SE_SYS_CURSOR);
		CommonButtons[6] = Object.Instantiate(_categoryButtonObj.GetComponent<NameEntryTextButtonObject>(), _positions[6]);
		CommonButtons[6].Initialize(MonitorIndex, InputManager.ButtonSetting.Button07, CommonButtonObject.LedColors.White);
		CommonButtons[6].SetSymbolSprite(_categoryButtonSprites[2], isFlip: false);
		CommonButtons[6].SetSE(Cue.SE_SYS_TAB);
		CommonButtons[7] = Object.Instantiate(_categoryButtonObj.GetComponent<NameEntryTextButtonObject>(), _positions[7]);
		CommonButtons[7].Initialize(MonitorIndex, InputManager.ButtonSetting.Button08, CommonButtonObject.LedColors.White);
		CommonButtons[7].SetSymbolSprite(_categoryButtonSprites[3], isFlip: false);
		CommonButtons[7].SetSE(Cue.SE_SYS_TAB);
		SetVisibleCategoryButton(isActive: false);
	}

	public void ChangeFlatButtonSymbol(int index, int spriteNum)
	{
		if (CommonButtons[index] != null)
		{
			CommonButtons[index].SetSymbol(ButtonControllerBase.GetFlatButtonParam(spriteNum).Image, isFlip: false);
			CommonButtons[index].ChangeColor(ButtonControllerBase.GetFlatButtonParam(spriteNum).LedColor);
			CommonButtons[index].SetSE(ButtonControllerBase.GetFlatButtonParam(spriteNum).Cue);
		}
	}

	public void PressCategoryButton(int buttonIndex, bool hold)
	{
		for (int i = 0; i < CategoryButtonIndex.Length; i++)
		{
			int num = CategoryButtonIndex[i];
			if (num == buttonIndex)
			{
				if (hold)
				{
					Hold(num);
				}
				else
				{
					Pressed(num);
				}
			}
			else
			{
				CommonButtons[num].SetLoop();
			}
		}
	}

	public void SetVisibleFlatButtons(bool isActive)
	{
		SetVisible(isActive, 3, 4);
	}

	public void SetVisibleCategoryButton(bool isActive)
	{
		SetVisible(isActive, 0, 1, 6, 7);
	}

	public void SetVisibleScrollButton(bool isActive)
	{
		if (isActive)
		{
			CommonButtons[2].SetLoop();
			CommonButtons[5].SetLoop();
		}
		else
		{
			CommonButtons[2].SetActiveButton(isActive: false);
			CommonButtons[5].SetActiveButton(isActive: false);
		}
	}

	public void Pressed(int buttonIndex)
	{
		CommonButtons[buttonIndex].Pressed();
	}

	public void Hold(int buttonIndex)
	{
		CommonButtons[buttonIndex].SetHold();
	}
}
