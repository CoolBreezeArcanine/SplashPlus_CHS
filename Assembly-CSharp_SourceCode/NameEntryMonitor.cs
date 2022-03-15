using System;
using DB;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.UserDatas;
using TMPro;
using UI.DaisyChainList;
using UnityEngine;

public class NameEntryMonitor : MonitorBase
{
	private enum ButtonPhase
	{
		InputName,
		NameConfirm,
		FinalDisp
	}

	[SerializeField]
	[Header("入力/確認フォームオブジェクト")]
	private GameObject _nameInputObj;

	[SerializeField]
	[Header("登録完了オブジェクト")]
	private GameObject _completedResistrationObj;

	[SerializeField]
	private NameEntryButtonController _buttonController;

	[SerializeField]
	[Header("入力名前リスト")]
	private InputNameFieldController _nameFieldController;

	[SerializeField]
	private TextMeshProUGUI _messageText;

	[SerializeField]
	private ConfirmNameWindow _nameWindow;

	[SerializeField]
	private CanvasGroup _blurCanvasGroup;

	[SerializeField]
	private GameObject _userEntryPlateObj;

	[SerializeField]
	private Transform _userEntryParent;

	[SerializeField]
	private GameObject _centerMarkObj;

	[SerializeField]
	[Header("文字列リスト")]
	private StringChainList _stringChainList;

	[SerializeField]
	private EndChainList _endChainList;

	[SerializeField]
	[Header("アニメーション管理用")]
	private NameEntryAnimationController _animationController;

	private INameEntryProcess _process;

	private AssetManager _assetManager;

	private PlayerNameInfo.InputType _prevInputType;

	private UserEntryPlate _userPlate;

	private ButtonControllerBase.FlatButtonType[,] flatButtonTypes = new ButtonControllerBase.FlatButtonType[3, 2]
	{
		{
			ButtonControllerBase.FlatButtonType.Ok,
			ButtonControllerBase.FlatButtonType.NameDelete
		},
		{
			ButtonControllerBase.FlatButtonType.Yes,
			ButtonControllerBase.FlatButtonType.Back
		},
		{
			ButtonControllerBase.FlatButtonType.Next,
			ButtonControllerBase.FlatButtonType.Back
		}
	};

	public void PrevInit(INameEntryProcess nameEntryProcess, AssetManager manager)
	{
		_process = nameEntryProcess;
		_assetManager = manager;
	}

	public override void Initialize(int monIndex, bool active)
	{
		Main.alpha = 0f;
		if (active)
		{
			_buttonController.Initialize(monIndex);
			_nameFieldController.Prepare();
			_nameWindow.Interrupt();
			_messageText.text = WindowMessageID.NameEntryDescription.GetName();
			SetActive(isInputSequence: true);
			_userPlate = UnityEngine.Object.Instantiate(_userEntryPlateObj, _userEntryParent).GetComponent<UserEntryPlate>();
			_userPlate.Initialize(monIndex);
		}
		else
		{
			_nameInputObj.SetActive(value: false);
			_completedResistrationObj.SetActive(value: false);
		}
		base.Initialize(monIndex, active);
	}

	public void ChangeInputType(PlayerNameInfo.InputType type, bool hold)
	{
		SetInputTypeList(type, hold);
		_prevInputType = type;
	}

	public void DeployStringChainList()
	{
		InitChainList();
		_stringChainList.Deploy();
	}

	public void DeployEndChainList(PlayerNameInfo.InputType type)
	{
		_prevInputType = type;
		InitChainList();
		ChangeEndList();
	}

	public void PrepareAnimation()
	{
		_animationController.PrepareAnimation();
	}

	public void StartAnimation(bool isFieldEnd, Action next)
	{
		_animationController.StartAnimation(delegate
		{
			if (isFieldEnd)
			{
				_buttonController.SetVisibleCategoryButton(isActive: false);
			}
			else
			{
				ChangeCategoryButtonanimation(_prevInputType, hold: true);
			}
			_buttonController.SetVisibleScrollButton(!isFieldEnd);
			_buttonController.SetVisibleFlatButtons(isActive: true);
			next?.Invoke();
		});
		Main.alpha = 1f;
	}

	private void InitChainList()
	{
		_stringChainList.Initialize();
		_stringChainList.AdvancedInitialize(_process, monitorIndex);
		_endChainList.Initialize();
	}

	public void ChangeEndList()
	{
		SetVisibleScrollCard(isVisible: false);
		_stringChainList.RemoveAll();
		_endChainList.Deploy();
		AllNonActiveField();
		_buttonController.SetVisibleCategoryButton(isActive: false);
		ChangeDesitionButtonTopImage(ButtonPhase.FinalDisp);
		_buttonController.SetVisibleScrollButton(isActive: false);
	}

	public void ChangeStringList(PlayerNameInfo.InputType type)
	{
		_endChainList.RemoveAll();
		_stringChainList.Deploy();
		SetVisibleBlurSet(isActive: false);
		ChangeCategoryButtonanimation(type, hold: true);
		_buttonController.SetVisibleScrollButton(isActive: true);
	}

	private void ChangeCategoryButtonanimation(PlayerNameInfo.InputType type, bool hold)
	{
		switch (type)
		{
		case PlayerNameInfo.InputType.Large:
			_buttonController.PressCategoryButton(6, hold);
			break;
		case PlayerNameInfo.InputType.Num:
			_buttonController.PressCategoryButton(0, hold);
			break;
		case PlayerNameInfo.InputType.Small:
			_buttonController.PressCategoryButton(7, hold);
			break;
		case PlayerNameInfo.InputType.Symbol:
			_buttonController.PressCategoryButton(1, hold);
			break;
		}
	}

	public void SetButtonAnimation(int buttonIndex)
	{
		_buttonController.SetAnimationActive(buttonIndex);
	}

	public void ScrollRight(bool isLongTap)
	{
		_stringChainList.Scroll(Direction.Right);
	}

	public void ScrollLeft(bool isLongTap)
	{
		_stringChainList.Scroll(Direction.Left);
	}

	private void SetInputTypeList(PlayerNameInfo.InputType type, bool hold)
	{
		ChangeCategoryButtonanimation(type, hold);
		PrepareList();
	}

	private void SetStringMax()
	{
		PrepareEndOnly();
	}

	public void SetVisibleScrollCard(bool isVisible)
	{
		_stringChainList.SetScrollCard(isVisible);
	}

	public override void ViewUpdate()
	{
		_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
		_stringChainList.ViewUpdate();
		_endChainList.ViewUpdate();
		if (_nameWindow.gameObject.activeSelf)
		{
			_nameWindow.UpdateView(GameManager.GetGameMSecAdd());
		}
	}

	private void PrepareList()
	{
		SetVisibleScrollCard(isVisible: false);
		_stringChainList.RemoveAll();
		_stringChainList.Deploy();
	}

	private void PrepareEndOnly()
	{
		SetVisibleScrollCard(isVisible: false);
		_stringChainList.RemoveAll();
		_stringChainList.DeployEndOnly();
	}

	public void SetString(int index, string str)
	{
		_nameFieldController.SetString(index, str);
	}

	public void ChangeActiveField(int index)
	{
		_nameFieldController.ChangeActiveField(index);
	}

	public void AllNonActiveField()
	{
		_nameFieldController.AllNonActiveField();
	}

	public void DeleteName(int index)
	{
		_nameFieldController.ChangeBlank(index);
	}

	private void ChangeDesitionButtonTopImage(ButtonPhase phase)
	{
		_buttonController.ChangeFlatButtonSymbol(3, (int)flatButtonTypes[(int)phase, 0]);
		if (phase != ButtonPhase.FinalDisp)
		{
			_buttonController.ChangeFlatButtonSymbol(4, (int)flatButtonTypes[(int)phase, 1]);
		}
	}

	public void ChangeSubSequence(NameEntryProcess.SubSequence next)
	{
		switch (next)
		{
		case NameEntryProcess.SubSequence.Confirm:
			Input2Confirm();
			break;
		case NameEntryProcess.SubSequence.TimeUp:
			GotoTimeUp();
			break;
		case NameEntryProcess.SubSequence.Completed:
			GotoCompleted();
			break;
		case NameEntryProcess.SubSequence.WaitNext:
			GotoWaitNext();
			break;
		case NameEntryProcess.SubSequence.Input:
			break;
		}
	}

	public void GotoTimeUp()
	{
		SetVisibleBlurSet(isActive: true);
		_buttonController.SetVisible(false, 4);
	}

	public void Confirm2Input()
	{
		SetVisibleBlurSet(isActive: false);
		_stringChainList.Deploy();
		_centerMarkObj.SetActive(value: true);
		_nameWindow.Interrupt();
		ChangeInputType(_prevInputType, hold: true);
		_buttonController.SetAnimationActive(4);
		_buttonController.SetVisibleScrollButton(isActive: true);
	}

	private void Input2Confirm()
	{
		SoundManager.PlayVoice(Cue.VO_000018, monitorIndex);
		SetVisibleBlurSet(isActive: true);
		_endChainList.RemoveAll();
		_endChainList.SetScrollCard(isVisible: false);
		_stringChainList.RemoveAll();
		_stringChainList.SetScrollCard(isVisible: false);
		_centerMarkObj.SetActive(value: false);
		_buttonController.SetVisibleScrollButton(isActive: false);
		_buttonController.SetVisibleCategoryButton(isActive: false);
	}

	private void GotoCompleted()
	{
		_buttonController.SetAnimationActive(3);
		_animationController.Out(delegate
		{
			SetActive(isInputSequence: false);
			SetUserData();
			_process.OpenWelcomInfo(monitorIndex);
			ChangeDesitionButtonTopImage(ButtonPhase.FinalDisp);
			_buttonController.SetVisible(false, 0, 1, 2, 4, 5, 6, 7);
		});
		_nameWindow.Interrupt();
		SetVisibleBlur(isActive: false);
		SoundManager.PlayVoice(Cue.VO_000019, monitorIndex);
	}

	private void SetActive(bool isInputSequence)
	{
		_nameInputObj.SetActive(isInputSequence);
		_completedResistrationObj.SetActive(!isInputSequence);
	}

	private void GotoWaitNext()
	{
		Main.alpha = 0f;
		_completedResistrationObj.SetActive(value: false);
	}

	public void SetUserData()
	{
		UserDetail detail = Singleton<UserDataManager>.Instance.GetUserData(base.MonitorIndex).Detail;
		UserOption option = Singleton<UserDataManager>.Instance.GetUserData(base.MonitorIndex).Option;
		MessageUserInformationData messageUserInformationData = new MessageUserInformationData(base.MonitorIndex, _assetManager, detail, option.DispRate, isSubMonitor: true);
		_userPlate.SetUserData(messageUserInformationData.Icon, messageUserInformationData.Name, (int)messageUserInformationData.Rateing, (int)messageUserInformationData.DispRate, messageUserInformationData.TotalAwake);
		_userPlate.AnimIn();
	}

	private void SetVisibleBlur(bool isActive)
	{
		_blurCanvasGroup.alpha = (isActive ? 1 : 0);
	}

	public void PrepareNameWindow(WindowMessageID id)
	{
		_nameWindow.Prepare(_nameFieldController.GetNameFieldArray(), id);
		SetVisibleBlur(isActive: true);
		_buttonController.SetVisibleScrollButton(isActive: false);
		_buttonController.SetVisibleCategoryButton(isActive: false);
	}

	private void SetVisibleBlurSet(bool isActive)
	{
		SetVisibleBlur(isActive);
		ChangeDesitionButtonTopImage(isActive ? ButtonPhase.NameConfirm : ButtonPhase.InputName);
	}

	public void SetDecisionButtonAnimationActive()
	{
		_buttonController.SetAnimationActive(3);
	}
}
