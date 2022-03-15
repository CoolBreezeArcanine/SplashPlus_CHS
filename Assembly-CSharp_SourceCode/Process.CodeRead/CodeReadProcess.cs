using System;
using System.Collections;
using System.Collections.Generic;
using DB;
using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.MaiStudio.CardTypeName;
using Manager.UserDatas;
using Monitor.CodeRead;
using UI.DaisyChainList;
using UnityEngine;

namespace Process.CodeRead
{
	public class CodeReadProcess : ProcessBase, ICodeReadProcess
	{
		public enum CodeReaderState : byte
		{
			Normal,
			Insert,
			Error
		}

		private enum CodeReadMainSequence : byte
		{
			FadeIn,
			Wait,
			Update,
			Released
		}

		private enum CodeReadSubSequence : byte
		{
			Init,
			FirstInformation,
			FristCardRead,
			CardSelect,
			ReadMessageWindow,
			Wait
		}

		private enum ResultStatus
		{
			Failed,
			IdenticalFailed,
			PromotionFailed,
			UsedReleaseFailed,
			UnownedFailed,
			ExpiredFailed,
			Success
		}

		public enum CardStatus : byte
		{
			Normal,
			Unowned,
			Expired
		}

		private class ReadResult
		{
			public ReadCard ReadCard { get; }

			public ResultStatus Status { get; }

			public int ID { get; }

			public ReadResult(int id, ReadCard readCard, ResultStatus status)
			{
				ID = id;
				ReadCard = readCard;
				Status = status;
			}
		}

		public class ReadCard
		{
			public CardKind Kind;

			public Sprite Background;

			public Sprite Character;

			public Sprite Frame;
		}

		private struct ReadData
		{
			public int CardID { get; private set; }

			public int CharaID { get; private set; }

			public void Set(int cardID, int charaID)
			{
				CardID = cardID;
				CharaID = charaID;
			}
		}

		public const int UseCardHistory = 4;

		private CodeReadMainSequence _state;

		private CodeReadSubSequence[] _subSequences;

		private CodeReadSubSequence[] _prevSequense;

		private CodeReadSubSequence[] _lastSequences;

		private CodeReaderState[] _codeReaderStates;

		private FusionType _tempFusionType;

		private CodeReadMonitor[] _monitors;

		private ReadResult[] _readResults;

		private QrCamera _qrReader;

		private ReadData[] _currentReadDatas;

		private ReadData[] _prevCurrentReadDatas;

		private List<int>[] _cardLists;

		private bool _isFadeIn;

		private float _timer;

		private float[] _userTimer;

		private float[] _readWaitTimer;

		private int[] _selectCardIndexs;

		private int[] _decisionIndexs;

		private int[] _prevReadCardIDs;

		private int[] _prevReadParams;

		private int[] _prevReadPromoIDs;

		private bool[] _isEntry;

		private bool[] _isUseReadCards;

		private Action<int>[] _onFirstInformations;

		private bool[] _isFirstInformationSkipShow;

		private string[] _cardEffectText;

		private string[] boostText;

		private ReadCard[] readCard;

		private CardStatus[] status;

		private IEnumerator _croutine;

		private readonly bool[] _debugRead = new bool[2];

		public int[] SelectCardIndexs => _selectCardIndexs;

		public CodeReadProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			if (_qrReader != null)
			{
				_qrReader.Pause();
				_qrReader = null;
			}
			if (_croutine != null)
			{
				container.monoBehaviour.StopCoroutine(_croutine);
				_croutine = null;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				UnityEngine.Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			_qrReader = MechaManager.QrReader;
			_isFadeIn = false;
			_codeReaderStates = new CodeReaderState[2];
			if (Singleton<SystemConfig>.Instance.config.IsDummyQrCamera)
			{
				for (int i = 0; i < 2; i++)
				{
					_codeReaderStates[i] = CodeReaderState.Normal;
				}
			}
			else
			{
				for (int j = 0; j < 2; j++)
				{
					if (!_qrReader.Exists(j))
					{
						_codeReaderStates[j] = CodeReaderState.Error;
					}
					else
					{
						_codeReaderStates[j] = CodeReaderState.Normal;
					}
				}
			}
			GameObject prefs = Resources.Load<GameObject>("Process/CodeRead/CodeReadProcess");
			_monitors = new CodeReadMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<CodeReadMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<CodeReadMonitor>()
			};
			_cardLists = new List<int>[_monitors.Length];
			_onFirstInformations = new Action<int>[_monitors.Length];
			_subSequences = new CodeReadSubSequence[_monitors.Length];
			_prevSequense = new CodeReadSubSequence[_monitors.Length];
			_lastSequences = new CodeReadSubSequence[_monitors.Length];
			status = new CardStatus[_monitors.Length];
			_readResults = new ReadResult[_monitors.Length];
			_currentReadDatas = new ReadData[_monitors.Length];
			_prevCurrentReadDatas = new ReadData[_monitors.Length];
			readCard = new ReadCard[_monitors.Length];
			_cardEffectText = new string[_monitors.Length];
			boostText = new string[_monitors.Length];
			_readWaitTimer = new float[_monitors.Length];
			_userTimer = new float[_monitors.Length];
			_selectCardIndexs = new int[_monitors.Length];
			_decisionIndexs = new int[_monitors.Length];
			_prevReadCardIDs = new int[_monitors.Length];
			_prevReadParams = new int[_monitors.Length];
			_prevReadPromoIDs = new int[_monitors.Length];
			_isEntry = new bool[_monitors.Length];
			_isUseReadCards = new bool[_monitors.Length];
			_isFirstInformationSkipShow = new bool[_monitors.Length];
			CardReadPlayer cardReadPlayer = CardReadPlayer.Player0102;
			bool isSinglePlay = false;
			for (int k = 0; k < _monitors.Length; k++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(k);
				_isEntry[k] = userData.IsEntry;
				_isUseReadCards[k] = true;
				_cardLists[k] = new List<int>(5) { 0 };
				_selectCardIndexs[k] = 0;
				_decisionIndexs[k] = -1;
				_prevReadCardIDs[k] = 0;
				_prevReadParams[k] = 0;
				_prevReadPromoIDs[k] = 0;
				_monitors[k].Initialize(k, _isEntry[k]);
				_prevSequense[k] = CodeReadSubSequence.Init;
				if (!_isEntry[k] || userData.IsGuest())
				{
					isSinglePlay = true;
					cardReadPlayer = ((k == 0) ? CardReadPlayer.Player02 : CardReadPlayer.Player01);
					_subSequences[k] = CodeReadSubSequence.Wait;
					continue;
				}
				bool flag = false;
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20010, k, _isEntry[k], flag));
				UserDetail detail = Singleton<UserDataManager>.Instance.GetUserData(k).Detail;
				UserOption option = Singleton<UserDataManager>.Instance.GetUserData(k).Option;
				MessageUserInformationData messageUserInformationData = new MessageUserInformationData(k, container.assetManager, detail, option.DispRate, isSubMonitor: true);
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20011, k, messageUserInformationData));
				container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20030, k, true));
				int num = 0;
				foreach (int selectedCard in userData.Extend.SelectedCardList)
				{
					if (selectedCard != 0)
					{
						_cardLists[k].Add(selectedCard);
						num++;
					}
					if (num >= 4)
					{
						break;
					}
				}
				if (num > 0)
				{
					_subSequences[k] = CodeReadSubSequence.CardSelect;
					continue;
				}
				_subSequences[k] = CodeReadSubSequence.FristCardRead;
				if (_codeReaderStates[k] == CodeReaderState.Error)
				{
					_subSequences[k] = CodeReadSubSequence.Wait;
				}
			}
			for (int l = 0; l < _monitors.Length; l++)
			{
				UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(l);
				bool flag2 = userData2.IsGuest();
				if (userData2.IsEntry && flag2)
				{
					container.processManager.EnqueueMessage(l, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
					_monitors[l].SetVisibleBlur(isVisible: true);
				}
				if (userData2.IsEntry)
				{
					_monitors[l].SetData(this, flag2, isSinglePlay);
				}
				if (_subSequences[l] == CodeReadSubSequence.FristCardRead)
				{
					_monitors[l].SetCardReadPlayer(cardReadPlayer);
				}
			}
			if (_subSequences[0] == CodeReadSubSequence.Wait && _subSequences[1] == CodeReadSubSequence.Wait)
			{
				OnTimeUp();
				return;
			}
			_qrReader.Play();
			container.processManager.PrepareTimer(60, 0, isEntry: false, OnTimeUp);
			container.processManager.NotificationFadeIn();
			_isFadeIn = true;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case CodeReadMainSequence.FadeIn:
				if (_timer >= 600f)
				{
					_timer = 0f;
					_state = CodeReadMainSequence.Update;
					for (int j = 0; j < _monitors.Length; j++)
					{
						StartLogic(j);
					}
					for (int k = 0; k < _monitors.Length; k++)
					{
						PostStartLogic(k);
					}
				}
				_timer += GameManager.GetGameMSecAdd();
				break;
			case CodeReadMainSequence.Update:
			{
				_qrReader.Update();
				for (int i = 0; i < _monitors.Length; i++)
				{
					LogicUpdate(i);
				}
				break;
			}
			}
			CodeReadMonitor[] monitors = _monitors;
			for (int l = 0; l < monitors.Length; l++)
			{
				monitors[l].ViewUpdate();
			}
		}

		public override void OnLateUpdate()
		{
		}

		private void StartLogic(int playerIndex)
		{
			switch (_subSequences[playerIndex])
			{
			case CodeReadSubSequence.FristCardRead:
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000020, playerIndex);
				_readWaitTimer[playerIndex] = 1500f;
				_monitors[playerIndex].FirstCardReadPlayIn(isInit: true);
				if (_codeReaderStates[playerIndex] == CodeReaderState.Error)
				{
					_monitors[playerIndex].ReadImageSetInsert(CodeReaderState.Error);
				}
				SetInputLockInfo(playerIndex, 1500f);
				break;
			case CodeReadSubSequence.CardSelect:
			{
				SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000020, playerIndex);
				_readWaitTimer[playerIndex] = 1500f;
				_monitors[playerIndex].CardSelectPlayIn(isDecision: true);
				ButtonCheck(playerIndex);
				GetUserCard(playerIndex, GetCardTypeData(GetCardListID(playerIndex, 0)).name.id, out var cardStatus);
				CardTypeData cardTypeData = GetCardTypeData((cardStatus != 0) ? 1 : GetCardListID(playerIndex, 0));
				_monitors[playerIndex].SetCardInfoData(cardTypeData.title, cardTypeData.effectText, GetBoostDeadLineText(playerIndex, GetCardListID(playerIndex, 0)));
				if (_codeReaderStates[playerIndex] == CodeReaderState.Error)
				{
					_monitors[playerIndex].ReadImageSetInsert(CodeReaderState.Error);
				}
				SetInputLockInfo(playerIndex, 800f);
				break;
			}
			case CodeReadSubSequence.Wait:
				if (_codeReaderStates[playerIndex] == CodeReaderState.Error && _isEntry[0] && _isEntry[1])
				{
					container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				}
				_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
				break;
			case CodeReadSubSequence.FirstInformation:
			case CodeReadSubSequence.ReadMessageWindow:
				break;
			}
		}

		private void PostStartLogic(int playerIndex)
		{
			CodeReadSubSequence codeReadSubSequence = _subSequences[playerIndex];
			if (codeReadSubSequence == CodeReadSubSequence.CardSelect)
			{
				SetDecision(playerIndex, isReadCard: false);
			}
		}

		private void FirstInformationComplete(int playerIndex)
		{
			_subSequences[playerIndex] = _prevSequense[playerIndex];
			StartLogic(playerIndex);
			PostStartLogic(playerIndex);
			SetInputLockInfo(playerIndex, 1500f);
		}

		private void FirstInformationReadMessageComplete(int playerIndex)
		{
			_subSequences[playerIndex] = CodeReadSubSequence.ReadMessageWindow;
			SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000022, playerIndex);
			_monitors[playerIndex].ReadMessageWindowPlayIn(isSuccess: true, _cardEffectText[playerIndex], CommonMessageID.CodeReadThisCardUse.GetName(), boostText[playerIndex], status[playerIndex], readCard[playerIndex]);
		}

		private void LogicUpdate(int playerIndex)
		{
			switch (_subSequences[playerIndex])
			{
			case CodeReadSubSequence.FirstInformation:
				if (_userTimer[playerIndex] >= 10000f)
				{
					container.processManager.CloseWindow(playerIndex);
					_onFirstInformations[playerIndex]?.Invoke(playerIndex);
				}
				_userTimer[playerIndex] += GameManager.GetGameMSecAdd();
				break;
			case CodeReadSubSequence.FristCardRead:
			case CodeReadSubSequence.CardSelect:
				_readResults[playerIndex] = null;
				if (_readWaitTimer[playerIndex] > 0f)
				{
					_readWaitTimer[playerIndex] -= GameManager.GetGameMSecAdd();
				}
				else
				{
					if (_codeReaderStates[playerIndex] == CodeReaderState.Error)
					{
						break;
					}
					if (UpdateCardReader(playerIndex, out _readResults[playerIndex]))
					{
						if (_codeReaderStates[playerIndex] == CodeReaderState.Insert)
						{
							_readWaitTimer[playerIndex] = 1000f;
							break;
						}
						bool flag = false;
						ReadResult readResult = _readResults[playerIndex];
						if (readResult != null)
						{
							if (readResult.Status == ResultStatus.Success)
							{
								UserCard userCard = GetUserCard(playerIndex, readResult.ID, out status[playerIndex]);
								CardTypeData cardTypeData = GetCardTypeData((status[playerIndex] != 0) ? 1 : readResult.ID);
								_cardEffectText[playerIndex] = cardTypeData.effectText;
								boostText[playerIndex] = "";
								if (status[playerIndex] == CardStatus.Normal && cardTypeData.kind == CardKind.Pass)
								{
									boostText[playerIndex] = CreateCardDate(userCard.endDataDate);
								}
								else if (status[playerIndex] == CardStatus.Expired)
								{
									UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
									if (!userData.Detail.ContentBit.IsFlagOn(ContentBitID.FirstOutOfServicePass))
									{
										flag = true;
										container.processManager.EnqueueMessage(playerIndex, WindowMessageID.CodeReadOutOfService);
										SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000228, playerIndex);
										userData.Detail.ContentBit.SetFlag(ContentBitID.FirstOutOfServicePass, flag: true);
									}
									boostText[playerIndex] = CommonMessageID.CodeReadBoostOutOfDate.GetName();
								}
								else if (status[playerIndex] == CardStatus.Unowned)
								{
									UserData userData2 = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
									if (!userData2.Detail.ContentBit.IsFlagOn(ContentBitID.FirstAnotherPass))
									{
										flag = true;
										container.processManager.EnqueueMessage(playerIndex, WindowMessageID.CodeReadNotHave);
										SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000228, playerIndex);
										userData2.Detail.ContentBit.SetFlag(ContentBitID.FirstAnotherPass, flag: true);
									}
									boostText[playerIndex] = "";
								}
								readCard[playerIndex] = readResult.ReadCard ?? GetReadCard(playerIndex, 1);
								if (!flag)
								{
									SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000022, playerIndex);
									_monitors[playerIndex].ReadMessageWindowPlayIn(isSuccess: true, _cardEffectText[playerIndex], CommonMessageID.CodeReadThisCardUse.GetName(), boostText[playerIndex], status[playerIndex], readCard[playerIndex]);
								}
							}
							else
							{
								string name = CommonMessageID.CodeReadNotUsed.GetName();
								if (readResult.Status == ResultStatus.PromotionFailed)
								{
									name = CommonMessageID.CodeReadPromoCodeUnmatch.GetName();
								}
								else if (readResult.Status == ResultStatus.UnownedFailed)
								{
									name = CommonMessageID.CodeReadNotBuy.GetName();
								}
								else if (readResult.Status == ResultStatus.UsedReleaseFailed)
								{
									name = CommonMessageID.CodeReadAllreadyRelease.GetName();
								}
								else if (readResult.Status == ResultStatus.IdenticalFailed)
								{
									_readWaitTimer[playerIndex] = 1000f;
									_codeReaderStates[playerIndex] = CodeReaderState.Insert;
									_monitors[playerIndex].ReadImageSetInsert(CodeReaderState.Insert);
									break;
								}
								string effectText = GetCardTypeData(readResult.ID).effectText;
								ReadCard card = GetReadCard(playerIndex, readResult.ID);
								_monitors[playerIndex].ReadMessageWindowPlayIn(isSuccess: false, effectText, name, "", CardStatus.Normal, card);
							}
						}
						else
						{
							container.processManager.EnqueueMessage(playerIndex, WindowMessageID.CardReadFailedMessage);
							_monitors[playerIndex].ReadFailedMessagePlay();
						}
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_CARD_READ, playerIndex);
						if (_subSequences[playerIndex] == CodeReadSubSequence.FristCardRead)
						{
							_monitors[playerIndex].FirstCardReadPlayOut();
						}
						else if (_subSequences[playerIndex] == CodeReadSubSequence.CardSelect)
						{
							_monitors[playerIndex].CardSelectPlayOut();
						}
						if (flag)
						{
							_userTimer[playerIndex] = 0f;
							_isFirstInformationSkipShow[playerIndex] = false;
							_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
							_onFirstInformations[playerIndex] = FirstInformationReadMessageComplete;
							_prevSequense[playerIndex] = _subSequences[playerIndex];
							_subSequences[playerIndex] = CodeReadSubSequence.FirstInformation;
						}
						else
						{
							_prevSequense[playerIndex] = _subSequences[playerIndex];
							_subSequences[playerIndex] = CodeReadSubSequence.ReadMessageWindow;
						}
						SetInputLockInfo(playerIndex, 1000f);
					}
					else if (_codeReaderStates[playerIndex] == CodeReaderState.Insert)
					{
						_codeReaderStates[playerIndex] = CodeReaderState.Normal;
						_monitors[playerIndex].ReadImageSetInsert(CodeReaderState.Normal);
						_prevReadCardIDs[playerIndex] = 0;
						_prevReadParams[playerIndex] = 0;
						_prevReadPromoIDs[playerIndex] = 0;
					}
				}
				break;
			case CodeReadSubSequence.ReadMessageWindow:
			case CodeReadSubSequence.Wait:
				break;
			}
		}

		protected override void UpdateInput(int playerIndex)
		{
			if (_state != CodeReadMainSequence.Update)
			{
				return;
			}
			switch (_subSequences[playerIndex])
			{
			case CodeReadSubSequence.FirstInformation:
				if (_userTimer[playerIndex] >= 3000f && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
					_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
					_userTimer[playerIndex] = 10000f;
				}
				if (!_isFirstInformationSkipShow[playerIndex] && _userTimer[playerIndex] >= 3000f)
				{
					_isFirstInformationSkipShow[playerIndex] = true;
					_monitors[playerIndex].SetFirstInformation();
				}
				break;
			case CodeReadSubSequence.FristCardRead:
				if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
					CheckNext(playerIndex);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
					SetInputLockInfo(playerIndex, 550f);
				}
				break;
			case CodeReadSubSequence.CardSelect:
				if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					if ((_cardLists[playerIndex][0] != 0) ? (_decisionIndexs[playerIndex] != _selectCardIndexs[playerIndex]) : (_decisionIndexs[playerIndex] != _selectCardIndexs[playerIndex] + 1))
					{
						SetDecision(playerIndex, _selectCardIndexs[playerIndex] == 0 && _currentReadDatas[playerIndex].CharaID != 0);
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000022, playerIndex);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
						_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
						_monitors[playerIndex].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 0);
					}
					else
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
						_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
						CheckNext(playerIndex);
					}
					SetInputLockInfo(playerIndex, 700f);
				}
				else if (InputManager.GetInputDown(playerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
				{
					if (_selectCardIndexs[playerIndex] + 1 >= GetGenerateCardNum(playerIndex))
					{
						break;
					}
					_selectCardIndexs[playerIndex]++;
					_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button03);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, playerIndex);
					int cardListID = GetCardListID(playerIndex, 0);
					GetUserCard(playerIndex, GetCardTypeData(cardListID).name.id, out var cardStatus2);
					CardTypeData cardTypeData2 = GetCardTypeData((cardStatus2 != 0) ? 1 : cardListID);
					if ((int)_tempFusionType > 0)
					{
						switch (_tempFusionType)
						{
						case FusionType.Platinum:
							if (cardListID == 4)
							{
								cardTypeData2 = GetCardTypeData(5);
							}
							break;
						case FusionType.Gold:
							if (cardListID == 3)
							{
								cardTypeData2 = GetCardTypeData(4);
							}
							break;
						case FusionType.Silver:
							if (cardListID == 2)
							{
								cardTypeData2 = GetCardTypeData(3);
							}
							break;
						}
					}
					_monitors[playerIndex].SetCardInfoData(cardTypeData2.title, cardTypeData2.effectText, GetBoostDeadLineText(playerIndex, cardTypeData2.GetID()));
					_monitors[playerIndex].Scroll(Direction.Left);
					SetInputLockInfo(playerIndex, 100f);
					ButtonCheck(playerIndex);
					int num = ((_cardLists[playerIndex][0] == 0) ? 1 : 0);
					if (_decisionIndexs[playerIndex] == _selectCardIndexs[playerIndex] + num)
					{
						_monitors[playerIndex].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 0);
					}
					else
					{
						_monitors[playerIndex].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 1);
					}
				}
				else
				{
					if (!InputManager.GetInputDown(playerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || _selectCardIndexs[playerIndex] - 1 < 0)
					{
						break;
					}
					_selectCardIndexs[playerIndex]--;
					_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button06);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, playerIndex);
					int cardListID2 = GetCardListID(playerIndex, 0);
					GetUserCard(playerIndex, GetCardTypeData(cardListID2).name.id, out var cardStatus3);
					CardTypeData cardTypeData3 = GetCardTypeData((cardStatus3 != 0) ? 1 : cardListID2);
					if ((int)_tempFusionType > 0)
					{
						switch (_tempFusionType)
						{
						case FusionType.Platinum:
							if (cardListID2 == 4)
							{
								cardTypeData3 = GetCardTypeData(5);
							}
							break;
						case FusionType.Gold:
							if (cardListID2 == 3)
							{
								cardTypeData3 = GetCardTypeData(4);
							}
							break;
						case FusionType.Silver:
							if (cardListID2 == 2)
							{
								cardTypeData3 = GetCardTypeData(3);
							}
							break;
						}
					}
					_monitors[playerIndex].SetCardInfoData(cardTypeData3.title, cardTypeData3.effectText, GetBoostDeadLineText(playerIndex, cardTypeData3.GetID()));
					_monitors[playerIndex].Scroll(Direction.Right);
					SetInputLockInfo(playerIndex, 100f);
					ButtonCheck(playerIndex);
					int num2 = ((_cardLists[playerIndex][0] == 0) ? 1 : 0);
					if (_decisionIndexs[playerIndex] == _selectCardIndexs[playerIndex] + num2)
					{
						_monitors[playerIndex].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 0);
					}
					else
					{
						_monitors[playerIndex].ChangeButtonSymbol(InputManager.ButtonSetting.Button04, 1);
					}
				}
				break;
			case CodeReadSubSequence.ReadMessageWindow:
			{
				ReadResult obj = _readResults[playerIndex];
				if (obj != null && obj.Status == ResultStatus.Success && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					_monitors[playerIndex].ReadMessageWindowPlayOut();
					int iD = _readResults[playerIndex].ID;
					GetUserCard(playerIndex, GetCardTypeData(iD).name.id, out var cardStatus);
					CardTypeData cardTypeData = GetCardTypeData((cardStatus != 0) ? 1 : iD);
					if (cardTypeData.kind == CardKind.ReleaseMusic)
					{
						Singleton<UserDataManager>.Instance.GetUserData(playerIndex).AddUnlockMusic(UserData.MusicUnlock.Base, cardTypeData.paramId);
						Texture2D jacketTexture2D = container.assetManager.GetJacketTexture2D(cardTypeData.paramId);
						Sprite jacket = Sprite.Create(jacketTexture2D, new Rect(0f, 0f, jacketTexture2D.width, jacketTexture2D.height), new Vector2(0.5f, 0.5f));
						string str = Singleton<DataManager>.Instance.GetMusic(cardTypeData.paramId).name.str;
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, playerIndex);
						_monitors[playerIndex].PlayGetMusic(jacket, str, GetWindowCallback);
						SetInputLockInfo(playerIndex, 5000f);
						_readWaitTimer[playerIndex] = 5000f;
					}
					else if (cardTypeData.kind == CardKind.ReleaseChara)
					{
						Singleton<UserDataManager>.Instance.GetUserData(playerIndex).AddCollections(UserData.Collection.Chara, cardTypeData.paramId);
						Texture2D characterTexture2D = container.assetManager.GetCharacterTexture2D(cardTypeData.paramId);
						Sprite character = Sprite.Create(characterTexture2D, new Rect(0f, 0f, characterTexture2D.width, characterTexture2D.height), new Vector2(0.5f, 0.5f));
						string str2 = Singleton<DataManager>.Instance.GetChara(cardTypeData.paramId).name.str;
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_MAP_GET, playerIndex);
						_monitors[playerIndex].PlayGetCharacter(character, str2, cardTypeData.paramId, GetWindowCallback);
						SetInputLockInfo(playerIndex, 5000f);
						_readWaitTimer[playerIndex] = 5000f;
					}
					else
					{
						_cardLists[playerIndex][0] = iD;
						_selectCardIndexs[playerIndex] = 0;
						SetDecision(playerIndex, isReadCard: true);
						_decisionIndexs[playerIndex] = _selectCardIndexs[playerIndex];
						_monitors[playerIndex].CardSelectPlayIn(isDecision: true);
						ButtonCheck(playerIndex);
						SetInputLockInfo(playerIndex, 500f);
						_readWaitTimer[playerIndex] = 1000f;
						_isUseReadCards[playerIndex] = true;
						SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000022, playerIndex);
					}
					_subSequences[playerIndex] = CodeReadSubSequence.CardSelect;
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, playerIndex);
					_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button04);
				}
				else
				{
					if (!InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button05))
					{
						break;
					}
					container.processManager.CloseWindow(playerIndex);
					_monitors[playerIndex].ReadMessageWindowPlayOut();
					ReadResult obj2 = _readResults[playerIndex];
					if ((obj2 == null || obj2.Status != ResultStatus.Success) && _prevSequense[playerIndex] == CodeReadSubSequence.FristCardRead)
					{
						_monitors[playerIndex].FirstCardReadPlayIn(isInit: false);
						_subSequences[playerIndex] = _prevSequense[playerIndex];
					}
					else
					{
						if (_readResults[playerIndex] != null)
						{
							_currentReadDatas[playerIndex] = _prevCurrentReadDatas[playerIndex];
						}
						_isUseReadCards[playerIndex] = false;
						if (_prevSequense[playerIndex] == CodeReadSubSequence.CardSelect)
						{
							_monitors[playerIndex].CardSelectPlayIn(_decisionIndexs[playerIndex] > -1);
						}
						else
						{
							_monitors[playerIndex].FirstCardReadPlayIn(isInit: false);
						}
						ButtonCheck(playerIndex);
						_subSequences[playerIndex] = _prevSequense[playerIndex];
					}
					SetInputLockInfo(playerIndex, 500f);
					_readWaitTimer[playerIndex] = 1000f;
					_monitors[playerIndex].PressedButton(InputManager.ButtonSetting.Button05);
				}
				break;
			}
			case CodeReadSubSequence.Wait:
				break;
			}
		}

		private void OnTimeUp()
		{
			container.processManager.SetVisibleTimers(isVisible: false);
			for (int i = 0; i < _monitors.Length; i++)
			{
				container.processManager.ForcedCloseWindow(i);
				_monitors[i].SetVisibleBlur(isVisible: false);
				if (_subSequences[i] != CodeReadSubSequence.Wait)
				{
					_lastSequences[i] = _subSequences[i];
				}
			}
			_state = CodeReadMainSequence.Released;
			if (_croutine != null)
			{
				container.monoBehaviour.StopCoroutine(_croutine);
				_croutine = null;
			}
			_croutine = TimeUpCoroutine();
			container.monoBehaviour.StartCoroutine(_croutine);
		}

		private void CheckNext(int playerIndex)
		{
			if (_subSequences[playerIndex] != CodeReadSubSequence.Wait)
			{
				_lastSequences[playerIndex] = _subSequences[playerIndex];
			}
			_prevSequense[playerIndex] = _subSequences[playerIndex];
			_subSequences[playerIndex] = CodeReadSubSequence.Wait;
			_monitors[playerIndex].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button04);
			int num = ((playerIndex == 0) ? 1 : 0);
			if (_subSequences[num] == CodeReadSubSequence.Wait)
			{
				OnTimeUp();
				return;
			}
			container.processManager.EnqueueMessage(playerIndex, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
			_monitors[playerIndex].SetVisibleBlur(isVisible: true);
			_monitors[playerIndex].SetLanDollyDecision(playerIndex);
			_monitors[num].SetLanDollyDecision(playerIndex);
		}

		private IEnumerator TimeUpCoroutine()
		{
			container.processManager.ClearTimeoutAction();
			for (int i = 0; i < 2; i++)
			{
				if (!Singleton<UserDataManager>.Instance.GetUserData(i).IsGuest())
				{
					_monitors[i].SetTimeUp(_lastSequences[i] == CodeReadSubSequence.FristCardRead);
				}
				if (_isEntry[i])
				{
					if (_decisionIndexs[i] < 0)
					{
						Singleton<UserDataManager>.Instance.GetUserData(i).Detail.CardType = 0;
					}
					else
					{
						Singleton<UserDataManager>.Instance.GetUserData(i).Detail.CardType = GetCardTypeData(_cardLists[i][_decisionIndexs[i]]).name.id;
					}
				}
			}
			yield return new WaitForSeconds(1f);
			for (int j = 0; j < 2; j++)
			{
				int cardType = Singleton<UserDataManager>.Instance.GetUserData(j).Detail.CardType;
				GetUserCard(j, cardType, out var cardStatus);
				if (cardStatus == CardStatus.Normal)
				{
					Singleton<UserDataManager>.Instance.GetUserData(j).Extend.UpsertCardLog(Singleton<UserDataManager>.Instance.GetUserData(j).Detail.CardType);
				}
			}
			float num = 0f;
			bool[] array = new bool[_monitors.Length];
			for (int k = 0; k < 2; k++)
			{
				if (!Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
				{
					array[k] = false;
					continue;
				}
				ReadResult result = null;
				UpdateCardReader(k, out result);
				if (result != null)
				{
					container.processManager.EnqueueMessage(k, WindowMessageID.CodeReadInsertCardMessage);
					_monitors[k].SetVisibleBlur(isVisible: true);
					num = 5f;
					array[k] = true;
				}
				else
				{
					array[k] = false;
				}
			}
			if (num > 0f)
			{
				for (int l = 0; l < array.Length; l++)
				{
					if (!array[l] && _isEntry[l])
					{
						container.processManager.EnqueueMessage(l, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
						_monitors[l].SetVisibleBlur(isVisible: true);
					}
				}
			}
			yield return new WaitForSeconds(num);
			if (_qrReader != null)
			{
				_qrReader.Pause();
				_qrReader = null;
			}
			for (int m = 0; m < _monitors.Length; m++)
			{
				container.processManager.CloseWindow(m);
			}
			yield return new WaitForEndOfFrame();
			for (int n = 0; n < 2; n++)
			{
				if (_monitors[n].IsActive())
				{
					int num2 = Singleton<UserDataManager>.Instance.GetUserData(n).Detail.CardType;
					GetUserCard(n, num2, out var cardStatus2);
					if (cardStatus2 != 0 && num2 != 0)
					{
						num2 = 1;
					}
					Singleton<UserDataManager>.Instance.GetUserData(n).Detail.CardType = num2;
					CardTypeData cardType2 = Singleton<DataManager>.Instance.GetCardType(num2);
					if (cardType2 != null)
					{
						GameManager.SetCardEffect(cardType2.extendBitParameter, n);
					}
					if (Singleton<UserDataManager>.Instance.GetUserData(n).Detail.CardType > 1)
					{
						string dxCardTypeText = GetDxCardTypeText((Table)Singleton<UserDataManager>.Instance.GetUserData(n).Detail.CardType);
						Sprite sprite = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_DXPass_" + dxCardTypeText);
						container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30005, n, sprite));
					}
				}
			}
			ProcessBase process = (GameManager.IsGotoPhotoShoot ? ((!_isFadeIn) ? ((ProcessBase)new PhotoShootProcess(container)) : ((ProcessBase)new FadeProcess(container, this, new PhotoShootProcess(container)))) : ((!GameManager.IsCourseMode && !GameManager.IsFreedomMode) ? ((!_isFadeIn) ? ((ProcessBase)new RegionalSelectProcess(container)) : ((ProcessBase)new FadeProcess(container, this, new RegionalSelectProcess(container)))) : ((!_isFadeIn) ? ((ProcessBase)new GetMusicProcess(container)) : ((ProcessBase)new FadeProcess(container, this, new GetMusicProcess(container))))));
			container.processManager.AddProcess(process, 50);
			if (!_isFadeIn)
			{
				container.processManager.ReleaseProcess(this);
			}
		}

		private void GetWindowCallback(int playerIndex)
		{
			if (_prevSequense[playerIndex] == CodeReadSubSequence.FristCardRead)
			{
				_monitors[playerIndex].FirstCardReadPlayIn(isInit: false);
				_subSequences[playerIndex] = _prevSequense[playerIndex];
			}
			else
			{
				_selectCardIndexs[playerIndex] = 0;
				_monitors[playerIndex].CardSelectPlayIn(isDecision: true);
				ButtonCheck(playerIndex);
			}
		}

		private void SetDecision(int playerIndex, bool isReadCard)
		{
			bool num = _decisionIndexs[playerIndex] != _selectCardIndexs[playerIndex];
			int num2;
			if (_cardLists[playerIndex][0] == 0)
			{
				num2 = _cardLists[playerIndex][_selectCardIndexs[playerIndex] + 1];
				_decisionIndexs[playerIndex] = _selectCardIndexs[playerIndex] + 1;
			}
			else
			{
				_decisionIndexs[playerIndex] = _selectCardIndexs[playerIndex];
				num2 = _cardLists[playerIndex][_selectCardIndexs[playerIndex]];
			}
			_monitors[playerIndex].SetDecision(isDecision: true);
			if (playerIndex == 0)
			{
				if (_isEntry[1])
				{
					_monitors[1].SetPartnerStatus(GetReadCard(0, num2, isReadCard), !IsValidCard(0, num2, out var cardStatus), cardStatus);
					_monitors[1].SetPartnerDecision(GetFusionType(num2));
				}
			}
			else if (_isEntry[0])
			{
				_monitors[0].SetPartnerStatus(GetReadCard(1, num2, isReadCard), !IsValidCard(1, num2, out var cardStatus2), cardStatus2);
				_monitors[0].SetPartnerDecision(GetFusionType(num2));
			}
			if (num || isReadCard)
			{
				if (!IsValidCard(playerIndex, num2, out var _))
				{
					num2 = 1;
				}
				CardTypeData cardTypeData = GetCardTypeData(num2);
				_monitors[playerIndex].SetCardInfoData(cardTypeData.title, cardTypeData.effectText, GetBoostDeadLineText(playerIndex, num2));
			}
			FusionSelectCenterStaging(playerIndex);
		}

		private void FusionSelectCenterStaging(int playerIndex)
		{
		}

		private string GetBoostDeadLineText(int playerIndex, int cardID)
		{
			string result = string.Empty;
			CardStatus cardStatus;
			UserCard userCard = GetUserCard(playerIndex, GetCardTypeData(cardID).name.id, out cardStatus);
			if (userCard != null)
			{
				result = CreateCardDate(userCard.endDataDate);
			}
			return result;
		}

		private string CreateCardDate(long endDataDate)
		{
			return string.Concat(str1: TimeManager.ToLogDateTime(DateTimeOffset.FromUnixTimeSeconds(endDataDate).LocalDateTime).ToString("yyyy/MM/dd"), str0: CommonMessageID.CodeReadBoostDate.GetName(), str2: CommonMessageID.CodeReadBoostDateAt.GetName());
		}

		private bool UpdateCardReader(int i, out ReadResult result)
		{
			result = null;
			ResultStatus resultStatus = ResultStatus.Success;
			if (_qrReader.TryParse(out var cardKind, out var promotionCode, out var cardId, out var param, out var gameId, i) || DebugReadResult(out cardKind, out promotionCode, out cardId, out param, out gameId, i))
			{
				if (gameId != 5915972)
				{
					resultStatus = ResultStatus.Failed;
					result = null;
					return true;
				}
				if (_prevReadCardIDs[i] != cardId || _prevReadParams[i] != param || _prevReadPromoIDs[i] != promotionCode)
				{
					_prevReadCardIDs[i] = cardId;
					_prevReadParams[i] = param;
					_prevReadPromoIDs[i] = promotionCode;
					CardData cardData = GetCardData(cardId);
					if (cardData == null)
					{
						resultStatus = ResultStatus.Failed;
						result = null;
						return true;
					}
					CardTypeData cardType = Singleton<DataManager>.Instance.GetCardType(cardData.type.id);
					if (cardType == null)
					{
						resultStatus = ResultStatus.Failed;
						result = null;
						return true;
					}
					CardStatus cardStatus;
					UserCard userCard = GetUserCard(i, cardType.name.id, out cardStatus);
					if (promotionCode != 0)
					{
						resultStatus = ResultStatus.PromotionFailed;
					}
					else if (userCard == null && cardType.kind != 0)
					{
						resultStatus = ResultStatus.UnownedFailed;
					}
					else if (userCard != null)
					{
						UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
						if (cardType.kind == CardKind.ReleaseMusic)
						{
							if (IsInPeriod(userCard.startDate, userCard.endDataDate) && cardStatus == CardStatus.Normal)
							{
								if (userData.MusicUnlockList.Contains(cardType.paramId))
								{
									resultStatus = ResultStatus.UsedReleaseFailed;
								}
							}
							else
							{
								resultStatus = ResultStatus.ExpiredFailed;
							}
						}
						else if (cardType.kind == CardKind.ReleaseChara)
						{
							if (IsInPeriod(userCard.startDate, userCard.endDataDate) && cardStatus == CardStatus.Normal)
							{
								foreach (UserChara chara in userData.CharaList)
								{
									if (chara.ID == cardType.paramId)
									{
										resultStatus = ResultStatus.UsedReleaseFailed;
										break;
									}
								}
							}
							else
							{
								resultStatus = ResultStatus.ExpiredFailed;
							}
						}
					}
					ReadCard readCard = null;
					if (resultStatus == ResultStatus.Success)
					{
						if (cardType.kind == CardKind.Pass)
						{
							CardCharaData cardChara = Singleton<DataManager>.Instance.GetCardChara(param);
							if (cardChara == null)
							{
								param = 101;
								cardChara = Singleton<DataManager>.Instance.GetCardChara(101);
							}
							readCard = new ReadCard
							{
								Background = GetCardBackgroundSprite(cardType.name.id, cardChara.mapId.id),
								Character = GetCardCharacterSprite(param),
								Frame = GetCardFrameSprite(cardType.name.id),
								Kind = cardType.kind
							};
							_prevCurrentReadDatas[i] = _currentReadDatas[i];
							_currentReadDatas[i].Set(cardType.name.id, param);
						}
						else
						{
							CardCharaData cardChara2 = Singleton<DataManager>.Instance.GetCardChara(userCard.charaId);
							readCard = new ReadCard
							{
								Background = GetCardBackgroundSprite(cardType.name.id, cardChara2.mapId.id),
								Character = GetCardCharacterSprite(userCard.charaId),
								Frame = GetCardFrameSprite(cardType.name.id),
								Kind = cardType.kind
							};
							_prevCurrentReadDatas[i] = _currentReadDatas[i];
							_currentReadDatas[i].Set(cardType.name.id, userCard.charaId);
						}
					}
					result = new ReadResult(cardType.name.id, readCard, resultStatus);
					return true;
				}
				resultStatus = ResultStatus.IdenticalFailed;
				result = new ReadResult(0, null, resultStatus);
			}
			else
			{
				_prevReadCardIDs[i] = 0;
				_prevReadParams[i] = 0;
				_prevReadPromoIDs[i] = 0;
				resultStatus = ResultStatus.Failed;
			}
			return false;
		}

		private bool DebugReadResult(out int kind, out int promo, out int cardID, out int param, out uint gameId, int i)
		{
			kind = 0;
			promo = 0;
			cardID = 1116;
			param = 0;
			gameId = 5915972u;
			bool result = _debugRead[i];
			_debugRead[i] = false;
			return result;
		}

		private void ButtonCheck(int playerIndex)
		{
			bool isVisible = _selectCardIndexs[playerIndex] > 0;
			bool isVisible2 = _selectCardIndexs[playerIndex] < GetGenerateCardNum(playerIndex) - 1;
			_monitors[playerIndex].SetVisibleButton(isVisible2, InputManager.ButtonSetting.Button03);
			_monitors[playerIndex].SetVisibleButton(isVisible, InputManager.ButtonSetting.Button06);
		}

		private FusionType SelectFusion()
		{
			FusionType result = FusionType.None;
			if (_decisionIndexs[0] == -1 || _decisionIndexs[1] == -1)
			{
				return result;
			}
			int cardID = _cardLists[0][_decisionIndexs[0]];
			int cardID2 = _cardLists[1][_decisionIndexs[1]];
			return Fusion(cardID, cardID2);
		}

		[Obsolete]
		public static FusionType Fusion(int cardID01, int cardID02)
		{
			FusionType result = FusionType.None;
			CardTypeData cardTypeData = GetCardTypeData(cardID01);
			if (cardTypeData == null)
			{
				return FusionType.None;
			}
			GetUserCard(0, cardTypeData.name.id, out var cardStatus);
			if (cardStatus != 0)
			{
				return result;
			}
			CardTypeData cardTypeData2 = GetCardTypeData(cardID02);
			if (cardTypeData2 == null)
			{
				return FusionType.None;
			}
			GetUserCard(1, cardTypeData2.name.id, out cardStatus);
			if (cardStatus != 0)
			{
				return result;
			}
			if (cardID01 == 2 && cardID02 == 2)
			{
				result = FusionType.Silver;
			}
			else if (cardID01 == 3 && cardID02 == 3)
			{
				result = FusionType.Gold;
			}
			else if (cardID01 == 4 && cardID02 == 4)
			{
				result = FusionType.Platinum;
			}
			return result;
		}

		private FusionType GetFusionType(int cardID)
		{
			return cardID switch
			{
				1 => FusionType.Blonz, 
				2 => FusionType.Silver, 
				3 => FusionType.Gold, 
				_ => FusionType.None, 
			};
		}

		public int GetCardListID(int playerIndex, int diffIndex)
		{
			int num = _selectCardIndexs[playerIndex] + diffIndex;
			if (num >= _cardLists[playerIndex].Count)
			{
				return 0;
			}
			if (_cardLists[playerIndex][0] == 0)
			{
				if (num + 1 >= _cardLists[playerIndex].Count)
				{
					return 0;
				}
				return _cardLists[playerIndex][num + 1];
			}
			return _cardLists[playerIndex][num];
		}

		public int GetGenerateCardNum(int playerIndex)
		{
			if (_cardLists[playerIndex][0] == 0)
			{
				return _cardLists[playerIndex].Count - 1;
			}
			return _cardLists[playerIndex].Count;
		}

		public bool IsValidCard(int playerIndex, int typeID, out CardStatus status)
		{
			UserCard userCard = null;
			foreach (UserCard card in Singleton<UserDataManager>.Instance.GetUserData(playerIndex).CardList)
			{
				if (card.cardTypeId == typeID)
				{
					userCard = card;
					break;
				}
			}
			if (userCard == null)
			{
				status = CardStatus.Unowned;
				return false;
			}
			if (!IsInPeriod(userCard.startDate, userCard.endDataDate))
			{
				status = CardStatus.Expired;
				return false;
			}
			CardData cardData = GetCardData(userCard.cardId);
			if (cardData != null)
			{
				if ((long)cardData.enableVersion.id < 4L)
				{
					status = CardStatus.Expired;
					return false;
				}
				status = CardStatus.Normal;
				return true;
			}
			status = CardStatus.Expired;
			return false;
		}

		public ReadCard GetReadCard(int playerIndex, int typeID)
		{
			UserCard userCard = null;
			foreach (UserCard card in Singleton<UserDataManager>.Instance.GetUserData(playerIndex).CardList)
			{
				if (card.cardTypeId == typeID)
				{
					userCard = card;
					break;
				}
			}
			if (userCard == null)
			{
				return GetScrapCard(playerIndex, typeID);
			}
			int charaId = userCard.charaId;
			CardTypeData cardType = Singleton<DataManager>.Instance.GetCardType(typeID);
			int id = Singleton<DataManager>.Instance.GetCardChara(charaId).mapId.id;
			return new ReadCard
			{
				Background = GetCardBackgroundSprite(typeID, id),
				Character = GetCardCharacterSprite(charaId),
				Frame = GetCardFrameSprite(cardType.name.id),
				Kind = cardType.kind
			};
		}

		public ReadCard GetReadCard(int playerIndex, int typeID, bool isReadCard)
		{
			UserCard userCard = null;
			foreach (UserCard card in Singleton<UserDataManager>.Instance.GetUserData(playerIndex).CardList)
			{
				if (card.cardTypeId == typeID)
				{
					userCard = card;
					break;
				}
			}
			if (userCard == null)
			{
				return GetScrapCard(playerIndex, typeID);
			}
			int num = userCard.charaId;
			if (isReadCard)
			{
				num = _currentReadDatas[playerIndex].CharaID;
			}
			CardTypeData cardType = Singleton<DataManager>.Instance.GetCardType(typeID);
			int id = Singleton<DataManager>.Instance.GetCardChara(num).mapId.id;
			return new ReadCard
			{
				Background = GetCardBackgroundSprite(typeID, id),
				Character = GetCardCharacterSprite(num),
				Frame = GetCardFrameSprite(cardType.name.id),
				Kind = cardType.kind
			};
		}

		private ReadCard GetScrapCard(int playerIndex, int typeID)
		{
			if (typeID == 5)
			{
				CardTypeData cardType = Singleton<DataManager>.Instance.GetCardType(typeID);
				int num = 101;
				int id = Singleton<DataManager>.Instance.GetCardChara(num).mapId.id;
				return new ReadCard
				{
					Background = GetCardBackgroundSprite(typeID, id),
					Character = GetCardCharacterSprite(num),
					Frame = GetCardFrameSprite(cardType.name.id),
					Kind = cardType.kind
				};
			}
			CardTypeData cardType2 = Singleton<DataManager>.Instance.GetCardType(typeID);
			int num2 = _currentReadDatas[playerIndex].CharaID;
			if (num2 == 0)
			{
				num2 = 101;
			}
			int id2 = Singleton<DataManager>.Instance.GetCardChara(num2).mapId.id;
			return new ReadCard
			{
				Background = GetCardBackgroundSprite(typeID, id2),
				Character = GetCardCharacterSprite(num2),
				Frame = GetCardFrameSprite(cardType2.name.id),
				Kind = cardType2.kind
			};
		}

		public ReadCard GetCurrentReadedCard(int playerIndex)
		{
			ReadCard result = null;
			if (_cardLists[playerIndex][0] == 0)
			{
				return result;
			}
			int cardID = _currentReadDatas[playerIndex].CardID;
			CardTypeData cardType = Singleton<DataManager>.Instance.GetCardType(cardID);
			int charaID = _currentReadDatas[playerIndex].CharaID;
			int id = Singleton<DataManager>.Instance.GetCardChara(charaID).mapId.id;
			return new ReadCard
			{
				Background = GetCardBackgroundSprite(cardID, id),
				Character = GetCardCharacterSprite(charaID),
				Frame = GetCardFrameSprite(cardType.name.id),
				Kind = cardType.kind
			};
		}

		public bool IsCardReaded(int playerIndex)
		{
			return _cardLists[playerIndex][0] != 0;
		}

		public bool IsUseReadCard(int playerIndex)
		{
			return _isUseReadCards[playerIndex];
		}

		public static CardData GetCardData(int cardID)
		{
			return Singleton<DataManager>.Instance.GetCard(cardID);
		}

		public static CardTypeData GetCardTypeData(int cardID)
		{
			return Singleton<DataManager>.Instance.GetCardType(cardID);
		}

		public Sprite GetCardCharacter(int playerIndex, int typeID)
		{
			foreach (UserCard card in Singleton<UserDataManager>.Instance.GetUserData(playerIndex).CardList)
			{
				if (card.cardTypeId == typeID)
				{
					return GetCardCharacterSprite(card.charaId);
				}
			}
			return null;
		}

		public static UserCard GetUserCard(int playerIndex, int typeID, out CardStatus status)
		{
			status = CardStatus.Unowned;
			foreach (UserCard card in Singleton<UserDataManager>.Instance.GetUserData(playerIndex).CardList)
			{
				if (card.cardTypeId != typeID)
				{
					continue;
				}
				if (IsInPeriod(card.startDate, card.endDataDate))
				{
					CardData cardData = GetCardData(card.cardId);
					if (cardData != null && (long)cardData.enableVersion.id >= 4L)
					{
						status = CardStatus.Normal;
					}
					else
					{
						status = CardStatus.Expired;
					}
				}
				else
				{
					status = CardStatus.Expired;
				}
				return card;
			}
			return null;
		}

		public static bool IsInPeriod(long startTime, long endTime)
		{
			if (startTime <= TimeManager.PlayBaseTime)
			{
				return TimeManager.PlayBaseTime <= endTime;
			}
			return false;
		}

		public bool IsDecisionCard(int playerIndex, int index)
		{
			return index == _decisionIndexs[playerIndex] - ((_cardLists[playerIndex][0] == 0) ? 1 : 0);
		}

		public Sprite GetCardBackgroundSprite(int typeID, int effectId)
		{
			Sprite result = null;
			Texture2D cardBaseTexture2D = container.assetManager.GetCardBaseTexture2D(typeID, effectId);
			if (cardBaseTexture2D != null)
			{
				result = Sprite.Create(cardBaseTexture2D, new Rect(0f, 0f, cardBaseTexture2D.width, cardBaseTexture2D.height), new Vector2(0.5f, 0.5f));
			}
			return result;
		}

		public Sprite GetCardCharacterSprite(int charaID)
		{
			Sprite result = null;
			Texture2D cardCharaTexture2D = container.assetManager.GetCardCharaTexture2D(charaID);
			if (cardCharaTexture2D != null)
			{
				result = Sprite.Create(cardCharaTexture2D, new Rect(0f, 0f, cardCharaTexture2D.width, cardCharaTexture2D.height), new Vector2(0.5f, 0.5f));
			}
			return result;
		}

		public Sprite GetCardFrameSprite(int cardId)
		{
			Sprite result = null;
			Texture2D cardFrameTexture2D = container.assetManager.GetCardFrameTexture2D(cardId);
			if (cardFrameTexture2D != null)
			{
				result = Sprite.Create(cardFrameTexture2D, new Rect(0f, 0f, cardFrameTexture2D.width, cardFrameTexture2D.height), new Vector2(0.5f, 0.5f));
			}
			return result;
		}

		public static string GetDxCardTypeText(Table typeId)
		{
			return typeId switch
			{
				Table.BronzePass => "Bronze", 
				Table.SilverPass => "Silver", 
				Table.GoldPass => "Gold", 
				Table.FreedomPass => "Freedom", 
				Table.PlatinumPass => "Platinum", 
				_ => "", 
			};
		}
	}
}
