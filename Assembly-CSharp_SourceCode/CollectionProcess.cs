using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DB;
using MAI2.Util;
using Mai2.Voice_000001;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Process;
using Process.CardMakerInfo;
using UnityEngine;
using Util;

public class CollectionProcess : ProcessBase, ICollectionProcess, ICollectionGenreIDList, ITitleList, IIconList, INamePlateList, IPartnerList, IFrameList
{
	private enum Sequence : byte
	{
		Init,
		Start,
		Update,
		GuestWait,
		Released,
		EndWait
	}

	public enum SubSequence : byte
	{
		SelectCollectionType,
		Title,
		Icon,
		NamePlate,
		Frame,
		Prtner,
		Exit,
		Information,
		AimeInfo,
		Max
	}

	private CollectionMonitor[] _monitors;

	private Sequence _sequence;

	private SubSequence[] _subSequence;

	private CollectionGenreID[] _currentCollectionType;

	private bool[] _isEntry;

	private CollectionInfo[] _icon;

	private CollectionInfo[] _title;

	private CollectionInfo[] _plate;

	private CollectionInfo[] _partner;

	private CollectionInfo[] _frame;

	private Stopwatch[] _timer;

	private int[] _totalCollectionNum;

	private float _timeCounter;

	private float[] SlideScrollTimer;

	protected int[] SlideScrollCount;

	protected bool[] SlideScrollToRight;

	protected float[] SlideScrollTime;

	private bool[] _isScrollInputLock;

	public const uint WindowTimeOut = 10000u;

	public const uint WindowInputWait = 3000u;

	private const int MaxNewCollectionNum = 10;

	private const string NewGetGenreName = "新規獲得";

	public int CurrentIconIndex(int monitorId)
	{
		return _icon[monitorId].Index;
	}

	public int CurrentTitleIndex(int monitorId)
	{
		return _title[monitorId].Index;
	}

	public int CurrentPlateIndex(int monitorId)
	{
		return _plate[monitorId].Index;
	}

	public int CurrentPartnerIndex(int monitorId)
	{
		return _partner[monitorId].Index;
	}

	public int CurrentFrameIndex(int monitorId)
	{
		return _frame[monitorId].Index;
	}

	public CollectionGenreID CurrentColletionType(int monitorId)
	{
		return _currentCollectionType[monitorId];
	}

	public SubSequence CurrentSubSequence(int monitorId)
	{
		return _subSequence[monitorId];
	}

	public int GetTotalCollectionNum(int monitorId)
	{
		return _totalCollectionNum[monitorId];
	}

	public CollectionProcess(ProcessDataContainer dataContainer)
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
			UnityEngine.Object.Destroy(_monitors[i].gameObject);
		}
	}

	public override void OnStart()
	{
		GameObject prefs = Resources.Load<GameObject>("Process/Collection/CollectionProcess");
		_monitors = new CollectionMonitor[2]
		{
			CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CollectionMonitor>(),
			CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CollectionMonitor>()
		};
		_sequence = Sequence.Init;
		_subSequence = new SubSequence[_monitors.Length];
		_currentCollectionType = new CollectionGenreID[_monitors.Length];
		_isEntry = new bool[_monitors.Length];
		_totalCollectionNum = new int[_monitors.Length];
		_icon = new CollectionInfo[2]
		{
			new CollectionInfo(),
			new CollectionInfo()
		};
		_title = new CollectionInfo[2]
		{
			new CollectionInfo(),
			new CollectionInfo()
		};
		_plate = new CollectionInfo[2]
		{
			new CollectionInfo(),
			new CollectionInfo()
		};
		_partner = new CollectionInfo[2]
		{
			new CollectionInfo(),
			new CollectionInfo()
		};
		_frame = new CollectionInfo[2]
		{
			new CollectionInfo(),
			new CollectionInfo()
		};
		_timer = new Stopwatch[2]
		{
			new Stopwatch(),
			new Stopwatch()
		};
		SlideScrollTimer = new float[2];
		SlideScrollCount = new int[2];
		SlideScrollToRight = new bool[2];
		SlideScrollTime = new float[2];
		_isScrollInputLock = new bool[2];
		LoadCollectionData();
		bool flag = false;
		for (int i = 0; i < _monitors.Length; i++)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
			_monitors[i].PrevInit(this, container.assetManager);
			_isEntry[i] = userData.IsEntry && userData.UserType != UserData.UserIDType.Guest;
			if (_isEntry[i])
			{
				if (userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstCollecitonCustom))
				{
					_subSequence[i] = SubSequence.SelectCollectionType;
					SetInputLockInfo(i, 2000f);
				}
				else
				{
					_subSequence[i] = SubSequence.Information;
					EnqueueMessage(i, WindowMessageID.CollectionCustomFirst);
					_timer[i].Restart();
					SetInputLockInfo(i, 3000f);
					userData.Detail.ContentBit.SetFlag(ContentBitID.FirstCollecitonCustom, flag: true);
				}
				flag = true;
				CollectionGenreID collectionGenreID = ((!IsHaveNewTitle(i)) ? (IsHaveNewIcon(i) ? CollectionGenreID.Icon : (IsHaveNewNamePlate(i) ? CollectionGenreID.Plate : (IsHaveNewFrame(i) ? CollectionGenreID.Frame : (IsHaveNewPartner(i) ? CollectionGenreID.Partner : CollectionGenreID.Exit)))) : CollectionGenreID.Title);
				_currentCollectionType[i] = collectionGenreID;
			}
			else
			{
				_subSequence[i] = SubSequence.Max;
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(i).UserType == UserData.UserIDType.Guest)
				{
					container.processManager.EnqueueMessage(i, WindowMessageID.AimeUseNotice, WindowPositionID.Middle);
					_monitors[i].SetVisibleBlur(isActive: true);
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000163, i);
				}
			}
			_monitors[i].Initialize(i, _isEntry[i], _subSequence[i]);
			if (_currentCollectionType[i] != CollectionGenreID.Exit)
			{
				_monitors[i].ChangeCategoryType();
			}
		}
		GC.Collect();
		container.processManager.NotificationFadeIn();
		if (!flag)
		{
			_sequence = Sequence.GuestWait;
		}
		_timeCounter = 0f;
	}

	private void LoadCollectionData()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			CreateTitleData(i);
			CreateIconData(i);
			CreatePlateData(i);
			CreatePartnerData(i);
			CreateFrameData(i);
			_totalCollectionNum[i] = 0;
			_totalCollectionNum[i] += GetAllTitleNum(i);
			_totalCollectionNum[i] += GetAllIconNum(i);
			_totalCollectionNum[i] += GetAllPlateNum(i);
			_totalCollectionNum[i] += GetAllPartnerNum(i);
			_totalCollectionNum[i] += GetAllFrameNum(i);
		}
	}

	public override void OnUpdate()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			_ = _isEntry[i];
		}
		base.OnUpdate();
		switch (_sequence)
		{
		case Sequence.Init:
			_sequence = Sequence.Start;
			break;
		case Sequence.Start:
		{
			Action both = delegate
			{
				GotoNextProcess();
			};
			container.processManager.PrepareTimer(60, 0, isEntry: false, both);
			container.processManager.SetCompleteOneSide(TimeUp);
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (_isEntry[j])
				{
					_monitors[j]._initGenreID = _currentCollectionType[j];
					_monitors[j].StartAnimation(_currentCollectionType[j], _subSequence[j]);
					if (_subSequence[j] == SubSequence.Information)
					{
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000227, j);
					}
					else
					{
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000156, j);
					}
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsGuest())
					{
						container.processManager.SetVisibleTimer(j, isVisible: false);
					}
				}
				else
				{
					container.processManager.SetVisibleTimer(j, isVisible: false);
				}
			}
			_sequence = Sequence.Update;
			break;
		}
		case Sequence.GuestWait:
			_timeCounter += Time.deltaTime;
			if (_timeCounter >= 5f)
			{
				GotoNextProcess();
			}
			break;
		case Sequence.Released:
			_timeCounter += Time.deltaTime;
			if (_timeCounter >= 0.5f)
			{
				_timeCounter = 0f;
				container.processManager.AddProcess(new FadeProcess(container, this, new CardMakerInfoProcess(container), FadeProcess.FadeType.Type3), 50);
				container.processManager.SetVisibleTimers(isVisible: false);
				_sequence = Sequence.EndWait;
			}
			break;
		}
		for (int k = 0; k < _monitors.Length; k++)
		{
			if (_isEntry[k])
			{
				_monitors[k].ViewUpdate();
			}
		}
	}

	public override void OnLateUpdate()
	{
	}

	private void InputInformationSequence(int monitorId)
	{
		if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04) || _timer[monitorId].ElapsedMilliseconds > 10000)
		{
			_subSequence[monitorId] = SubSequence.SelectCollectionType;
			SetInputLockInfo(monitorId, 550f);
			container.processManager.CloseWindow(monitorId);
			_monitors[monitorId].SetVisibleBlur(isActive: false);
			_monitors[monitorId].InformationToCollection();
			container.processManager.IsTimeCounting(monitorId, isTimeCount: true);
			SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000156, monitorId);
		}
	}

	private void InputCollectionTypeSequence(int monitorId)
	{
		if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
		{
			if (_currentCollectionType[monitorId] + 1 < CollectionGenreID.End)
			{
				_currentCollectionType[monitorId]++;
				_monitors[monitorId].ScrollCollectionListLeft(isLongTap: false);
				_monitors[monitorId].SetCollectionGenreIDListButton(_currentCollectionType[monitorId]);
				if (_currentCollectionType[monitorId] == CollectionGenreID.Exit)
				{
					_monitors[monitorId].ChangeCategoryExit();
				}
			}
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6))
		{
			if (_currentCollectionType[monitorId] - 1 >= CollectionGenreID.Title)
			{
				if (_currentCollectionType[monitorId] == CollectionGenreID.Exit)
				{
					_monitors[monitorId].ChangeCategoryType();
				}
				_currentCollectionType[monitorId]--;
				_monitors[monitorId].ScrollCollectionListRight(isLongTap: false);
				_monitors[monitorId].SetCollectionGenreIDListButton(_currentCollectionType[monitorId]);
			}
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.B1, InputManager.TouchPanelArea.B8))
		{
			if (_currentCollectionType[monitorId] + 1 < CollectionGenreID.End)
			{
				SkipExit(monitorId);
			}
		}
		else
		{
			if (!InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
			{
				return;
			}
			SubSequence subSequence = ConvertToSubSequence(_currentCollectionType[monitorId]);
			_subSequence[monitorId] = subSequence;
			if (SubSequence.Exit == subSequence)
			{
				container.processManager.ForceTimeUp(monitorId);
				TimeUp(monitorId);
				_monitors[monitorId].SetActiveButtonAnimation(2);
			}
			else
			{
				_monitors[monitorId].ChangeSubSequence(subSequence);
				switch (subSequence)
				{
				case SubSequence.Icon:
					_icon[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID);
					break;
				case SubSequence.Title:
					_title[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID);
					break;
				case SubSequence.NamePlate:
					_plate[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID);
					break;
				case SubSequence.Prtner:
					_partner[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID);
					break;
				case SubSequence.Frame:
					_frame[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID);
					break;
				}
			}
			SetInputLockInfo(monitorId, 550f);
		}
	}

	private void SlideScrollRight(int monitorId, ref CollectionInfo info)
	{
		SetInputLockInfo(monitorId, 100f);
		_isScrollInputLock[monitorId] = true;
		SlideScrollToRight[monitorId] = true;
		SlideScrollTime[monitorId] = 50f;
		for (int num = -1; num >= -10; num--)
		{
			bool flag = false;
			int overCount;
			switch (_subSequence[monitorId])
			{
			case SubSequence.Icon:
				flag = IsIconBoundary(monitorId, num, out overCount);
				break;
			case SubSequence.NamePlate:
				flag = IsPlateBoundary(monitorId, num, out overCount);
				break;
			case SubSequence.Title:
				flag = IsTitleBoundary(monitorId, num, out overCount);
				break;
			case SubSequence.Prtner:
				flag = IsPartnerBoundary(monitorId, num, out overCount);
				break;
			case SubSequence.Frame:
				flag = IsFrameBoundary(monitorId, num, out overCount);
				break;
			}
			if (flag)
			{
				break;
			}
			SlideScrollCount[monitorId]++;
		}
		SlideScrollCount[monitorId]--;
		SlideScrollUpdate(monitorId, ref info);
	}

	private void SlideScrollLeft(int monitorId, ref CollectionInfo info)
	{
		SetInputLockInfo(monitorId, 100f);
		_isScrollInputLock[monitorId] = true;
		SlideScrollToRight[monitorId] = false;
		for (int i = 1; i <= 10; i++)
		{
			bool flag = false;
			int overCount;
			switch (_subSequence[monitorId])
			{
			case SubSequence.Icon:
				flag = IsIconBoundary(monitorId, i, out overCount);
				break;
			case SubSequence.NamePlate:
				flag = IsPlateBoundary(monitorId, i, out overCount);
				break;
			case SubSequence.Title:
				flag = IsTitleBoundary(monitorId, i, out overCount);
				break;
			case SubSequence.Prtner:
				flag = IsPartnerBoundary(monitorId, i, out overCount);
				break;
			case SubSequence.Frame:
				flag = IsFrameBoundary(monitorId, i, out overCount);
				break;
			}
			if (flag)
			{
				break;
			}
			SlideScrollCount[monitorId]++;
		}
		SlideScrollTime[monitorId] = 50f;
		SlideScrollCount[monitorId]--;
		SlideScrollUpdate(monitorId, ref info);
	}

	private void SlideScrollUpdate(int monitorId, ref CollectionInfo info)
	{
		if (SlideScrollToRight[monitorId])
		{
			info.CollectionListRight(isLongTap: true);
		}
		else
		{
			info.CollectionListLeft(isLongTap: true);
		}
	}

	private void SkipExit(int monitorId)
	{
		SetInputLockInfo(monitorId, 100f);
		_isScrollInputLock[monitorId] = true;
		SlideScrollToRight[monitorId] = false;
		SlideScrollTime[monitorId] = 50f;
		SlideScrollCount[monitorId] = (int)(5 - _currentCollectionType[monitorId] - 1);
		SkipExitUpdate(monitorId);
	}

	private void SkipExitUpdate(int monitorId)
	{
		_currentCollectionType[monitorId]++;
		_monitors[monitorId].ScrollCollectionListLeft(isLongTap: false);
		_monitors[monitorId].SetCollectionGenreIDListButton(_currentCollectionType[monitorId]);
		if (_currentCollectionType[monitorId] == CollectionGenreID.Exit)
		{
			_monitors[monitorId].ChangeCategoryExit();
		}
	}

	protected override void UpdateInput(int monitorId)
	{
		if (!_isEntry[monitorId] || _sequence == Sequence.Init || _sequence == Sequence.Start)
		{
			return;
		}
		if (_isScrollInputLock[monitorId])
		{
			SlideScrollTimer[monitorId] += GameManager.GetGameMSecAdd();
			if (!(SlideScrollTimer[monitorId] > 100f - SlideScrollTime[monitorId]))
			{
				return;
			}
			SlideScrollTimer[monitorId] = 0f;
			if (SlideScrollCount[monitorId] > 0)
			{
				SlideScrollCount[monitorId]--;
				switch (_subSequence[monitorId])
				{
				case SubSequence.Icon:
					SlideScrollUpdate(monitorId, ref _icon[monitorId]);
					break;
				case SubSequence.NamePlate:
					SlideScrollUpdate(monitorId, ref _plate[monitorId]);
					break;
				case SubSequence.Title:
					SlideScrollUpdate(monitorId, ref _title[monitorId]);
					break;
				case SubSequence.Prtner:
					SlideScrollUpdate(monitorId, ref _partner[monitorId]);
					break;
				case SubSequence.Frame:
					SlideScrollUpdate(monitorId, ref _frame[monitorId]);
					break;
				case SubSequence.SelectCollectionType:
				case SubSequence.Information:
					SkipExitUpdate(monitorId);
					break;
				case SubSequence.Exit:
					break;
				}
			}
			else
			{
				SlideScrollTime[monitorId] = 0f;
				_isScrollInputLock[monitorId] = false;
			}
		}
		else
		{
			switch (_subSequence[monitorId])
			{
			case SubSequence.Information:
				InputInformationSequence(monitorId);
				break;
			case SubSequence.SelectCollectionType:
				InputCollectionTypeSequence(monitorId);
				break;
			case SubSequence.Icon:
				InputIconSequence(monitorId);
				break;
			case SubSequence.NamePlate:
				InputNamePlateSequence(monitorId);
				break;
			case SubSequence.Title:
				InputTitleSequence(monitorId);
				break;
			case SubSequence.Prtner:
				InputPartnerSequence(monitorId);
				break;
			case SubSequence.Frame:
				InputFrameSequence(monitorId);
				break;
			case SubSequence.Exit:
			case SubSequence.AimeInfo:
			case SubSequence.Max:
				break;
			}
		}
	}

	private SubSequence ConvertToSubSequence(CollectionGenreID type)
	{
		return type switch
		{
			CollectionGenreID.Title => SubSequence.Title, 
			CollectionGenreID.Icon => SubSequence.Icon, 
			CollectionGenreID.Plate => SubSequence.NamePlate, 
			CollectionGenreID.Partner => SubSequence.Prtner, 
			CollectionGenreID.Frame => SubSequence.Frame, 
			CollectionGenreID.Exit => SubSequence.Exit, 
			_ => SubSequence.SelectCollectionType, 
		};
	}

	public CollectionGenreID ConvertToCollectionGenreID(SubSequence subSequence)
	{
		return subSequence switch
		{
			SubSequence.Icon => CollectionGenreID.Icon, 
			SubSequence.Title => CollectionGenreID.Title, 
			SubSequence.NamePlate => CollectionGenreID.Plate, 
			SubSequence.Prtner => CollectionGenreID.Partner, 
			SubSequence.Frame => CollectionGenreID.Frame, 
			SubSequence.Exit => CollectionGenreID.Exit, 
			_ => CollectionGenreID.End, 
		};
	}

	public int GetCurrentCategoryMax(int monitorId)
	{
		return _subSequence[monitorId] switch
		{
			SubSequence.Title => _title[monitorId].GetIndexNum(), 
			SubSequence.Icon => _icon[monitorId].GetIndexNum(), 
			SubSequence.NamePlate => _plate[monitorId].GetIndexNum(), 
			SubSequence.Prtner => _partner[monitorId].GetIndexNum(), 
			SubSequence.Frame => _frame[monitorId].GetIndexNum(), 
			_ => 0, 
		};
	}

	public string CategoryName(int monitorId, int diff)
	{
		return _subSequence[monitorId] switch
		{
			SubSequence.Title => TitleCategoryName(monitorId, diff), 
			SubSequence.Icon => IconCategoryName(monitorId, diff), 
			SubSequence.NamePlate => PlateCategoryName(monitorId, diff), 
			SubSequence.Prtner => PartnerCategoryName(monitorId, diff), 
			SubSequence.Frame => FrameCategoryName(monitorId, diff), 
			_ => "", 
		};
	}

	private List<CollectionTabData> GetColletionTabData(int monitorId)
	{
		return _subSequence[monitorId] switch
		{
			SubSequence.Title => _title[monitorId].TabData, 
			SubSequence.Icon => _icon[monitorId].TabData, 
			SubSequence.NamePlate => _plate[monitorId].TabData, 
			SubSequence.Prtner => _partner[monitorId].TabData, 
			SubSequence.Frame => _frame[monitorId].TabData, 
			_ => null, 
		};
	}

	public string CurrentElementText(int monitorId)
	{
		return _subSequence[monitorId] switch
		{
			SubSequence.Title => CurrentTitleIndex(monitorId) + 1 + " / " + GetCurrentCategoryMax(monitorId), 
			SubSequence.Icon => CurrentIconIndex(monitorId) + 1 + " / " + GetCurrentCategoryMax(monitorId), 
			SubSequence.NamePlate => CurrentPlateIndex(monitorId) + 1 + " / " + GetCurrentCategoryMax(monitorId), 
			SubSequence.Prtner => CurrentPartnerIndex(monitorId) + 1 + " / " + GetCurrentCategoryMax(monitorId), 
			SubSequence.Frame => CurrentFrameIndex(monitorId) + 1 + " / " + GetCurrentCategoryMax(monitorId), 
			_ => "", 
		};
	}

	public int CurrentCollectionIndex(int monitorId)
	{
		int result = 0;
		switch (_subSequence[monitorId])
		{
		case SubSequence.Title:
			result = CurrentTitleIndex(monitorId);
			break;
		case SubSequence.Icon:
			result = CurrentIconIndex(monitorId);
			break;
		case SubSequence.NamePlate:
			result = CurrentPlateIndex(monitorId);
			break;
		case SubSequence.Prtner:
			result = CurrentPartnerIndex(monitorId);
			break;
		case SubSequence.Frame:
			result = CurrentFrameIndex(monitorId);
			break;
		}
		return result;
	}

	public int CurrentCategoryCollectionNum(int monitorId)
	{
		int result = 0;
		switch (_subSequence[monitorId])
		{
		case SubSequence.Title:
		case SubSequence.Icon:
		case SubSequence.NamePlate:
		case SubSequence.Frame:
		case SubSequence.Prtner:
			result = GetCurrentCategoryMax(monitorId);
			break;
		case SubSequence.SelectCollectionType:
			result = 6;
			break;
		}
		return result;
	}

	public int CurrentCategoryIndex(int monitorId)
	{
		int num = 0;
		return _subSequence[monitorId] switch
		{
			SubSequence.Title => _title[monitorId].CategoryIndex, 
			SubSequence.Icon => _icon[monitorId].CategoryIndex, 
			SubSequence.NamePlate => _plate[monitorId].CategoryIndex, 
			SubSequence.Prtner => _partner[monitorId].CategoryIndex, 
			SubSequence.Frame => _frame[monitorId].CategoryIndex, 
			_ => 0, 
		};
	}

	public List<CollectionTabData> GetTabDatas(int monitorId)
	{
		SubSequence subSequence = _subSequence[monitorId];
		if (subSequence - 1 <= SubSequence.Frame)
		{
			return GetColletionTabData(monitorId);
		}
		return null;
	}

	public void EnqueueMessage(int monitorId, WindowMessageID messageId)
	{
		container.processManager.EnqueueMessage(monitorId, messageId);
	}

	public void SetSubSequence(int monitorId, SubSequence subSequence)
	{
		_subSequence[monitorId] = subSequence;
	}

	private void TimeUp(int monitorId)
	{
		if (!_subSequence.Any((SubSequence sub) => sub == SubSequence.Max))
		{
			EnqueueMessage(monitorId, WindowMessageID.PlayPreparationWait);
		}
		_subSequence[monitorId] = SubSequence.Max;
		container.processManager.SetVisibleTimer(monitorId, isVisible: false);
		_monitors[monitorId].SetVisibleBlur(isActive: true);
		int num = 0;
		for (int i = 0; i < _subSequence.Length; i++)
		{
			if (_subSequence[i] == SubSequence.Max)
			{
				num++;
			}
		}
		if (num == 2)
		{
			GotoNextProcess();
		}
	}

	private void GotoNextProcess()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			_subSequence[i] = SubSequence.Max;
		}
		container.processManager.ForceTimeUp();
		SaveCollectionData();
		_sequence = Sequence.Released;
	}

	private void SaveCollectionData()
	{
		for (int i = 0; i < _monitors.Length; i++)
		{
			if (_isEntry[i])
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				userData.FavoriteTitleList.Clear();
				userData.FavoriteIconList.Clear();
				userData.FavoritePlateList.Clear();
				userData.FavoriteFrameList.Clear();
			}
		}
	}

	private void CreateDummyData()
	{
	}

	private void InputFrameSequence(int monitorId)
	{
		if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
		{
			bool inputLongPush = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L);
			bool flag = _frame[monitorId].CollectionListLeft(inputLongPush);
			_frame[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID);
			SetInputLockInfo(monitorId, flag ? 200 : 100);
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
		{
			bool inputLongPush2 = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L);
			bool flag2 = _frame[monitorId].CollectionListRight(inputLongPush2);
			_frame[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID);
			SetInputLockInfo(monitorId, flag2 ? 200 : 100);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E2))
		{
			_frame[monitorId].ShiftCategoryLeft();
			_frame[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E8))
		{
			_frame[monitorId].ShiftCategoryRight();
			_frame[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
		{
			_subSequence[monitorId] = SubSequence.SelectCollectionType;
			_monitors[monitorId].ChangeSubSequence(SubSequence.SelectCollectionType);
			_frame[monitorId].RefreshIndex();
			SetInputLockInfo(monitorId, 550f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			CollectionData currentCenterCollectionData = _frame[monitorId].GetCurrentCenterCollectionData();
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID != currentCenterCollectionData.GetID() && currentCenterCollectionData.IsHave)
			{
				ChangeEquipmentFrame(monitorId, currentCenterCollectionData.GetID());
				_frame[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID);
				SetInputLockInfo(monitorId, 550f);
			}
		}
		else if (InputManager.SlideAreaLr(monitorId))
		{
			SlideScrollRight(monitorId, ref _frame[monitorId]);
		}
		else if (InputManager.SlideAreaRl(monitorId))
		{
			SlideScrollLeft(monitorId, ref _frame[monitorId]);
		}
		else
		{
			_monitors[monitorId].SetScrollCard(isVisible: false);
		}
	}

	private void CreateFrameData(int monitorId)
	{
		UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
		List<CollectionTabData> list = new List<CollectionTabData>();
		List<int> list2 = userData.FrameList.Select((UserItem i) => i.itemId).ToList();
		List<int> newFrameList = userData.NewFrameList;
		_ = userData.FavoriteFrameList;
		SortedDictionary<int, List<int>> sortedDictionary = new SortedDictionary<int, List<int>>();
		if (0 < newFrameList.Count)
		{
			CollectionGenreData collectionGenre = Singleton<DataManager>.Instance.GetCollectionGenre(2);
			List<FrameData> list3 = new List<FrameData>();
			List<int> list4 = new List<int>();
			int num = ((10 < newFrameList.Count) ? 10 : newFrameList.Count);
			for (int j = 0; j < num; j++)
			{
				FrameData frame = Singleton<DataManager>.Instance.GetFrame(newFrameList[j]);
				list3.Add(frame);
				list4.Add(frame.GetID());
			}
			sortedDictionary.Add(collectionGenre.GetID(), list4);
		}
		foreach (KeyValuePair<int, FrameData> item3 in from x in Singleton<DataManager>.Instance.GetFrames()
			orderby x.Value.priority
			select x)
		{
			int id2 = item3.Value.genre.id;
			bool num2 = list2.Contains(item3.Value.GetID());
			bool flag = item3.Value.dispCond == ItemDispKind.None && Singleton<EventManager>.Instance.IsOpenEvent(item3.Value.eventName.id);
			if (num2 || flag)
			{
				if (sortedDictionary.ContainsKey(id2))
				{
					sortedDictionary[id2].Add(item3.Value.GetID());
					continue;
				}
				List<int> value = new List<int> { item3.Value.GetID() };
				sortedDictionary.Add(id2, value);
			}
		}
		List<CollectionData> list5 = new List<CollectionData>();
		FrameData frameData = new FrameData();
		List<ReadOnlyCollection<int>> list6 = new List<ReadOnlyCollection<int>>();
		foreach (KeyValuePair<int, CollectionGenreData> item4 in from x in Singleton<DataManager>.Instance.GetCollectionGenres()
			orderby x.Value.priority
			select x)
		{
			if (sortedDictionary.ContainsKey(item4.Value.GetID()))
			{
				ReadOnlyCollection<int> item = new ReadOnlyCollection<int>(sortedDictionary[item4.Value.GetID()]);
				list6.Add(item);
				Sprite sprite = Resources.Load<Sprite>(SelectorTab.CategoryTabImagePath + item4.Value.FileName);
				list.Add(new CollectionTabData(Utility.ConvertColor(item4.Value.Color), sprite, item4.Value.genreNameTwoLine));
			}
		}
		for (int k = 0; k < list6.Count; k++)
		{
			for (int l = 0; l < list6[k].Count; l++)
			{
				int id = list6[k][l];
				if (!list5.Any((CollectionData c) => c.GetID() == id))
				{
					frameData = Singleton<DataManager>.Instance.GetFrame(id);
					bool flag2 = list2.Contains(frameData.GetID());
					bool flag3 = frameData.dispCond == ItemDispKind.None;
					if (flag2 || flag3)
					{
						bool isNew = newFrameList.Contains(frameData.GetID());
						CollectionData item2 = new CollectionData
						{
							NetOpenName = frameData.netOpenName,
							DataName = frameData.dataName,
							Disable = frameData.disable,
							Genre = frameData.genre,
							IsDefault = frameData.isDefault,
							IsHave = flag2,
							IsNew = isNew,
							IsDisp = flag3,
							NormText = frameData.normText,
							ReleaseTagName = frameData.releaseTagName,
							Priority = frameData.priority,
							ID = frameData.GetID(),
							NameStr = frameData.name.str
						};
						list5.Add(item2);
					}
				}
			}
		}
		if (list5.Count > 0 && userData.Detail.EquipFrameID == 0)
		{
			int setting_id = 1;
			userData.Detail.EquipFrameID = setting_id;
			int num3 = list5.FindIndex((CollectionData m) => m.ID == setting_id);
			if (num3 >= 0)
			{
				list5[num3].IsDefault = true;
				list5[num3].IsHave = true;
			}
		}
		_frame[monitorId].Initialize(list6, list5, list, this, _monitors[monitorId], newFrameList.Any(), userData.Detail.EquipFrameID);
	}

	public bool IsHaveNewFrame(int monitorId)
	{
		return _frame[monitorId].IsHaveNewIcon();
	}

	public CollectionData GetFrameById(int monitorId, int targetId)
	{
		return _frame[monitorId].GetCollectionById(targetId);
	}

	public CollectionData GetFrame(int monitorId, int diffIndex)
	{
		return _frame[monitorId].GetCollection(diffIndex);
	}

	public bool IsFrameBoundary(int monitorId, int diffIndex, out int overCount)
	{
		return _frame[monitorId].IsCollectionBoundary(diffIndex, out overCount);
	}

	public int GetAllFrameNum(int monitorId)
	{
		return _frame[monitorId].GetAllCollectionNum();
	}

	public int GetCurrentFrameIndex(int monitorId)
	{
		return _frame[monitorId].GetCurrentIndex();
	}

	private string FrameCategoryName(int monitorId, int diff)
	{
		int num = _frame[monitorId].CategoryIndex + diff;
		if (num < 0)
		{
			num = _frame[monitorId].TabData.Count - 1;
		}
		else if (_frame[monitorId].TabData.Count <= num)
		{
			num = 0;
		}
		return _frame[monitorId].CategoryName(num);
	}

	private void ChangeEquipmentFrame(int monitorId, int collectionId)
	{
		_monitors[monitorId].PushDesitionButton(_subSequence[monitorId]);
		Texture2D frameTexture2D = container.assetManager.GetFrameTexture2D(collectionId);
		bool flag = false;
		Texture2D texture2D = null;
		Texture2D texture2D2 = null;
		FrameData frame = Singleton<DataManager>.Instance.GetFrame(collectionId);
		if (frame != null && frame.isEffect)
		{
			flag = true;
			texture2D = container.assetManager.GetFrameTexture2D(collectionId);
			texture2D2 = container.assetManager.GetFrameMaskTexture2D(collectionId);
		}
		else
		{
			flag = false;
			texture2D = null;
			texture2D2 = null;
		}
		container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30009, monitorId, frameTexture2D, flag, texture2D, texture2D2));
		Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipFrameID = collectionId;
	}

	private void InputIconSequence(int monitorId)
	{
		if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
		{
			bool inputLongPush = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L);
			bool flag = _icon[monitorId].CollectionListLeft(inputLongPush);
			_icon[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID);
			SetInputLockInfo(monitorId, flag ? 200 : 100);
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
		{
			bool inputLongPush2 = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L);
			bool flag2 = _icon[monitorId].CollectionListRight(inputLongPush2);
			_icon[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID);
			SetInputLockInfo(monitorId, flag2 ? 200 : 100);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E2))
		{
			_icon[monitorId].ShiftCategoryLeft();
			_icon[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E8))
		{
			_icon[monitorId].ShiftCategoryRight();
			_icon[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
		{
			_subSequence[monitorId] = SubSequence.SelectCollectionType;
			_monitors[monitorId].ChangeSubSequence(SubSequence.SelectCollectionType);
			_icon[monitorId].RefreshIndex();
			SetInputLockInfo(monitorId, 550f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			CollectionData currentCenterCollectionData = _icon[monitorId].GetCurrentCenterCollectionData();
			int equipIconID = Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID;
			int iD = currentCenterCollectionData.GetID();
			if (equipIconID != iD && currentCenterCollectionData.IsHave)
			{
				ChangeEquipmentIcon(monitorId, iD);
				_icon[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID);
			}
		}
		else if (InputManager.SlideAreaLr(monitorId))
		{
			SlideScrollRight(monitorId, ref _icon[monitorId]);
		}
		else if (InputManager.SlideAreaRl(monitorId))
		{
			SlideScrollLeft(monitorId, ref _icon[monitorId]);
		}
		else
		{
			_monitors[monitorId].SetScrollCard(isVisible: false);
		}
	}

	private void CreateIconData(int monitorId)
	{
		UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
		List<CollectionTabData> list = new List<CollectionTabData>();
		List<int> list2 = userData.IconList.Select((UserItem i) => i.itemId).ToList();
		List<int> newIconList = userData.NewIconList;
		_ = userData.FavoriteIconList;
		SortedDictionary<int, List<int>> sortedDictionary = new SortedDictionary<int, List<int>>();
		if (0 < newIconList.Count)
		{
			CollectionGenreData collectionGenre = Singleton<DataManager>.Instance.GetCollectionGenre(2);
			List<IconData> list3 = new List<IconData>();
			List<int> list4 = new List<int>();
			int num = ((10 < newIconList.Count) ? 10 : newIconList.Count);
			for (int j = 0; j < num; j++)
			{
				IconData icon = Singleton<DataManager>.Instance.GetIcon(newIconList[j]);
				list3.Add(icon);
				list4.Add(icon.GetID());
			}
			sortedDictionary.Add(collectionGenre.GetID(), list4);
		}
		foreach (KeyValuePair<int, IconData> item3 in from x in Singleton<DataManager>.Instance.GetIcons()
			orderby x.Value.priority
			select x)
		{
			int id2 = item3.Value.genre.id;
			bool num2 = list2.Contains(item3.Value.GetID());
			bool flag = item3.Value.dispCond == ItemDispKind.None && Singleton<EventManager>.Instance.IsOpenEvent(item3.Value.eventName.id);
			if (num2 || flag)
			{
				if (sortedDictionary.ContainsKey(id2))
				{
					sortedDictionary[id2].Add(item3.Value.GetID());
					continue;
				}
				List<int> value = new List<int> { item3.Value.GetID() };
				sortedDictionary.Add(id2, value);
			}
		}
		List<CollectionData> list5 = new List<CollectionData>();
		IconData iconData = new IconData();
		List<ReadOnlyCollection<int>> list6 = new List<ReadOnlyCollection<int>>();
		foreach (KeyValuePair<int, CollectionGenreData> item4 in from x in Singleton<DataManager>.Instance.GetCollectionGenres()
			orderby x.Value.priority
			select x)
		{
			if (sortedDictionary.ContainsKey(item4.Value.GetID()))
			{
				ReadOnlyCollection<int> item = new ReadOnlyCollection<int>(sortedDictionary[item4.Value.GetID()]);
				list6.Add(item);
				Sprite sprite = Resources.Load<Sprite>(SelectorTab.CategoryTabImagePath + item4.Value.FileName);
				list.Add(new CollectionTabData(Utility.ConvertColor(item4.Value.Color), sprite, item4.Value.genreNameTwoLine));
			}
		}
		for (int k = 0; k < list6.Count; k++)
		{
			for (int l = 0; l < list6[k].Count; l++)
			{
				int id = list6[k][l];
				if (!list5.Any((CollectionData c) => c.GetID() == id))
				{
					iconData = Singleton<DataManager>.Instance.GetIcon(id);
					bool flag2 = list2.Contains(iconData.GetID());
					bool flag3 = iconData.dispCond == ItemDispKind.None;
					if (flag2 || flag3)
					{
						bool isNew = newIconList.Contains(iconData.GetID());
						CollectionData item2 = new CollectionData
						{
							NetOpenName = iconData.netOpenName,
							DataName = iconData.dataName,
							Disable = iconData.disable,
							Genre = iconData.genre,
							IsDefault = iconData.isDefault,
							IsHave = flag2,
							IsNew = isNew,
							IsDisp = flag3,
							NormText = iconData.normText,
							ReleaseTagName = iconData.releaseTagName,
							Priority = iconData.priority,
							ID = iconData.GetID(),
							NameStr = iconData.name.str
						};
						list5.Add(item2);
					}
				}
			}
		}
		if (list5.Count > 0 && userData.Detail.EquipIconID == 0)
		{
			int setting_id = 1;
			userData.Detail.EquipIconID = setting_id;
			int num3 = list5.FindIndex((CollectionData m) => m.ID == setting_id);
			if (num3 >= 0)
			{
				list5[num3].IsDefault = true;
				list5[num3].IsHave = true;
			}
		}
		_icon[monitorId].Initialize(list6, list5, list, this, _monitors[monitorId], newIconList.Any(), userData.Detail.EquipIconID);
	}

	public bool IsHaveNewIcon(int monitorId)
	{
		return _icon[monitorId].IsHaveNewIcon();
	}

	public CollectionData GetIconById(int monitorId, int targetId)
	{
		return _icon[monitorId].GetCollectionById(targetId);
	}

	public CollectionData GetIcon(int monitorId, int diffIndex)
	{
		return _icon[monitorId].GetCollection(diffIndex);
	}

	public bool IsIconBoundary(int monitorId, int diffIndex, out int overCount)
	{
		return _icon[monitorId].IsCollectionBoundary(diffIndex, out overCount);
	}

	public int GetAllIconNum(int monitorId)
	{
		return _icon[monitorId].GetAllCollectionNum();
	}

	public int GetCurrentIconIndex(int monitorId)
	{
		return _icon[monitorId].GetCurrentIndex();
	}

	private string IconCategoryName(int monitorId, int diff)
	{
		int num = _icon[monitorId].CategoryIndex + diff;
		if (num < 0)
		{
			num = _icon[monitorId].TabData.Count - 1;
		}
		else if (_icon[monitorId].TabData.Count <= num)
		{
			num = 0;
		}
		return _icon[monitorId].CategoryName(num);
	}

	private void ChangeEquipmentIcon(int monitorId, int collectionId)
	{
		_monitors[monitorId].PushDesitionButton(_subSequence[monitorId]);
		Texture2D iconTexture2D = container.assetManager.GetIconTexture2D(monitorId, collectionId);
		container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30002, monitorId, iconTexture2D));
		Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipIconID = collectionId;
	}

	private void InputNamePlateSequence(int monitorId)
	{
		if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
		{
			bool inputLongPush = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L);
			bool flag = _plate[monitorId].CollectionListLeft(inputLongPush);
			_plate[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID);
			SetInputLockInfo(monitorId, flag ? 200 : 100);
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
		{
			bool inputLongPush2 = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L);
			bool flag2 = _plate[monitorId].CollectionListRight(inputLongPush2);
			_plate[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID);
			SetInputLockInfo(monitorId, flag2 ? 200 : 100);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E2))
		{
			_plate[monitorId].ShiftCategoryLeft();
			_plate[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E8))
		{
			_plate[monitorId].ShiftCategoryRight();
			_plate[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
		{
			_subSequence[monitorId] = SubSequence.SelectCollectionType;
			_monitors[monitorId].ChangeSubSequence(SubSequence.SelectCollectionType);
			_plate[monitorId].RefreshIndex();
			SetInputLockInfo(monitorId, 550f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			CollectionData currentCenterCollectionData = _plate[monitorId].GetCurrentCenterCollectionData();
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID != currentCenterCollectionData.GetID() && currentCenterCollectionData.IsHave)
			{
				ChangeEquipmentPlate(monitorId, currentCenterCollectionData.GetID());
				_plate[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID);
				SetInputLockInfo(monitorId, 550f);
			}
		}
		else if (InputManager.SlideAreaLr(monitorId))
		{
			SlideScrollRight(monitorId, ref _plate[monitorId]);
		}
		else if (InputManager.SlideAreaRl(monitorId))
		{
			SlideScrollLeft(monitorId, ref _plate[monitorId]);
		}
		else
		{
			_monitors[monitorId].SetScrollCard(isVisible: false);
		}
	}

	private void CreatePlateData(int monitorId)
	{
		UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
		List<CollectionTabData> list = new List<CollectionTabData>();
		List<int> list2 = userData.PlateList.Select((UserItem i) => i.itemId).ToList();
		List<int> newPlateList = userData.NewPlateList;
		_ = userData.FavoritePlateList;
		SortedDictionary<int, List<int>> sortedDictionary = new SortedDictionary<int, List<int>>();
		if (0 < newPlateList.Count)
		{
			CollectionGenreData collectionGenre = Singleton<DataManager>.Instance.GetCollectionGenre(2);
			List<PlateData> list3 = new List<PlateData>();
			List<int> list4 = new List<int>();
			int num = ((10 < newPlateList.Count) ? 10 : newPlateList.Count);
			for (int j = 0; j < num; j++)
			{
				PlateData plate = Singleton<DataManager>.Instance.GetPlate(newPlateList[j]);
				list3.Add(plate);
				list4.Add(plate.GetID());
			}
			sortedDictionary.Add(collectionGenre.GetID(), list4);
		}
		foreach (KeyValuePair<int, PlateData> item3 in from x in Singleton<DataManager>.Instance.GetPlates()
			orderby x.Value.priority
			select x)
		{
			int id2 = item3.Value.genre.id;
			bool num2 = list2.Contains(item3.Value.GetID());
			bool flag = item3.Value.dispCond == ItemDispKind.None && Singleton<EventManager>.Instance.IsOpenEvent(item3.Value.eventName.id);
			if (num2 || flag)
			{
				if (sortedDictionary.ContainsKey(id2))
				{
					sortedDictionary[id2].Add(item3.Value.GetID());
					continue;
				}
				List<int> value = new List<int> { item3.Value.GetID() };
				sortedDictionary.Add(id2, value);
			}
		}
		List<CollectionData> list5 = new List<CollectionData>();
		PlateData plateData = new PlateData();
		List<ReadOnlyCollection<int>> list6 = new List<ReadOnlyCollection<int>>();
		foreach (KeyValuePair<int, CollectionGenreData> item4 in from x in Singleton<DataManager>.Instance.GetCollectionGenres()
			orderby x.Value.priority
			select x)
		{
			if (sortedDictionary.ContainsKey(item4.Value.GetID()))
			{
				ReadOnlyCollection<int> item = new ReadOnlyCollection<int>(sortedDictionary[item4.Value.GetID()]);
				list6.Add(item);
				Sprite sprite = Resources.Load<Sprite>(SelectorTab.CategoryTabImagePath + item4.Value.FileName);
				list.Add(new CollectionTabData(Utility.ConvertColor(item4.Value.Color), sprite, item4.Value.genreNameTwoLine));
			}
		}
		for (int k = 0; k < list6.Count; k++)
		{
			for (int l = 0; l < list6[k].Count; l++)
			{
				int id = list6[k][l];
				if (!list5.Any((CollectionData c) => c.GetID() == id))
				{
					plateData = Singleton<DataManager>.Instance.GetPlate(id);
					bool flag2 = list2.Contains(plateData.GetID());
					bool flag3 = plateData.dispCond == ItemDispKind.None;
					if (flag2 || flag3)
					{
						bool isNew = newPlateList.Contains(plateData.GetID());
						CollectionData item2 = new CollectionData
						{
							NetOpenName = plateData.netOpenName,
							DataName = plateData.dataName,
							Disable = plateData.disable,
							Genre = plateData.genre,
							IsDefault = plateData.isDefault,
							IsHave = flag2,
							IsNew = isNew,
							IsDisp = flag3,
							NormText = plateData.normText,
							ReleaseTagName = plateData.releaseTagName,
							Priority = plateData.priority,
							ID = plateData.GetID(),
							NameStr = plateData.name.str
						};
						list5.Add(item2);
					}
				}
			}
		}
		if (list5.Count > 0 && userData.Detail.EquipPlateID == 0)
		{
			int setting_id = 1;
			userData.Detail.EquipPlateID = setting_id;
			int num3 = list5.FindIndex((CollectionData m) => m.ID == setting_id);
			if (num3 >= 0)
			{
				list5[num3].IsDefault = true;
				list5[num3].IsHave = true;
			}
		}
		_plate[monitorId].Initialize(list6, list5, list, this, _monitors[monitorId], newPlateList.Any(), userData.Detail.EquipPlateID);
	}

	public bool IsHaveNewNamePlate(int monitorId)
	{
		return _plate[monitorId].IsHaveNewIcon();
	}

	public CollectionData GetPlateById(int monitorId, int targetId)
	{
		return _plate[monitorId].GetCollectionById(targetId);
	}

	public CollectionData GetPlate(int monitorId, int diffIndex)
	{
		return _plate[monitorId].GetCollection(diffIndex);
	}

	public bool IsPlateBoundary(int monitorId, int diffIndex, out int overCount)
	{
		return _plate[monitorId].IsCollectionBoundary(diffIndex, out overCount);
	}

	public int GetAllPlateNum(int monitorId)
	{
		return _plate[monitorId].GetAllCollectionNum();
	}

	public int GetCurrentPlateIndex(int monitorId)
	{
		return _plate[monitorId].GetCurrentIndex();
	}

	private string PlateCategoryName(int monitorId, int diff)
	{
		int num = _plate[monitorId].CategoryIndex + diff;
		if (num < 0)
		{
			num = _plate[monitorId].TabData.Count - 1;
		}
		else if (_plate[monitorId].TabData.Count <= num)
		{
			num = 0;
		}
		return _plate[monitorId].CategoryName(num);
	}

	private void ChangeEquipmentPlate(int monitorId, int collectionId)
	{
		_monitors[monitorId].PushDesitionButton(_subSequence[monitorId]);
		Texture2D plateTexture2D = container.assetManager.GetPlateTexture2D(collectionId);
		container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30004, monitorId, plateTexture2D));
		Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPlateID = collectionId;
	}

	private void InputPartnerSequence(int monitorId)
	{
		if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
		{
			bool inputLongPush = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L);
			bool flag = _partner[monitorId].CollectionListLeft(inputLongPush);
			_partner[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID);
			SetInputLockInfo(monitorId, flag ? 200 : 100);
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
		{
			bool inputLongPush2 = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L);
			bool flag2 = _partner[monitorId].CollectionListRight(inputLongPush2);
			_partner[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID);
			SetInputLockInfo(monitorId, flag2 ? 200 : 100);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E2))
		{
			_partner[monitorId].ShiftCategoryLeft();
			_partner[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E8))
		{
			_partner[monitorId].ShiftCategoryRight();
			_partner[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
		{
			_subSequence[monitorId] = SubSequence.SelectCollectionType;
			_monitors[monitorId].ChangeSubSequence(SubSequence.SelectCollectionType);
			_partner[monitorId].RefreshIndex();
			SetInputLockInfo(monitorId, 550f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			CollectionData currentCenterCollectionData = _partner[monitorId].GetCurrentCenterCollectionData();
			int equipPartnerID = Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID;
			int iD = currentCenterCollectionData.GetID();
			if (equipPartnerID != iD && currentCenterCollectionData.IsHave)
			{
				SoundManager.SetPartnerVoiceCue(monitorId, currentCenterCollectionData.GetID());
				SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000251, monitorId);
				ChangeEquipmentPartner(monitorId, iD);
				_partner[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID);
			}
		}
		else if (InputManager.SlideAreaLr(monitorId))
		{
			SlideScrollRight(monitorId, ref _partner[monitorId]);
		}
		else if (InputManager.SlideAreaRl(monitorId))
		{
			SlideScrollLeft(monitorId, ref _partner[monitorId]);
		}
		else
		{
			_monitors[monitorId].SetScrollCard(isVisible: false);
		}
	}

	private void CreatePartnerData(int monitorId)
	{
		UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
		List<CollectionTabData> list = new List<CollectionTabData>();
		List<int> list2 = userData.PartnerList.Select((UserItem p) => p.itemId).ToList();
		List<int> newPartnerList = userData.NewPartnerList;
		List<CollectionData> list3 = new List<CollectionData>();
		List<ReadOnlyCollection<int>> list4 = new List<ReadOnlyCollection<int>>();
		SortedDictionary<int, List<int>> sortedDictionary = new SortedDictionary<int, List<int>>();
		if (0 < newPartnerList.Count)
		{
			CollectionGenreData collectionGenre = Singleton<DataManager>.Instance.GetCollectionGenre(2);
			List<PartnerData> list5 = new List<PartnerData>();
			List<int> list6 = new List<int>();
			int num = ((10 < newPartnerList.Count) ? 10 : newPartnerList.Count);
			for (int i = 0; i < num; i++)
			{
				PartnerData partner = Singleton<DataManager>.Instance.GetPartner(newPartnerList[i]);
				list5.Add(partner);
				list6.Add(partner.GetID());
			}
			sortedDictionary.Add(collectionGenre.GetID(), list6);
		}
		foreach (KeyValuePair<int, PartnerData> item3 in from x in Singleton<DataManager>.Instance.GetPartners()
			orderby x.Value.priority
			select x)
		{
			int id2 = item3.Value.genre.id;
			bool num2 = list2.Contains(item3.Value.GetID());
			bool flag = item3.Value.dispCond == ItemDispKind.None && Singleton<EventManager>.Instance.IsOpenEvent(item3.Value.eventName.id);
			if (num2 || flag)
			{
				if (sortedDictionary.ContainsKey(id2))
				{
					sortedDictionary[id2].Add(item3.Value.GetID());
					continue;
				}
				List<int> value = new List<int> { item3.Value.GetID() };
				sortedDictionary.Add(id2, value);
			}
		}
		foreach (KeyValuePair<int, CollectionGenreData> item4 in from x in Singleton<DataManager>.Instance.GetCollectionGenres()
			orderby x.Value.priority
			select x)
		{
			if (sortedDictionary.ContainsKey(item4.Value.GetID()))
			{
				ReadOnlyCollection<int> item = new ReadOnlyCollection<int>(sortedDictionary[item4.Value.GetID()]);
				list4.Add(item);
				Sprite sprite = Resources.Load<Sprite>(SelectorTab.CategoryTabImagePath + item4.Value.FileName);
				list.Add(new CollectionTabData(Utility.ConvertColor(item4.Value.Color), sprite, item4.Value.genreNameTwoLine));
			}
		}
		for (int j = 0; j < list4.Count; j++)
		{
			for (int k = 0; k < list4[j].Count; k++)
			{
				int id = list4[j][k];
				if (!list3.Any((CollectionData c) => c.GetID() == id))
				{
					PartnerData partner2 = Singleton<DataManager>.Instance.GetPartner(id);
					bool flag2 = list2.Contains(partner2.GetID());
					bool flag3 = partner2.dispCond == ItemDispKind.None;
					if (flag2 || flag3)
					{
						bool isNew = newPartnerList.Contains(partner2.GetID());
						CollectionData item2 = new CollectionData
						{
							NetOpenName = partner2.netOpenName,
							DataName = partner2.dataName,
							Disable = partner2.disable,
							Genre = partner2.genre,
							IsDefault = partner2.isDefault,
							IsHave = flag2,
							IsNew = isNew,
							IsDisp = flag3,
							NormText = partner2.normText,
							ReleaseTagName = partner2.releaseTagName,
							Priority = partner2.priority,
							ID = partner2.GetID(),
							NameStr = partner2.name.str
						};
						list3.Add(item2);
					}
				}
			}
		}
		if (list3.Count > 0 && userData.Detail.EquipPartnerID == 0)
		{
			int setting_id = 17;
			userData.Detail.EquipPartnerID = setting_id;
			int num3 = list3.FindIndex((CollectionData m) => m.ID == setting_id);
			if (num3 >= 0)
			{
				list3[num3].IsDefault = true;
				list3[num3].IsHave = true;
			}
		}
		_partner[monitorId].Initialize(list4, list3, list, this, _monitors[monitorId], newPartnerList.Any(), userData.Detail.EquipPartnerID);
	}

	public CollectionData GetPartnerById(int monitorId, int targetId)
	{
		return _partner[monitorId].GetCollectionById(targetId);
	}

	public CollectionData GetPartner(int monitorId, int diffIndex)
	{
		return _partner[monitorId].GetCollection(diffIndex);
	}

	public int GetAllPartnerNum(int monitorId)
	{
		return _partner[monitorId].GetAllCollectionNum();
	}

	public bool IsPartnerBoundary(int monitorId, int diffIndex, out int overCount)
	{
		return _partner[monitorId].IsCollectionBoundary(diffIndex, out overCount);
	}

	private string PartnerCategoryName(int monitorId, int diff)
	{
		int num = _partner[monitorId].CategoryIndex + diff;
		if (num < 0)
		{
			num = _partner[monitorId].TabData.Count - 1;
		}
		else if (_partner[monitorId].TabData.Count <= num)
		{
			num = 0;
		}
		return _partner[monitorId].CategoryName(num);
	}

	public bool IsHaveNewPartner(int monitorId)
	{
		return _partner[monitorId].IsHaveNewIcon();
	}

	private void ChangeEquipmentPartner(int monitorId, int collectionId)
	{
		_monitors[monitorId].PushDesitionButton(_subSequence[monitorId]);
		Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipPartnerID = collectionId;
	}

	private void InputTitleSequence(int monitorId)
	{
		if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
		{
			bool inputLongPush = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L);
			bool flag = _title[monitorId].CollectionListLeft(inputLongPush);
			_title[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID);
			SetInputLockInfo(monitorId, flag ? 200 : 100);
		}
		else if (InputManager.GetInputDown(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
		{
			bool inputLongPush2 = InputManager.GetInputLongPush(monitorId, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L);
			bool flag2 = _title[monitorId].CollectionListRight(inputLongPush2);
			_title[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID);
			SetInputLockInfo(monitorId, flag2 ? 200 : 100);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E2))
		{
			_title[monitorId].ShiftCategoryLeft();
			_title[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetTouchPanelAreaDown(monitorId, InputManager.TouchPanelArea.E8))
		{
			_title[monitorId].ShiftCategoryRight();
			_title[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID);
			SetInputLockInfo(monitorId, 200f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button05))
		{
			_subSequence[monitorId] = SubSequence.SelectCollectionType;
			_title[monitorId].RefreshIndex();
			_monitors[monitorId].ChangeSubSequence(SubSequence.SelectCollectionType);
			SetInputLockInfo(monitorId, 550f);
		}
		else if (InputManager.GetButtonDown(monitorId, InputManager.ButtonSetting.Button04))
		{
			CollectionData currentCenterCollectionData = _title[monitorId].GetCurrentCenterCollectionData();
			int equipTitleID = Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID;
			int iD = currentCenterCollectionData.GetID();
			if (equipTitleID != iD && currentCenterCollectionData.IsHave)
			{
				ChangeEquipmentTitle(monitorId, currentCenterCollectionData.NameStr, currentCenterCollectionData.TrophyRareType.ToString(), iD);
				_title[monitorId].CheckSetButton(Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID);
			}
		}
		else if (InputManager.SlideAreaLr(monitorId))
		{
			SlideScrollRight(monitorId, ref _title[monitorId]);
		}
		else if (InputManager.SlideAreaRl(monitorId))
		{
			SlideScrollLeft(monitorId, ref _title[monitorId]);
		}
		else
		{
			_monitors[monitorId].SetScrollCard(isVisible: false);
		}
	}

	private void CreateTitleData(int monitorId)
	{
		UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorId);
		List<CollectionTabData> list = new List<CollectionTabData>();
		List<int> list2 = userData.TitleList.Select((UserItem i) => i.itemId).ToList();
		List<int> newTitleList = userData.NewTitleList;
		_ = userData.FavoriteTitleList;
		SortedDictionary<int, List<int>> sortedDictionary = new SortedDictionary<int, List<int>>();
		if (0 < newTitleList.Count)
		{
			CollectionGenreData collectionGenre = Singleton<DataManager>.Instance.GetCollectionGenre(2);
			List<TitleData> list3 = new List<TitleData>();
			List<int> list4 = new List<int>();
			int num = ((10 < newTitleList.Count) ? 10 : newTitleList.Count);
			for (int j = 0; j < num; j++)
			{
				TitleData title = Singleton<DataManager>.Instance.GetTitle(newTitleList[j]);
				list3.Add(title);
				list4.Add(title.GetID());
			}
			sortedDictionary.Add(collectionGenre.GetID(), list4);
		}
		foreach (KeyValuePair<int, TitleData> item3 in from x in Singleton<DataManager>.Instance.GetTitles()
			orderby x.Value.priority
			select x)
		{
			int id2 = item3.Value.genre.id;
			bool num2 = list2.Contains(item3.Value.GetID());
			bool flag = item3.Value.dispCond == ItemDispKind.None && Singleton<EventManager>.Instance.IsOpenEvent(item3.Value.eventName.id);
			if (num2 || flag)
			{
				if (sortedDictionary.ContainsKey(id2))
				{
					sortedDictionary[id2].Add(item3.Value.GetID());
					continue;
				}
				List<int> value = new List<int> { item3.Value.GetID() };
				sortedDictionary.Add(id2, value);
			}
		}
		List<CollectionData> list5 = new List<CollectionData>();
		TitleData titleData = new TitleData();
		List<ReadOnlyCollection<int>> list6 = new List<ReadOnlyCollection<int>>();
		foreach (KeyValuePair<int, CollectionGenreData> item4 in from x in Singleton<DataManager>.Instance.GetCollectionGenres()
			orderby x.Value.priority
			select x)
		{
			if (sortedDictionary.ContainsKey(item4.Value.GetID()))
			{
				ReadOnlyCollection<int> item = new ReadOnlyCollection<int>(sortedDictionary[item4.Value.GetID()]);
				list6.Add(item);
				Sprite sprite = Resources.Load<Sprite>(SelectorTab.CategoryTabImagePath + item4.Value.FileName);
				list.Add(new CollectionTabData(Utility.ConvertColor(item4.Value.Color), sprite, item4.Value.genreNameTwoLine));
			}
		}
		for (int k = 0; k < list6.Count; k++)
		{
			for (int l = 0; l < list6[k].Count; l++)
			{
				int id = list6[k][l];
				if (!list5.Any((CollectionData c) => c.GetID() == id))
				{
					titleData = Singleton<DataManager>.Instance.GetTitle(id);
					bool flag2 = list2.Contains(titleData.GetID());
					bool flag3 = titleData.dispCond == ItemDispKind.None;
					if (flag2 || flag3)
					{
						bool isNew = newTitleList.Contains(titleData.GetID());
						CollectionData item2 = new CollectionData
						{
							NetOpenName = titleData.netOpenName,
							DataName = titleData.dataName,
							Disable = titleData.disable,
							Genre = titleData.genre,
							IsDefault = titleData.isDefault,
							IsHave = flag2,
							IsNew = isNew,
							IsDisp = flag3,
							NormText = titleData.normText,
							TrophyRareType = titleData.rareType,
							ReleaseTagName = titleData.releaseTagName,
							Priority = titleData.priority,
							ID = titleData.GetID(),
							NameStr = titleData.name.str
						};
						list5.Add(item2);
					}
				}
			}
		}
		if (list5.Count > 0 && userData.Detail.EquipTitleID == 0)
		{
			int setting_id = 1;
			userData.Detail.EquipTitleID = setting_id;
			int num3 = list5.FindIndex((CollectionData m) => m.ID == setting_id);
			if (num3 >= 0)
			{
				list5[num3].IsDefault = true;
				list5[num3].IsHave = true;
			}
		}
		_title[monitorId].Initialize(list6, list5, list, this, _monitors[monitorId], newTitleList.Any(), userData.Detail.EquipTitleID);
	}

	public bool IsHaveNewTitle(int monitorId)
	{
		return _title[monitorId].IsHaveNewIcon();
	}

	public CollectionData GetTitleById(int monitorId, int targetId)
	{
		return _title[monitorId].GetCollectionById(targetId);
	}

	public CollectionData GetTitle(int monitorId, int diffIndex)
	{
		return _title[monitorId].GetCollection(diffIndex);
	}

	public bool IsTitleBoundary(int monitorId, int diffIndex, out int overCount)
	{
		return _title[monitorId].IsCollectionBoundary(diffIndex, out overCount);
	}

	public int GetAllTitleNum(int monitorId)
	{
		return _title[monitorId].GetAllCollectionNum();
	}

	public int GetCurrentTitleIndex(int monitorId)
	{
		return _title[monitorId].GetCurrentIndex();
	}

	private string TitleCategoryName(int monitorId, int diff)
	{
		int num = _title[monitorId].CategoryIndex + diff;
		if (num < 0)
		{
			num = _title[monitorId].TabData.Count - 1;
		}
		else if (_title[monitorId].TabData.Count <= num)
		{
			num = 0;
		}
		return _title[monitorId].CategoryName(num);
	}

	private void ChangeEquipmentTitle(int monitorId, string title, string trophyRareType, int collectionId)
	{
		_monitors[monitorId].PushDesitionButton(_subSequence[monitorId]);
		Sprite sprite = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_Shougou_" + trophyRareType);
		container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30003, monitorId, title, sprite));
		Singleton<UserDataManager>.Instance.GetUserData(monitorId).Detail.EquipTitleID = collectionId;
	}
}
