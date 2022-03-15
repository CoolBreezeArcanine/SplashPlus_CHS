using System;
using System.Diagnostics;
using DB;
using IO;
using MAI2.Util;
using MAI2System;
using Manager;
using TMPro;
using Util;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageTouchPanel1PTest : TestModePage
	{
		protected enum MenuItem
		{
			SensValue,
			Status,
			SensUp,
			SensDown,
			SensSet,
			Max
		}

		protected enum State
		{
			Start,
			Initialize,
			Proc,
			Error,
			Finish
		}

		protected int TouchSensParamNow;

		protected int TouchSensParamPre;

		protected Stopwatch Timer = new Stopwatch();

		protected bool Updating;

		protected int PlayerIndex;

		protected State _state;

		protected const int WriteTimeOutMsec = 10000;

		protected override void OnCreate()
		{
			base.OnCreate();
			TouchSensParamPre = (TouchSensParamNow = GetTouchPanelLevel());
			Updating = false;
			PlayerIndex = 0;
			foreach (Item item in ItemList)
			{
				if ((bool)item.ValueText)
				{
					item.ValueText.alignment = TextAlignmentOptions.TopLeft;
				}
			}
			_state = State.Start;
			Timer.Restart();
		}

		protected override void OnUpdate()
		{
			switch (_state)
			{
			case State.Start:
				ProcInit();
				break;
			case State.Initialize:
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status == NewTouchPanel.StatusEnum.Drive)
				{
					ProcInitializeEnd();
				}
				else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status == NewTouchPanel.StatusEnum.Error)
				{
					ProcInitializeEnd();
					_state = State.Error;
				}
				break;
			case State.Proc:
				if ((Updating && !SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].IsSenssivity()) || Timer.ElapsedMilliseconds > 10000)
				{
					Updating = false;
				}
				break;
			case State.Error:
			case State.Finish:
				break;
			}
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				if (TouchSensParamPre != TouchSensParamNow)
				{
					item.SetValueString(GetParamToString(TouchSensParamPre) + " => " + GetParamToString(TouchSensParamNow));
				}
				else
				{
					item.SetValueString(GetParamToString(TouchSensParamPre));
				}
				break;
			case 1:
				switch (_state)
				{
				case State.Initialize:
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status < NewTouchPanel.StatusEnum.Drive)
					{
						item.SetValueString(Utility.isBlinkDisp(Timer) ? TestmodeTouchpanel1pID.Status0.GetName() : "");
					}
					else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status == NewTouchPanel.StatusEnum.Drive)
					{
						item.SetValueString(TestmodeTouchpanel1pID.Status1.GetName());
						ProcInitializeEnd();
					}
					else if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status == NewTouchPanel.StatusEnum.Error)
					{
						ConstParameter.ErrorID lastErrorPs = SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].GetLastErrorPs();
						ProcInitializeEnd();
						if (lastErrorPs == GetTouchPanelOpenErrorID())
						{
							item.SetValueString(TestmodeTouchpanel1pID.Error0.GetName());
						}
						else
						{
							item.SetValueString(TestmodeTouchpanel1pID.Error1.GetName());
						}
						_state = State.Error;
					}
					break;
				case State.Proc:
					if (Updating)
					{
						item.SetValueString(Utility.isBlinkDisp(Timer) ? TestmodeTouchpanel1pID.Status2.GetName() : "");
					}
					else
					{
						item.SetValueString(TestmodeTouchpanel1pID.Status1.GetName());
					}
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status == NewTouchPanel.StatusEnum.Error)
					{
						ConstParameter.ErrorID lastErrorPs2 = SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].GetLastErrorPs();
						ProcInitializeEnd();
						if (lastErrorPs2 == GetTouchPanelOpenErrorID())
						{
							item.SetValueString(TestmodeTouchpanel1pID.Error0.GetName());
						}
						else
						{
							item.SetValueString(TestmodeTouchpanel1pID.Error1.GetName());
						}
						_state = State.Error;
					}
					break;
				case State.Error:
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status == NewTouchPanel.StatusEnum.Error)
					{
						if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].GetLastErrorPs() == GetTouchPanelOpenErrorID())
						{
							item.SetValueString(TestmodeTouchpanel1pID.Error0.GetName());
						}
						else
						{
							item.SetValueString(TestmodeTouchpanel1pID.Error1.GetName());
						}
						_state = State.Error;
					}
					else
					{
						item.SetValueString(SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].Status.ToString());
					}
					break;
				}
				break;
			}
			switch (index)
			{
			case 5:
				if (Updating)
				{
					item.SetState(Item.State.UnselectableTemp);
				}
				else
				{
					item.SetState(Item.State.Selectable);
				}
				break;
			default:
				if (Updating || _state == State.Error || _state == State.Initialize)
				{
					item.SetState(Item.State.UnselectableTemp);
				}
				else
				{
					item.SetState(Item.State.Selectable);
				}
				break;
			case 0:
			case 1:
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			switch (index)
			{
			case 2:
				TouchSensParamNow++;
				if (TouchSensParamNow >= 11)
				{
					TouchSensParamNow = 10;
				}
				break;
			case 4:
				if (TouchSensParamNow != GetTouchPanelLevel())
				{
					SetTouchPanelLevel(TouchSensParamNow);
					SingletonStateMachine<AmManager, AmManager.EState>.Instance.NewTouchPanel[PlayerIndex].ReqSenssivity();
					Timer.Reset();
					Timer.Start();
					Updating = true;
					TouchSensParamPre = TouchSensParamNow;
				}
				break;
			case 3:
				TouchSensParamNow--;
				if (TouchSensParamNow < 0)
				{
					TouchSensParamNow = 0;
				}
				break;
			case 1:
				break;
			}
		}

		private string GetParamToString(int param)
		{
			param -= 5;
			if (param > 0)
			{
				return $"+{param:##0}";
			}
			return $"{param:##0}";
		}

		private void ProcInit()
		{
			_state = State.Initialize;
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.StartTouchPanel(PlayerIndex);
		}

		private void ProcInitializeEnd()
		{
			_state = State.Proc;
			Updating = false;
		}

		protected virtual int GetTouchPanelLevel()
		{
			return SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.systemSetting.touchSens1P;
		}

		protected virtual void SetTouchPanelLevel(int level)
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.systemSetting.touchSens1P = level;
		}

		protected virtual ConstParameter.ErrorID GetTouchPanelOpenErrorID()
		{
			return ConstParameter.ErrorID.TouchPanel_Left_OpenError;
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeTouchpanel1pID)Enum.Parse(typeof(TestmodeTouchpanel1pID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeTouchpanel1pID)Enum.Parse(typeof(TestmodeTouchpanel1pID), GetTitleName(index))).GetName();
		}
	}
}
