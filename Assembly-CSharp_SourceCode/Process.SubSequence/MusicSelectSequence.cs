using IO;
using Mai2.Mai2Cue;
using Mai2.Voice_Partner_000001;
using MAI2System;
using Manager;
using Monitor;
using UnityEngine;

namespace Process.SubSequence
{
	public class MusicSelectSequence : SequenceBase
	{
		private bool _moveAfter;

		private const int CATEGORY_IN_TIME = 750;

		private const int DIFFICULTY_IN_TIME = 1200;

		public override void Initialize()
		{
		}

		public MusicSelectSequence(int index, bool isValidity, IMusicSelectProcessProcessing processing)
			: base(index, isValidity, processing)
		{
		}

		public override void OnStartSequence()
		{
			if (!base.IsValidity)
			{
				return;
			}
			MechaManager.LedIf[PlayerIndex].SetColor(2, Color.white);
			MechaManager.LedIf[PlayerIndex].SetColor(5, Color.white);
			CheckButton();
			SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000184, PlayerIndex);
			MusicSelectMonitor[] monitorArray = ProcessProcessing.MonitorArray;
			foreach (MusicSelectMonitor musicSelectMonitor in monitorArray)
			{
				musicSelectMonitor.ActiveGhostData(isActive: true);
				musicSelectMonitor.ExtraInfoReset();
				musicSelectMonitor.CheckGhostJump();
				if (ProcessProcessing.IsChallengeFolder())
				{
					musicSelectMonitor.SetChallengeMusicData();
				}
			}
			_moveAfter = false;
		}

		public override bool Update()
		{
			if (!base.IsValidity)
			{
				return false;
			}
			if (!IsStart)
			{
				ProcessProcessing.ChangeBGM();
				IsStart = true;
			}
			MusicSelectMonitor[] monitorArray = ProcessProcessing.MonitorArray;
			bool result = false;
			if (ProcessProcessing.SharedIsInputLock)
			{
				MusicSelectMonitor[] array = monitorArray;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].AnimationUpdate(Timer / 100f);
				}
				Timer += GameManager.GetGameMSecAdd();
				if (Timer > 100f - ProcessProcessing.SharedSlideScrollTime)
				{
					Timer = 0f;
					if (0 < ProcessProcessing.SharedSlideScrollCount)
					{
						ProcessProcessing.SharedSlideScrollCount--;
						ScrollUpdate(monitorArray, ProcessProcessing.SharedSlideScrollCount == 0);
					}
					else
					{
						ProcessProcessing.SharedSlideScrollTime = 0f;
						ProcessProcessing.SharedIsInputLock = false;
						IsChange = true;
					}
					monitorArray[0].AnimationUpdate(1f);
					monitorArray[1].AnimationUpdate(1f);
				}
				result = true;
			}
			else
			{
				if (ProcessProcessing.IsInputLocking(PlayerIndex))
				{
					return false;
				}
				if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button04))
				{
					if (IsNextEnable())
					{
						if (IsChange)
						{
							ProcessProcessing.ChangeBGM();
							IsChange = false;
						}
						for (int j = 0; j < monitorArray.Length; j++)
						{
							if (base.IsValidity)
							{
								monitorArray[j].ChangeButtonImage(InputManager.ButtonSetting.Button04, 14);
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, j);
								monitorArray[j].SetScrollMusicCard(isVisible: false);
								monitorArray[j].OutGenreTab();
								ProcessProcessing.SetInputLockInfo(j, 1200f);
								if (ProcessProcessing.IsExtraFolder(0))
								{
									ProcessProcessing.CurrentDifficulty[j] = (MusicDifficultyID)ProcessProcessing.GetMusic(0).Difficulty;
								}
							}
						}
						if (ProcessProcessing.IsConnectionFolder())
						{
							ProcessProcessing.JoinActive = true;
						}
						SyncNext(MusicSelectProcess.SubSequence.Difficulty);
						result = true;
					}
				}
				else if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Button05))
				{
					if (IsBackEnable())
					{
						if (IsChange)
						{
							ProcessProcessing.ChangeBGM();
							IsChange = false;
						}
						for (int k = 0; k < monitorArray.Length; k++)
						{
							if (base.IsValidity)
							{
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CANCEL, k);
								monitorArray[k].SetScrollMusicCard(isVisible: false);
								ProcessProcessing.SetInputLockInfo(k, 750f);
							}
						}
						MusicSelectMonitor[] array = monitorArray;
						for (int i = 0; i < array.Length; i++)
						{
							array[i].ExtraInfoReset();
						}
						SyncNext(MusicSelectProcess.SubSequence.Genre);
						result = true;
						IsStart = false;
					}
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
				{
					if (ProcessProcessing.CurrentMusicSelect + 1 >= ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect].Count)
					{
						ProcessProcessing.CurrentCategorySelect = ((ProcessProcessing.CurrentCategorySelect + 1 < ProcessProcessing.CategoryNameList.Count) ? (ProcessProcessing.CurrentCategorySelect + 1) : 0);
						ProcessProcessing.CurrentMusicSelect = 0;
					}
					else
					{
						ProcessProcessing.CurrentMusicSelect++;
					}
					for (int l = 0; l < monitorArray.Length; l++)
					{
						if (base.IsValidity)
						{
							monitorArray[l].ScrollMusicLeft();
							monitorArray[l].SetScrollMusicCard(InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L));
							SoundManager.PreviewEnd();
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, l);
						}
					}
					CheckButton();
					ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
					ProcessProcessing.SharedIsInputLock = true;
					result = true;
					_moveAfter = true;
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
				{
					if (ProcessProcessing.CurrentMusicSelect - 1 < 0)
					{
						ProcessProcessing.CurrentCategorySelect = ((ProcessProcessing.CurrentCategorySelect - 1 >= 0) ? (ProcessProcessing.CurrentCategorySelect - 1) : (ProcessProcessing.CategoryNameList.Count - 1));
						ProcessProcessing.CurrentMusicSelect = ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect].Count - 1;
					}
					else
					{
						ProcessProcessing.CurrentMusicSelect--;
					}
					for (int m = 0; m < monitorArray.Length; m++)
					{
						if (base.IsValidity)
						{
							monitorArray[m].ScrollMusicRight();
							monitorArray[m].SetScrollMusicCard(InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L));
							SoundManager.PreviewEnd();
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, m);
						}
					}
					CheckButton();
					ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
					ProcessProcessing.SharedIsInputLock = true;
					result = true;
					_moveAfter = true;
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button01, InputManager.TouchPanelArea.A1))
				{
					if (!ProcessProcessing.IsExtraFolder(0))
					{
						AddDifficulty(monitorArray);
					}
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8))
				{
					if (!ProcessProcessing.IsExtraFolder(0))
					{
						SubDifficulty(monitorArray);
					}
				}
				else if (EachPushDecision(PlayerIndex, 50L))
				{
					if (ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect][ProcessProcessing.CurrentMusicSelect].isChangeScoreKind())
					{
						if (!ProcessProcessing.IsExtraFolder(0))
						{
							ScoreKindChange(monitorArray);
						}
					}
					else if (ProcessProcessing.ScoreKindMove())
					{
						ScoreKindMove(monitorArray);
					}
				}
				else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.E2))
				{
					result = true;
					CategoryScrollRight(monitorArray);
					ProcessProcessing.ChangeBGM();
				}
				else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.E8))
				{
					result = true;
					CategoryScrollLeft(monitorArray);
					ProcessProcessing.ChangeBGM();
				}
				else if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Select))
				{
					if (!_moveAfter)
					{
						for (int n = 0; n < monitorArray.Length; n++)
						{
							if (base.IsValidity)
							{
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, n);
							}
						}
						SyncNext(MusicSelectProcess.SubSequence.SortSetting);
						result = true;
					}
					MusicSelectMonitor[] array = monitorArray;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].ExtraInfoReset();
					}
				}
				else if (InputManager.SlideAreaLr(PlayerIndex))
				{
					ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
					ProcessProcessing.SharedIsInputLock = true;
					ProcessProcessing.SharedSlideScrollToRight = true;
					result = true;
					ProcessProcessing.SharedSlideScrollTime = 50f;
					int num = -1;
					int overCount;
					while (num >= -10 && !ProcessProcessing.IsMusicBoundary(num, out overCount))
					{
						ProcessProcessing.SharedSlideScrollCount++;
						num--;
					}
					ProcessProcessing.SharedSlideScrollCount--;
					ScrollUpdate(monitorArray, ProcessProcessing.SharedSlideScrollCount == 0);
				}
				else if (InputManager.SlideAreaRl(PlayerIndex))
				{
					ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
					ProcessProcessing.SharedIsInputLock = true;
					ProcessProcessing.SharedSlideScrollToRight = false;
					result = true;
					int overCount2;
					for (int num2 = 1; num2 <= 10 && !ProcessProcessing.IsMusicBoundary(num2, out overCount2); num2++)
					{
						ProcessProcessing.SharedSlideScrollCount++;
					}
					ProcessProcessing.SharedSlideScrollTime = 50f;
					ProcessProcessing.SharedSlideScrollCount--;
					ScrollUpdate(monitorArray, ProcessProcessing.SharedSlideScrollCount == 0);
				}
				else if (InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B8) || InputManager.GetTouchPanelAreaDown(PlayerIndex, InputManager.TouchPanelArea.B1))
				{
					if (ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect][ProcessProcessing.CurrentMusicSelect].IsJumpGhostBattle(diff: ProcessProcessing.GetDifficulty(PlayerIndex), scoreType: ProcessProcessing.ScoreType))
					{
						ProcessProcessing.SetGhostJumpIndex();
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_TAB, PlayerIndex);
					}
				}
				else
				{
					if (IsChange)
					{
						ProcessProcessing.ChangeBGM();
						for (int num3 = 0; num3 < monitorArray.Length; num3++)
						{
							monitorArray[num3].SetScrollMusicCard(isVisible: false);
						}
						IsChange = false;
					}
					_moveAfter = false;
				}
			}
			return result;
		}

		private bool EachPushDecision(int playerIndex, long msec = 50L)
		{
			bool result = false;
			if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button02) && InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button07))
			{
				result = true;
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button02))
			{
				long buttonPushTime = InputManager.GetButtonPushTime(playerIndex, InputManager.ButtonSetting.Button07);
				if (buttonPushTime > 0 && buttonPushTime <= msec)
				{
					result = true;
				}
			}
			else if (InputManager.GetButtonDown(playerIndex, InputManager.ButtonSetting.Button07))
			{
				long buttonPushTime2 = InputManager.GetButtonPushTime(playerIndex, InputManager.ButtonSetting.Button02);
				if (buttonPushTime2 > 0 && buttonPushTime2 <= msec)
				{
					result = true;
				}
			}
			return result;
		}

		private void ScrollUpdate(MusicSelectMonitor[] monitorArray, bool ScrollEnd)
		{
			if (ProcessProcessing.SharedSlideScrollToRight)
			{
				if (ProcessProcessing.CurrentMusicSelect - 1 < 0)
				{
					ProcessProcessing.CurrentCategorySelect = ((ProcessProcessing.CurrentCategorySelect - 1 >= 0) ? (ProcessProcessing.CurrentCategorySelect - 1) : (ProcessProcessing.CategoryNameList.Count - 1));
					ProcessProcessing.CurrentMusicSelect = ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect].Count - 1;
				}
				else
				{
					ProcessProcessing.CurrentMusicSelect--;
				}
			}
			else if (ProcessProcessing.CurrentMusicSelect + 1 >= ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect].Count)
			{
				ProcessProcessing.CurrentCategorySelect = ((ProcessProcessing.CurrentCategorySelect + 1 < ProcessProcessing.CategoryNameList.Count) ? (ProcessProcessing.CurrentCategorySelect + 1) : 0);
				ProcessProcessing.CurrentMusicSelect = 0;
			}
			else
			{
				ProcessProcessing.CurrentMusicSelect++;
			}
			for (int i = 0; i < monitorArray.Length; i++)
			{
				if (ProcessProcessing.SharedSlideScrollToRight)
				{
					monitorArray[i].ScrollMusicRight(ScrollEnd);
				}
				else
				{
					monitorArray[i].ScrollMusicLeft(ScrollEnd);
				}
				SoundManager.PreviewEnd();
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
			}
			CheckButton();
		}

		private void AddDifficulty(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.IsLevelTab())
			{
				MusicSelectProcess.MusicSelectData musicSelectData = ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect][ProcessProcessing.CurrentMusicSelect].musicSelectData[(int)ProcessProcessing.ScoreType];
				if (musicSelectData.Difficulty + 1 <= 4 && (ProcessProcessing.IsRemasterEnable() || musicSelectData.Difficulty + 1 < 4))
				{
					MusicSelectProcess.LevelCategoryData levelToListPositoin = ProcessProcessing.GetLevelToListPositoin(musicSelectData.MusicData.name.id, musicSelectData.Difficulty + 1);
					ProcessProcessing.CurrentCategorySelect = levelToListPositoin.Category;
					ProcessProcessing.CurrentMusicSelect = levelToListPositoin.Index;
					for (int i = 0; i < monitorArray.Length; i++)
					{
						monitorArray[i].AddDifficulty((MusicDifficultyID)(musicSelectData.Difficulty + 1), InputManager.ButtonSetting.Button01);
						SoundManager.PlaySE(MusicSelectProcess.DifficultySe[musicSelectData.Difficulty + 1], i);
					}
				}
			}
			else if (ProcessProcessing.CurrentDifficulty[PlayerIndex] + 1 <= MusicDifficultyID.ReMaster && (ProcessProcessing.IsRemasterEnable() || ProcessProcessing.CurrentDifficulty[PlayerIndex] + 1 < MusicDifficultyID.ReMaster))
			{
				ProcessProcessing.CurrentDifficulty[PlayerIndex]++;
				ProcessProcessing.ReCalcLevelSortData();
				monitorArray[PlayerIndex].setForceDiffControlParam(ProcessProcessing.CurrentDifficulty[PlayerIndex] - 1);
				monitorArray[PlayerIndex].AddDifficulty(ProcessProcessing.CurrentDifficulty[PlayerIndex], InputManager.ButtonSetting.Button01);
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)ProcessProcessing.CurrentDifficulty[PlayerIndex]], PlayerIndex);
			}
			for (int j = 0; j < monitorArray.Length; j++)
			{
				ProcessProcessing.SetInputLockInfo(PlayerIndex, 50f);
			}
		}

		private void SubDifficulty(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.IsLevelTab())
			{
				MusicSelectProcess.MusicSelectData musicSelectData = ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect][ProcessProcessing.CurrentMusicSelect].musicSelectData[(int)ProcessProcessing.ScoreType];
				if (musicSelectData.Difficulty - 1 >= 0)
				{
					MusicSelectProcess.LevelCategoryData levelToListPositoin = ProcessProcessing.GetLevelToListPositoin(musicSelectData.MusicData.name.id, musicSelectData.Difficulty - 1);
					ProcessProcessing.CurrentCategorySelect = levelToListPositoin.Category;
					ProcessProcessing.CurrentMusicSelect = levelToListPositoin.Index;
					for (int i = 0; i < monitorArray.Length; i++)
					{
						monitorArray[i].SubDifficulty((MusicDifficultyID)(musicSelectData.Difficulty - 1), InputManager.ButtonSetting.Button08);
						SoundManager.PlaySE(MusicSelectProcess.DifficultySe[musicSelectData.Difficulty - 1], i);
					}
				}
			}
			else if (ProcessProcessing.CurrentDifficulty[PlayerIndex] - 1 >= MusicDifficultyID.Basic)
			{
				ProcessProcessing.CurrentDifficulty[PlayerIndex]--;
				ProcessProcessing.ReCalcLevelSortData();
				monitorArray[PlayerIndex].setForceDiffControlParam(ProcessProcessing.CurrentDifficulty[PlayerIndex] + 1);
				monitorArray[PlayerIndex].SubDifficulty(ProcessProcessing.CurrentDifficulty[PlayerIndex], InputManager.ButtonSetting.Button08);
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)ProcessProcessing.CurrentDifficulty[PlayerIndex]], PlayerIndex);
			}
			for (int j = 0; j < monitorArray.Length; j++)
			{
				ProcessProcessing.SetInputLockInfo(PlayerIndex, 50f);
			}
		}

		private void ScoreKindChange(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.ScoreType == ConstParameter.ScoreKind.Deluxe)
			{
				ProcessProcessing.ScoreType = ConstParameter.ScoreKind.Standard;
			}
			else if (ProcessProcessing.ScoreType == ConstParameter.ScoreKind.Standard)
			{
				ProcessProcessing.ScoreType = ConstParameter.ScoreKind.Deluxe;
			}
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_TAB, PlayerIndex);
			ProcessProcessing.ReCalcLevelSortData();
			for (int i = 0; i < monitorArray.Length; i++)
			{
				MusicDifficultyID difficulty = ProcessProcessing.CurrentDifficulty[i];
				if (ProcessProcessing.IsLevelTab())
				{
					difficulty = (MusicDifficultyID)ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect][ProcessProcessing.CurrentMusicSelect].musicSelectData[(int)ProcessProcessing.ScoreType].Difficulty;
				}
				ProcessProcessing.MonitorArray[i].ChangeScoreKind(difficulty);
				ProcessProcessing.SetInputLockInfo(PlayerIndex, 200f);
			}
		}

		private void ScoreKindMove(MusicSelectMonitor[] monitorArray)
		{
			ProcessProcessing.GetCombineMusic(0).GetID(ProcessProcessing.ScoreType);
			_ = 10000;
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_TAB, PlayerIndex);
			ProcessProcessing.ReCalcLevelSortData();
			for (int i = 0; i < monitorArray.Length; i++)
			{
				MusicDifficultyID difficulty = ProcessProcessing.CurrentDifficulty[i];
				if (ProcessProcessing.IsLevelTab())
				{
					difficulty = (MusicDifficultyID)ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect][ProcessProcessing.CurrentMusicSelect].musicSelectData[(int)ProcessProcessing.ScoreType].Difficulty;
				}
				ProcessProcessing.MonitorArray[i].ChangeScoreKind(difficulty);
				ProcessProcessing.SetInputLockInfo(PlayerIndex, 200f);
			}
		}

		private void CategoryScrollLeft(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.CurrentCategorySelect - 1 < 0)
			{
				ProcessProcessing.CurrentCategorySelect = ((ProcessProcessing.CurrentCategorySelect - 1 >= 0) ? (ProcessProcessing.CurrentCategorySelect - 1) : (ProcessProcessing.CategoryNameList.Count - 1));
			}
			else
			{
				ProcessProcessing.CurrentCategorySelect--;
			}
			ProcessProcessing.CurrentMusicSelect = ProcessProcessing.CombineMusicDataList[ProcessProcessing.CurrentCategorySelect].Count - 1;
			for (int i = 0; i < monitorArray.Length; i++)
			{
				monitorArray[i].ScrollCategoryRight(InputManager.ButtonSetting.Button08);
			}
			ProcessProcessing.SharedIsInputLock = true;
			CheckButton();
			ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
		}

		private void CategoryScrollRight(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.CurrentCategorySelect + 1 < ProcessProcessing.CategoryNameList.Count)
			{
				ProcessProcessing.CurrentCategorySelect++;
			}
			else
			{
				ProcessProcessing.CurrentCategorySelect = 0;
			}
			ProcessProcessing.CurrentMusicSelect = 0;
			for (int i = 0; i < monitorArray.Length; i++)
			{
				monitorArray[i].ScrollCategoryLeft(InputManager.ButtonSetting.Button01);
			}
			ProcessProcessing.SharedIsInputLock = true;
			CheckButton();
			ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
		}

		private bool IsBackEnable()
		{
			return !ProcessProcessing.IsConnectionFolder();
		}

		private bool IsNextEnable()
		{
			if (ProcessProcessing.IsConnectionFolder())
			{
				return ProcessProcessing.RecruitData != null;
			}
			return true;
		}

		private void CheckButton()
		{
			MusicSelectMonitor[] monitorArray = ProcessProcessing.MonitorArray;
			for (int i = 0; i < monitorArray.Length; i++)
			{
				monitorArray[i].SetVisibleButton(IsNextEnable(), InputManager.ButtonSetting.Button04);
				monitorArray[i].SetVisibleButton(IsBackEnable(), InputManager.ButtonSetting.Button05);
			}
		}
	}
}
