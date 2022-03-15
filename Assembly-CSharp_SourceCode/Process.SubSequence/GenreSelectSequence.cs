using IO;
using Mai2.Mai2Cue;
using Mai2.Voice_Partner_000001;
using Manager;
using Monitor;
using UnityEngine;

namespace Process.SubSequence
{
	public class GenreSelectSequence : SequenceBase
	{
		private const int CATEGORY_OUT_TIME = 750;

		private int _inCategoryIndex;

		public override void Initialize()
		{
		}

		public GenreSelectSequence(int index, bool isValidity, IMusicSelectProcessProcessing processing)
			: base(index, isValidity, processing)
		{
		}

		public override void OnStartSequence()
		{
			if (base.IsValidity)
			{
				MechaManager.LedIf[PlayerIndex].SetColor(2, Color.white);
				MechaManager.LedIf[PlayerIndex].SetColor(5, Color.white);
				MusicSelectMonitor obj = ProcessProcessing.MonitorArray[PlayerIndex];
				obj.OnStartGenreSelect();
				obj.SetChallengeBGHide();
				_inCategoryIndex = ProcessProcessing.CurrentCategorySelect;
				MusicSelectMonitor[] monitorArray = ProcessProcessing.MonitorArray;
				CheckButton(monitorArray);
				SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000182, PlayerIndex);
			}
		}

		public override bool Update()
		{
			if (!base.IsValidity)
			{
				return false;
			}
			MusicSelectMonitor[] monitorArray = ProcessProcessing.MonitorArray;
			bool result = false;
			for (int i = 0; i < monitorArray.Length; i++)
			{
				monitorArray[i].UpdateGenreBackground();
				monitorArray[i].ChangeGenreBackground();
			}
			if (ProcessProcessing.SharedIsInputLock)
			{
				MusicSelectMonitor[] array = monitorArray;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].AnimationUpdate(Timer / 100f);
				}
				Timer += GameManager.GetGameMSecAdd();
				if (Timer > 100f - ProcessProcessing.SharedSlideScrollTime)
				{
					Timer = 0f;
					if (0 < ProcessProcessing.SharedSlideScrollCount)
					{
						ProcessProcessing.SharedSlideScrollCount--;
						ScrollUpdate(monitorArray);
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
					if (_inCategoryIndex != ProcessProcessing.CurrentCategorySelect)
					{
						ProcessProcessing.CurrentMusicSelect = 0;
					}
					for (int k = 0; k < monitorArray.Length; k++)
					{
						if (base.IsValidity)
						{
							ProcessProcessing.MonitorArray[k].OnStartMusicSelect();
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, k);
							monitorArray[k].SetScrollGenreCard(isVisible: false);
							ProcessProcessing.SetInputLockInfo(k, 750f);
						}
					}
					SoundManager.StopBGM(2);
					SyncNext(MusicSelectProcess.SubSequence.Music);
					result = true;
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3) || InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L))
				{
					int num = ProcessProcessing.CombineMusicDataList.Count;
					if (!GameManager.IsFreedomMode)
					{
						num--;
					}
					if (ProcessProcessing.CurrentCategorySelect + 1 < num)
					{
						ProcessProcessing.CurrentCategorySelect++;
						for (int l = 0; l < monitorArray.Length; l++)
						{
							if (base.IsValidity)
							{
								monitorArray[l].ScrollGenreCategoryLeft();
								monitorArray[l].SetScrollGenreCard(InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button03, InputManager.TouchPanelArea.A3, 1000L));
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, l);
								CheckButton(monitorArray);
							}
						}
						ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
						ProcessProcessing.SharedIsInputLock = true;
						result = true;
					}
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6) || InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L))
				{
					if (ProcessProcessing.CurrentCategorySelect - 1 >= 0)
					{
						ProcessProcessing.CurrentCategorySelect--;
						for (int m = 0; m < monitorArray.Length; m++)
						{
							if (base.IsValidity)
							{
								monitorArray[m].ScrollGenreCategoryRight();
								monitorArray[m].SetScrollGenreCard(InputManager.GetInputLongPush(PlayerIndex, InputManager.ButtonSetting.Button06, InputManager.TouchPanelArea.A6, 1000L));
								CheckButton(monitorArray);
								SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, m);
							}
						}
						ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
						ProcessProcessing.SharedIsInputLock = true;
						result = true;
					}
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button01, InputManager.TouchPanelArea.A1))
				{
					AddDifficulty(monitorArray);
				}
				else if (InputManager.GetInputDown(PlayerIndex, InputManager.ButtonSetting.Button08, InputManager.TouchPanelArea.A8))
				{
					SubDifficulty(monitorArray);
				}
				else if (InputManager.GetButtonDown(PlayerIndex, InputManager.ButtonSetting.Select))
				{
					for (int n = 0; n < monitorArray.Length; n++)
					{
						if (base.IsValidity)
						{
							SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_FIX, n);
							monitorArray[n].SetScrollGenreCard(isVisible: false);
						}
					}
					ProcessProcessing.SetInputLockInfo(PlayerIndex, 200f);
					SyncNext(MusicSelectProcess.SubSequence.SortSetting);
					result = true;
				}
				else if (InputManager.SlideAreaLr(PlayerIndex))
				{
					if (ProcessProcessing.CurrentCategorySelect > 0)
					{
						ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
						ProcessProcessing.SharedIsInputLock = true;
						ProcessProcessing.SharedSlideScrollToRight = true;
						result = true;
						ProcessProcessing.SharedSlideScrollTime = 50f;
						ProcessProcessing.IsMaiListFromCategoryIndex(-1);
						ProcessProcessing.SharedSlideScrollCount++;
						while (ProcessProcessing.CurrentCategorySelect - ProcessProcessing.SharedSlideScrollCount > 0 && ProcessProcessing.IsMaiListFromCategoryIndex(-ProcessProcessing.SharedSlideScrollCount) == ProcessProcessing.IsMaiListFromCategoryIndex(-ProcessProcessing.SharedSlideScrollCount - 1))
						{
							ProcessProcessing.SharedSlideScrollCount++;
							if (ProcessProcessing.SharedSlideScrollCount >= 10)
							{
								break;
							}
						}
						ProcessProcessing.SharedSlideScrollCount--;
						ScrollUpdate(monitorArray);
					}
				}
				else if (InputManager.SlideAreaRl(PlayerIndex))
				{
					int num2 = ProcessProcessing.CombineMusicDataList.Count;
					if (!GameManager.IsFreedomMode)
					{
						num2--;
					}
					if (ProcessProcessing.CurrentCategorySelect + 1 < num2)
					{
						ProcessProcessing.SetInputLockInfo(PlayerIndex, 100f);
						ProcessProcessing.SharedIsInputLock = true;
						ProcessProcessing.SharedSlideScrollToRight = false;
						result = true;
						ProcessProcessing.SharedSlideScrollCount++;
						while (ProcessProcessing.CurrentCategorySelect + 1 + ProcessProcessing.SharedSlideScrollCount < num2 && ProcessProcessing.IsMaiListFromCategoryIndex(ProcessProcessing.SharedSlideScrollCount - 1) == ProcessProcessing.IsMaiListFromCategoryIndex(ProcessProcessing.SharedSlideScrollCount))
						{
							ProcessProcessing.SharedSlideScrollCount++;
							if (ProcessProcessing.SharedSlideScrollCount >= 10)
							{
								break;
							}
						}
						ProcessProcessing.SharedSlideScrollTime = 50f;
						ProcessProcessing.SharedSlideScrollCount--;
						ScrollUpdate(monitorArray);
					}
				}
				else if (IsChange)
				{
					for (int num3 = 0; num3 < monitorArray.Length; num3++)
					{
						monitorArray[num3].SetScrollGenreCard(isVisible: false);
					}
					IsChange = false;
				}
			}
			return result;
		}

		private void ScrollUpdate(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.SharedSlideScrollToRight)
			{
				ProcessProcessing.CurrentCategorySelect--;
			}
			else
			{
				ProcessProcessing.CurrentCategorySelect++;
			}
			for (int i = 0; i < monitorArray.Length; i++)
			{
				if (ProcessProcessing.SharedSlideScrollToRight)
				{
					monitorArray[i].ScrollGenreCategoryRight();
				}
				else
				{
					monitorArray[i].ScrollGenreCategoryLeft();
				}
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_SYS_CURSOR, i);
			}
			CheckButton(monitorArray);
		}

		private void AddDifficulty(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.CurrentDifficulty[PlayerIndex] + 1 <= MusicDifficultyID.ReMaster)
			{
				ProcessProcessing.CurrentDifficulty[PlayerIndex]++;
				ProcessProcessing.ReCalcGenreSelectData();
				ProcessProcessing.MonitorArray[PlayerIndex].AddChategoryDifficulty(ProcessProcessing.CurrentDifficulty[PlayerIndex], InputManager.ButtonSetting.Button01);
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)ProcessProcessing.CurrentDifficulty[PlayerIndex]], PlayerIndex);
			}
			ProcessProcessing.SetInputLockInfo(PlayerIndex, 50f);
		}

		private void SubDifficulty(MusicSelectMonitor[] monitorArray)
		{
			if (ProcessProcessing.CurrentDifficulty[PlayerIndex] - 1 >= MusicDifficultyID.Basic)
			{
				ProcessProcessing.CurrentDifficulty[PlayerIndex]--;
				ProcessProcessing.ReCalcGenreSelectData();
				ProcessProcessing.MonitorArray[PlayerIndex].SubCategoryDifficulty(ProcessProcessing.CurrentDifficulty[PlayerIndex], InputManager.ButtonSetting.Button08);
				SoundManager.PlaySE(MusicSelectProcess.DifficultySe[(int)ProcessProcessing.CurrentDifficulty[PlayerIndex]], PlayerIndex);
			}
			ProcessProcessing.SetInputLockInfo(PlayerIndex, 50f);
		}

		private void CheckButton(MusicSelectMonitor[] monitorArray)
		{
			int num = ProcessProcessing.CombineMusicDataList.Count;
			if (!GameManager.IsFreedomMode)
			{
				num--;
			}
			for (int i = 0; i < monitorArray.Length; i++)
			{
				monitorArray[i].SetVisibleButton(ProcessProcessing.CurrentCategorySelect + 1 != num, InputManager.ButtonSetting.Button03);
				monitorArray[i].SetVisibleButton(ProcessProcessing.CurrentCategorySelect != 0, InputManager.ButtonSetting.Button06);
			}
		}
	}
}
