using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Monitor;
using Process.MusicSelectInfo;
using Process.TicketSelect;
using UI.DaisyChainList;
using UnityEngine;
using Util;

namespace Process
{
	public class CharacterSelectProces : ProcessBase, ICharacterSelectProcess
	{
		public enum ArrowDirection : byte
		{
			Up,
			Stay,
			Down
		}

		private enum CharacterSelectSequence
		{
			Init,
			FadeIn,
			Reset,
			Wait,
			Update,
			Skip,
			Release
		}

		private enum CharacterSelectSubSequence
		{
			FirstInformation,
			SlotView,
			CharacterSelect,
			FavoriteList,
			AutoSetMessage,
			Newcomer,
			Wait
		}

		private const int PlayerNumber = 2;

		private readonly List<List<ReadOnlyCollection<CharacterData>>> _userCharaList = new List<List<ReadOnlyCollection<CharacterData>>>();

		private readonly List<ReadOnlyCollection<CharacterTabData>> _characterTabDataList = new List<ReadOnlyCollection<CharacterTabData>>();

		private Queue<CharacterData>[] _newcomerCharacterDatas;

		private Queue<FirstInformationData>[] _firstInformationQueues;

		private int[] _skipTimes;

		private IEnumerator[] _routine;

		private List<CharacterData>[] _userFavoriteList;

		private List<CharacterData>[] _tempFavorites;

		private int[] _currentCharacterIndex;

		private int[] _currentCategoryIndex;

		private int[] _currentSelectSlot;

		private int[] _favoriteListCurrentIndex;

		private int[] _selectSlotCharacterDistance;

		private uint[] _currentTotalDistance;

		private int[] _slideScrollCount;

		private int[] _toDistance;

		private float[] _countUpTimer;

		private float[] _slideScrollTimer;

		private float _timer;

		private float[] _userTimer;

		private bool[] _isFirstInfoButton;

		private bool[] _isInitiative;

		private bool[] _isCountUpAnimation;

		private bool[] _isBlanks;

		private int[] _iHoldLocks;

		private Direction _slideScrollToDirection;

		private CharacterSelectSubSequence[] _subSequence;

		private Action<int> _onReturn;

		private Action<int>[] _onFirstInfoActions;

		private CharacterData[][] _slotArray;

		private CharacterSelectMonitor[] _monitor;

		private CharacterSelectSequence _mainSequence;

		private bool[] _isNewcomerMode;

		private readonly bool _isSkipProcess;

		private const int CategoryChangeTime = 1000;

		private const int DesigionTime = 500;

		private const int SlideTime = 250;

		private const int FavoriteTime = 100;

		public CharacterData GetNewcomerData(int playerIndex)
		{
			return _newcomerCharacterDatas[playerIndex].Peek();
		}

		public List<CharacterTabData> GetTabDataList(int playerIndex)
		{
			return new List<CharacterTabData>(_characterTabDataList[playerIndex]);
		}

		public CharacterSelectProces(ProcessDataContainer dataContainer, bool isSkipProcess = false)
			: base(dataContainer)
		{
			_isSkipProcess = isSkipProcess;
		}

		public CharacterSelectProces(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
			_isSkipProcess = false;
		}

		public override void OnAddProcess()
		{
		}

		public void SetCallback(Action<int> onEnd)
		{
			_onReturn = onEnd;
		}

		public override void OnStart()
		{
			if (!_isSkipProcess)
			{
				GameObject prefs = Resources.Load<GameObject>("Process/CharacterSelect/CharacterSelectProcess");
				_monitor = new CharacterSelectMonitor[2]
				{
					CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CharacterSelectMonitor>(),
					CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CharacterSelectMonitor>()
				};
				_routine = new IEnumerator[2];
			}
			_onFirstInfoActions = new Action<int>[2];
			_isInitiative = new bool[2];
			_isCountUpAnimation = new bool[2];
			_isNewcomerMode = new bool[2];
			_isFirstInfoButton = new bool[2];
			_isBlanks = new bool[2];
			_iHoldLocks = new int[2];
			_currentSelectSlot = new int[2];
			_favoriteListCurrentIndex = new int[2];
			_selectSlotCharacterDistance = new int[2];
			_skipTimes = new int[2];
			_currentCharacterIndex = new int[2];
			_currentCategoryIndex = new int[2];
			_toDistance = new int[2];
			_slideScrollCount = new int[2];
			_countUpTimer = new float[2];
			_slideScrollTimer = new float[2];
			_userTimer = new float[2];
			_currentTotalDistance = new uint[2];
			_subSequence = new CharacterSelectSubSequence[2];
			_userFavoriteList = new List<CharacterData>[2];
			_tempFavorites = new List<CharacterData>[2];
			_slotArray = new CharacterData[2][];
			_firstInformationQueues = new Queue<FirstInformationData>[2];
			_newcomerCharacterDatas = new Queue<CharacterData>[2];
			for (int i = 0; i < 2; i++)
			{
				_subSequence[i] = CharacterSelectSubSequence.Wait;
				_firstInformationQueues[i] = new Queue<FirstInformationData>();
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (!_isSkipProcess)
				{
					_monitor[i].SetData(this);
					_monitor[i].Initialize(i, userData.IsEntry && !userData.IsGuest());
				}
				_isNewcomerMode[i] = 0 < userData.NewCharaList.Count;
				int selectMapID = userData.Detail.SelectMapID;
				Sprite mapBgSprite = AssetManager.Instance().GetMapBgSprite(selectMapID, "UI_RunBG_01");
				Sprite mapBgSprite2 = AssetManager.Instance().GetMapBgSprite(selectMapID, "UI_RunBG_02");
				if (!_isSkipProcess)
				{
					_monitor[i].SetBackground(mapBgSprite, mapBgSprite2);
					_monitor[i].SetNewcomerMode(_isNewcomerMode[i]);
				}
				_slideScrollCount[i] = 0;
				_slotArray[i] = new CharacterData[5];
				_userFavoriteList[i] = new List<CharacterData>();
				_slideScrollTimer[i] = 0f;
				List<CharacterTabData> list = new List<CharacterTabData>();
				SortedDictionary<int, List<CharacterData>> sortedDictionary = new SortedDictionary<int, List<CharacterData>>();
				foreach (UserChara chara3 in Singleton<UserDataManager>.Instance.GetUserData(i).CharaList)
				{
					if (chara3.ID == 0)
					{
						continue;
					}
					CharaData chara = Singleton<DataManager>.Instance.GetChara(chara3.ID);
					if (!string.IsNullOrEmpty(chara.name.str))
					{
						MapColorData mapColorData = Singleton<DataManager>.Instance.GetMapColorData(chara.color.id);
						Color color;
						Color shadowColor;
						if (mapColorData == null)
						{
							color = new Color(1f, 1f, 1f);
							shadowColor = new Color(0.5f, 0.5f, 0.5f);
						}
						else
						{
							color = Utility.ConvertColor(mapColorData.Color);
							shadowColor = Utility.ConvertColor(mapColorData.ColorDark);
						}
						Texture2D characterTexture2D = container.assetManager.GetCharacterTexture2D(chara.imageFile);
						CharacterData item = new CharacterData
						{
							ID = chara.GetID(),
							UserChara = chara3,
							Data = chara,
							Texture = characterTexture2D,
							Color = color,
							ShadowColor = shadowColor
						};
						int priority = Singleton<DataManager>.Instance.GetCharaGenre(chara.genre.id).priority;
						if (userData.IsFavorite(UserData.Collection.Chara, chara3.ID))
						{
							_userFavoriteList[i].Add(item);
						}
						if (sortedDictionary.ContainsKey(priority))
						{
							sortedDictionary[priority].Add(item);
							continue;
						}
						List<CharacterData> value = new List<CharacterData> { item };
						sortedDictionary.Add(priority, value);
					}
				}
				_userCharaList.Add(new List<ReadOnlyCollection<CharacterData>>());
				foreach (int key in sortedDictionary.Keys)
				{
					sortedDictionary[key].Sort((CharacterData a, CharacterData b) => (a.ID >= b.ID) ? ((a.ID > b.ID) ? 1 : 0) : (-1));
					CharaGenreData charaGenre = Singleton<DataManager>.Instance.GetCharaGenre(sortedDictionary[key][0].Data.genre.id);
					Sprite sprite = Resources.Load<Sprite>(SelectorTab.CategoryTabImagePath + charaGenre.FileName);
					list.Add(new CharacterTabData(Utility.ConvertColor(charaGenre.Color), sprite, charaGenre.genreNameTwoLine));
					ReadOnlyCollection<CharacterData> item2 = new ReadOnlyCollection<CharacterData>(sortedDictionary[key]);
					_userCharaList[i].Add(item2);
				}
				_characterTabDataList.Add(new ReadOnlyCollection<CharacterTabData>(list));
				_isInitiative[i] = false;
				if (!_isSkipProcess)
				{
					_monitor[i].OnReturn();
				}
				if (!userData.IsEntry || userData.IsGuest() || !_isNewcomerMode[i])
				{
					continue;
				}
				List<int> newCharaList = userData.NewCharaList;
				_newcomerCharacterDatas[i] = new Queue<CharacterData>();
				foreach (int item4 in newCharaList)
				{
					CharaData chara2 = Singleton<DataManager>.Instance.GetChara(item4);
					Texture2D characterTexture2D2 = container.assetManager.GetCharacterTexture2D(chara2.imageFile);
					MapColorData mapColorData2 = Singleton<DataManager>.Instance.GetMapColorData(chara2.color.id);
					CharacterData item3 = new CharacterData
					{
						ID = chara2.GetID(),
						UserChara = new UserChara(item4),
						Data = chara2,
						Texture = characterTexture2D2,
						Color = new Color(1f, 1f, 1f),
						ShadowColor = Utility.ConvertColor(mapColorData2.ColorDark)
					};
					_newcomerCharacterDatas[i].Enqueue(item3);
				}
				if (!_isSkipProcess)
				{
					_monitor[i].SetNewcomer(_newcomerCharacterDatas[i].Peek());
				}
				userData.NewCharaList.Clear();
			}
			if (_isSkipProcess)
			{
				for (int j = 0; j < 2; j++)
				{
					_subSequence[j] = CharacterSelectSubSequence.Wait;
					if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
					{
						int[] charaSlot = Singleton<UserDataManager>.Instance.GetUserData(j).Detail.CharaSlot;
						_slotArray[j] = AutoSetCharacters(j, charaSlot[0]);
					}
				}
				_mainSequence = CharacterSelectSequence.Skip;
			}
			else
			{
				for (int k = 0; k < 2; k++)
				{
					SoundManager.PlayBGM(Mai2.Mai2Cue.Cue.BGM_ENTRY, k);
				}
			}
		}

		private void SetSlotView(int playerIndex)
		{
			_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
			_monitor[playerIndex].SetFirstInformationResume();
		}

		private void ShowFirstInformation(int playerIndex)
		{
			FirstInformationData firstInformationData = _firstInformationQueues[playerIndex].Dequeue();
			container.processManager.EnqueueMessage(playerIndex, firstInformationData.MessageID);
			SoundManager.PlayVoice(firstInformationData.VoiceCue, playerIndex);
			_skipTimes[playerIndex] = firstInformationData.SkipButtonTime;
		}

		public void Reset(int playerIndex)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
			if (!userData.IsGuest())
			{
				if (!userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstCharaSelect))
				{
					_firstInformationQueues[playerIndex].Enqueue(new FirstInformationData(WindowMessageID.CharaSelectFirst, Mai2.Voice_000001.Cue.VO_000179, 3000));
					userData.Detail.ContentBit.SetFlag(ContentBitID.FirstCharaSelect, flag: true);
				}
				bool flag = false;
				int[] charaSlot = userData.Detail.CharaSlot;
				foreach (int num in charaSlot)
				{
					if (num != 0 && !userData.Detail.IsMatchColor(playerIndex, num))
					{
						flag = true;
					}
				}
				if (flag && !userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstCharaGoodBad))
				{
					_firstInformationQueues[playerIndex].Enqueue(new FirstInformationData(WindowMessageID.CharaSelectGoodBad, 3000));
					userData.Detail.ContentBit.SetFlag(ContentBitID.FirstCharaGoodBad, flag: true);
				}
				if (!userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstCharaSelectDxPlus))
				{
					_firstInformationQueues[playerIndex].Enqueue(new FirstInformationData(WindowMessageID.CharaLockInfo, 3000));
					userData.Detail.ContentBit.SetFlag(ContentBitID.FirstCharaSelectDxPlus, flag: true);
				}
			}
			_monitor[playerIndex].OnReset();
			int[] charaSlot2 = userData.Detail.CharaSlot;
			int num2 = 0;
			for (int j = 0; j < charaSlot2.Length; j++)
			{
				int[] charaLockSlot = Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot;
				_monitor[playerIndex].SetLockIcon(j, charaLockSlot[j] > 0);
				if (charaSlot2[j] == 0)
				{
					_monitor[playerIndex].SetActiveCharacterSlot(j, isSlotActive: false);
					continue;
				}
				_monitor[playerIndex].SetActiveCharacterSlot(j, isSlotActive: true);
				CharaData data = Singleton<DataManager>.Instance.GetChara(charaSlot2[j]);
				UserChara userChara = userData.CharaList.Find((UserChara a) => a.ID == data.GetID());
				CharacterData characterData = null;
				foreach (ReadOnlyCollection<CharacterData> item in _userCharaList[playerIndex])
				{
					bool flag2 = false;
					foreach (CharacterData item2 in item)
					{
						if (item2.ID == charaSlot2[j])
						{
							characterData = item2;
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						break;
					}
				}
				if (characterData != null)
				{
					_slotArray[playerIndex][j] = characterData;
					_monitor[playerIndex].SetCharacterSlotData(j, characterData);
					num2 += (int)userChara.GetMovementParam(IsMatchColor(playerIndex, userChara.ID), j == 0);
				}
			}
			CheckNewcomerBlankSlot(playerIndex);
			_monitor[playerIndex].SetBlankSlots(_isBlanks[playerIndex]);
			if (_firstInformationQueues[playerIndex].Count > 0)
			{
				_subSequence[playerIndex] = CharacterSelectSubSequence.FirstInformation;
				_userTimer[playerIndex] = 0f;
				_isFirstInfoButton[playerIndex] = false;
				_onFirstInfoActions[playerIndex] = SetSlotView;
			}
			else
			{
				SetSlotView(playerIndex);
			}
			SetToDistance(playerIndex, num2);
			_monitor[playerIndex].SetCharacterSlotView(isSlotActive: true);
			ArrowDirection direction;
			int selectCharacotrDistance = GetSelectCharacotrDistance(playerIndex, out direction);
			_monitor[playerIndex].ResetCharacotrSelectController(selectCharacotrDistance, direction);
			_currentCharacterIndex[playerIndex] = 0;
			_currentCategoryIndex[playerIndex] = 0;
			_isInitiative[playerIndex] = true;
			SetInputLockInfo(playerIndex, 500f);
		}

		private void PostReset(int playerIndex)
		{
			if (_isNewcomerMode[playerIndex])
			{
				bool flag = true;
				bool flag2 = true;
				CharacterData[] array = _slotArray[playerIndex];
				foreach (CharacterData characterData in array)
				{
					if (characterData != null && characterData.ID == 0)
					{
						flag2 = false;
					}
				}
				array = _slotArray[playerIndex];
				foreach (CharacterData characterData2 in array)
				{
					if (characterData2 != null && characterData2.ID != 0)
					{
						flag = false;
						break;
					}
				}
				SoundManager.PlayVoice(flag ? Mai2.Voice_000001.Cue.VO_000179 : (flag2 ? Mai2.Voice_000001.Cue.VO_000178 : Mai2.Voice_000001.Cue.VO_000180), playerIndex);
			}
			else if (_firstInformationQueues[playerIndex].Count == 0)
			{
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000110, playerIndex);
			}
			if (_firstInformationQueues[playerIndex].Count > 0)
			{
				_monitor[playerIndex].SetFirstInformation();
				ShowFirstInformation(playerIndex);
			}
		}

		public void Escape(int playerIndex)
		{
			if (_isInitiative[playerIndex])
			{
				_isInitiative[playerIndex] = false;
				if (!_isSkipProcess)
				{
					_monitor[playerIndex].OnReturn();
				}
			}
		}

		private void CheckNextProcess()
		{
			bool flag = true;
			for (int i = 0; i < _monitor.Length; i++)
			{
				if (_subSequence[i] != CharacterSelectSubSequence.Wait)
				{
					flag = false;
				}
			}
			if (flag)
			{
				CleanUp();
			}
		}

		public void OnTimeUp(int i)
		{
		}

		private void OnTimeUp()
		{
			for (int i = 0; i < 2; i++)
			{
				if (_isNewcomerMode[i] && _slotArray[i] != null && _isBlanks[i])
				{
					int leaderID = ((_slotArray[i][0] == null) ? (-1) : _slotArray[i][0].ID);
					_slotArray[i] = AutoSetCharacters(i, leaderID);
				}
			}
			CleanUp();
		}

		private void CleanUp()
		{
			for (int i = 0; i < 2; i++)
			{
				Escape(i);
				Apply(i);
			}
			NextProcess();
		}

		public void NextProcess()
		{
			container.processManager.ClearTimeoutAction();
			container.processManager.SetVisibleTimers(isVisible: false);
			if (_isSkipProcess)
			{
				if (GameManager.MusicTrackNumber == 1)
				{
					container.processManager.AddProcess(new TicketSelectProcess(container), 50);
				}
				else
				{
					container.processManager.AddProcess(new MusicSelectInfoProcess(container), 50);
				}
				container.processManager.ReleaseProcess(this);
			}
			else if (GameManager.MusicTrackNumber == 1)
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new TicketSelectProcess(container)), 50);
			}
			else
			{
				container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectInfoProcess(container)), 50);
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_mainSequence)
			{
			case CharacterSelectSequence.Init:
				_mainSequence = CharacterSelectSequence.FadeIn;
				break;
			case CharacterSelectSequence.FadeIn:
				if (_timer >= 100f)
				{
					container.processManager.PrepareTimer(60, 0, isEntry: false, OnTimeUp);
					for (int k = 0; k < _monitor.Length; k++)
					{
						SetInputLockInfo(k, 1000f);
						if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
						{
							if (Singleton<UserDataManager>.Instance.GetUserData(k).IsGuest())
							{
								container.processManager.SetVisibleTimer(k, isVisible: false);
							}
							else
							{
								Reset(k);
							}
						}
						else
						{
							container.processManager.SetVisibleTimer(k, isVisible: false);
						}
					}
					container.processManager.NotificationFadeIn();
					_mainSequence = CharacterSelectSequence.Wait;
					_timer = 0f;
				}
				_timer += GameManager.GetGameMSecAdd();
				break;
			case CharacterSelectSequence.Wait:
				if (600f <= _timer)
				{
					for (int l = 0; l < _monitor.Length; l++)
					{
						SetInputLockInfo(l, 1000f);
						if (Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry)
						{
							if (Singleton<UserDataManager>.Instance.GetUserData(l).IsGuest())
							{
								container.processManager.EnqueueMessage(l, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
							}
							else
							{
								PostReset(l);
							}
						}
					}
					_mainSequence = CharacterSelectSequence.Reset;
					_timer = 0f;
				}
				else
				{
					_timer += GameManager.GetGameMSecAdd();
				}
				break;
			case CharacterSelectSequence.Reset:
			{
				for (int m = 0; m < 2; m++)
				{
					_monitor[m].ViewUpdate();
				}
				bool flag = false;
				for (int n = 0; n < 2; n++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(n).IsEntry && !Singleton<UserDataManager>.Instance.GetUserData(n).IsGuest() && _isInitiative[n])
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (1000f <= _timer)
					{
						_mainSequence = CharacterSelectSequence.Update;
						_timer = 0f;
					}
					else
					{
						_timer += GameManager.GetGameMSecAdd();
					}
				}
				break;
			}
			case CharacterSelectSequence.Update:
			{
				for (int i = 0; i < 2; i++)
				{
					if (_isInitiative[i] && Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && !Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
					{
						switch (_subSequence[i])
						{
						case CharacterSelectSubSequence.FirstInformation:
							UpdateFirstInformation(i);
							break;
						case CharacterSelectSubSequence.SlotView:
							UpdateSlotSelect(i);
							break;
						case CharacterSelectSubSequence.CharacterSelect:
							UpdateCharactorSelect(i);
							break;
						case CharacterSelectSubSequence.FavoriteList:
							UpdateFavoriteList(i);
							break;
						case CharacterSelectSubSequence.AutoSetMessage:
							UpdateAutoSetMessage(i);
							break;
						case CharacterSelectSubSequence.Newcomer:
							UpdateNewcomerCheck(i);
							break;
						}
						CountUpAnimationUpdate(i);
					}
				}
				for (int j = 0; j < 2; j++)
				{
					_monitor[j].ViewUpdate();
				}
				break;
			}
			case CharacterSelectSequence.Skip:
				OnTimeUp();
				_mainSequence = CharacterSelectSequence.Release;
				break;
			case CharacterSelectSequence.Release:
				break;
			}
		}

		public void StartCountUp(int playerIndex)
		{
			_isCountUpAnimation[playerIndex] = true;
		}

		private void CountUpAnimationUpdate(int playerIndex)
		{
			if (_isCountUpAnimation[playerIndex])
			{
				int characterMovementDistance = (int)Mathf.Lerp(0f, _toDistance[playerIndex], _countUpTimer[playerIndex] / 500f);
				_monitor[playerIndex].SetCharacterMovementDistance(characterMovementDistance);
				if (_countUpTimer[playerIndex] >= 500f)
				{
					_isCountUpAnimation[playerIndex] = false;
					_countUpTimer[playerIndex] = 0f;
					_monitor[playerIndex].SetCharacterMovementDistance(_toDistance[playerIndex]);
				}
				else
				{
					_countUpTimer[playerIndex] += GameManager.GetGameMSecAdd();
				}
			}
		}

		private void UpdateFirstInformation(int playerIndex)
		{
			_userTimer[playerIndex] += GameManager.GetGameMSecAdd();
			if (IsInputLock(playerIndex))
			{
				return;
			}
			if (_userTimer[playerIndex] >= (float)_skipTimes[playerIndex] && !_isFirstInfoButton[playerIndex])
			{
				_isFirstInfoButton[playerIndex] = true;
				_monitor[playerIndex].SetFirstInformationButton();
			}
			if (_userTimer[playerIndex] >= (float)_skipTimes[playerIndex] && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
			{
				_userTimer[playerIndex] = 10000f;
				_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
			}
			if (_userTimer[playerIndex] >= 10000f)
			{
				container.processManager.CloseWindow(playerIndex);
				if (_firstInformationQueues[playerIndex].Count > 0)
				{
					_userTimer[playerIndex] = 0f;
					_isFirstInfoButton[playerIndex] = false;
					_monitor[playerIndex].SetFirstInformationNext();
					ShowFirstInformation(playerIndex);
				}
				else
				{
					_onFirstInfoActions[playerIndex]?.Invoke(playerIndex);
				}
			}
		}

		private void UpdateSlotSelect(int playerIndex)
		{
			if (IsInputLock(playerIndex))
			{
				return;
			}
			bool flag = true;
			if (_isNewcomerMode[playerIndex])
			{
				flag = !_isBlanks[playerIndex];
			}
			if (flag && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
			{
				SetInputLockInfo(playerIndex, 1000f);
				_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
				if (_isNewcomerMode[playerIndex])
				{
					CheckNextNewcomer(playerIndex, isChange: false);
					return;
				}
				Apply(playerIndex);
				Escape(playerIndex);
				_onReturn?.Invoke(playerIndex);
				_subSequence[playerIndex] = CharacterSelectSubSequence.Wait;
				int num = ((playerIndex == 0) ? 1 : 0);
				if (Singleton<UserDataManager>.Instance.GetUserData(num).IsEntry)
				{
					container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				}
				CheckNextProcess();
			}
			else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.A6))
			{
				SelectSlot(playerIndex, 0);
			}
			else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.A7))
			{
				SelectSlot(playerIndex, 1);
			}
			else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.A8))
			{
				SelectSlot(playerIndex, 2);
			}
			else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.A1))
			{
				SelectSlot(playerIndex, 3);
			}
			else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.A2))
			{
				SelectSlot(playerIndex, 4);
			}
			else if (InputManager.GetInputDown(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
			{
				container.processManager.EnqueueMessage(playerIndex, WindowMessageID.CharacterSelectAutoMessage);
				_subSequence[playerIndex] = CharacterSelectSubSequence.AutoSetMessage;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
				_monitor[playerIndex].ToAutoSetMessage();
				SetInputLockInfo(playerIndex, 500f);
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button06))
			{
				Lock(playerIndex, 0);
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button07))
			{
				Lock(playerIndex, 1);
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button08))
			{
				Lock(playerIndex, 2);
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button01))
			{
				Lock(playerIndex, 3);
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button02))
			{
				Lock(playerIndex, 4);
			}
		}

		private void UpdateNewcomerCheck(int playerIndex)
		{
			if (IsInputLock(playerIndex))
			{
				return;
			}
			if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
			{
				ExchangeCharactor(playerIndex);
				_monitor[playerIndex].SetScrollCharacterCard(isVisible: false);
				uint num = 0u;
				for (int i = 0; i < _slotArray[playerIndex].Length; i++)
				{
					if (_slotArray[playerIndex][i] != null)
					{
						_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: true);
						_monitor[playerIndex].SetCharacterSlotData(i, _slotArray[playerIndex][i]);
						num += _slotArray[playerIndex][i].UserChara.GetMovementParam(IsMatchColor(playerIndex, _slotArray[playerIndex][i].UserChara.ID), i == 0);
					}
					else
					{
						_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: false);
					}
				}
				SetToDistance(playerIndex, (int)num);
				SetInputLockInfo(playerIndex, 1000f);
				_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
				CheckNextNewcomer(playerIndex, isChange: true);
			}
			else
			{
				if (!InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button05))
				{
					return;
				}
				_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button05);
				_monitor[playerIndex].ToSlotView(delegate
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[_currentSelectSlot[playerIndex]] > 0)
					{
						_monitor[playerIndex].SetLockIcon(_currentSelectSlot[playerIndex], isVisible: true);
					}
				});
				SetInputLockInfo(playerIndex, 1000f);
				_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
			}
		}

		private void UpdateCharactorSelect(int playerIndex)
		{
			if (_slideScrollCount[playerIndex] > 0)
			{
				if (_slideScrollTimer[playerIndex] >= 50f)
				{
					_slideScrollTimer[playerIndex] = 0f;
					_slideScrollCount[playerIndex]--;
					ScrollUpdate(playerIndex);
				}
				else
				{
					_slideScrollTimer[playerIndex] += GameManager.GetGameMSecAdd();
				}
			}
			else
			{
				if (IsInputLock(playerIndex))
				{
					return;
				}
				if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					ExchangeCharactor(playerIndex);
					_monitor[playerIndex].SetScrollCharacterCard(isVisible: false);
					uint num = 0u;
					for (int i = 0; i < _slotArray[playerIndex].Length; i++)
					{
						if (_slotArray[playerIndex][i] != null)
						{
							_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: true);
							_monitor[playerIndex].SetCharacterSlotData(i, _slotArray[playerIndex][i]);
							num += _slotArray[playerIndex][i].UserChara.GetMovementParam(IsMatchColor(playerIndex, _slotArray[playerIndex][i].UserChara.ID), i == 0);
						}
						else
						{
							_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: false);
						}
					}
					SetToDistance(playerIndex, (int)num);
					_monitor[playerIndex].ToSlotView(delegate
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[_currentSelectSlot[playerIndex]] != _slotArray[playerIndex][_currentSelectSlot[playerIndex]].ID)
						{
							Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[_currentSelectSlot[playerIndex]] = 0;
						}
						else if (Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[_currentSelectSlot[playerIndex]] > 0)
						{
							_monitor[playerIndex].SetLockIcon(_currentSelectSlot[playerIndex], isVisible: true);
						}
					});
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000110, playerIndex);
					SetInputLockInfo(playerIndex, 1000f);
					_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
					_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
				}
				else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button05))
				{
					_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button05);
					_monitor[playerIndex].SetImmediateChangeLockIcon(_currentSelectSlot[playerIndex], isVisible: false);
					_monitor[playerIndex].ToSlotView(delegate
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[_currentSelectSlot[playerIndex]] > 0)
						{
							_monitor[playerIndex].SetLockIcon(_currentSelectSlot[playerIndex], isVisible: true);
						}
					});
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000110, playerIndex);
					_monitor[playerIndex].SetScrollCharacterCard(isVisible: false);
					SetInputLockInfo(playerIndex, 1000f);
					_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
				}
				else if (InputManager.GetInputDown(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
				{
					if (_currentCharacterIndex[playerIndex] + 1 >= _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count)
					{
						_currentCategoryIndex[playerIndex] = ((_currentCategoryIndex[playerIndex] + 1 < _userCharaList[playerIndex].Count) ? (_currentCategoryIndex[playerIndex] + 1) : 0);
						_currentCharacterIndex[playerIndex] = 0;
					}
					else
					{
						_currentCharacterIndex[playerIndex]++;
					}
					ArrowDirection direction;
					int selectCharacotrDistance = GetSelectCharacotrDistance(playerIndex, out direction);
					_monitor[playerIndex].SetMovementDistance(selectCharacotrDistance, direction);
					_monitor[playerIndex].ScrollCharacter(Direction.Left, _currentCharacterIndex[playerIndex], _currentCategoryIndex[playerIndex], _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count);
					_monitor[playerIndex].SetScrollCharacterCard(InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L));
					SetInputLockInfo(playerIndex, 100f);
				}
				else if (InputManager.GetInputDown(playerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
				{
					if (_currentCharacterIndex[playerIndex] - 1 < 0)
					{
						if (_currentCategoryIndex[playerIndex] - 1 < 0)
						{
							_currentCategoryIndex[playerIndex] = _userCharaList[playerIndex].Count - 1;
							_currentCharacterIndex[playerIndex] = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count - 1;
						}
						else
						{
							_currentCategoryIndex[playerIndex]--;
							_currentCharacterIndex[playerIndex] = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count - 1;
						}
					}
					else
					{
						_currentCharacterIndex[playerIndex]--;
					}
					ArrowDirection direction2;
					int selectCharacotrDistance2 = GetSelectCharacotrDistance(playerIndex, out direction2);
					_monitor[playerIndex].SetMovementDistance(selectCharacotrDistance2, direction2);
					_monitor[playerIndex].ScrollCharacter(Direction.Right, _currentCharacterIndex[playerIndex], _currentCategoryIndex[playerIndex], _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count);
					_monitor[playerIndex].SetScrollCharacterCard(InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L));
					SetInputLockInfo(playerIndex, 100f);
				}
				else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.E2))
				{
					if (_currentCategoryIndex[playerIndex] + 1 >= _userCharaList[playerIndex].Count)
					{
						_currentCategoryIndex[playerIndex] = 0;
					}
					else
					{
						_currentCategoryIndex[playerIndex]++;
					}
					_currentCharacterIndex[playerIndex] = 0;
					_monitor[playerIndex].ScrollCategory(Direction.Left, _currentCategoryIndex[playerIndex]);
				}
				else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.E8))
				{
					if (_currentCategoryIndex[playerIndex] - 1 < 0)
					{
						_currentCategoryIndex[playerIndex] = _userCharaList[playerIndex].Count - 1;
					}
					else
					{
						_currentCategoryIndex[playerIndex]--;
					}
					_currentCharacterIndex[playerIndex] = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count - 1;
					_monitor[playerIndex].ScrollCategory(Direction.Right, _currentCategoryIndex[playerIndex]);
				}
				else if (InputManager.SlideAreaLr(playerIndex))
				{
					SetInputLockInfo(playerIndex, 250f);
					_slideScrollCount[playerIndex] = 0;
					_slideScrollToDirection = Direction.Right;
					int num2 = -1;
					int overCount;
					while (num2 >= -10 && !IsChangeCharacterCategory(playerIndex, num2, out overCount))
					{
						_slideScrollCount[playerIndex]++;
						num2--;
					}
					_slideScrollCount[playerIndex]--;
					ScrollUpdate(playerIndex);
				}
				else if (InputManager.SlideAreaRl(playerIndex))
				{
					SetInputLockInfo(playerIndex, 250f);
					_slideScrollCount[playerIndex] = 0;
					_slideScrollToDirection = Direction.Left;
					int overCount2;
					for (int j = 1; j <= 10 && !IsChangeCharacterCategory(playerIndex, j, out overCount2); j++)
					{
						_slideScrollCount[playerIndex]++;
					}
					_slideScrollCount[playerIndex]--;
					ScrollUpdate(playerIndex);
				}
				else
				{
					_monitor[playerIndex].SetScrollCharacterCard(isVisible: false);
				}
			}
		}

		private void UpdateFavoriteList(int playerIndex)
		{
			if (IsInputLock(playerIndex))
			{
				return;
			}
			if (_slideScrollCount[playerIndex] > 0)
			{
				_slideScrollCount[playerIndex]--;
				ScrollUpdate(playerIndex);
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
			{
				ExchangeCharactor(playerIndex);
				_monitor[playerIndex].SetScrollCharacterCard(isVisible: false);
				SetInputLockInfo(playerIndex, 1000f);
				_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
				_tempFavorites[playerIndex].Sort(delegate(CharacterData a, CharacterData b)
				{
					if (a.ID < b.ID)
					{
						return -1;
					}
					return (a.ID > b.ID) ? 1 : 0;
				});
				uint num = 0u;
				for (int i = 0; i < _slotArray[playerIndex].Length; i++)
				{
					if (_slotArray[playerIndex][i] != null)
					{
						_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: true);
						_monitor[playerIndex].SetCharacterSlotData(i, _slotArray[playerIndex][i]);
						num += _slotArray[playerIndex][i].UserChara.GetMovementParam(IsMatchColor(playerIndex, _slotArray[playerIndex][i].UserChara.ID), i == 0);
					}
					else
					{
						_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: false);
					}
				}
				SetToDistance(playerIndex, (int)num);
				_userFavoriteList[playerIndex] = new List<CharacterData>(_tempFavorites[playerIndex]);
				_monitor[playerIndex].ToSlotView();
				_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button05))
			{
				_subSequence[playerIndex] = CharacterSelectSubSequence.CharacterSelect;
				_tempFavorites[playerIndex].Sort(delegate(CharacterData a, CharacterData b)
				{
					if (a.ID < b.ID)
					{
						return -1;
					}
					return (a.ID > b.ID) ? 1 : 0;
				});
				_userFavoriteList[playerIndex] = new List<CharacterData>(_tempFavorites[playerIndex]);
				_monitor[playerIndex].ToCharacterSelect();
				SetInputLockInfo(playerIndex, 1000f);
				_monitor[playerIndex].PressedButton(InputManager.ButtonSetting.Button05);
			}
			else if (InputManager.GetInputDown(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
			{
				if (_favoriteListCurrentIndex[playerIndex] + 1 >= _userFavoriteList[playerIndex].Count)
				{
					_favoriteListCurrentIndex[playerIndex] = 0;
				}
				else
				{
					_favoriteListCurrentIndex[playerIndex]++;
				}
				_monitor[playerIndex].ScrollCharacter(Direction.Left, _favoriteListCurrentIndex[playerIndex], _currentCategoryIndex[playerIndex], _userFavoriteList[playerIndex].Count);
				_monitor[playerIndex].SetScrollCharacterCard(InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L));
				SetInputLockInfo(playerIndex, 100f);
			}
			else if (InputManager.GetInputDown(playerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
			{
				if (_favoriteListCurrentIndex[playerIndex] - 1 < 0)
				{
					_favoriteListCurrentIndex[playerIndex] = _userFavoriteList[playerIndex].Count - 1;
				}
				else
				{
					_favoriteListCurrentIndex[playerIndex]--;
				}
				_monitor[playerIndex].ScrollCharacter(Direction.Right, _favoriteListCurrentIndex[playerIndex], _currentCategoryIndex[playerIndex], _userFavoriteList[playerIndex].Count);
				_monitor[playerIndex].SetScrollCharacterCard(InputManager.GetInputLongPush(playerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L));
				SetInputLockInfo(playerIndex, 100f);
			}
			else if (InputManager.GetTouchPanelAreaDown(playerIndex, InputManager.TouchPanelArea.B5, InputManager.TouchPanelArea.B4, InputManager.TouchPanelArea.E5))
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
				int id = _userFavoriteList[playerIndex][_favoriteListCurrentIndex[playerIndex]].UserChara.ID;
				UserChara favo = userData.CharaList.Find((UserChara a) => a.ID == id);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
				if (userData.FlipFavorite(UserData.Collection.Chara, favo.ID))
				{
					_tempFavorites[playerIndex].Add(GetUserCharaData(playerIndex, 0));
				}
				else
				{
					_tempFavorites[playerIndex].Remove(_tempFavorites[playerIndex].Find((CharacterData a) => a.ID == favo.ID));
				}
				SetInputLockInfo(playerIndex, 100f);
				_monitor[playerIndex].SetVisibleFavoriteSlot(IsFavorite(playerIndex, favo));
			}
			else if (InputManager.SlideAreaLr(playerIndex))
			{
				SetInputLockInfo(playerIndex, 250f);
				_slideScrollCount[playerIndex] = 0;
				_slideScrollToDirection = Direction.Right;
				int num2 = -1;
				int overCount;
				while (num2 >= -10 && !IsChangeCharacterCategory(playerIndex, num2, out overCount))
				{
					_slideScrollCount[playerIndex]++;
					num2--;
				}
				_slideScrollCount[playerIndex]--;
				ScrollUpdate(playerIndex);
			}
			else if (InputManager.SlideAreaRl(playerIndex))
			{
				SetInputLockInfo(playerIndex, 250f);
				_slideScrollCount[playerIndex] = 0;
				_slideScrollToDirection = Direction.Left;
				int overCount2;
				for (int j = 1; j <= 10 && !IsChangeCharacterCategory(playerIndex, j, out overCount2); j++)
				{
					_slideScrollCount[playerIndex]++;
				}
				_slideScrollCount[playerIndex]--;
				ScrollUpdate(playerIndex);
			}
			else
			{
				_monitor[playerIndex].SetScrollCharacterCard(isVisible: false);
			}
		}

		private void UpdateAutoSetMessage(int playerIndex)
		{
			if (IsInputLock(playerIndex))
			{
				return;
			}
			if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
			{
				container.processManager.CloseWindow(playerIndex);
				if (_routine != null)
				{
					if (_routine[playerIndex] != null)
					{
						container.monoBehaviour.StopCoroutine(_routine[playerIndex]);
						_routine[playerIndex] = null;
					}
					_routine[playerIndex] = AutoSetExecuteCoroutine(playerIndex);
					container.monoBehaviour.StartCoroutine(_routine[playerIndex]);
				}
				SetInputLockInfo(playerIndex, 1000.3f);
				_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button05))
			{
				container.processManager.CloseWindow(playerIndex);
				_monitor[playerIndex].OutAutoSetMessage();
				SetInputLockInfo(playerIndex, 500f);
				_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, playerIndex);
			}
		}

		private CharacterData[] AutoSetCharacters(int playerIndex, int leaderID)
		{
			CharacterData[] array = new CharacterData[5];
			CharacterData[] array2 = AutoElection(playerIndex, leaderID);
			int num = 0;
			CharacterData[] array3 = array2;
			foreach (CharacterData characterData in array3)
			{
				array[num++] = characterData;
			}
			return array;
		}

		private IEnumerator AutoSetExecuteCoroutine(int playerIndex)
		{
			_slotArray[playerIndex] = AutoSetCharacters(playerIndex, _slotArray[playerIndex][0]?.ID ?? (-1));
			_monitor[playerIndex].OutAutoSetMessage();
			_monitor[playerIndex].ToSlotViewAutoSet();
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
			yield return null;
			uint num = 0u;
			for (int i = 0; i < _slotArray[playerIndex].Length; i++)
			{
				if (_slotArray[playerIndex][i] != null)
				{
					_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: true);
					_monitor[playerIndex].SetCharacterSlotData(i, _slotArray[playerIndex][i]);
					num += _slotArray[playerIndex][i].UserChara.GetMovementParam(IsMatchColor(playerIndex, _slotArray[playerIndex][i].UserChara.ID), i == 0);
					CharaData chara = Singleton<DataManager>.Instance.GetChara(_slotArray[playerIndex][i].ID);
					MessageCharactorInfomationData messageCharactorInfomationData = new MessageCharactorInfomationData(i, chara.genre.id, _slotArray[playerIndex][i].Texture, _slotArray[playerIndex][i].UserChara.Level, _slotArray[playerIndex][i].UserChara.Awakening, _slotArray[playerIndex][i].UserChara.NextAwakePercent, _slotArray[playerIndex][i].Color);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20021, playerIndex, messageCharactorInfomationData));
				}
				else
				{
					_monitor[playerIndex].SetActiveCharacterSlot(i, isSlotActive: false);
				}
			}
			SetToDistance(playerIndex, (int)num);
			if (_isNewcomerMode[playerIndex])
			{
				CheckNextNewcomer(playerIndex, isChange: false);
			}
		}

		private CharacterData[] AutoElection(int playerIndex, int leaderID)
		{
			CharacterData[] array = new CharacterData[5];
			List<int> list = null;
			if (_isNewcomerMode[playerIndex] && !_isSkipProcess)
			{
				list = new List<int>();
				foreach (CharacterData item in _newcomerCharacterDatas[playerIndex])
				{
					if (item.ID != _newcomerCharacterDatas[playerIndex].Peek().ID)
					{
						list.Add(item.ID);
					}
				}
			}
			if (leaderID >= 0)
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(leaderID);
			}
			UserChara[] array2 = Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.AutoComposeCharacterSlot(playerIndex, list, leaderID);
			List<ReadOnlyCollection<CharacterData>> list2 = _userCharaList[playerIndex];
			for (int i = 0; i < array2.Length; i++)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					int num = 0;
					while (num < list2[j].Count)
					{
						if (array2[i] == null || array2[i].ID != list2[j][num].ID)
						{
							num++;
							continue;
						}
						goto IL_00eb;
					}
					continue;
					IL_00eb:
					array[i] = list2[j][num];
					break;
				}
			}
			return array;
		}

		public override void OnRelease()
		{
			if (_routine != null)
			{
				for (int i = 0; i < 2; i++)
				{
					if (_routine[i] != null)
					{
						container.monoBehaviour.StopCoroutine(_routine[i]);
						_routine[i] = null;
					}
				}
			}
			_routine = null;
			if (_monitor != null)
			{
				for (int j = 0; j < 2; j++)
				{
					if (null != _monitor[j])
					{
						UnityEngine.Object.Destroy(_monitor[j].gameObject);
					}
				}
			}
			_monitor = null;
			Resources.UnloadUnusedAssets();
		}

		public override void OnLateUpdate()
		{
		}

		private void CheckNextNewcomer(int playerIndex, bool isChange)
		{
			_newcomerCharacterDatas[playerIndex].Dequeue();
			if (_newcomerCharacterDatas[playerIndex].Count > 0)
			{
				_subSequence[playerIndex] = CharacterSelectSubSequence.SlotView;
				CheckNewcomerBlankSlot(playerIndex);
				_monitor[playerIndex].SetNewcomer(_newcomerCharacterDatas[playerIndex].Peek());
				if (isChange)
				{
					_monitor[playerIndex].ToSlotView();
				}
				else
				{
					_monitor[playerIndex].NextNewcomer();
				}
				return;
			}
			int num = ((playerIndex == 0) ? 1 : 0);
			if (Singleton<UserDataManager>.Instance.GetUserData(num).IsEntry && _subSequence[num] != CharacterSelectSubSequence.Wait)
			{
				container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait);
			}
			_subSequence[playerIndex] = CharacterSelectSubSequence.Wait;
			bool flag = true;
			for (int i = 0; i < _monitor.Length; i++)
			{
				if (_subSequence[i] != CharacterSelectSubSequence.Wait)
				{
					flag = false;
				}
			}
			Action onAction = null;
			if (flag)
			{
				onAction = CleanUp;
			}
			_monitor[playerIndex].NewcomerStaging(onAction, isChange);
		}

		public int GetSelectCharacotrDistance(int playerIndex, out ArrowDirection direction)
		{
			int num;
			if (_subSequence[playerIndex] == CharacterSelectSubSequence.FavoriteList)
			{
				num = (int)((_favoriteListCurrentIndex[playerIndex] < _userFavoriteList[playerIndex].Count) ? _userFavoriteList[playerIndex][_favoriteListCurrentIndex[playerIndex]].UserChara.GetMovementParam(IsMatchColor(playerIndex, _userFavoriteList[playerIndex][_favoriteListCurrentIndex[playerIndex]].UserChara.ID)) : 0);
			}
			else
			{
				UserChara userChara = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][_currentCharacterIndex[playerIndex]].UserChara;
				bool matchColor = IsMatchColor(playerIndex, userChara.ID);
				num = (int)_userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][_currentCharacterIndex[playerIndex]].UserChara.GetMovementParam(matchColor);
			}
			if (num == _selectSlotCharacterDistance[playerIndex])
			{
				direction = ArrowDirection.Stay;
			}
			else if (num > _selectSlotCharacterDistance[playerIndex])
			{
				direction = ArrowDirection.Up;
			}
			else
			{
				direction = ArrowDirection.Down;
			}
			int num2 = 0;
			if (_subSequence[playerIndex] == CharacterSelectSubSequence.CharacterSelect || _subSequence[playerIndex] == CharacterSelectSubSequence.Newcomer)
			{
				num2 = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][_currentCharacterIndex[playerIndex]].ID;
			}
			else if (_subSequence[playerIndex] == CharacterSelectSubSequence.FavoriteList)
			{
				num2 = _userFavoriteList[playerIndex][_favoriteListCurrentIndex[playerIndex]].ID;
			}
			CharacterData[] array = _slotArray[playerIndex];
			foreach (CharacterData obj in array)
			{
				if (obj != null && obj.ID == num2)
				{
					direction = ArrowDirection.Stay;
					return (int)_currentTotalDistance[playerIndex];
				}
			}
			return (int)_currentTotalDistance[playerIndex] - _selectSlotCharacterDistance[playerIndex] + num;
		}

		public ArrowDirection GetArrowDirection(int playerIndex, int checkDistance)
		{
			int num = _selectSlotCharacterDistance[playerIndex];
			if (num < checkDistance)
			{
				return ArrowDirection.Up;
			}
			if (num > checkDistance)
			{
				return ArrowDirection.Down;
			}
			return ArrowDirection.Stay;
		}

		public bool IsMatchColor(int playerIndex, int ID)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.IsMatchColor(playerIndex, ID);
		}

		private void SelectSlot(int playerIndex, int slotIndex)
		{
			switch (_subSequence[playerIndex])
			{
			case CharacterSelectSubSequence.SlotView:
			{
				if (_slotArray[playerIndex][slotIndex] == null)
				{
					_currentCharacterIndex[playerIndex] = 0;
					_selectSlotCharacterDistance[playerIndex] = 0;
				}
				else
				{
					if (_isNewcomerMode[playerIndex] && _isBlanks[playerIndex])
					{
						break;
					}
					int id = _slotArray[playerIndex][slotIndex].Data.genre.id;
					for (int i = 0; i < _userCharaList[playerIndex].Count; i++)
					{
						if (_userCharaList[playerIndex][i][0].Data.genre.id == id)
						{
							_currentCategoryIndex[playerIndex] = i;
						}
					}
					for (int j = 0; j < _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count; j++)
					{
						if (_userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][j].ID == _slotArray[playerIndex][slotIndex].ID)
						{
							_currentCharacterIndex[playerIndex] = j;
							break;
						}
					}
					_selectSlotCharacterDistance[playerIndex] = (int)_slotArray[playerIndex][slotIndex].UserChara.GetMovementParam(IsMatchColor(playerIndex, _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][_currentCharacterIndex[playerIndex]].UserChara.ID), slotIndex == 0);
				}
				_currentSelectSlot[playerIndex] = slotIndex;
				_subSequence[playerIndex] = (_isNewcomerMode[playerIndex] ? CharacterSelectSubSequence.Newcomer : CharacterSelectSubSequence.CharacterSelect);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
				_monitor[playerIndex].SetImmediateChangeLockIcon(slotIndex, isVisible: false);
				_monitor[playerIndex].ToCharacterSelect(slotIndex);
				uint num = 0u;
				CharacterData[] array = _slotArray[playerIndex];
				foreach (CharacterData characterData in array)
				{
					if (characterData != null)
					{
						bool matchColor = IsMatchColor(playerIndex, characterData.UserChara.ID);
						num += characterData.UserChara.GetMovementParam(matchColor);
					}
				}
				_currentTotalDistance[playerIndex] = num;
				ArrowDirection direction;
				int selectCharacotrDistance = GetSelectCharacotrDistance(playerIndex, out direction);
				_monitor[playerIndex].SetMovementDistance(selectCharacotrDistance, direction);
				if (_isNewcomerMode[playerIndex])
				{
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000178, playerIndex);
				}
				else
				{
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000111, playerIndex);
				}
				SetInputLockInfo(playerIndex, 1000f);
				break;
			}
			case CharacterSelectSubSequence.CharacterSelect:
				SetInputLockInfo(playerIndex, 1000f);
				break;
			}
		}

		private void ExchangeCharactor(int playerIndex)
		{
			int num = 0;
			if (_subSequence[playerIndex] == CharacterSelectSubSequence.CharacterSelect)
			{
				num = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][_currentCharacterIndex[playerIndex]].ID;
			}
			else if (_subSequence[playerIndex] == CharacterSelectSubSequence.FavoriteList)
			{
				num = _userFavoriteList[playerIndex][_favoriteListCurrentIndex[playerIndex]].ID;
			}
			else if (_subSequence[playerIndex] == CharacterSelectSubSequence.Newcomer)
			{
				num = _newcomerCharacterDatas[playerIndex].Peek().ID;
			}
			int num2 = -1;
			for (int i = 0; i < _slotArray[playerIndex].Length; i++)
			{
				if (_slotArray[playerIndex][i] != null && _slotArray[playerIndex][i].ID == num)
				{
					num2 = i;
					break;
				}
			}
			CharacterData obj = _slotArray[playerIndex][_currentSelectSlot[playerIndex]];
			if (obj != null && obj.ID == num)
			{
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_ANSWER, playerIndex);
				return;
			}
			Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[_currentSelectSlot[playerIndex]] = 0;
			_monitor[playerIndex].SetLockIcon(_currentSelectSlot[playerIndex], isVisible: false);
			if (num2 >= 0)
			{
				Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[num2] = 0;
				_monitor[playerIndex].SetLockIcon(num2, isVisible: false);
			}
			int num3 = -1;
			for (int j = 0; j < _slotArray[playerIndex].Length; j++)
			{
				if (_slotArray[playerIndex][j] == null)
				{
					num3 = j;
					break;
				}
			}
			if (0 <= num3 && num3 < _currentSelectSlot[playerIndex])
			{
				_currentSelectSlot[playerIndex] = num3;
			}
			if (num2 >= 0)
			{
				if (_slotArray[playerIndex][_currentSelectSlot[playerIndex]] == null)
				{
					_currentSelectSlot[playerIndex] = num2;
				}
				_slotArray[playerIndex][num2] = _slotArray[playerIndex][_currentSelectSlot[playerIndex]];
				CharacterData characterData = ((!_isNewcomerMode[playerIndex]) ? ((_subSequence[playerIndex] == CharacterSelectSubSequence.CharacterSelect) ? _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][_currentCharacterIndex[playerIndex]] : _userFavoriteList[playerIndex][_favoriteListCurrentIndex[playerIndex]]) : _newcomerCharacterDatas[playerIndex].Peek());
				_slotArray[playerIndex][_currentSelectSlot[playerIndex]] = characterData;
				if (_slotArray[playerIndex][num2] == null)
				{
					_monitor[playerIndex].ResetCharacterSlotData(num2);
					_monitor[playerIndex].SetActiveCharacterSlot(num2, isSlotActive: false);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20023, playerIndex, num2));
				}
				else
				{
					_monitor[playerIndex].SetCharacterSlotData(num2, _slotArray[playerIndex][num2]);
					_monitor[playerIndex].SetActiveCharacterSlot(num2, isSlotActive: true);
					CharaData chara = Singleton<DataManager>.Instance.GetChara(_slotArray[playerIndex][num2].ID);
					MessageCharactorInfomationData messageCharactorInfomationData = new MessageCharactorInfomationData(num2, chara.genre.id, _slotArray[playerIndex][num2].Texture, _slotArray[playerIndex][num2].UserChara.Level, _slotArray[playerIndex][num2].UserChara.Awakening, _slotArray[playerIndex][num2].UserChara.NextAwakePercent, _slotArray[playerIndex][num2].Color);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20021, playerIndex, messageCharactorInfomationData));
				}
				_monitor[playerIndex].SetActiveCharacterSlot(_currentSelectSlot[playerIndex], isSlotActive: true);
				_monitor[playerIndex].SetCharacterSlotData(_currentSelectSlot[playerIndex], _slotArray[playerIndex][_currentSelectSlot[playerIndex]]);
			}
			else
			{
				CharacterData characterData2 = ((!_isNewcomerMode[playerIndex]) ? ((_subSequence[playerIndex] == CharacterSelectSubSequence.CharacterSelect) ? _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]][_currentCharacterIndex[playerIndex]] : _userFavoriteList[playerIndex][_favoriteListCurrentIndex[playerIndex]]) : _newcomerCharacterDatas[playerIndex].Peek());
				_slotArray[playerIndex][_currentSelectSlot[playerIndex]] = characterData2;
				_monitor[playerIndex].SetActiveCharacterSlot(_currentSelectSlot[playerIndex], isSlotActive: true);
				_monitor[playerIndex].SetCharacterSlotData(_currentSelectSlot[playerIndex], _slotArray[playerIndex][_currentSelectSlot[playerIndex]]);
			}
			CharaData chara2 = Singleton<DataManager>.Instance.GetChara(_slotArray[playerIndex][_currentSelectSlot[playerIndex]].ID);
			MessageCharactorInfomationData messageCharactorInfomationData2 = new MessageCharactorInfomationData(_currentSelectSlot[playerIndex], chara2.genre.id, _slotArray[playerIndex][_currentSelectSlot[playerIndex]].Texture, _slotArray[playerIndex][_currentSelectSlot[playerIndex]].UserChara.Level, _slotArray[playerIndex][_currentSelectSlot[playerIndex]].UserChara.Awakening, _slotArray[playerIndex][_currentSelectSlot[playerIndex]].UserChara.NextAwakePercent, _slotArray[playerIndex][_currentSelectSlot[playerIndex]].Color);
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20021, playerIndex, messageCharactorInfomationData2));
		}

		private void Apply(int playerIndex)
		{
			int[] array = new int[5];
			for (int i = 0; i < _slotArray[playerIndex].Length; i++)
			{
				if (_slotArray[playerIndex][i] != null)
				{
					array[i] = _slotArray[playerIndex][i].ID;
				}
			}
			Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaSlot = array;
		}

		private void Cancel(int playerIndex)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
			int[] charaSlot = userData.Detail.CharaSlot;
			List<UserChara> charaList = userData.CharaList;
			for (int i = 0; i < charaSlot.Length; i++)
			{
				CharaData charaData = Singleton<DataManager>.Instance.GetChara(charaSlot[i]);
				UserChara userChara = charaList.Find((UserChara a) => a.ID == charaData.GetID());
				_monitor[playerIndex].ResetCharacterSlotData(i);
				if (userChara != null)
				{
					MapColorData mapColorData = Singleton<DataManager>.Instance.GetMapColorData(charaData.color.id);
					Color regionColor = new Color32(mapColorData.Color.R, mapColorData.Color.G, mapColorData.Color.B, byte.MaxValue);
					Texture2D characterTexture2D = container.assetManager.GetCharacterTexture2D(charaData.imageFile);
					MessageCharactorInfomationData messageCharactorInfomationData = new MessageCharactorInfomationData(i, charaData.genre.id, characterTexture2D, userChara.Level, userChara.Awakening, userChara.NextAwakePercent, regionColor);
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20021, playerIndex, messageCharactorInfomationData));
				}
				else
				{
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20023, playerIndex, i));
				}
			}
		}

		private void Lock(int playerIndex, int slotIndex)
		{
			if (_slotArray[playerIndex][slotIndex] != null)
			{
				int[] charaLockSlot = Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot;
				if (charaLockSlot[slotIndex] == 0)
				{
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_BOOKMARK_ON, playerIndex);
					charaLockSlot[slotIndex] = _slotArray[playerIndex][slotIndex].ID;
					_monitor[playerIndex].SetLockIcon(slotIndex, isVisible: true);
				}
				else
				{
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_BOOKMARK_OFF, playerIndex);
					charaLockSlot[slotIndex] = 0;
					_monitor[playerIndex].SetLockIcon(slotIndex, isVisible: false);
				}
				Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot = charaLockSlot;
			}
		}

		private void CheckNewcomerBlankSlot(int i)
		{
			_isBlanks[i] = false;
			for (int j = 0; j < _slotArray[i].Length; j++)
			{
				if (_slotArray[i][j] == null)
				{
					_isBlanks[i] = true;
					break;
				}
			}
			_monitor[i].SetBlankSlots(_isBlanks[i]);
			for (int k = 0; k < _slotArray[i].Length; k++)
			{
				bool isVisible = !_isBlanks[i] || _slotArray[i][k] == null;
				_monitor[i].SetSlotTouchMessageVisible(k, isVisible);
			}
		}

		private void ScrollUpdate(int playerIndex)
		{
			int currentIndex;
			int count;
			if (_slideScrollToDirection == Direction.Right)
			{
				if (_subSequence[playerIndex] == CharacterSelectSubSequence.CharacterSelect)
				{
					if (_currentCharacterIndex[playerIndex] - 1 < 0)
					{
						if (_currentCategoryIndex[playerIndex] - 1 < 0)
						{
							_currentCategoryIndex[playerIndex] = _userCharaList[playerIndex].Count - 1;
							_currentCharacterIndex[playerIndex] = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count - 1;
						}
						else
						{
							_currentCategoryIndex[playerIndex]--;
							_currentCharacterIndex[playerIndex] = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count - 1;
						}
					}
					else
					{
						_currentCharacterIndex[playerIndex]--;
					}
					currentIndex = _currentCharacterIndex[playerIndex];
					count = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count;
				}
				else
				{
					if (_favoriteListCurrentIndex[playerIndex] - 1 < 0)
					{
						_favoriteListCurrentIndex[playerIndex] = _userFavoriteList[playerIndex].Count - 1;
					}
					else
					{
						_favoriteListCurrentIndex[playerIndex]--;
					}
					currentIndex = _favoriteListCurrentIndex[playerIndex];
					count = _userFavoriteList[playerIndex].Count;
				}
			}
			else if (_subSequence[playerIndex] == CharacterSelectSubSequence.CharacterSelect)
			{
				if (_currentCharacterIndex[playerIndex] + 1 >= _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count)
				{
					_currentCategoryIndex[playerIndex] = ((_currentCategoryIndex[playerIndex] + 1 < _userCharaList[playerIndex].Count) ? (_currentCategoryIndex[playerIndex] + 1) : 0);
					_currentCharacterIndex[playerIndex] = 0;
				}
				else
				{
					_currentCharacterIndex[playerIndex]++;
				}
				currentIndex = _currentCharacterIndex[playerIndex];
				count = _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count;
			}
			else
			{
				if (_favoriteListCurrentIndex[playerIndex] + 1 >= _userFavoriteList[playerIndex].Count)
				{
					_favoriteListCurrentIndex[playerIndex] = 0;
				}
				else
				{
					_favoriteListCurrentIndex[playerIndex]++;
				}
				currentIndex = _favoriteListCurrentIndex[playerIndex];
				count = _userFavoriteList[playerIndex].Count;
			}
			_monitor[playerIndex].ScrollCharacter(_slideScrollToDirection, currentIndex, _currentCategoryIndex[playerIndex], count);
			ArrowDirection direction;
			int selectCharacotrDistance = GetSelectCharacotrDistance(playerIndex, out direction);
			_monitor[playerIndex].SetMovementDistance(selectCharacotrDistance, direction);
		}

		private void SetToDistance(int playerIndex, int distance)
		{
			_monitor[playerIndex].SetCharacterMovementDistance(0);
			_toDistance[playerIndex] = distance;
		}

		private bool IsFavorite(int index, UserChara chara)
		{
			return CharacterSelectMonitor.IsFavorite(index, chara);
		}

		public CharacterData GetSlotData(int playerIndex, int slotIndex)
		{
			return _slotArray[playerIndex][slotIndex];
		}

		public bool IsSelectedCharacter(int playerIndex, int id)
		{
			CharacterData[] array = _slotArray[playerIndex];
			foreach (CharacterData obj in array)
			{
				if (obj != null && obj.ID == id)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsLeaderCharacter(int playerIndex, int id)
		{
			for (int i = 0; i < _slotArray[playerIndex].Length; i++)
			{
				if (_slotArray[playerIndex][i] != null && _slotArray[playerIndex][i].ID == id && i == 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsNewcomerModeSlotActive(int playerIndex, int slotIndex)
		{
			bool flag = false;
			if (!_isNewcomerMode[playerIndex])
			{
				return true;
			}
			if (_slotArray[playerIndex][slotIndex] == null)
			{
				return true;
			}
			return !_isBlanks[playerIndex];
		}

		public CharacterData GetUserCharaData(int playerIndex, int diffIndex)
		{
			int num = _currentCharacterIndex[playerIndex] + diffIndex;
			int num2 = _currentCategoryIndex[playerIndex];
			CharacterSelectSubSequence characterSelectSubSequence = _subSequence[playerIndex];
			if (characterSelectSubSequence != CharacterSelectSubSequence.CharacterSelect && characterSelectSubSequence == CharacterSelectSubSequence.FavoriteList)
			{
				for (num = _favoriteListCurrentIndex[playerIndex] + diffIndex; num >= _userFavoriteList[playerIndex].Count; num -= _userFavoriteList[playerIndex].Count)
				{
				}
				while (num < 0)
				{
					num = _userFavoriteList[playerIndex].Count + num;
				}
				if (num >= _userFavoriteList[playerIndex].Count)
				{
					return CharacterData.Empty();
				}
				return _userFavoriteList[playerIndex][num];
			}
			if (num >= _userCharaList[playerIndex][num2].Count)
			{
				while (num >= _userCharaList[playerIndex][num2].Count)
				{
					num -= _userCharaList[playerIndex][num2].Count;
					num2 = ((num2 + 1 < _userCharaList[playerIndex].Count) ? (num2 + 1) : 0);
				}
			}
			else if (num < 0)
			{
				while (num < 0)
				{
					num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_userCharaList[playerIndex].Count - 1));
					num = _userCharaList[playerIndex][num2].Count + num;
				}
			}
			return _userCharaList[playerIndex][num2][num];
		}

		public bool IsChangeCharacterCategory(int playerIndex, int diffIndex, out int overCount)
		{
			int num = _currentCharacterIndex[playerIndex] + diffIndex;
			int num2 = _currentCategoryIndex[playerIndex];
			overCount = 0;
			if (_subSequence[playerIndex] == CharacterSelectSubSequence.FavoriteList)
			{
				return false;
			}
			if (num >= _userCharaList[playerIndex][num2].Count)
			{
				while (num >= _userCharaList[playerIndex][num2].Count)
				{
					overCount++;
					num = num - _userCharaList[playerIndex][num2].Count - 1;
					num2 = ((num2 + 1 < _userCharaList[playerIndex].Count) ? (num2 + 1) : 0);
				}
			}
			else if (num < -1)
			{
				while (num < 0)
				{
					overCount--;
					num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_userCharaList[playerIndex].Count - 1));
					num = _userCharaList[playerIndex][num2].Count + num + 1;
				}
			}
			if (num != _userCharaList[playerIndex][num2].Count)
			{
				return num == -1;
			}
			return true;
		}

		public string GetCharacterCategoryName(int playerIndex, int diffIndex)
		{
			int num = _currentCategoryIndex[playerIndex] + diffIndex;
			if (_userCharaList[playerIndex].Count > 0)
			{
				while (num >= _userCharaList[playerIndex].Count)
				{
					num -= _userCharaList[playerIndex].Count;
				}
				while (num < 0)
				{
					num = _userCharaList[playerIndex].Count + num;
				}
			}
			int iD = _userCharaList[playerIndex][num][0].UserChara.ID;
			return Singleton<DataManager>.Instance.GetChara(iD).genre.str;
		}

		public CharacterMapColorData GetMapColorData(int colorID)
		{
			if (MapMaster.GetSlotData(colorID) != null)
			{
				MapMaster.GetSlotData(colorID).Load();
			}
			return MapMaster.GetSlotData(colorID);
		}

		public int GetCurrentCharacterListMax(int playerIndex)
		{
			return _userCharaList[playerIndex][_currentCategoryIndex[playerIndex]].Count;
		}

		public int GetCurrentCharacterIndex(int playerIndex)
		{
			return _currentCharacterIndex[playerIndex];
		}

		public int GetCurrentCategoryIndex(int playerIndex)
		{
			return _currentCategoryIndex[playerIndex];
		}

		public bool IsLockedSlot(int playerIndex, int slotIndex)
		{
			return Singleton<UserDataManager>.Instance.GetUserData(playerIndex).Detail.CharaLockSlot[slotIndex] > 0;
		}
	}
}
