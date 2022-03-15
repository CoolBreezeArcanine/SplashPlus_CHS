using System;
using System.Collections.Generic;
using System.Diagnostics;
using DB;
using IO;
using IO.TouchPanel;
using MAI2.Util;
using MAI2System;
using UnityEngine;

namespace Manager
{
	public static class InputManager
	{
		public enum ButtonSetting
		{
			Button01,
			Button02,
			Button03,
			Button04,
			Button05,
			Button06,
			Button07,
			Button08,
			Select,
			End
		}

		public enum TouchPanelArea
		{
			A1,
			A2,
			A3,
			A4,
			A5,
			A6,
			A7,
			A8,
			B1,
			B2,
			B3,
			B4,
			B5,
			B6,
			B7,
			B8,
			C1,
			C2,
			D1,
			D2,
			D3,
			D4,
			D5,
			D6,
			D7,
			D8,
			E1,
			E2,
			E3,
			E4,
			E5,
			E6,
			E7,
			E8,
			Blank,
			End
		}

		public enum SystemButtonSetting
		{
			ButtonTest,
			ButtonService,
			Max
		}

		private const int ButtonDelayFlame = 2;

		private const int NewestButtonFlame = 1;

		private const int PanelDelayFlame = 2;

		private const int NewestPanelFlame = 1;

		private static readonly TouchPanelArea[] TouchAreaA;

		private static readonly TouchPanelArea[] TouchAreaB;

		private static readonly TouchPanelArea[] TouchAreaC;

		private static readonly TouchPanelArea[] TouchAreaD;

		private static readonly TouchPanelArea[] TouchAreaE;

		public static readonly JvsButtonID[] InputSystemTable;

		public static readonly JvsButtonID[,] InputPlayerTable;

		private static readonly bool[] SystemButtonPush;

		private static readonly bool[,] PlayerButtonPush;

		private static readonly bool[] SystemButtonDown;

		private static readonly bool[,] PlayerButtonDown;

		private static readonly bool[] RawSystemButtonPush;

		private static readonly bool[,] RawPlayerButtonPush;

		private static readonly bool[] RawSystemButtonDown;

		private static readonly bool[,] RawPlayerButtonDown;

		private static readonly bool[,,] GameButtonPush;

		private static readonly bool[,,] GameButtonDown;

		private static readonly bool[,,] GamePanelPush;

		private static readonly bool[,,] GamePanelDown;

		private static readonly long[,] PlayerButtonPushTime;

		private static readonly long[,] PlayerTouchPanelPushTime;

		private static readonly long FirstRepeatTime;

		private static readonly long SecondRepeatTime;

		private static readonly long[,] PlayerButtonRepeatTime;

		private static readonly long[,] PlayerTouchPanelRepeatTime;

		private static readonly bool[] AnyButtonDown;

		private static readonly bool[] AnyButtonPush;

		private static readonly bool[] AnyTouchPanelDown;

		private static readonly bool[] AnyTouchPanelPush;

		private static readonly Stopwatch InputStopWatch;

		private static readonly List<int>[] DownButtonList;

		private static readonly List<int>[] PushButtonList;

		private static readonly bool[,] GameInputFrameOnceInput;

		public static ulong[] TpNow;

		public static ulong[] TpPre;

		public static uint[] TpCounterNow;

		public static uint[] TpCounterPre;

		private static readonly GameInputLog TpLog;

		private static readonly TouchPanelSlideAssist TpSlide;

		public static readonly MouseTouchPanel[] MouseTouchPanel;

		private static bool _isMouseTouchPanelVisible;

		public static bool IsButtonDown()
		{
			return IsButtonDown(0) | IsButtonDown(1);
		}

		public static bool IsButtonDown(int target)
		{
			return AnyButtonDown[target];
		}

		public static bool IsTouchPanelDown()
		{
			return IsTouchPanelDown(0) | IsTouchPanelDown(1);
		}

		public static bool IsTouchPanelDown(int target)
		{
			return AnyTouchPanelDown[target];
		}

		public static bool IsButtonPush()
		{
			return IsButtonPush(0) | IsButtonPush(1);
		}

		public static bool IsButtonPush(int target)
		{
			return AnyButtonPush[target];
		}

		public static bool IsTouchPanelPush()
		{
			return IsTouchPanelPush(0) | IsTouchPanelPush(1);
		}

		public static bool IsTouchPanelPush(int target)
		{
			return AnyTouchPanelPush[target];
		}

		public static void RegisterMouseTouchPanel(int target, MouseTouchPanel targetMouseTouchPanel)
		{
			MouseTouchPanel[target] = targetMouseTouchPanel;
		}

		public static bool IsMouseTouchButtonInside(int target, Vector3 screenPosition, out TouchPanelArea touch)
		{
			touch = TouchPanelArea.End;
			return MouseTouchPanel[target].IsInside(screenPosition, out touch);
		}

		public static bool IsMouseTouchButtonInsideFromWorld(int target, Vector3 worldPosition, out TouchPanelArea touch)
		{
			return IsMouseTouchButtonInside(target, RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition), out touch);
		}

		public static bool GetInputDown(int monitorId, ButtonSetting button, TouchPanelArea panelArea)
		{
			if (!GetButtonDown(monitorId, button))
			{
				return GetTouchPanelAreaDown(monitorId, panelArea);
			}
			return true;
		}

		public static bool GetInputDown(ButtonSetting button, TouchPanelArea panelArea)
		{
			if (!GetMonitorButtonDown(button))
			{
				return GetTouchPanelAreaDown(panelArea);
			}
			return true;
		}

		public static bool GetInputLongPush(int monitoId, ButtonSetting button, TouchPanelArea panelArea, long msec)
		{
			if (!GetButtonLongPush(monitoId, button, msec))
			{
				return GetTouchPanelAreaLongPush(monitoId, panelArea, msec);
			}
			return true;
		}

		public static bool GetInputLongPush(ButtonSetting button, TouchPanelArea panelArea, long msec)
		{
			if (!GetMonitorButtonLongPush(button, msec))
			{
				return GetTouchPanelAreaLongPush(panelArea, msec);
			}
			return true;
		}

		public static bool GetInputRepeat(int monitoId, ButtonSetting button, TouchPanelArea panelArea)
		{
			if (!GetMonitorButtonRepeat(out var _, button))
			{
				return GetTouchPanelAreaRepeat(monitoId, panelArea);
			}
			return true;
		}

		public static bool GetInputRepeat(ButtonSetting button, TouchPanelArea panelArea)
		{
			if (!GetMonitorButtonRepeat(button))
			{
				return GetTouchPanelAreaRepeat(panelArea);
			}
			return true;
		}

		public static bool GetButtonDown(int monitorId, ButtonSetting button)
		{
			if (IsButtonDown(monitorId) && PlayerButtonDown[monitorId, (int)button])
			{
				return true;
			}
			return false;
		}

		public static bool GetButtonPush(int monitorId, ButtonSetting button)
		{
			if (IsButtonPush(monitorId) && PlayerButtonPush[monitorId, (int)button])
			{
				return true;
			}
			return false;
		}

		public static bool GetButtonLongPush(int monitorId, ButtonSetting button, long msec)
		{
			if (IsButtonPush(monitorId) && PlayerButtonPushTime[monitorId, (int)button] >= msec)
			{
				return true;
			}
			return false;
		}

		public static bool GetMonitorButtonDown(ButtonSetting button)
		{
			int monitorId;
			return GetMonitorButtonDown(out monitorId, button);
		}

		public static bool GetMonitorButtonPush(ButtonSetting button)
		{
			int monitorId;
			return GetMonitorButtonPush(out monitorId, button);
		}

		public static bool GetMonitorButtonDown(out int monitorId, ButtonSetting button)
		{
			monitorId = -1;
			if (IsButtonDown())
			{
				for (int i = 0; i < 2; i++)
				{
					if (PlayerButtonDown[i, (int)button])
					{
						monitorId = i;
						return true;
					}
				}
			}
			return false;
		}

		public static bool GetMonitorButtonPush(out int monitorId, ButtonSetting button)
		{
			monitorId = -1;
			if (IsButtonPush())
			{
				for (int i = 0; i < 2; i++)
				{
					if (PlayerButtonPush[i, (int)button])
					{
						monitorId = i;
						return true;
					}
				}
			}
			return false;
		}

		public static bool GetMonitorButtonLongPush(ButtonSetting button, long msec)
		{
			int monitorId;
			return GetMonitorButtonLongPush(out monitorId, button, msec);
		}

		public static bool GetMonitorButtonLongPush(out int monitorId, ButtonSetting button, long msec)
		{
			monitorId = -1;
			if (IsButtonPush())
			{
				for (int i = 0; i < 2; i++)
				{
					if (PlayerButtonPushTime[i, (int)button] >= msec)
					{
						monitorId = i;
						return true;
					}
				}
			}
			return false;
		}

		public static bool GetMonitorButtonRepeat(ButtonSetting button)
		{
			int monitorId;
			return GetMonitorButtonRepeat(out monitorId, button);
		}

		public static bool GetMonitorButtonRepeat(out int monitorId, ButtonSetting button)
		{
			monitorId = -1;
			if (IsButtonPush())
			{
				for (int i = 0; i < 2; i++)
				{
					if (PlayerButtonRepeatTime[i, (int)button] == 0L)
					{
						monitorId = i;
						return true;
					}
				}
			}
			return false;
		}

		public static bool GetMonitorMultiButtonDown(out int[][] inputMonitorButtonArrays)
		{
			inputMonitorButtonArrays = null;
			if (IsButtonDown())
			{
				inputMonitorButtonArrays = new int[2][]
				{
					DownButtonList[0].ToArray(),
					DownButtonList[1].ToArray()
				};
				return true;
			}
			return false;
		}

		public static bool GetMonitorMultiButtonPush(out int[][] inputMonitorButtonArrays)
		{
			inputMonitorButtonArrays = null;
			if (IsButtonPush())
			{
				inputMonitorButtonArrays = new int[2][]
				{
					PushButtonList[0].ToArray(),
					PushButtonList[1].ToArray()
				};
				return true;
			}
			return false;
		}

		public static long GetButtonPushTime(int monitorId, ButtonSetting button)
		{
			if (IsButtonPush())
			{
				return PlayerButtonPushTime[monitorId, (int)button];
			}
			return 0L;
		}

		public static bool IsUsedThisFrame(int monitorId, TouchPanelArea area)
		{
			return GameInputFrameOnceInput[monitorId, (int)area];
		}

		public static bool IsUsedThisFrame(int monitorId, ButtonSetting button)
		{
			return GameInputFrameOnceInput[monitorId, (int)TouchAreaA[(int)button]];
		}

		public static void SetUsedThisFrame(int monitorId, TouchPanelArea area)
		{
			GameInputFrameOnceInput[monitorId, (int)area] = true;
		}

		public static void SetUsedThisFrame(int monitorId, ButtonSetting button)
		{
			GameInputFrameOnceInput[monitorId, (int)TouchAreaA[(int)button]] = true;
		}

		public static bool GetSystemInputDown(SystemButtonSetting button)
		{
			if (SystemButtonDown[(int)button])
			{
				return true;
			}
			return false;
		}

		public static bool GetSystemInputPush(SystemButtonSetting button)
		{
			if (SystemButtonPush[(int)button])
			{
				return true;
			}
			return false;
		}

		public static void Initialize()
		{
			InputStopWatch.Reset();
			InputStopWatch.Start();
			for (int i = 0; i < 2; i++)
			{
				TpNow[i] = 0uL;
				TpPre[i] = 0uL;
				TpCounterNow[i] = 0u;
				TpCounterPre[i] = 0u;
				for (int j = 0; j < 9; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						GameButtonPush[k, i, j] = false;
						GameButtonDown[k, i, j] = false;
					}
					PlayerButtonPush[i, j] = false;
					PlayerButtonDown[i, j] = false;
					PlayerButtonPushTime[i, j] = 0L;
				}
				for (int l = 0; l < 35; l++)
				{
					for (int m = 0; m < 2; m++)
					{
						GamePanelPush[m, i, l] = false;
						GamePanelDown[m, i, l] = false;
					}
				}
				AnyButtonDown[i] = false;
				AnyButtonPush[i] = false;
				DownButtonList[i].Clear();
				PushButtonList[i].Clear();
			}
			for (int n = 0; n < 2; n++)
			{
				SystemButtonPush[n] = false;
				SystemButtonDown[n] = false;
			}
		}

		public static void UpdateAmInput()
		{
			if (DebugInput.GetKeyDown(KeyCode.G))
			{
				_isMouseTouchPanelVisible = !_isMouseTouchPanelVisible;
				MouseTouchPanel[] mouseTouchPanel = MouseTouchPanel;
				for (int i = 0; i < mouseTouchPanel.Length; i++)
				{
					mouseTouchPanel[i].SetVisible(_isMouseTouchPanelVisible);
				}
			}
			for (int j = 0; j < 2; j++)
			{
				AnyButtonDown[j] = false;
				AnyButtonPush[j] = false;
				AnyTouchPanelDown[j] = false;
				AnyTouchPanelPush[j] = false;
				DownButtonList[j].Clear();
				PushButtonList[j].Clear();
			}
			if (!MechaManager.IsInitialized)
			{
				return;
			}
			for (int k = 0; k < 2; k++)
			{
				RawSystemButtonPush[k] |= MechaManager.Jvs.GetRawState(InputSystemTable[k]);
				RawSystemButtonDown[k] |= MechaManager.Jvs.GetTriggerOn(InputSystemTable[k]);
			}
			for (int l = 0; l < 2; l++)
			{
				for (int m = 0; m < 9; m++)
				{
					RawPlayerButtonPush[l, m] |= MechaManager.Jvs.GetRawState(InputPlayerTable[l, m]);
					RawPlayerButtonDown[l, m] |= MechaManager.Jvs.GetTriggerOn(InputPlayerTable[l, m]);
				}
			}
			for (int n = 0; n < 2; n++)
			{
				for (int num = 0; num < 1; num++)
				{
					for (int num2 = 0; num2 < 9; num2++)
					{
						GameButtonPush[num, n, num2] = GameButtonPush[num + 1, n, num2];
						GameButtonDown[num, n, num2] = GameButtonDown[num + 1, n, num2];
					}
				}
				for (int num3 = 0; num3 < 1; num3++)
				{
					for (int num4 = 0; num4 < 35; num4++)
					{
						GamePanelPush[num3, n, num4] = GamePanelPush[num3 + 1, n, num4];
						GamePanelDown[num3, n, num4] = GamePanelDown[num3 + 1, n, num4];
					}
				}
			}
			for (int num5 = 0; num5 < 2; num5++)
			{
				SystemButtonPush[num5] = RawSystemButtonPush[num5];
				SystemButtonDown[num5] = RawSystemButtonDown[num5];
				RawSystemButtonPush[num5] = false;
				RawSystemButtonDown[num5] = false;
			}
			for (int num6 = 0; num6 < 2; num6++)
			{
				for (int num7 = 0; num7 < 9; num7++)
				{
					GameButtonPush[1, num6, num7] = RawPlayerButtonPush[num6, num7];
					GameButtonDown[1, num6, num7] = RawPlayerButtonDown[num6, num7];
					PlayerButtonPush[num6, num7] = RawPlayerButtonPush[num6, num7];
					PlayerButtonDown[num6, num7] = RawPlayerButtonDown[num6, num7];
					RawPlayerButtonPush[num6, num7] = false;
					RawPlayerButtonDown[num6, num7] = false;
				}
				for (int num8 = 0; num8 < 35; num8++)
				{
					GamePanelPush[1, num6, num8] = TpLog.StateOn((uint)num6, (TouchPanelArea)num8);
					GamePanelDown[1, num6, num8] = TpLog.TriggerOn((uint)num6, (TouchPanelArea)num8);
				}
			}
			long elapsedMilliseconds = InputStopWatch.ElapsedMilliseconds;
			for (int num9 = 0; num9 < 2; num9++)
			{
				for (int num10 = 0; num10 < 9; num10++)
				{
					if (PlayerButtonDown[num9, num10])
					{
						AnyButtonDown[num9] = true;
						DownButtonList[num9].Add(num10);
					}
					if (PlayerButtonPush[num9, num10])
					{
						AnyButtonPush[num9] = true;
						PushButtonList[num9].Add(num10);
						if (PlayerButtonRepeatTime[num9, num10] != 0L)
						{
							if (PlayerButtonRepeatTime[num9, num10] > 0)
							{
								if (PlayerButtonRepeatTime[num9, num10] > elapsedMilliseconds)
								{
									PlayerButtonRepeatTime[num9, num10] -= elapsedMilliseconds;
								}
								else
								{
									PlayerButtonRepeatTime[num9, num10] = 0L;
								}
							}
						}
						else
						{
							PlayerButtonRepeatTime[num9, num10] = SecondRepeatTime;
						}
						if (!PlayerButtonDown[num9, num10])
						{
							PlayerButtonPushTime[num9, num10] += elapsedMilliseconds;
						}
						else
						{
							PlayerButtonRepeatTime[num9, num10] = FirstRepeatTime;
						}
					}
					else
					{
						PlayerButtonPushTime[num9, num10] = 0L;
						PlayerButtonRepeatTime[num9, num10] = -1L;
					}
				}
			}
			for (int num11 = 0; num11 < 2; num11++)
			{
				for (int num12 = 0; num12 < 35; num12++)
				{
					if (GetTouchPanelAreaDown(num11, (TouchPanelArea)num12))
					{
						AnyTouchPanelDown[num11] = true;
					}
					if (GetTouchPanelAreaPush(num11, (TouchPanelArea)num12))
					{
						AnyTouchPanelPush[num11] = true;
						if (PlayerTouchPanelRepeatTime[num11, num12] != 0L)
						{
							if (PlayerTouchPanelRepeatTime[num11, num12] > 0)
							{
								if (PlayerTouchPanelRepeatTime[num11, num12] > elapsedMilliseconds)
								{
									PlayerTouchPanelRepeatTime[num11, num12] -= elapsedMilliseconds;
								}
								else
								{
									PlayerTouchPanelRepeatTime[num11, num12] = 0L;
								}
							}
						}
						else
						{
							PlayerTouchPanelRepeatTime[num11, num12] = SecondRepeatTime;
						}
						if (!GetTouchPanelAreaDown(num11, (TouchPanelArea)num12))
						{
							PlayerTouchPanelPushTime[num11, num12] += elapsedMilliseconds;
						}
						else
						{
							PlayerTouchPanelRepeatTime[num11, num12] = FirstRepeatTime;
						}
					}
					else
					{
						PlayerTouchPanelPushTime[num11, num12] = 0L;
						PlayerTouchPanelRepeatTime[num11, num12] = -1L;
					}
				}
			}
			for (int num13 = 0; num13 < 2; num13++)
			{
				for (int num14 = 0; num14 < 35; num14++)
				{
					GameInputFrameOnceInput[num13, num14] = false;
				}
			}
			InputStopWatch.Reset();
			InputStopWatch.Start();
		}

		public static bool SetNewTouchPanel(uint index, ulong inputData, uint counter)
		{
			if (TpCounterNow[index] != counter)
			{
				TpNow[index] = inputData;
				TpCounterNow[index] = counter;
				return true;
			}
			return false;
		}

		public static void UpdateTouchPanel()
		{
			for (int i = 0; i < 2; i++)
			{
				if (Singleton<SystemConfig>.Instance.config.IsDummyTouchPanel)
				{
					TpNow[i] |= MouseTouchPanel[i].GetPushFlag;
					TpLog.Execute(i, TpNow[i]);
					SetNewTouchPanel((uint)i, 0uL, TpCounterNow[i] + 1);
				}
				else
				{
					TpLog.Execute(i, TpNow[i], TpCounterNow[i] != TpCounterPre[i]);
					TpCounterPre[i] = TpCounterNow[i];
				}
			}
			TpSlide.Execute(TpLog);
		}

		public static bool GetTouchPanelAreaDown(TouchPanelArea button)
		{
			return GetTouchPanelAreaDown(0, button) | GetTouchPanelAreaDown(1, button);
		}

		public static bool GetTouchPanelAreaDown(int player, TouchPanelArea button)
		{
			return TpLog.TriggerOn((uint)player, button);
		}

		public static bool GetTouchPanelAreaDown(int player, ButtonSetting button)
		{
			return TpLog.TriggerOn((uint)player, TouchAreaA[(int)button]);
		}

		public static bool GetTouchPanelAreaDown(int player, params TouchPanelArea[] button)
		{
			bool flag = false;
			foreach (TouchPanelArea type in button)
			{
				flag |= TpLog.TriggerOn((uint)player, type);
			}
			return flag;
		}

		public static bool GetTouchPanelArea_B_Down(int player, ButtonSetting button)
		{
			return TpLog.TriggerOn((uint)player, TouchAreaB[(int)button]);
		}

		public static bool GetTouchPanelArea_C_Down(int player)
		{
			if (!TpLog.TriggerOn((uint)player, TouchAreaC[0]))
			{
				return TpLog.TriggerOn((uint)player, TouchAreaC[1]);
			}
			return true;
		}

		public static bool GetTouchPanelArea_D_Down(int player, ButtonSetting button)
		{
			return TpLog.TriggerOn((uint)player, TouchAreaD[(int)button]);
		}

		public static bool GetTouchPanelArea_E_Down(int player, ButtonSetting button)
		{
			return TpLog.TriggerOn((uint)player, TouchAreaE[(int)button]);
		}

		public static bool GetTouchPanelAreaPush(TouchPanelArea button)
		{
			return GetTouchPanelAreaPush(0, button) | GetTouchPanelAreaPush(1, button);
		}

		public static bool GetTouchPanelAreaPush(int player, TouchPanelArea button)
		{
			return TpLog.StateOn((uint)player, button);
		}

		public static bool GetTouchPanelAreaPush(int player, ButtonSetting button)
		{
			return TpLog.StateOn((uint)player, TouchAreaA[(int)button]);
		}

		public static bool GetTouchPanelArea_C_Push(int player)
		{
			if (!TpLog.StateOn((uint)player, TouchAreaC[0]))
			{
				return TpLog.StateOn((uint)player, TouchAreaC[1]);
			}
			return true;
		}

		public static bool GetTouchPanelArea_D_Push(int player, ButtonSetting button)
		{
			return TpLog.StateOn((uint)player, TouchAreaD[(int)button]);
		}

		public static bool GetTouchPanelArea_E_Push(int player, ButtonSetting button)
		{
			return TpLog.StateOn((uint)player, TouchAreaE[(int)button]);
		}

		public static bool GetTouchPanelAreaLongPush(int monitorId, TouchPanelArea area, long msec)
		{
			if (IsTouchPanelPush(monitorId) && PlayerTouchPanelPushTime[monitorId, (int)area] >= msec)
			{
				return true;
			}
			return false;
		}

		public static bool GetTouchPanelAreaRepeat(int monitorId, TouchPanelArea area)
		{
			if (IsTouchPanelPush(monitorId) && PlayerTouchPanelRepeatTime[monitorId, (int)area] == 0L)
			{
				return true;
			}
			return false;
		}

		public static bool GetTouchPanelAreaLongPush(TouchPanelArea area, long msec)
		{
			return GetTouchPanelAreaLongPush(0, area, msec) | GetTouchPanelAreaLongPush(1, area, msec);
		}

		public static bool GetTouchPanelAreaRepeat(TouchPanelArea area)
		{
			return GetTouchPanelAreaRepeat(0, area) | GetTouchPanelAreaRepeat(1, area);
		}

		public static long GetTouchPanelAreaPushTime(int monitorId, TouchPanelArea area)
		{
			return PlayerTouchPanelPushTime[monitorId, (int)area];
		}

		public static bool SlideAreaLr()
		{
			return SlideAreaLr(0) | SlideAreaLr(1);
		}

		public static bool SlideAreaLr(int player)
		{
			return TpSlide.IsMoveLr((uint)player);
		}

		public static uint SlideAreaLrLevel(int player)
		{
			return TpSlide.IsMoveLrLevel((uint)player);
		}

		public static bool SlideAreaRl()
		{
			return SlideAreaRl(0) | SlideAreaRl(1);
		}

		public static bool SlideAreaRl(int player)
		{
			return TpSlide.IsMoveRl((uint)player);
		}

		public static uint SlideAreaRlLevel(int player)
		{
			return TpSlide.IsMoveRlLevel((uint)player);
		}

		public static void SetButtonDown(int monitorId, ButtonSetting button)
		{
			PlayerButtonDown[monitorId, (int)button] = true;
			AnyButtonDown[monitorId] = true;
		}

		public static bool InGameButtonDown(int monitorId, ButtonSetting button)
		{
			return GameButtonDown[0, monitorId, (int)button];
		}

		public static bool InGameButtonPush(int monitorId, ButtonSetting button)
		{
			return GameButtonPush[0, monitorId, (int)button];
		}

		public static bool InGameTouchPanelAreaDown(int player, TouchPanelArea button)
		{
			return GamePanelDown[0, player, (int)button];
		}

		public static bool InGameTouchPanelAreaDown(int player, ButtonSetting button)
		{
			return GamePanelDown[0, player, (int)TouchAreaA[(int)button]];
		}

		public static bool InGameTouchPanelArea_B_Down(int player, ButtonSetting button)
		{
			return GamePanelDown[0, player, (int)TouchAreaB[(int)button]];
		}

		public static bool InGameTouchPanelArea_C_Down(int player)
		{
			if (!GamePanelDown[0, player, (int)TouchAreaC[0]])
			{
				return GamePanelDown[0, player, (int)TouchAreaC[1]];
			}
			return true;
		}

		public static bool InGameTouchPanelArea_D_Down(int player, ButtonSetting button)
		{
			return GamePanelDown[0, player, (int)TouchAreaD[(int)button]];
		}

		public static bool InGameTouchPanelArea_E_Down(int player, ButtonSetting button)
		{
			return GamePanelDown[0, player, (int)TouchAreaE[(int)button]];
		}

		public static bool InGameTouchPanelAreaPush(int player, TouchPanelArea button)
		{
			return GamePanelPush[0, player, (int)button];
		}

		public static bool InGameTouchPanelAreaPush(int player, ButtonSetting button)
		{
			return GamePanelPush[0, player, (int)TouchAreaA[(int)button]];
		}

		public static bool InGameTouchPanelArea_C_Push(int player)
		{
			if (!GamePanelPush[0, player, (int)TouchAreaC[0]])
			{
				return GamePanelPush[0, player, (int)TouchAreaC[1]];
			}
			return true;
		}

		public static bool InGameTouchPanelArea_D_Push(int player, ButtonSetting button)
		{
			return GamePanelPush[0, player, (int)TouchAreaD[(int)button]];
		}

		public static bool InGameTouchPanelArea_E_Push(int player, ButtonSetting button)
		{
			return GamePanelPush[0, player, (int)TouchAreaE[(int)button]];
		}

		public static bool ConvertTouchPanelRotatePush(int monitorID, TouchPanelArea area, int buttonID)
		{
			switch (area)
			{
			case TouchPanelArea.A1:
			case TouchPanelArea.A2:
			case TouchPanelArea.A3:
			case TouchPanelArea.A4:
			case TouchPanelArea.A5:
			case TouchPanelArea.A6:
			case TouchPanelArea.A7:
			case TouchPanelArea.A8:
			{
				int num = Array.IndexOf(TouchAreaA, area);
				num = (num + buttonID) % TouchAreaA.Length;
				return InGameTouchPanelAreaPush(monitorID, TouchAreaA[num]);
			}
			case TouchPanelArea.B1:
			case TouchPanelArea.B2:
			case TouchPanelArea.B3:
			case TouchPanelArea.B4:
			case TouchPanelArea.B5:
			case TouchPanelArea.B6:
			case TouchPanelArea.B7:
			case TouchPanelArea.B8:
			{
				int num = Array.IndexOf(TouchAreaB, area);
				num = (num + buttonID) % TouchAreaB.Length;
				return InGameTouchPanelAreaPush(monitorID, TouchAreaB[num]);
			}
			case TouchPanelArea.C1:
			case TouchPanelArea.C2:
				return InGameTouchPanelArea_C_Push(monitorID);
			case TouchPanelArea.D1:
			case TouchPanelArea.D2:
			case TouchPanelArea.D3:
			case TouchPanelArea.D4:
			case TouchPanelArea.D5:
			case TouchPanelArea.D6:
			case TouchPanelArea.D7:
			case TouchPanelArea.D8:
			{
				int num = Array.IndexOf(TouchAreaD, area);
				num = (num + buttonID) % TouchAreaD.Length;
				return InGameTouchPanelAreaPush(monitorID, TouchAreaD[num]);
			}
			case TouchPanelArea.E1:
			case TouchPanelArea.E2:
			case TouchPanelArea.E3:
			case TouchPanelArea.E4:
			case TouchPanelArea.E5:
			case TouchPanelArea.E6:
			case TouchPanelArea.E7:
			case TouchPanelArea.E8:
			{
				int num = Array.IndexOf(TouchAreaE, area);
				num = (num + buttonID) % TouchAreaE.Length;
				return InGameTouchPanelAreaPush(monitorID, TouchAreaE[num]);
			}
			default:
				return false;
			}
		}

		static InputManager()
		{
			TouchAreaA = new TouchPanelArea[8]
			{
				TouchPanelArea.A1,
				TouchPanelArea.A2,
				TouchPanelArea.A3,
				TouchPanelArea.A4,
				TouchPanelArea.A5,
				TouchPanelArea.A6,
				TouchPanelArea.A7,
				TouchPanelArea.A8
			};
			TouchAreaB = new TouchPanelArea[8]
			{
				TouchPanelArea.B1,
				TouchPanelArea.B2,
				TouchPanelArea.B3,
				TouchPanelArea.B4,
				TouchPanelArea.B5,
				TouchPanelArea.B6,
				TouchPanelArea.B7,
				TouchPanelArea.B8
			};
			TouchAreaC = new TouchPanelArea[2]
			{
				TouchPanelArea.C1,
				TouchPanelArea.C2
			};
			TouchAreaD = new TouchPanelArea[8]
			{
				TouchPanelArea.D1,
				TouchPanelArea.D2,
				TouchPanelArea.D3,
				TouchPanelArea.D4,
				TouchPanelArea.D5,
				TouchPanelArea.D6,
				TouchPanelArea.D7,
				TouchPanelArea.D8
			};
			TouchAreaE = new TouchPanelArea[8]
			{
				TouchPanelArea.E1,
				TouchPanelArea.E2,
				TouchPanelArea.E3,
				TouchPanelArea.E4,
				TouchPanelArea.E5,
				TouchPanelArea.E6,
				TouchPanelArea.E7,
				TouchPanelArea.E8
			};
			InputSystemTable = new JvsButtonID[2]
			{
				JvsButtonID.Test,
				JvsButtonID.Service
			};
			InputPlayerTable = new JvsButtonID[2, 9]
			{
				{
					JvsButtonID.Button1_1P,
					JvsButtonID.Button2_1P,
					JvsButtonID.Button3_1P,
					JvsButtonID.Button4_1P,
					JvsButtonID.Button5_1P,
					JvsButtonID.Button6_1P,
					JvsButtonID.Button7_1P,
					JvsButtonID.Button8_1P,
					JvsButtonID.Select_1P
				},
				{
					JvsButtonID.Button1_2P,
					JvsButtonID.Button2_2P,
					JvsButtonID.Button3_2P,
					JvsButtonID.Button4_2P,
					JvsButtonID.Button5_2P,
					JvsButtonID.Button6_2P,
					JvsButtonID.Button7_2P,
					JvsButtonID.Button8_2P,
					JvsButtonID.Select_2P
				}
			};
			SystemButtonPush = new bool[2];
			PlayerButtonPush = new bool[2, 9];
			SystemButtonDown = new bool[2];
			PlayerButtonDown = new bool[2, 9];
			RawSystemButtonPush = new bool[2];
			RawPlayerButtonPush = new bool[2, 9];
			RawSystemButtonDown = new bool[2];
			RawPlayerButtonDown = new bool[2, 9];
			GameButtonPush = new bool[2, 2, 9];
			GameButtonDown = new bool[2, 2, 9];
			GamePanelPush = new bool[2, 2, 35];
			GamePanelDown = new bool[2, 2, 35];
			PlayerButtonPushTime = new long[2, 9];
			PlayerTouchPanelPushTime = new long[2, 35];
			FirstRepeatTime = 1000L;
			SecondRepeatTime = 200L;
			PlayerButtonRepeatTime = new long[2, 9];
			PlayerTouchPanelRepeatTime = new long[2, 35];
			AnyButtonDown = new bool[2];
			AnyButtonPush = new bool[2];
			AnyTouchPanelDown = new bool[2];
			AnyTouchPanelPush = new bool[2];
			InputStopWatch = new Stopwatch();
			DownButtonList = new List<int>[2]
			{
				new List<int>(),
				new List<int>()
			};
			PushButtonList = new List<int>[2]
			{
				new List<int>(),
				new List<int>()
			};
			GameInputFrameOnceInput = new bool[2, 35];
			TpNow = new ulong[2];
			TpPre = new ulong[2];
			TpCounterNow = new uint[2];
			TpCounterPre = new uint[2];
			TpLog = new GameInputLog();
			TpSlide = new TouchPanelSlideAssist();
			MouseTouchPanel = new MouseTouchPanel[2];
		}
	}
}
