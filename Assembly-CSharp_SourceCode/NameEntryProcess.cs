using System;
using System.Collections.Generic;
using System.Text;
using DB;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Monitor.TakeOver;
using Process;
using UnityEngine;

public class NameEntryProcess : ProcessBase, INameEntryProcess
{
	private enum State : byte
	{
		Idle,
		Init,
		Wait,
		Update,
		Released
	}

	public enum SubSequence : byte
	{
		Idle,
		Init,
		StartAnim,
		Input,
		Confirm,
		TimeUp,
		Completed,
		WaitNext
	}

	public const string SPACE_TAG = "␣";

	public const string SPACE = "\u3000";

	public const string END = "终";

	public const int MAX_NAME_NUM = 8;

	private NameEntryMonitor[] _monitors;

	private bool[] _isEntry;

	private bool[] _isOperating;

	private State _state;

	private SubSequence[] _subSequence;

	private PlayerNameInfo[] _playerNameInfo;

	private List<string> _ngWordJpList;

	private List<string> _ngWordExList;

	private List<string> _symbolList;

	private StringBuilder[] _tmp;

	private SlideScrollController[] _slideController;

	public uint[] _rom_version = new uint[2];

	public TakeOverMonitor.MajorRomVersion[] _major_version = new TakeOverMonitor.MajorRomVersion[2];

	public bool _isArgument;

	public NameEntryProcess(ProcessDataContainer dataContainer)
		: base(dataContainer)
	{
	}

	public NameEntryProcess(ProcessDataContainer dataContainer, params object[] args)
		: base(dataContainer)
	{
		if (args == null)
		{
			return;
		}
		uint num = (uint)args.Length;
		if (num == 2)
		{
			for (int i = 0; i < 2; i++)
			{
				_rom_version[i] = (uint)args[i];
				TakeOverMajorVersion takeOverMajorVersion = new TakeOverMajorVersion();
				_major_version[i] = takeOverMajorVersion.GetMajorRomVersion(_rom_version[i]);
			}
			_isArgument = true;
		}
	}

	public override void OnAddProcess()
	{
	}

	public override void OnRelease()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			UnityEngine.Object.Destroy(_monitors[i].gameObject);
		}
	}

	public override void OnStart()
	{
		GameObject prefs = Resources.Load<GameObject>("Process/NameEntry/NameEntryProcess");
		_monitors = new NameEntryMonitor[2]
		{
			CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<NameEntryMonitor>(),
			CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<NameEntryMonitor>()
		};
		int num = _monitors.Length;
		_playerNameInfo = new PlayerNameInfo[num];
		_subSequence = new SubSequence[num];
		_ngWordJpList = new List<string>();
		_ngWordExList = new List<string>();
		_tmp = new StringBuilder[num];
		_isEntry = new bool[num];
		_isOperating = new bool[num];
		_slideController = new SlideScrollController[num];
		CreateNgWordList();
		CreateSymbolList();
		for (int i = 0; i < num; i++)
		{
			_isEntry[i] = Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry;
			_monitors[i].PrevInit(this, container.assetManager);
			UserData.UserIDType userType = Singleton<UserDataManager>.Instance.GetUserData(i).UserType;
			_isOperating[i] = ((!_isEntry[i]) ? _isEntry[i] : (userType == UserData.UserIDType.Inherit || userType == UserData.UserIDType.New));
			_monitors[i].Initialize(i, _isOperating[i]);
			if (_isOperating[i])
			{
				InitializeDetail(i, userType);
			}
		}
	}

	private void InitializeDetail(int monitorId, UserData.UserIDType type)
	{
		_tmp[monitorId] = new StringBuilder();
		_playerNameInfo[monitorId] = new PlayerNameInfo();
		switch (type)
		{
		case UserData.UserIDType.New:
			_playerNameInfo[monitorId].Initialize("");
			SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
			_monitors[monitorId].DeployStringChainList();
			break;
		case UserData.UserIDType.Inherit:
			_playerNameInfo[monitorId].Initialize(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.UserName);
			_monitors[monitorId].ChangeActiveField(_playerNameInfo[monitorId].NameFieldIndex);
			InitName(monitorId, _playerNameInfo[monitorId].CurrentName());
			_playerNameInfo[monitorId].GotoEnd();
			if (_playerNameInfo[monitorId].CurrentName().Length < 8)
			{
				SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
				_monitors[monitorId].DeployStringChainList();
			}
			else
			{
				_playerNameInfo[monitorId].IsFieldEnd = true;
				_playerNameInfo[monitorId].SetInputTypeAndListIndex();
				_monitors[monitorId].DeployEndChainList(_playerNameInfo[monitorId].Type);
			}
			break;
		}
		_monitors[monitorId].PrepareAnimation();
		_subSequence[monitorId] = SubSequence.Idle;
		_slideController[monitorId] = new SlideScrollController();
		_slideController[monitorId].SetAction(monitorId, SlideScrollLeft, SlideScrollRight);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		switch (_state)
		{
		case State.Idle:
			_state = State.Init;
			break;
		case State.Init:
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_isOperating[i])
				{
					SoundManager.PlayVoice(Cue.VO_000017, i);
					_monitors[i].StartAnimation(_playerNameInfo[i].IsFieldEnd, Init);
				}
			}
			container.processManager.NotificationFadeIn();
			_state = State.Wait;
			break;
		}
		case State.Update:
			UpdateAutoScroll();
			break;
		}
		for (int j = 0; j < _monitors.Length; j++)
		{
			if (_isOperating[j])
			{
				_monitors[j].ViewUpdate();
			}
		}
	}

	private void Init()
	{
		_state = State.Update;
		container.processManager.PrepareTimer(99, 0, isEntry: false, GotoTimeUp);
		for (int i = 0; i < _monitors.Length; i++)
		{
			container.processManager.SetVisibleTimer(i, _isOperating[i]);
			if (_isEntry[i] && !_isOperating[i])
			{
				_subSequence[i] = SubSequence.WaitNext;
				_monitors[i].ChangeSubSequence(_subSequence[i]);
				container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
			}
			else
			{
				_subSequence[i] = SubSequence.Input;
			}
		}
	}

	public override void OnLateUpdate()
	{
	}

	protected override void UpdateInput(int monitorId)
	{
		if (_isOperating[monitorId] && _state == State.Update)
		{
			switch (_subSequence[monitorId])
			{
			case SubSequence.Input:
				DetailInput(monitorId);
				break;
			case SubSequence.Confirm:
				DetailInputConfirm(monitorId);
				break;
			case SubSequence.TimeUp:
				DetailTimeUp(monitorId);
				break;
			case SubSequence.Completed:
				DetailInputCompleted(monitorId);
				break;
			case SubSequence.Idle:
			case SubSequence.Init:
			case SubSequence.StartAnim:
				break;
			}
		}
	}

	private void DetailInput(int monitorId)
	{
		if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
		{
			if (!_playerNameInfo[monitorId].IsFieldEnd)
			{
				bool inputLongPush = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L);
				ScrollLeft(monitorId, inputLongPush);
				SetInputLockInfo(monitorId, 100f);
			}
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
		{
			if (!_playerNameInfo[monitorId].IsFieldEnd)
			{
				bool inputLongPush2 = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L);
				ScrollRight(monitorId, inputLongPush2);
				SetInputLockInfo(monitorId, 100f);
			}
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button02, InputManager.TouchPanelArea.A2))
		{
			if (!_playerNameInfo[monitorId].IsFieldEnd)
			{
				ChangeInputType(monitorId, PlayerNameInfo.InputType.Symbol);
			}
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button07, InputManager.TouchPanelArea.A7))
		{
			if (!_playerNameInfo[monitorId].IsFieldEnd)
			{
				ChangeInputType(monitorId, PlayerNameInfo.InputType.Large);
				SetInputLockInfo(monitorId, 100f);
			}
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8))
		{
			if (!_playerNameInfo[monitorId].IsFieldEnd)
			{
				ChangeInputType(monitorId, PlayerNameInfo.InputType.Small);
				SetInputLockInfo(monitorId, 100f);
			}
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button01, InputManager.TouchPanelArea.A1))
		{
			if (!_playerNameInfo[monitorId].IsFieldEnd)
			{
				ChangeInputType(monitorId, PlayerNameInfo.InputType.Num);
				SetInputLockInfo(monitorId, 100f);
			}
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			_monitors[monitorId].SetButtonAnimation(3);
			if (_playerNameInfo[monitorId].IsFieldEnd)
			{
				CheckName(monitorId);
				_subSequence[monitorId] = SubSequence.Confirm;
				_monitors[monitorId].ChangeSubSequence(_subSequence[monitorId]);
				SetInputLockInfo(monitorId, 550f);
				return;
			}
			string @string = GetString(monitorId);
			if (IsEnd(@string))
			{
				CheckName(monitorId);
				_subSequence[monitorId] = SubSequence.Confirm;
				_monitors[monitorId].ChangeSubSequence(_subSequence[monitorId]);
			}
			else
			{
				AddString(monitorId, @string);
				if (_playerNameInfo[monitorId].NameFieldIndex + 1 < 8)
				{
					_playerNameInfo[monitorId].NameFieldIndex++;
					_monitors[monitorId].ChangeActiveField(_playerNameInfo[monitorId].NameFieldIndex);
					SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, @string);
				}
				else
				{
					_playerNameInfo[monitorId].IsFieldEnd = true;
					_playerNameInfo[monitorId].SetInputTypeAndListIndex();
					_monitors[monitorId].ChangeEndList();
					_playerNameInfo[monitorId].ChangeEnd();
				}
			}
			SetInputLockInfo(monitorId, 100f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
		{
			_monitors[monitorId].SetButtonAnimation(4);
			if (_playerNameInfo[monitorId].IsFieldEnd)
			{
				_playerNameInfo[monitorId].DeleteFieldEnd();
				_monitors[monitorId].ChangeStringList(_playerNameInfo[monitorId].Type);
				_monitors[monitorId].ChangeActiveField(_playerNameInfo[monitorId].NameFieldIndex);
			}
			else if (0 <= _playerNameInfo[monitorId].NameFieldIndex - 1)
			{
				DeleteName(monitorId);
				_monitors[monitorId].ChangeActiveField(_playerNameInfo[monitorId].NameFieldIndex);
				SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
			}
			SetInputLockInfo(monitorId, 100f);
		}
	}

	private void GotoTimeUp()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isOperating[i] && _subSequence[i] != SubSequence.Completed)
			{
				if (_subSequence[i] == SubSequence.Input && !_playerNameInfo[i].IsFieldEnd)
				{
					_monitors[i].DeleteName(_playerNameInfo[i].NameFieldIndex);
				}
				_subSequence[i] = SubSequence.TimeUp;
				_monitors[i].ChangeSubSequence(_subSequence[i]);
				CheckName(i);
			}
		}
		Action both = delegate
		{
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (_isOperating[j] && _subSequence[j] != SubSequence.Completed)
				{
					GotoComplete(j);
				}
			}
		};
		container.processManager.PrepareTimer(5, 0, isEntry: false, both, isVisible: false);
	}

	private void DetailTimeUp(int monitorId)
	{
		if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			GotoComplete(monitorId);
		}
	}

	private void DetailInputConfirm(int monitorId)
	{
		if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			GotoComplete(monitorId);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
		{
			_subSequence[monitorId] = SubSequence.Input;
			if (_playerNameInfo[monitorId].IsFieldEnd)
			{
				_playerNameInfo[monitorId].DeleteFieldEnd();
				_monitors[monitorId].ChangeActiveField(_playerNameInfo[monitorId].NameFieldIndex);
			}
			else
			{
				SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
			}
			_monitors[monitorId].Confirm2Input();
			container.processManager.CloseWindow(monitorId);
		}
	}

	private void DetailInputCompleted(int monitorId)
	{
		if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			if (IsCanGotoNextProcess())
			{
				GotoNextProcess();
				_monitors[monitorId].SetDecisionButtonAnimationActive();
				SetInputLockInfo(0, 100f);
				SetInputLockInfo(1, 100f);
			}
			else
			{
				_monitors[monitorId].SetDecisionButtonAnimationActive();
				GotoWaitNext(monitorId);
				SetInputLockInfo(monitorId, 100f);
			}
		}
	}

	private void ScrollLeft(int monitorId, bool isLongTap)
	{
		_playerNameInfo[monitorId].ScrollLeft();
		_monitors[monitorId].ScrollLeft(isLongTap);
		_monitors[monitorId].SetButtonAnimation(2);
		SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
	}

	private void ScrollRight(int monitorId, bool isLongTap)
	{
		_playerNameInfo[monitorId].ScrollRight();
		_monitors[monitorId].ScrollRight(isLongTap);
		_monitors[monitorId].SetButtonAnimation(5);
		SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
	}

	private void SlideScrollLeft(int monitorId)
	{
		_playerNameInfo[monitorId].ScrollLeft();
		_monitors[monitorId].ScrollLeft(isLongTap: false);
		SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
	}

	private void SlideScrollRight(int monitorId)
	{
		_playerNameInfo[monitorId].ScrollRight();
		_monitors[monitorId].ScrollRight(isLongTap: false);
		SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
	}

	private void UpdateAutoScroll()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isOperating[i])
			{
				_slideController[i].UpdateAutoScroll();
			}
		}
	}

	private void GotoComplete(int monitorId)
	{
		_subSequence[monitorId] = SubSequence.Completed;
		string text = _playerNameInfo[monitorId].CurrentName();
		text = text.Replace("␣", "\u3000");
		Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.UserName = text;
		_monitors[monitorId].ChangeSubSequence(_subSequence[monitorId]);
		if (!IsCanGotoNextProcess())
		{
			return;
		}
		container.processManager.PrepareTimer(5, 0, isEntry: false, GotoNextProcess, isVisible: false);
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isOperating[i])
			{
				SetInputLockInfo(i, 2000f);
			}
		}
	}

	public void OpenWelcomInfo(int monitorId)
	{
		container.processManager.SetVisibleTimer(monitorId, isVisible: false);
		container.processManager.CloseWindow(monitorId, WindowPositionID.Middle);
		container.processManager.EnqueueMessage(monitorId, WindowMessageID.NameEntryWelcomeInfo);
	}

	private void GotoWaitNext(int monitorId)
	{
		_subSequence[monitorId] = SubSequence.WaitNext;
		_monitors[monitorId].ChangeSubSequence(_subSequence[monitorId]);
		container.processManager.ForcedCloseWindow(monitorId);
		container.processManager.EnqueueMessage(monitorId, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
	}

	private bool IsCanGotoNextProcess()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isOperating[i] && _subSequence[i] != SubSequence.Completed && _subSequence[i] != SubSequence.WaitNext)
			{
				return false;
			}
		}
		return true;
	}

	private void GotoNextProcess()
	{
		_state = State.Released;
		container.processManager.ClearTimeoutAction();
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(i).UserType == UserData.UserIDType.Inherit)
			{
				if (!_isArgument)
				{
					container.processManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(container), FadeProcess.FadeType.Type3), 50);
					return;
				}
				container.processManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(container, _rom_version[0], _rom_version[1]), FadeProcess.FadeType.Type3), 50);
				return;
			}
		}
		if (!_isArgument)
		{
			container.processManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(container), FadeProcess.FadeType.Type3), 50);
			return;
		}
		container.processManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(container, _rom_version[0], _rom_version[1]), FadeProcess.FadeType.Type3), 50);
	}

	public string GetStringByAdjustIndex(int monitorId, int diff)
	{
		return _playerNameInfo[monitorId].GetString(diff);
	}

	private string GetString(int monitorId)
	{
		return _playerNameInfo[monitorId].GetString();
	}

	private void ChangeInputType(int monitorId, PlayerNameInfo.InputType type)
	{
		if (_playerNameInfo[monitorId].ChangeInputType(type))
		{
			_monitors[monitorId].ChangeInputType(type, hold: false);
			SetName(monitorId, _playerNameInfo[monitorId].NameFieldIndex, GetString(monitorId));
		}
	}

	private void InitName(int monitorId, string name)
	{
		for (int i = 0; i < name.Length; i++)
		{
			_monitors[monitorId].SetString(i, name[i].ToString());
		}
	}

	private void SetName(int monitorId, int index, string str)
	{
		_monitors[monitorId].SetString(index, str);
	}

	private void AddString(int monitorId, string str)
	{
		_playerNameInfo[monitorId].AddString(str);
	}

	private void DeleteName(int monitorId)
	{
		_monitors[monitorId].DeleteName(_playerNameInfo[monitorId].NameFieldIndex);
		_playerNameInfo[monitorId].DeleteName();
	}

	private void ReplaceDefaultName(int monitorId)
	{
		_playerNameInfo[monitorId].ReplaceDefaultName();
		_monitors[monitorId].ChangeActiveField(_playerNameInfo[monitorId].NameFieldIndex);
		for (int i = 0; i < 8; i++)
		{
			if (i < CommonMessageID.DefaultUserName.GetName().Length)
			{
				SetName(monitorId, i, CommonMessageID.DefaultUserName.GetName()[i].ToString());
			}
			else
			{
				SetName(monitorId, i, "");
			}
		}
	}

	private bool CheckName(int monitorId)
	{
		WindowMessageID windowMessageID = WindowMessageID.EntryConfirmGuest;
		if (IsIncludeNgWord(monitorId, _playerNameInfo[monitorId].CurrentName()))
		{
			ReplaceDefaultName(monitorId);
			windowMessageID = ((_subSequence[monitorId] == SubSequence.Input) ? WindowMessageID.NameEntryNgwordInfo : ((_subSequence[monitorId] == SubSequence.TimeUp) ? WindowMessageID.NameEntryTimeUpInfo02 : windowMessageID));
		}
		else if (IsNotEntered(_playerNameInfo[monitorId].CurrentName()))
		{
			ReplaceDefaultName(monitorId);
			windowMessageID = ((_subSequence[monitorId] == SubSequence.Input) ? WindowMessageID.NameEntryNotEnteredInfo : ((_subSequence[monitorId] == SubSequence.TimeUp) ? WindowMessageID.NameEntryTimeUpInfo03 : windowMessageID));
		}
		else
		{
			windowMessageID = ((_subSequence[monitorId] == SubSequence.TimeUp) ? WindowMessageID.NameEntryTimeupInfo01 : WindowMessageID.NameEntryConfirm);
		}
		_monitors[monitorId].PrepareNameWindow(WindowMessageID.NameEntryConfirm);
		if (windowMessageID != WindowMessageID.NameEntryConfirm)
		{
			container.processManager.EnqueueMessage(monitorId, windowMessageID);
			return false;
		}
		return true;
	}

	private bool IsEnd(string str)
	{
		return str == "终";
	}

	private bool IsIncludeNgWord(int monitorId, string str)
	{
		str = RemoveUnnessaryString(monitorId, str);
		str = str.ToLower();
		return IsIncludeNgwordJp(str);
	}

	private bool IsNotEntered(string name)
	{
		string text = name;
		text = text.Replace("␣", "");
		return text.Length == 0;
	}

	private void CreateNgWordList()
	{
		int end = NgwordJpIDEnum.GetEnd();
		for (int i = 0; i < end; i++)
		{
			_ngWordJpList.Add(((NgwordJpID)i).GetName());
		}
		end = NgwordExIDEnum.GetEnd();
		for (int j = 0; j < end; j++)
		{
			_ngWordExList.Add(((NgwordExID)j).GetName());
		}
	}

	private void CreateSymbolList()
	{
		int end = CharlistSymboleIDEnum.GetEnd();
		_symbolList = new List<string>();
		for (int i = 0; i < end; i++)
		{
			_symbolList.Add(((CharlistSymboleID)i).GetName());
		}
	}

	private string RemoveUnnessaryString(int monitorId, string str)
	{
		_tmp[monitorId].Length = 0;
		str = str.Replace("␣", "");
		str = str.Replace("终", "");
		for (int i = 0; i < str.Length; i++)
		{
			if (!IsEqualsymbol(str[i]))
			{
				_tmp[monitorId].Append(str[i]);
			}
		}
		return _tmp[monitorId].ToString();
	}

	private bool IsIncludeNgwordJp(string str)
	{
		str = str.ToLower();
		for (int i = 0; i < _ngWordJpList.Count; i++)
		{
			if (str.Contains(_ngWordJpList[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsIncludeNgwordEx(string str)
	{
		for (int i = 0; i < _ngWordExList.Count; i++)
		{
			if (str.Contains(_ngWordExList[i]))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsEqualsymbol(char ch)
	{
		string text = ch.ToString();
		for (int i = 0; i < _symbolList.Count; i++)
		{
			if (_symbolList[i] == text)
			{
				return true;
			}
		}
		return false;
	}
}
