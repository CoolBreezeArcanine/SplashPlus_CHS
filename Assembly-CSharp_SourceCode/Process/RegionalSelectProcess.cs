using System.Collections.Generic;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using Process.ModeSelect;
using UnityEngine;
using Util;

namespace Process
{
	public class RegionalSelectProcess : ProcessBase, IRegionalSelectProcess
	{
		public enum RegionSelectState
		{
			Initialize,
			Update,
			ToRelease,
			Release
		}

		public enum RegionSelectIndividualState
		{
			Initialize,
			FirstInformation,
			FirstInfoWait,
			Discover,
			ToUpdate,
			ToUpdateWait,
			Update,
			Setting,
			FocusRegionList,
			ToDecide,
			RewardWait,
			Decide,
			ToWait,
			Wait,
			Release
		}

		private int[] _selectCursolIndex;

		private RegionSelectState _state;

		private RegionSelectIndividualState[] _subStates;

		private UserData[] _userDatas;

		private RegionalSelectMonitor[] _monitors;

		private List<UserMapData>[] _userMapDataList;

		private Queue<int>[] _discoverList;

		private Queue<FirstInformationData>[] _messageQueue;

		private int[] _normalRegionIndex;

		private int[] _eventBannerIndex;

		private float[] _timer;

		private float[] _skipBorderTime;

		private bool[] _isEventOpen;

		private bool[] _isShowToggleButton;

		private bool[] _isgotoCharaSelect;

		private bool _addNewcomer;

		public ModeSelectProcess.ProccessActiveMode _active_mode;

		public RegionalSelectProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public RegionalSelectProcess(ProcessDataContainer dataContainer, ProcessType type)
			: base(dataContainer, type)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			Singleton<MapMaster>.Instance.CreateMapData();
			GameObject prefs = Resources.Load<GameObject>("Process/RegionSelect/RegionSelectProcess");
			_monitors = new RegionalSelectMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<RegionalSelectMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<RegionalSelectMonitor>()
			};
			_subStates = new RegionSelectIndividualState[2];
			_userDatas = new UserData[2];
			_userMapDataList = new List<UserMapData>[2];
			_discoverList = new Queue<int>[2];
			_messageQueue = new Queue<FirstInformationData>[2];
			_normalRegionIndex = new int[2];
			_selectCursolIndex = new int[2];
			_eventBannerIndex = new int[2];
			_timer = new float[2];
			_skipBorderTime = new float[2];
			_isShowToggleButton = new bool[2];
			_isgotoCharaSelect = new bool[2];
			_isEventOpen = new bool[2];
			_addNewcomer = false;
			for (int i = 0; i < _monitors.Length; i++)
			{
				int num = 0;
				_discoverList[i] = new Queue<int>();
				_timer[i] = 0f;
				_skipBorderTime[i] = 0f;
				_userDatas[i] = Singleton<UserDataManager>.Instance.GetUserData(i);
				_subStates[i] = ((!_userDatas[i].IsEntry || _userDatas[i].IsGuest()) ? RegionSelectIndividualState.ToWait : RegionSelectIndividualState.Initialize);
				if (_subStates[i] == RegionSelectIndividualState.Initialize)
				{
					_isgotoCharaSelect[i] = _userDatas[i].Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCharaSelect);
					_userMapDataList[i] = SortUserMapData(Singleton<MapMaster>.Instance.RefUserMapList[i], out _normalRegionIndex[i], out _isEventOpen[i]);
					_isShowToggleButton[i] = false;
					_discoverList[i] = GenerateDiscoverList(_userMapDataList[i], Singleton<MapMaster>.Instance.RefExistMapIDList[i]);
					if (0 < _discoverList[i].Count)
					{
						foreach (UserMapData item in _userMapDataList[i])
						{
							if (_discoverList[i].Contains(item.ID))
							{
								_userDatas[i].Activity.MapFound(item.ID);
								item.IsFinishedOpening = true;
								item.IsLock = false;
							}
						}
					}
					_eventBannerIndex[i] = -1;
					num = FirstFocusIndex(_userDatas[i], _userMapDataList[i], _discoverList[i], out _eventBannerIndex[i]);
					if (num < 0 || _userMapDataList[i].Count == 0)
					{
						num = 0;
					}
					_selectCursolIndex[i] = num;
					if (0 <= _eventBannerIndex[i])
					{
						int iD = _userMapDataList[i][_eventBannerIndex[i]].ID;
						Sprite mapBgSprite = container.assetManager.GetMapBgSprite(iD, "UI_EventBanner");
						_monitors[i].SetActiveEventBanner(!_userMapDataList[i][_selectCursolIndex[i]].IsEvent);
						_monitors[i].SetEventBanner(mapBgSprite);
					}
					else
					{
						_monitors[i].SetActiveEventBanner(isActive: false);
					}
					_messageQueue[i] = new Queue<FirstInformationData>();
					if (!_userDatas[i].Detail.ContentBit.IsFlagOn(ContentBitID.FirstMapSelect))
					{
						_messageQueue[i].Enqueue(new FirstInformationData(WindowMessageID.MapSelectFirst, Mai2.Voice_000001.Cue.VO_000063, 3000));
						_userDatas[i].Detail.ContentBit.SetFlag(ContentBitID.FirstMapSelect, flag: true);
					}
					_monitors[i].Initialize(this, i, num, _userDatas[i].IsEntry && !_userDatas[i].IsGuest(), _isEventOpen[i], container.assetManager);
					_monitors[i].Discover.Initialize(i, 0 < _discoverList[i].Count, container.assetManager);
					bool state = _userDatas[i].Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCodeRead);
					bool state2 = _userDatas[i].Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoIconPhotoShoot);
					bool state3 = _userDatas[i].Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.PhotoAgree);
					bool state4 = _userDatas[i].Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCharaSelect);
					OptionHeadphonevolumeID headPhoneVolume = _userDatas[i].Option.HeadPhoneVolume;
					SoundManager.SetHeadPhoneVolume(i, headPhoneVolume.GetValue());
					Singleton<UserDataManager>.Instance.GetUserData(i).Option.HeadPhoneVolume = headPhoneVolume;
					_monitors[i].SetVolume(headPhoneVolume);
					_monitors[i].SetSettingState(0, state, isActive: true, nextTime: true);
					_monitors[i].SetSettingState(1, state2, isActive: true, nextTime: true);
					_monitors[i].SetSettingState(2, state4, isActive: true, nextTime: false);
					_monitors[i].SetSettingState(3, state3, isActive: true, nextTime: false);
				}
				else
				{
					_isgotoCharaSelect[i] = false;
					_monitors[i].Disabled();
				}
			}
			if (_subStates[0] == RegionSelectIndividualState.ToWait && _subStates[1] == RegionSelectIndividualState.ToWait)
			{
				_state = RegionSelectState.ToRelease;
			}
			SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_ENTRY, 0);
			SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_ENTRY, 1);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case RegionSelectState.Initialize:
				container.processManager.NotificationFadeIn();
				_state = RegionSelectState.Update;
				break;
			case RegionSelectState.Update:
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					switch (_subStates[i])
					{
					case RegionSelectIndividualState.Initialize:
						_monitors[i].SetActiveSettingIndicator(isActive: false);
						if (0 < _messageQueue[i].Count)
						{
							_subStates[i] = RegionSelectIndividualState.FirstInformation;
							break;
						}
						_monitors[i].SetVisibleButton(false, 3, 7);
						if (0 < _discoverList[i].Count)
						{
							_monitors[i].Discover.Play(_discoverList[i]);
							_subStates[i] = RegionSelectIndividualState.Discover;
						}
						else
						{
							_subStates[i] = RegionSelectIndividualState.ToUpdate;
						}
						break;
					case RegionSelectIndividualState.FirstInformation:
						if (!(_timer[i] <= 0f))
						{
							break;
						}
						if (0 < _messageQueue[i].Count)
						{
							ShowFirstMessage(i, _messageQueue[i].Dequeue());
							_subStates[i] = RegionSelectIndividualState.FirstInfoWait;
							break;
						}
						_monitors[i].SetVisibleButton(false, 3, 7);
						container.processManager.CloseWindow(i);
						if (0 < _discoverList[i].Count)
						{
							_monitors[i].Discover.Play(_discoverList[i]);
							_subStates[i] = RegionSelectIndividualState.Discover;
						}
						else
						{
							_subStates[i] = RegionSelectIndividualState.ToUpdate;
						}
						break;
					case RegionSelectIndividualState.FirstInfoWait:
						if (_timer[i] <= _skipBorderTime[i])
						{
							_monitors[i].SetVisibleButton(true, 3);
						}
						_timer[i] -= GameManager.GetGameMSecAdd();
						if (_timer[i] <= 0f)
						{
							_subStates[i] = RegionSelectIndividualState.FirstInformation;
						}
						break;
					case RegionSelectIndividualState.Discover:
						if (_monitors[i].Discover.IsDone())
						{
							SetRegionButton(i);
							_subStates[i] = RegionSelectIndividualState.ToUpdate;
						}
						else if (_monitors[i].Discover.IsSkippable())
						{
							if (!_monitors[i].Discover.IsShowSkipButton)
							{
								_monitors[i].Discover.IsShowSkipButton = true;
								_monitors[i].SetVisibleButton(true, 3);
							}
						}
						else if (_monitors[i].Discover.IsShowSkipButton)
						{
							_monitors[i].Discover.IsShowSkipButton = false;
							_monitors[i].SetVisibleButton(false, 3);
						}
						break;
					case RegionSelectIndividualState.ToUpdate:
						_monitors[i].Play();
						_timer[i] = 1000f;
						SetInputLockInfo(i, 1500f);
						_monitors[i].SetVisibleButton(false, 3);
						_monitors[i].SetActiveSettingIndicator(isActive: true);
						_subStates[i] = RegionSelectIndividualState.ToUpdateWait;
						break;
					case RegionSelectIndividualState.ToUpdateWait:
						if (_timer[i] <= 0f)
						{
							_monitors[i].ChangeDecisionButton();
							_monitors[i].SetVisibleButton(true, 2, 5, 7);
							int num = ((i == 0) ? 1 : 0);
							if (_subStates[num] > RegionSelectIndividualState.Discover)
							{
								container.processManager.PrepareTimer(30, 0, isEntry: false, TimeUp);
								if (_subStates[num] >= RegionSelectIndividualState.Wait)
								{
									container.processManager.SetVisibleTimer(num, isVisible: false);
								}
							}
							UserMapData userMapData = _userMapDataList[i][_selectCursolIndex[i]];
							_monitors[i].InitRegionButton(userMapData.IsEvent);
							_subStates[i] = RegionSelectIndividualState.Update;
						}
						else
						{
							_timer[i] -= GameManager.GetGameMSecAdd();
						}
						break;
					case RegionSelectIndividualState.FocusRegionList:
						if (_timer[i] <= 0f)
						{
							RegionSelectDecide(i);
						}
						else
						{
							_timer[i] -= GameManager.GetGameMSecAdd();
						}
						break;
					case RegionSelectIndividualState.ToDecide:
						_subStates[i] = RegionSelectIndividualState.Decide;
						break;
					case RegionSelectIndividualState.Decide:
						Decide(i);
						break;
					case RegionSelectIndividualState.ToWait:
						if (_userDatas[i].IsEntry && _userDatas[i].IsGuest())
						{
							container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
						}
						_subStates[i] = RegionSelectIndividualState.Wait;
						break;
					}
				}
				break;
			}
			case RegionSelectState.ToRelease:
				if (500f <= _timer[0])
				{
					Finish();
					_state = RegionSelectState.Release;
				}
				else
				{
					_timer[0] += GameManager.GetGameMSecAdd();
				}
				break;
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				_monitors[j].ViewUpdate();
			}
		}

		protected override void UpdateInput(int id)
		{
			base.UpdateInput(id);
			if (_state == RegionSelectState.Update)
			{
				switch (_subStates[id])
				{
				case RegionSelectIndividualState.FirstInfoWait:
					InputFirstState(id);
					break;
				case RegionSelectIndividualState.Discover:
					InputDiscover(id);
					break;
				case RegionSelectIndividualState.Update:
					InputUpdateState(id);
					break;
				case RegionSelectIndividualState.Setting:
					InputSettingState(id);
					break;
				case RegionSelectIndividualState.ToUpdate:
				case RegionSelectIndividualState.ToUpdateWait:
					break;
				}
			}
		}

		private void InputFirstState(int id)
		{
			if (_timer[id] <= _skipBorderTime[id] && InputManager.GetButtonDown(id, InputManager.ButtonSetting.Button04))
			{
				_monitors[id].PushButton(3);
				_timer[id] = 0f;
			}
		}

		private void InputDiscover(int id)
		{
			if (_monitors[id].Discover.IsSkippable() && InputManager.GetButtonDown(id, InputManager.ButtonSetting.Button04))
			{
				_monitors[id].PushButton(3);
				_monitors[id].SetVisibleButton(true, 3);
				_monitors[id].Discover.Skip();
				SetInputLockInfo(id, 500f);
			}
		}

		private void InputUpdateState(int id)
		{
			if (InputManager.GetInputDown(id, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(id, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
			{
				if (_selectCursolIndex[id] + 1 < _userMapDataList[id].Count)
				{
					_selectCursolIndex[id]++;
				}
				else
				{
					_selectCursolIndex[id] = 0;
				}
				SetRegionButton(id);
				_monitors[id].PushButton(2);
				SetShowDecideButton(id);
				_monitors[id].ListScroll(_selectCursolIndex[id]);
				SetInputLockInfo(id, 100f);
			}
			else if (InputManager.GetInputDown(id, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(id, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
			{
				if (0 <= _selectCursolIndex[id] - 1)
				{
					_selectCursolIndex[id]--;
				}
				else
				{
					_selectCursolIndex[id] = _userMapDataList[id].Count - 1;
				}
				SetRegionButton(id);
				_monitors[id].PushButton(5);
				SetShowDecideButton(id);
				_monitors[id].ListScroll(_selectCursolIndex[id]);
				SetInputLockInfo(id, 100f);
			}
			else if (_isEventOpen[id] && InputManager.GetButtonDown(id, InputManager.ButtonSetting.Button02))
			{
				if (IsCursorPositionCollabo(id))
				{
					_selectCursolIndex[id] = _normalRegionIndex[id];
					SetInputLockInfo(id, 200f);
					_monitors[id].ListScroll(_selectCursolIndex[id]);
				}
				else
				{
					_selectCursolIndex[id] = 0;
					SetInputLockInfo(id, 200f);
					_monitors[id].ListScroll(_selectCursolIndex[id]);
				}
				SetRegionButton(id);
				_monitors[id].PushButton(1);
			}
			else if (0 <= _eventBannerIndex[id] && !_userMapDataList[id][_selectCursolIndex[id]].IsEvent && InputManager.GetInputDown(id, InputManager.ButtonSetting.Button07, InputManager.TouchPanelArea.A7))
			{
				_selectCursolIndex[id] = _eventBannerIndex[id];
				SetRegionButton(id);
				SetInputLockInfo(id, 200f);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, id);
				_monitors[id].ListScroll(_selectCursolIndex[id]);
			}
			else if (CanVisitRegion(id, _selectCursolIndex[id]) && InputManager.GetButtonDown(id, InputManager.ButtonSetting.Button04))
			{
				_monitors[id].PushButton(3);
				SetInputLockInfo(id, 550f);
				RegionSelectDecide(id);
			}
			else if (InputManager.GetInputDown(id, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8))
			{
				_monitors[id].PushButton(7);
				_monitors[id].OpenSettingWindow();
				_subStates[id] = RegionSelectIndividualState.Setting;
				_monitors[id].SetVisibleButton(false, 1, 2, 3, 5);
				_monitors[id].ChangeSettingButton(state: false);
			}
		}

		private void InputSettingState(int id)
		{
			if (InputManager.GetInputDown(id, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8))
			{
				SetShowDecideButton(id);
				_monitors[id].SetVisibleButton(true, 2, 5);
				if (_isEventOpen[id])
				{
					_monitors[id].SetVisibleButton(true, 1);
				}
				_monitors[id].ChangeSettingButton(state: true);
				_monitors[id].PushButton(7);
				_monitors[id].CloseSettingWindow();
				_subStates[id] = RegionSelectIndividualState.Update;
			}
			else if (InputManager.GetTouchPanelAreaDown(id, InputManager.TouchPanelArea.A1, InputManager.TouchPanelArea.D2))
			{
				bool flag = !Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCodeRead);
				_monitors[id].PressedToggle(0, flag);
				Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, flag);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, id);
			}
			else if (InputManager.GetTouchPanelAreaDown(id, InputManager.TouchPanelArea.E2))
			{
				bool flag2 = !Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoIconPhotoShoot);
				_monitors[id].PressedToggle(1, flag2);
				Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoIconPhotoShoot, flag2);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, id);
			}
			else if (InputManager.GetTouchPanelAreaDown(id, InputManager.TouchPanelArea.B2))
			{
				bool flag3 = !Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.GotoCharaSelect);
				_monitors[id].PressedToggle(2, flag3);
				Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCharaSelect, flag3);
				_isgotoCharaSelect[id] = flag3;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, id);
			}
			else if (InputManager.GetTouchPanelAreaDown(id, InputManager.TouchPanelArea.B3))
			{
				bool flag4 = !Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.PhotoAgree);
				_monitors[id].PressedToggle(3, flag4);
				Singleton<UserDataManager>.Instance.GetUserData(id).Extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, flag4);
				GameManager.IsPhotoAgree = flag4;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, id);
			}
			else if (InputManager.GetTouchPanelAreaDown(id, InputManager.TouchPanelArea.E6) || InputManager.GetTouchPanelAreaLongPush(id, InputManager.TouchPanelArea.E6, 1000L))
			{
				OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(id).Option.HeadPhoneVolume;
				if (headPhoneVolume - 1 >= OptionHeadphonevolumeID.Vol1)
				{
					headPhoneVolume--;
					_monitors[id].SetVolume(headPhoneVolume);
					SoundManager.SetHeadPhoneVolume(id, headPhoneVolume.GetValue());
					Singleton<UserDataManager>.Instance.GetUserData(id).Option.HeadPhoneVolume = headPhoneVolume;
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, id);
					SetInputLockInfo(id, 100f);
				}
				if (InputManager.GetTouchPanelAreaLongPush(id, InputManager.TouchPanelArea.E6, 1000L))
				{
					_monitors[id].HoldMinusButton();
				}
				else
				{
					_monitors[id].PressedMinusButton();
				}
			}
			else if (InputManager.GetTouchPanelAreaDown(id, InputManager.TouchPanelArea.E4) || InputManager.GetTouchPanelAreaLongPush(id, InputManager.TouchPanelArea.E4, 1000L))
			{
				OptionHeadphonevolumeID headPhoneVolume2 = Singleton<UserDataManager>.Instance.GetUserData(id).Option.HeadPhoneVolume;
				if (headPhoneVolume2 + 1 < OptionHeadphonevolumeID.End)
				{
					headPhoneVolume2++;
					_monitors[id].SetVolume(headPhoneVolume2);
					SoundManager.SetHeadPhoneVolume(id, headPhoneVolume2.GetValue());
					Singleton<UserDataManager>.Instance.GetUserData(id).Option.HeadPhoneVolume = headPhoneVolume2;
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, id);
					SetInputLockInfo(id, 100f);
				}
				if (InputManager.GetTouchPanelAreaLongPush(id, InputManager.TouchPanelArea.E4, 1000L))
				{
					_monitors[id].HoldPlusButton();
				}
				else
				{
					_monitors[id].PressedPlusButton();
				}
			}
		}

		private bool IsCursorPositionCollabo(int id)
		{
			return _selectCursolIndex[id] < _normalRegionIndex[id];
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
			_monitors = null;
			Resources.UnloadUnusedAssets();
		}

		private List<UserMapData> SortUserMapData(List<UserMapData> userMapList, out int normalRegionIndex, out bool isEvent)
		{
			List<UserMapData> list = new List<UserMapData>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			List<int> list5 = new List<int>();
			foreach (UserMapData userMap in userMapList)
			{
				MapData mapData = Singleton<DataManager>.Instance.GetMapData(userMap.ID);
				if (!Singleton<EventManager>.Instance.IsOpenEvent(mapData.OpenEventId.id))
				{
					continue;
				}
				bool flag = false;
				for (int i = 0; i < mapData.TreasureExDatas.Count; i++)
				{
					int id = mapData.TreasureExDatas[i].TreasureId.id;
					if (Singleton<DataManager>.Instance.GetMapTreasureData(id) == null)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (userMap.IsEvent)
					{
						list2.Add(userMap.ID);
					}
					else if (userMap.IsDeluxe)
					{
						list4.Add(userMap.ID);
					}
					else
					{
						list3.Add(userMap.ID);
					}
				}
			}
			normalRegionIndex = list2.Count;
			isEvent = 0 < list2.Count;
			list2.Sort();
			list3.Sort();
			list4.Sort();
			list5.AddRange(list2);
			list5.AddRange(list3);
			list5.AddRange(list4);
			foreach (int item in list5)
			{
				foreach (UserMapData userMap2 in userMapList)
				{
					if (userMap2.ID == item)
					{
						list.Add(userMap2);
						break;
					}
				}
			}
			return list;
		}

		private Queue<int> GenerateDiscoverList(List<UserMapData> userMapList, List<int> existIDList)
		{
			Queue<int> queue = new Queue<int>(userMapList.Count);
			foreach (UserMapData mapData in userMapList)
			{
				if (!mapData.IsLock && !mapData.IsFinishedOpening && 0 >= mapData.Distance && !mapData.IsEvent && existIDList.FindIndex((int n) => n == mapData.ID) < 0)
				{
					queue.Enqueue(mapData.ID);
					mapData.IsFinishedOpening = true;
				}
			}
			return queue;
		}

		private int FirstFocusIndex(UserData userData, List<UserMapData> userMapList, Queue<int> discoverList, out int eventIndex)
		{
			eventIndex = -1;
			int num = -1;
			long num2 = 0L;
			for (int i = 0; i < userMapList.Count; i++)
			{
				UserMapData userMapData = userMapList[i];
				if (!userMapData.IsEvent || userMapData.IsLock || userMapData.IsComplete)
				{
					continue;
				}
				MapData mapData = Singleton<DataManager>.Instance.GetMapData(userMapData.ID);
				if (mapData == null || !Singleton<EventManager>.Instance.IsNewEvent(mapData.OpenEventId.id))
				{
					continue;
				}
				long eventStartUnixTime = Singleton<EventManager>.Instance.GetEventStartUnixTime(mapData.OpenEventId.id);
				if (num2 <= eventStartUnixTime)
				{
					num2 = eventStartUnixTime;
					long unixTime = TimeManager.GetUnixTime(userData.Detail.EventWatchedDate);
					if (Singleton<EventManager>.Instance.IsDateNewAndOpen(mapData.OpenEventId.id, unixTime))
					{
						num = i;
					}
					eventIndex = i;
				}
			}
			if (num >= 0)
			{
				return num;
			}
			if (0 < discoverList.Count)
			{
				if (userData.Detail.SelectMapID > 0)
				{
					int num3 = userMapList.FindIndex((UserMapData n) => n.ID == userData.Detail.SelectMapID);
					if (num3 >= 0 && !userMapList[num3].IsLock && !userMapList[num3].IsComplete)
					{
						return num3;
					}
				}
				foreach (int id in discoverList)
				{
					int num4 = userMapList.FindIndex((UserMapData n) => n.ID == id);
					if (num4 >= 0 && !userMapList[num4].IsLock && !userMapList[num4].IsComplete)
					{
						return num4;
					}
				}
			}
			int num5 = userMapList.FindIndex((UserMapData n) => n.ID == userData.Detail.SelectMapID);
			if (num5 >= 0 && !userMapList[num5].IsLock && !userMapList[num5].IsComplete)
			{
				return num5;
			}
			for (int j = 0; j < userMapList.Count; j++)
			{
				if (!userMapList[j].IsEvent && !userMapList[j].IsDeluxe && !userMapList[j].IsLock && !userMapList[j].IsComplete)
				{
					return j;
				}
			}
			for (int k = 0; k < userMapList.Count; k++)
			{
				if (!userMapList[k].IsEvent && userMapList[k].IsDeluxe && !userMapList[k].IsLock && !userMapList[k].IsComplete)
				{
					return k;
				}
			}
			return num;
		}

		private void ShowFirstMessage(int id, FirstInformationData message)
		{
			container.processManager.EnqueueMessage(id, message.MessageID);
			_timer[id] = 10000f;
			_skipBorderTime[id] = _timer[id] - (float)message.SkipButtonTime;
			SoundManager.PlayVoice(message.VoiceCue, id);
			_monitors[id].SetVisibleButton(false, 3, 7);
		}

		private bool CanVisitRegion(int id, int cursorIndex)
		{
			UserMapData userMapData = _userMapDataList[id][cursorIndex];
			if (!userMapData.IsLock)
			{
				return !userMapData.IsComplete;
			}
			return false;
		}

		private void SetShowDecideButton(int id)
		{
			if (CanVisitRegion(id, _selectCursolIndex[id]))
			{
				_monitors[id].SetVisibleButton(true, 3);
			}
			else
			{
				_monitors[id].SetVisibleButton(false, 3);
			}
		}

		private void RegionSelectDecide(int id)
		{
			UserMapData userMapData = _userMapDataList[id][_selectCursolIndex[id]];
			container.processManager.SetVisibleTimer(id, isVisible: false);
			_monitors[id].SetVisibleButton(false, 1, 2, 3, 5, 7);
			_monitors[id].Decide();
			if (userMapData.Distance == 0)
			{
				MapData mapData = Singleton<DataManager>.Instance.GetMapData(userMapData.ID);
				if (mapData?.TreasureExDatas[0] != null && mapData.TreasureExDatas[0].Distance == 0)
				{
					_subStates[id] = RegionSelectIndividualState.RewardWait;
					MapTreasureData mapTreasureData = Singleton<DataManager>.Instance.GetMapTreasureData(mapData.TreasureExDatas[0].TreasureId.id);
					switch (mapTreasureData.TreasureType)
					{
					case MapTreasureType.Character:
					{
						Singleton<UserDataManager>.Instance.GetUserData(id).AddCollections(UserData.Collection.Chara, mapTreasureData.CharacterId.id);
						SoundManager.PlayVoice(mapTreasureData.CharacterId.id switch
						{
							102 => Mai2.Voice_000001.Cue.VO_000239, 
							103 => Mai2.Voice_000001.Cue.VO_000241, 
							104 => Mai2.Voice_000001.Cue.VO_000240, 
							105 => Mai2.Voice_000001.Cue.VO_000242, 
							_ => Mai2.Voice_000001.Cue.VO_000145, 
						}, id);
						CharaData chara = Singleton<DataManager>.Instance.GetChara(mapTreasureData.CharacterId.id);
						Texture2D characterTexture2D = container.assetManager.GetCharacterTexture2D(mapTreasureData.CharacterId.id);
						Sprite chara2 = Sprite.Create(characterTexture2D, new Rect(0f, 0f, characterTexture2D.width, characterTexture2D.height), new Vector2(0.5f, 0.5f));
						_monitors[id].RewardCharaGetWindow(chara2, chara.name.str, mapTreasureData.CharacterId.id, OnCallbackReward);
						_addNewcomer = true;
						break;
					}
					case MapTreasureType.MusicNew:
					{
						Singleton<UserDataManager>.Instance.GetUserData(id).AddUnlockMusic(UserData.MusicUnlock.Base, mapTreasureData.MusicId.id);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, id);
						MusicData music = Singleton<DataManager>.Instance.GetMusic(mapTreasureData.MusicId.id);
						Texture2D jacketTexture2D = container.assetManager.GetJacketTexture2D(mapTreasureData.MusicId.id);
						Sprite jacket = Sprite.Create(jacketTexture2D, new Rect(0f, 0f, jacketTexture2D.width, jacketTexture2D.height), new Vector2(0.5f, 0.5f));
						_monitors[id].RewardMusicGetWindow(jacket, music.dataName, OnCallbackReward);
						break;
					}
					case MapTreasureType.NamePlate:
					{
						Singleton<UserDataManager>.Instance.GetUserData(id).AddCollections(UserData.Collection.Plate, mapTreasureData.NamePlate.id);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET_02, id);
						Texture2D plateTexture2D = container.assetManager.GetPlateTexture2D(mapTreasureData.NamePlate.id);
						Sprite plate = Sprite.Create(plateTexture2D, new Rect(0f, 0f, plateTexture2D.width, plateTexture2D.height), new Vector2(0.5f, 0.5f));
						_monitors[id].RewardNamePlateGetWindow(plate, mapTreasureData.NamePlate.str, OnCallbackReward);
						break;
					}
					case MapTreasureType.LevelUpAll:
					case MapTreasureType.Otomodachi:
						break;
					}
				}
				else
				{
					_subStates[id] = RegionSelectIndividualState.ToDecide;
				}
			}
			else
			{
				_subStates[id] = RegionSelectIndividualState.ToDecide;
			}
		}

		private void OnCallbackReward(int id)
		{
			_subStates[id] = RegionSelectIndividualState.ToDecide;
			int num = ((id == 0) ? 1 : 0);
			if (_subStates[num] <= RegionSelectIndividualState.RewardWait)
			{
				container.processManager.EnqueueMessage(id, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
			}
		}

		private int GetRegionDivertIndex(int id)
		{
			int num = -1;
			foreach (UserMapData item in _userMapDataList[id])
			{
				num++;
				if (!item.IsEvent && !item.IsLock && !item.IsComplete)
				{
					return num;
				}
			}
			return GetDefaultRegionIndex(id);
		}

		private int GetDefaultRegionIndex(int id)
		{
			for (int i = 0; i < _userMapDataList[id].Count; i++)
			{
				if (_userMapDataList[id][i].ID == 0)
				{
					return i;
				}
			}
			return -1;
		}

		private void Decide(int id)
		{
			_userDatas[id].Detail.SelectMapID = _userMapDataList[id][_selectCursolIndex[id]].ID;
			SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000065, id);
			int num = ((id == 0) ? 1 : 0);
			if (_subStates[num] <= RegionSelectIndividualState.RewardWait)
			{
				container.processManager.EnqueueMessage(id, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
			}
			_subStates[id] = RegionSelectIndividualState.Wait;
			if (_subStates[id] == RegionSelectIndividualState.Wait && _subStates[num] == RegionSelectIndividualState.Wait)
			{
				_subStates[id] = RegionSelectIndividualState.Release;
				_subStates[num] = RegionSelectIndividualState.Release;
				_timer[0] = 0f;
				_state = RegionSelectState.ToRelease;
			}
		}

		public void TimeUp()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_subStates[i] <= RegionSelectIndividualState.Setting)
				{
					if (!CanVisitRegion(i, _selectCursolIndex[i]))
					{
						_selectCursolIndex[i] = GetRegionDivertIndex(i);
						_monitors[i].ListScroll(_selectCursolIndex[i]);
						_timer[i] = 500f;
						_subStates[i] = RegionSelectIndividualState.FocusRegionList;
					}
					else
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, _monitors[i].MonitorIndex);
						RegionSelectDecide(i);
					}
				}
			}
		}

		private void Finish()
		{
			GameManager.IsGotoPhotoShoot = _isgotoCharaSelect[0] || _isgotoCharaSelect[1];
			bool isSkipProcess = !GameManager.IsGotoPhotoShoot;
			container.processManager.ClearTimeoutAction();
			for (int i = 0; i < _monitors.Length; i++)
			{
				container.processManager.CloseWindow(i);
				Singleton<MapMaster>.Instance.CreateUserDataMapList(i);
				if (_userDatas[i].IsEntry)
				{
					GameManager.IsPhotoAgree |= Singleton<UserDataManager>.Instance.GetUserData(i).Extend.ExtendContendBit.IsFlagOn(ExtendContentBitID.PhotoAgree);
				}
				_monitors[i].SetVisibleButton(false, 3, 5, 7);
				MechaManager.LedIf[i].ButtonLedReset();
			}
			if ((_userDatas[0].IsEntry && !_userDatas[0].IsGuest()) || (_userDatas[1].IsEntry && !_userDatas[1].IsGuest()))
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new CharacterSelectProces(container, isSkipProcess)), 50);
				return;
			}
			container.processManager.AddProcess(new GetMusicProcess(container), 50);
			container.processManager.ReleaseProcess(this);
		}

		private void SetToggleButton(int id)
		{
			UserMapData userMapData = _userMapDataList[id][_selectCursolIndex[id]];
			if (userMapData.IsLock || userMapData.IsComplete)
			{
				if (_isShowToggleButton[id])
				{
					_isShowToggleButton[id] = false;
					_monitors[id].SetActiveToggleButton(isActive: false, _isgotoCharaSelect[id]);
				}
			}
			else if (!_isShowToggleButton[id])
			{
				_isShowToggleButton[id] = true;
				_monitors[id].SetActiveToggleButton(isActive: true, _isgotoCharaSelect[id]);
			}
		}

		private void SetRegionButton(int id)
		{
			UserMapData userMapData = _userMapDataList[id][_selectCursolIndex[id]];
			_monitors[id].ChangeRegionButtonState(userMapData.IsEvent);
			if (0 <= _eventBannerIndex[id])
			{
				_monitors[id].SetActiveEventBanner(!userMapData.IsEvent);
			}
		}

		private bool IsTravel(int id)
		{
			foreach (UserMapData item in _userMapDataList[id])
			{
				if (item.IsLock || item.IsComplete)
				{
					return true;
				}
			}
			return false;
		}

		public List<UserMapData> GetUserMapDatas(int index)
		{
			return _userMapDataList[index];
		}

		public int SelectCursolIndex(int index)
		{
			return _selectCursolIndex[index];
		}
	}
}
