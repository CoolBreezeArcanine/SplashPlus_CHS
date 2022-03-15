using System;
using DB;
using Mai2.Mai2Cue;
using Mai2.Voice_Partner_000001;
using Manager;
using Monitor;

namespace Process.SubSequence
{
	public class SortSettingSequence : SequenceBase
	{
		private float _waitTimer;

		private bool _isInputLock;

		private readonly Action<int, bool> _onSortChange;

		public SortSettingSequence(int index, bool isValidity, IMusicSelectProcessProcessing processing, Action<int, bool> onSortChange)
			: base(index, isValidity, processing)
		{
			_onSortChange = onSortChange;
		}

		public override void Initialize()
		{
		}

		public override void OnStartSequence()
		{
			if (!base.IsValidity)
			{
				return;
			}
			SortRootID currentSortRootID = ProcessProcessing.CurrentSortRootID;
			MusicSelectMonitor[] monitorArray = ProcessProcessing.MonitorArray;
			foreach (MusicSelectMonitor musicSelectMonitor in monitorArray)
			{
				musicSelectMonitor.SetOptionHide();
				musicSelectMonitor.SetSortSetting(currentSortRootID, ProcessProcessing.GetBeforeSubSeq());
				musicSelectMonitor.SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B4);
				musicSelectMonitor.SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B5);
				musicSelectMonitor.SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button06);
				musicSelectMonitor.SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
				if (ProcessProcessing.GetSortValueIndex(currentSortRootID) == 0)
				{
					musicSelectMonitor.SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B5);
				}
				if (ProcessProcessing.GetSortValueIndex(currentSortRootID) >= ProcessProcessing.GetSortMax(currentSortRootID) - 1)
				{
					musicSelectMonitor.SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B4);
				}
				musicSelectMonitor.SetSideMessage(CommonMessageID.Scroll_Category_Sort_Setting.GetName());
				musicSelectMonitor.SetChallengeBGHide();
			}
			SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000189, PlayerIndex);
		}

		public override bool Update()
		{
			if (!base.IsValidity)
			{
				return false;
			}
			bool result = false;
			SortRootID currentSortRootID = ProcessProcessing.CurrentSortRootID;
			MusicSelectMonitor[] monitorArray = ProcessProcessing.MonitorArray;
			ProcessProcessing.IsInputLocking(PlayerIndex);
			if (_isInputLock)
			{
				Timer += GameManager.GetGameMSecAdd();
				for (int i = 0; i < monitorArray.Length; i++)
				{
					monitorArray[i].AnimationUpdate(Timer / 100f);
				}
				if (Timer > _waitTimer)
				{
					Timer = 0f;
					IsChange = true;
					_isInputLock = false;
				}
			}
			else
			{
				int playerIndex = PlayerIndex;
				if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button04))
				{
					_onSortChange(playerIndex, arg2: false);
					ProcessProcessing.SortDecidePlayer = playerIndex;
					for (int j = 0; j < monitorArray.Length; j++)
					{
						switch (ProcessProcessing.GetBeforeSubSeq())
						{
						case MusicSelectProcess.SubSequence.Music:
							monitorArray[j].OnStartMusicSelect();
							break;
						case MusicSelectProcess.SubSequence.Genre:
							monitorArray[j].OnStartGenreSelect();
							break;
						}
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, j);
						ProcessProcessing.SetInputLockInfo(j, 200f);
					}
					result = true;
					SyncNext(ProcessProcessing.GetBeforeSubSeq());
					ProcessProcessing.CurrentSortRootID = SortRootID.Tab;
				}
				else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B4))
				{
					if (ProcessProcessing.GetSortValueIndex(currentSortRootID) + 1 >= ProcessProcessing.GetSortMax(currentSortRootID))
					{
						return true;
					}
					ProcessProcessing.AddSort(currentSortRootID);
					for (int k = 0; k < monitorArray.Length; k++)
					{
						if (!ProcessProcessing.IsEntry(k))
						{
							continue;
						}
						monitorArray[k].SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B5);
						monitorArray[k].SetSortValueChange(currentSortRootID);
						monitorArray[k].PressedTouchPanel(InputManager.TouchPanelArea.B4);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, k);
						switch (currentSortRootID)
						{
						case SortRootID.Tab:
							switch (ProcessProcessing.GetSortValueIndex(currentSortRootID))
							{
							case 0:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000190, k);
								break;
							case 1:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000194, k);
								break;
							case 2:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000191, k);
								break;
							case 3:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000192, k);
								break;
							case 4:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000193, k);
								break;
							}
							break;
						case SortRootID.Music:
							switch (ProcessProcessing.GetSortValueIndex(currentSortRootID))
							{
							case 0:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000195, k);
								break;
							case 1:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000196, k);
								break;
							case 2:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000243, k);
								break;
							case 3:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000197, k);
								break;
							case 4:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000198, k);
								break;
							case 5:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000199, k);
								break;
							}
							break;
						}
						if (ProcessProcessing.GetSortValueIndex(currentSortRootID) + 1 >= ProcessProcessing.GetSortMax(currentSortRootID))
						{
							monitorArray[k].SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B4);
						}
						ProcessProcessing.SetInputLockInfo(k, 100f);
					}
					_waitTimer = 200f;
					_isInputLock = true;
					result = true;
				}
				else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B5))
				{
					if (ProcessProcessing.GetSortValueIndex(currentSortRootID) - 1 < 0)
					{
						return true;
					}
					ProcessProcessing.SubSort(currentSortRootID);
					for (int l = 0; l < monitorArray.Length; l++)
					{
						if (!ProcessProcessing.IsEntry(l))
						{
							continue;
						}
						monitorArray[l].SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B4);
						monitorArray[l].SetSortValueChange(currentSortRootID);
						monitorArray[l].PressedTouchPanel(InputManager.TouchPanelArea.B5);
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, l);
						switch (currentSortRootID)
						{
						case SortRootID.Tab:
							switch (ProcessProcessing.GetSortValueIndex(currentSortRootID))
							{
							case 0:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000190, l);
								break;
							case 1:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000194, l);
								break;
							case 2:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000191, l);
								break;
							case 3:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000192, l);
								break;
							case 4:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000193, l);
								break;
							}
							break;
						case SortRootID.Music:
							switch (ProcessProcessing.GetSortValueIndex(currentSortRootID))
							{
							case 0:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000195, l);
								break;
							case 1:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000196, l);
								break;
							case 2:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000243, l);
								break;
							case 3:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000197, l);
								break;
							case 4:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000198, l);
								break;
							case 5:
								SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000199, l);
								break;
							}
							break;
						}
						if (ProcessProcessing.GetSortValueIndex(currentSortRootID) - 1 < 0)
						{
							monitorArray[l].SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B5);
						}
						ProcessProcessing.SetInputLockInfo(l, 100f);
					}
					_waitTimer = 200f;
					_isInputLock = true;
					result = true;
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3))
				{
					if (currentSortRootID + 1 < SortRootID.End)
					{
						currentSortRootID++;
						for (int m = 0; m < monitorArray.Length; m++)
						{
							if (ProcessProcessing.IsEntry(m))
							{
								monitorArray[m].ScrollLeft();
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, m);
								monitorArray[m].SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B4);
								monitorArray[m].SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B5);
								if (ProcessProcessing.GetSortValueIndex(currentSortRootID) == 0)
								{
									monitorArray[m].SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B5);
								}
								if (ProcessProcessing.GetSortValueIndex(currentSortRootID) >= ProcessProcessing.GetSortMax(currentSortRootID) - 1)
								{
									monitorArray[m].SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B4);
								}
								monitorArray[m].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button06);
								monitorArray[m].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button03);
								ProcessProcessing.SetInputLockInfo(m, 100f);
							}
						}
						_waitTimer = 100f;
						_isInputLock = true;
						result = true;
						ProcessProcessing.CurrentSortRootID = currentSortRootID;
					}
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) && currentSortRootID - 1 >= SortRootID.Tab)
				{
					currentSortRootID--;
					for (int n = 0; n < monitorArray.Length; n++)
					{
						if (ProcessProcessing.IsEntry(n))
						{
							monitorArray[n].ScrollRight();
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, n);
							monitorArray[n].SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B4);
							monitorArray[n].SetVisibleCardTouchButton(isVisible: true, InputManager.TouchPanelArea.B5);
							if (ProcessProcessing.GetSortValueIndex(currentSortRootID) == 0)
							{
								monitorArray[n].SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B5);
							}
							if (ProcessProcessing.GetSortValueIndex(currentSortRootID) >= ProcessProcessing.GetSortMax(currentSortRootID) - 1)
							{
								monitorArray[n].SetVisibleCardTouchButton(isVisible: false, InputManager.TouchPanelArea.B4);
							}
							monitorArray[n].SetVisibleButton(isVisible: false, InputManager.ButtonSetting.Button06);
							monitorArray[n].SetVisibleButton(isVisible: true, InputManager.ButtonSetting.Button03);
							ProcessProcessing.SetInputLockInfo(n, 100f);
						}
					}
					_waitTimer = 100f;
					_isInputLock = true;
					result = true;
					ProcessProcessing.CurrentSortRootID = currentSortRootID;
				}
			}
			return result;
		}
	}
}
