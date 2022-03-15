using System.Linq;
using DB;
using MAI2.Util;
using Manager;
using Process;
using UnityEngine;

public class CollectionGetProcess : ProcessBase
{
	private enum State : byte
	{
		Init,
		GotoEnd,
		StartAnim,
		Wait,
		Update,
		Finish,
		Released
	}

	private CollectionGetMonitor[] _monitors;

	private bool[] _isEntry;

	private State _state;

	private bool _isPassedInit;

	private bool[] _isComplete;

	private bool _isFinished;

	public CollectionGetProcess(ProcessDataContainer dataContainer)
		: base(dataContainer)
	{
	}

	public override void OnAddProcess()
	{
	}

	public override void OnRelease()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			Object.Destroy(_monitors[i].gameObject);
		}
	}

	public override void OnStart()
	{
		GameObject prefs = Resources.Load<GameObject>("Process/CollectionGet/CollectionGetProcess");
		_monitors = new CollectionGetMonitor[2]
		{
			CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CollectionGetMonitor>(),
			CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CollectionGetMonitor>()
		};
		_isEntry = new bool[_monitors.Length];
		_isComplete = new bool[_monitors.Length];
		for (int i = 0; i < _monitors.Length; i++)
		{
			_isEntry[i] = Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry;
			_isComplete[i] = !_isEntry[i];
			_monitors[i].Initialize(i, _isEntry[i]);
			_monitors[i].SetFixedWindowMessage(WindowMessageID.CollectionGetAnnounceInfo.GetName());
			bool flag = _monitors[i].StartAnimation(Init, container.assetManager);
			if ((!_isComplete[i] && !flag) || Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
			{
				_isComplete[i] = true;
			}
		}
		if (_isComplete[0] && _isComplete[1])
		{
			_state = State.GotoEnd;
			return;
		}
		SetMessege();
		_state = State.StartAnim;
		container.processManager.NotificationFadeIn();
	}

	private void Init()
	{
		if (_isPassedInit)
		{
			return;
		}
		container.processManager.PrepareTimer(10, 0, isEntry: false, ChangeFinishState);
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isComplete[i])
			{
				container.processManager.SetVisibleTimer(i, isVisible: false);
			}
			else
			{
				_monitors[i].SetActiveButton();
			}
		}
		_isPassedInit = true;
		_state = State.Update;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		switch (_state)
		{
		case State.GotoEnd:
			GotoSkipNextProcess();
			break;
		case State.Finish:
			GotoNextProcess();
			break;
		}
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isEntry[i])
			{
				_monitors[i].ViewUpdate();
			}
		}
	}

	private void SetMessege()
	{
		if (!_isComplete.Contains(value: false))
		{
			ChangeFinishState();
			container.processManager.ClearTimeoutAction();
			return;
		}
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isEntry[i] && _isComplete[i])
			{
				container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait);
				_monitors[i].SetVisibleBlur(isActive: true);
				container.processManager.SetVisibleTimer(i, isVisible: false);
			}
		}
	}

	protected override void UpdateInput(int monitorId)
	{
		if (_isEntry[monitorId] && !_isComplete[monitorId])
		{
			switch (_state)
			{
			case State.Update:
				UpdateDetailInput(monitorId);
				break;
			case State.Init:
			case State.GotoEnd:
			case State.StartAnim:
			case State.Wait:
			case State.Finish:
			case State.Released:
				break;
			}
		}
	}

	private void UpdateDetailInput(int monitorId)
	{
		if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04) && !_isComplete[monitorId])
		{
			_isComplete[monitorId] = true;
			_monitors[monitorId].SetButtonAnimationActive();
			SetMessege();
			SetInputLockInfo(monitorId, 100f);
		}
	}

	public override void OnLateUpdate()
	{
	}

	private void ChangeFinishState()
	{
		if (!_isFinished)
		{
			_state = State.Finish;
			_isFinished = true;
		}
	}

	private void GotoSkipNextProcess()
	{
		container.processManager.AddProcess(new CollectionProcess(container), 50);
		_state = State.Released;
		container.processManager.ReleaseProcess(this);
	}

	private void GotoNextProcess()
	{
		container.processManager.AddProcess(new FadeProcess(container, this, new CollectionProcess(container), FadeProcess.FadeType.Type3), 50);
		container.processManager.ForceTimeUp();
		container.processManager.SetVisibleTimers(isVisible: false);
		_state = State.Released;
	}

	private void CreateDummyData()
	{
	}
}
