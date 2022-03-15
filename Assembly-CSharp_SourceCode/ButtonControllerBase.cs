using Mai2.Mai2Cue;
using Manager;
using UI;
using UnityEngine;

public class ButtonControllerBase : MonoBehaviour
{
	public struct ButtonInformation
	{
		public readonly CommonButtonObject.LedColors LedColor;

		public readonly string FileName;

		public readonly Cue Cue;

		public Sprite Image;

		public ButtonInformation(CommonButtonObject.LedColors ledColor, string fileName, Cue cue)
		{
			LedColor = ledColor;
			FileName = "Common/Sprites/Button/" + fileName;
			Cue = cue;
			Image = null;
		}
	}

	public enum FlatButtonType
	{
		Yes,
		No,
		Ok,
		Cancel,
		Skip,
		Next,
		Back,
		TimeSkip,
		Accesscode,
		Agree,
		Disagree,
		Freedom,
		GuestPlay,
		DetailOption,
		TrackStart,
		TutorialSkip,
		NameDelete,
		Upload,
		Plus,
		Minus,
		Set,
		Category,
		Decide,
		TakeIcon,
		Custom,
		Entry,
		Setting,
		SettingBack,
		EMoneyChange,
		CollaboArea,
		OriginalArea,
		End
	}

	protected const string CommonSpritePath = "Common/Sprites/Button/";

	protected const string ArrowSpriteName = "UI_CMN_Arrow";

	protected const string ArrowSelectorSpriteName = "UI_MDS_Btn_Arrow";

	protected const Cue ScrollCue = Cue.SE_SYS_CURSOR;

	protected const Cue TabCue = Cue.SE_SYS_TAB;

	private static bool _isFlatButtonLoaded = false;

	protected static readonly ButtonInformation[] FlatButtonParam = new ButtonInformation[31]
	{
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_Yes", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_No", Cue.SE_SYS_CANCEL),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_OK", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_Cancel", Cue.SE_SYS_CANCEL),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_Skip", Cue.SE_SYS_NEXT),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_NameNext", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_Back", Cue.SE_SYS_CANCEL),
		new ButtonInformation(CommonButtonObject.LedColors.Green, "UI_CMN_Btn_TimeSkip", Cue.SE_SYS_SKIP),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_Accesscode", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_Agree", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_Disagree", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_ENT_Btn_Freedom", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_GuestPlay", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Purple, "UI_CMN_Btn_Option", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_TrackStart", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_TutorialSkip", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_NameDelete", Cue.SE_SYS_CANCEL),
		new ButtonInformation(CommonButtonObject.LedColors.Orange, "UI_CMN_Btn_Upload", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_VolPlus", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_VolMinus", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_Set", Cue.SE_COLLECTION_SET),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_Category", Cue.SE_SYS_CANCEL),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_NameDecide", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_camera", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_Custom", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_Yes", Cue.SE_ENTRY_BUTTON),
		new ButtonInformation(CommonButtonObject.LedColors.Purple, "UI_CMN_Setting_01_Btn", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Purple, "UI_CMN_Setting_02_Btn", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Orange, "UI_CMN_Btn_Emoney", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Blue, "UI_CMN_Btn_CollaboArea", Cue.SE_SYS_FIX),
		new ButtonInformation(CommonButtonObject.LedColors.Red, "UI_CMN_Btn_OriginalArea", Cue.SE_SYS_FIX)
	};

	protected static Sprite ArrowSprite;

	protected static Sprite ArrowSelectorSprite;

	[SerializeField]
	protected Transform[] _positions;

	protected CommonButtonObject[] CommonButtons;

	protected bool IsActive;

	protected int MonitorIndex;

	protected float SyncTimer;

	public static ButtonInformation GetFlatButtonParam(int index)
	{
		_ = FlatButtonParam.Length;
		return GetFlatButtonParam((FlatButtonType)index);
	}

	public static ButtonInformation GetFlatButtonParam(FlatButtonType type)
	{
		_ = FlatButtonParam.Length;
		return FlatButtonParam[(int)type];
	}

	protected void SetCommonButtons(params CommonButtonObject[] buttons)
	{
		CommonButtons = new CommonButtonObject[buttons.Length];
		CommonButtons = buttons;
		SetAnimationActive(isActive: true);
	}

	public virtual void Initialize(int monitorIndex)
	{
		MonitorIndex = monitorIndex;
		SetAnimationActive(isActive: true);
	}

	public static void LoadDefaultResources()
	{
		if (!_isFlatButtonLoaded)
		{
			for (int i = 0; i < FlatButtonParam.Length; i++)
			{
				FlatButtonParam[i].Image = Resources.Load<Sprite>(FlatButtonParam[i].FileName);
			}
			ArrowSprite = Resources.Load<Sprite>("Common/Sprites/Button/UI_CMN_Arrow");
			ArrowSelectorSprite = Resources.Load<Sprite>("Common/Sprites/Button/UI_MDS_Btn_Arrow");
			_isFlatButtonLoaded = true;
		}
	}

	protected void SetAnimationActive(bool isActive)
	{
		IsActive = isActive;
	}

	public virtual void ViewUpdate(float gameMsecAdd)
	{
		if (!IsActive)
		{
			return;
		}
		SyncTimer += gameMsecAdd / 1000f;
		for (int i = 0; i < CommonButtons.Length; i++)
		{
			if (CommonButtons[i] != null)
			{
				CommonButtons[i].ViewUpdate(SyncTimer);
			}
		}
		if (SyncTimer > 1f)
		{
			SyncTimer = 0f;
		}
	}

	public virtual void SetVisible(bool visible, params int[] ids)
	{
		if (IsActive)
		{
			foreach (int num in ids)
			{
				CommonButtons[num]?.SetActiveButton(visible);
			}
		}
	}

	public virtual void SetVisibleFlip(bool visible, bool isFlip = false, params int[] ids)
	{
		if (IsActive)
		{
			foreach (int num in ids)
			{
				CommonButtons[num]?.SetActiveButtonFlip(visible, isFlip);
			}
		}
	}

	public void SetVisible(bool visible, params InputManager.ButtonSetting[] buttons)
	{
		if (!IsActive)
		{
			return;
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			int num = (int)buttons[i];
			if (CommonButtons[num] != null)
			{
				CommonButtons[num].SetActiveButton(visible);
			}
		}
	}

	public void SetVisibleFlip(bool visible, bool isFlip = false, params InputManager.ButtonSetting[] buttons)
	{
		if (!IsActive)
		{
			return;
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			int num = (int)buttons[i];
			if (CommonButtons[num] != null)
			{
				CommonButtons[num].SetActiveButtonFlip(visible, isFlip);
			}
		}
	}

	public void SetVisibleImmediate(bool isVisible, params int[] ids)
	{
		if (!IsActive)
		{
			return;
		}
		foreach (int num in ids)
		{
			if (CommonButtons[num] != null)
			{
				CommonButtons[num].SetActiveImmediateButton(isVisible);
			}
		}
	}

	public void SetShow(bool isShow, params int[] ids)
	{
		if (!IsActive)
		{
			return;
		}
		foreach (int num in ids)
		{
			if (CommonButtons[num] != null)
			{
				CommonButtons[num].SetNonActive(isShow);
			}
		}
	}

	public virtual void SetAnimationActive(int index)
	{
		if (IsActive && CommonButtons[index] != null)
		{
			CommonButtons[index].Pressed();
		}
	}

	public virtual void SetAnimationActiveFlip(int index, bool isFlip = false)
	{
		if (IsActive && CommonButtons[index] != null)
		{
			if (!isFlip)
			{
				CommonButtons[index].Pressed();
			}
			else
			{
				CommonButtons[index].PressedFlip(isFlip);
			}
		}
	}

	public virtual void ChangeButtonSymbol(InputManager.ButtonSetting button, int index)
	{
	}
}
