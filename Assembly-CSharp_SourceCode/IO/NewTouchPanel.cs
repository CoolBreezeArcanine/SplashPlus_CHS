using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using MAI2.Util;
using MAI2System;
using Manager;

namespace IO
{
	public class NewTouchPanel
	{
		public class TouchSensorSetting
		{
			public byte[] Sensitivity;

			public byte[] FarmSensitivity;

			public TouchSensorSetting()
			{
				Sensitivity = new byte[34];
				FarmSensitivity = new byte[34];
			}
		}

		private enum ErrorState
		{
			Non,
			Rxover,
			Break,
			Frame,
			Overrun,
			Rxparity,
			End
		}

		public enum TouchpanelLevelState
		{
			WriteInit,
			WriteIdle,
			WriteSend,
			WriteRecv,
			WriteAllOver,
			WriteError
		}

		public enum StatusEnum
		{
			None,
			BackUpWait,
			Reset,
			ResetWait,
			Init,
			InitWait,
			SetRatio,
			SetSennsitive,
			CommandEndWait,
			EndCondition,
			Drive,
			Error,
			GotoError,
			Max
		}

		private const uint WaitIssuableTime = 1000u;

		private const uint WriteWaitTime = 3000u;

		private const uint WriteIdleTime = 3000u;

		private const uint WriteRetryCount = 3u;

		private const uint SwitchDataLength = 9u;

		private const uint SwitchMaindataLength = 7u;

		private const uint SwitchSendLength = 6u;

		private const int ComSettingRecvSize = 6;

		private const byte SwitchStartData = 40;

		private const byte SwitchEndData = 41;

		private const byte SwitchDynamicDataMask = 31;

		private const uint SwitchDynamicDataBit = 4u;

		private const uint PanelTouchcensorNum = 34u;

		private const byte TouchOnOffRatio = 50;

		private uint _monitorIndex;

		private bool _testmodeSettingReq;

		public const int TouchLevelNum = 11;

		public const int TouchLevelDefault = 5;

		public string[] PortName = new string[2] { "COM3", "COM4" };

		public int BaudRate = 9600;

		private SerialPort _serialPort;

		private Thread _thread;

		private bool _isRunning;

		private List<List<byte>> _queCommand = new List<List<byte>>();

		private List<List<byte>> _queRecv = new List<List<byte>>();

		private List<byte> _buffer = new List<byte>();

		private byte[] _buff = new byte[9];

		private bool _isModeConditioning;

		private bool _issuableCommand;

		private ConstParameter.ErrorID _psError;

		private TouchSensorSetting tsSet = new TouchSensorSetting();

		private uint _countRecv;

		private uint _retryWrite;

		private bool _errorWrite;

		private Stopwatch _waitComTimer = new Stopwatch();

		private byte[] _valSensitivity = new byte[35];

		public TouchpanelLevelState StateWrite;

		private int _levelWrite = 5;

		public Mutex Mutex = new Mutex();

		private ulong _lastPanelData;

		private ulong _margedPanelData;

		private uint _dataCounter;

		private byte[] _sendDataPacket = new byte[6];

		private byte[] readTempBuff = new byte[4096];

		public StatusEnum Status;

		private readonly byte[] _switchAsciiTbl = new byte[35]
		{
			65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
			75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
			85, 86, 87, 88, 89, 90, 91, 92, 93, 94,
			95, 96, 97, 98, 0
		};

		private readonly byte[,] _defSensTblRaw = new byte[35, 11]
		{
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				90, 80, 70, 60, 50, 40, 30, 26, 23, 20,
				10
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			},
			{
				70, 60, 50, 40, 30, 20, 15, 10, 5, 1,
				1
			}
		};

		private readonly List<byte> commandRSET = new List<byte> { 123, 82, 83, 69, 84, 125 };

		private readonly List<byte> commandHALT = new List<byte> { 123, 72, 65, 76, 84, 125 };

		private readonly List<byte> commandSTAT = new List<byte> { 123, 83, 84, 65, 84, 125 };

		private readonly List<byte> commandRatio = new List<byte> { 123, 42, 42, 114, 42, 125 };

		private readonly List<byte> commandSens = new List<byte> { 123, 42, 42, 107, 42, 125 };

		private readonly List<byte> commandSensCheck = new List<byte> { 123, 42, 42, 116, 104, 125 };

		public void Initialize(uint index)
		{
			_monitorIndex = index;
			Start();
		}

		public void Execute()
		{
			if (Status == StatusEnum.Drive)
			{
				Mutex.WaitOne();
				try
				{
					if (InputManager.SetNewTouchPanel(_monitorIndex, _margedPanelData, _dataCounter))
					{
						_margedPanelData = 0uL;
					}
				}
				finally
				{
					Mutex.ReleaseMutex();
				}
			}
			else
			{
				InputManager.SetNewTouchPanel(_monitorIndex, _margedPanelData, _dataCounter);
			}
			if (Status == StatusEnum.GotoError)
			{
				Close();
				Status = StatusEnum.Error;
			}
		}

		public void Terminate()
		{
			Close();
		}

		private void Start()
		{
			Close();
			_psError = ConstParameter.ErrorID.None;
			if ((from s in SerialPort.GetPortNames()
				select (s)).All((string s) => s != PortName[_monitorIndex]))
			{
				_isRunning = false;
				Status = StatusEnum.GotoError;
				_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_OpenError : ConstParameter.ErrorID.TouchPanel_Right_OpenError);
				return;
			}
			Open();
			if (!_serialPort.IsOpen)
			{
				_isRunning = false;
				Status = StatusEnum.GotoError;
				_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_OpenError : ConstParameter.ErrorID.TouchPanel_Right_OpenError);
				return;
			}
			_isRunning = true;
			_serialPort.DiscardInBuffer();
			_serialPort.DiscardOutBuffer();
			_lastPanelData = 0uL;
			_margedPanelData = 0uL;
			Status = StatusEnum.BackUpWait;
			_thread = new Thread(UpdateThread);
			_thread.Priority = ThreadPriority.AboveNormal;
			_thread.Start();
		}

		private void UpdateThread()
		{
			while (_isRunning && _serialPort != null && _serialPort.IsOpen)
			{
				switch (Status)
				{
				case StatusEnum.BackUpWait:
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.State == Backup.EState.Ready)
					{
						Status = StatusEnum.Reset;
					}
					break;
				case StatusEnum.Reset:
					_queCommand.Clear();
					SendResetCommand();
					Status = StatusEnum.ResetWait;
					_waitComTimer.Reset();
					_waitComTimer.Start();
					break;
				case StatusEnum.ResetWait:
					if (1000 <= _waitComTimer.ElapsedMilliseconds)
					{
						Status = StatusEnum.Init;
					}
					break;
				case StatusEnum.Init:
				{
					int[] array = new int[2]
					{
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.systemSetting.touchSens1P,
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.systemSetting.touchSens2P
					};
					for (uint num2 = 0u; num2 < 34; num2++)
					{
						tsSet.Sensitivity[num2] = GetDefSensitiveLevel((byte)num2, array[_monitorIndex]);
					}
					StartConditioningMode();
					Status = StatusEnum.InitWait;
					_waitComTimer.Reset();
					_waitComTimer.Start();
					break;
				}
				case StatusEnum.InitWait:
					if (1000 <= _waitComTimer.ElapsedMilliseconds)
					{
						Status = StatusEnum.SetRatio;
						_queRecv.Clear();
					}
					break;
				case StatusEnum.SetRatio:
					if (IssuableCommand())
					{
						Status = StatusEnum.SetSennsitive;
						SetTouchPanelRatio();
					}
					break;
				case StatusEnum.SetSennsitive:
					if (WriteTouchPanelRatio())
					{
						List<byte> list = new List<byte>();
						for (uint num = 0u; num < 34; num++)
						{
							byte item = tsSet.Sensitivity[num];
							list.Add(item);
						}
						SetTouchPanelSensitivity(list);
						Status = StatusEnum.CommandEndWait;
					}
					break;
				case StatusEnum.CommandEndWait:
					if (WriteTouchPanelSensitivity())
					{
						Status = StatusEnum.EndCondition;
					}
					break;
				case StatusEnum.EndCondition:
					EndConditioningMode();
					Status = StatusEnum.Drive;
					break;
				}
				try
				{
					Send();
					Recv();
				}
				catch (Exception)
				{
				}
			}
		}

		private void Open()
		{
			try
			{
				_serialPort = new SerialPort(PortName[_monitorIndex], BaudRate, Parity.None, 8, StopBits.One);
				_serialPort.ReadTimeout = 1;
				_serialPort.WriteTimeout = 1000;
				_serialPort.Open();
			}
			catch
			{
			}
		}

		private void Close()
		{
			_isRunning = false;
			if (_thread != null && _thread.IsAlive)
			{
				_thread.Join();
			}
			if (_serialPort != null)
			{
				if (_serialPort.IsOpen)
				{
					_serialPort.Close();
					_serialPort.Dispose();
					Thread.Sleep(1000);
				}
				_serialPort = null;
			}
		}

		private bool IssuableCommand()
		{
			return _issuableCommand;
		}

		private bool NoCommand()
		{
			return _queCommand.Count == 0;
		}

		private void Send()
		{
			int num = 0;
			int num2 = ((_queCommand.Count > 0) ? 1 : 0);
			for (uint num3 = 0u; num3 < num2; num3++)
			{
				_sendDataPacket = _queCommand[0].ToArray();
				if ((long)(num + _sendDataPacket.Length) > 6L)
				{
					break;
				}
				num += _sendDataPacket.Length;
				_queCommand.RemoveAt(0);
			}
			if (num <= 0)
			{
				return;
			}
			try
			{
				_serialPort.Write(_sendDataPacket, 0, num);
			}
			catch (Exception ex)
			{
				if (!(ex is TimeoutException))
				{
					Status = StatusEnum.GotoError;
					_isRunning = false;
					_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_OpenError : ConstParameter.ErrorID.TouchPanel_Right_OpenError);
				}
			}
		}

		private void Recv()
		{
			try
			{
				int num = _serialPort.Read(readTempBuff, 0, readTempBuff.Length);
				if (num != 0)
				{
					byte[] array = new byte[num];
					Array.Copy(readTempBuff, array, num);
					_buffer.AddRange(array);
					if (!_isModeConditioning)
					{
						_issuableCommand = false;
					}
				}
				if (_issuableCommand)
				{
					int num2 = _buffer.IndexOf(40);
					if (num2 < 0)
					{
						return;
					}
					_buffer.RemoveRange(0, num2);
					int num3 = _buffer.Count / 6;
					for (int i = 0; i < num3; i++)
					{
						List<byte> list = new List<byte>();
						for (int j = 0; j < 6; j++)
						{
							list.Add(_buffer[j]);
						}
						_queRecv.Add(list);
						_buffer.RemoveRange(0, 6);
					}
					return;
				}
				_countRecv = 0u;
				int num4 = _buffer.IndexOf(40);
				if (num4 < 0)
				{
					return;
				}
				_buffer.RemoveRange(0, num4);
				int num5 = _buffer.Count / 9;
				if (num5 <= 0)
				{
					return;
				}
				Mutex.WaitOne();
				try
				{
					ClearData();
					for (uint num6 = 0u; num6 < num5; num6++)
					{
						for (int k = 0; (long)k < 9L; k++)
						{
							_buff[k] |= _buffer[k];
						}
						_buffer.RemoveRange(0, 9);
					}
					Parse();
				}
				finally
				{
					Mutex.ReleaseMutex();
				}
			}
			catch (Exception ex)
			{
				if (ex is TimeoutException)
				{
					if (_isModeConditioning)
					{
						if (!_issuableCommand)
						{
							_serialPort.DiscardInBuffer();
							_serialPort.DiscardOutBuffer();
							_buffer.Clear();
							_queRecv.Clear();
						}
						_issuableCommand = true;
					}
				}
				else
				{
					Status = StatusEnum.GotoError;
					_isRunning = false;
					_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_OpenError : ConstParameter.ErrorID.TouchPanel_Right_OpenError);
				}
			}
		}

		private void Parse()
		{
			if (_buff[0] == 40 && _buff[8] == 41)
			{
				GetSwitchData();
			}
		}

		private void GetSwitchData()
		{
			byte[] array = new byte[7];
			Buffer.BlockCopy(_buff, 1, array, 0, 7);
			uint num = 0u;
			for (int i = 0; (long)i < 7L; i++)
			{
				byte b = array[i];
				b = (byte)(b & 0x1Fu);
				byte b2 = 0;
				while ((uint)b2 <= 4u)
				{
					if (((uint)(b >> (int)b2) & (true ? 1u : 0u)) != 0)
					{
						_lastPanelData |= (ulong)(1L << (int)num);
					}
					b2 = (byte)(b2 + 1);
					num++;
				}
			}
			if (num >= 64)
			{
				_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_InitError : ConstParameter.ErrorID.TouchPanel_Right_InitError);
				Status = StatusEnum.GotoError;
				_isRunning = false;
			}
			_margedPanelData |= _lastPanelData;
			_dataCounter++;
		}

		private bool isBlank(InputManager.TouchPanelArea data)
		{
			if (data == InputManager.TouchPanelArea.Blank)
			{
				return true;
			}
			return false;
		}

		private void ClearData()
		{
			_lastPanelData = 0uL;
			byte b = 0;
			while ((uint)b < 9u)
			{
				_buff[b] = 0;
				b = (byte)(b + 1);
			}
		}

		private void InitData()
		{
			_lastPanelData = 0uL;
			int num = _buffer.IndexOf(40);
			if (num > 0)
			{
				_buffer.RemoveRange(0, num);
			}
			byte b = 0;
			while ((uint)b < 9u)
			{
				_buff[b] = 0;
				b = (byte)(b + 1);
			}
		}

		private void SetTouchPanelLevel(int level)
		{
			_levelWrite = level;
			StateWrite = TouchpanelLevelState.WriteInit;
			_retryWrite = 0u;
			_errorWrite = false;
		}

		private void GetTouchPanelSensitivity()
		{
			_levelWrite = 12;
			StateWrite = TouchpanelLevelState.WriteInit;
			_retryWrite = 0u;
			_errorWrite = false;
		}

		private void SetTouchPanelRatio()
		{
			_levelWrite = 12;
			StateWrite = TouchpanelLevelState.WriteInit;
			_retryWrite = 0u;
			_errorWrite = false;
		}

		private void SetTouchPanelSensitivity(List<byte> sensitivity)
		{
			if ((long)sensitivity.Count == 34)
			{
				_levelWrite = 12;
				for (int i = 0; i < sensitivity.Count; i++)
				{
					_valSensitivity[i] = sensitivity[i];
				}
				StateWrite = TouchpanelLevelState.WriteInit;
				_retryWrite = 0u;
				_errorWrite = false;
			}
		}

		private bool WriteTouchPanelSensitivity()
		{
			switch (StateWrite)
			{
			case TouchpanelLevelState.WriteInit:
				_countRecv = 0u;
				_retryWrite = 0u;
				StateWrite = TouchpanelLevelState.WriteSend;
				break;
			case TouchpanelLevelState.WriteIdle:
				if (3000 <= _waitComTimer.ElapsedMilliseconds)
				{
					StateWrite = TouchpanelLevelState.WriteRecv;
				}
				if (_retryWrite >= 3)
				{
					_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_InitError : ConstParameter.ErrorID.TouchPanel_Right_InitError);
					_errorWrite = true;
					StateWrite = TouchpanelLevelState.WriteError;
					Status = StatusEnum.GotoError;
					_isRunning = false;
				}
				break;
			case TouchpanelLevelState.WriteSend:
			{
				_waitComTimer.Reset();
				_waitComTimer.Start();
				uint data2 = _countRecv % 34u;
				byte valAtLevel2 = GetValAtLevel((InputManager.TouchPanelArea)data2, _levelWrite);
				SetSenserSensitivity((InputManager.TouchPanelArea)data2, valAtLevel2);
				StateWrite = TouchpanelLevelState.WriteRecv;
				break;
			}
			case TouchpanelLevelState.WriteRecv:
				if (_queRecv.Count != 0)
				{
					int num = 0;
					List<byte> list = _queRecv[0];
					num++;
					num++;
					byte b = list[num++];
					if (b == 64)
					{
						_retryWrite++;
						_waitComTimer.Reset();
						_waitComTimer.Start();
						StateWrite = TouchpanelLevelState.WriteIdle;
					}
					else
					{
						uint data = _countRecv % 34u;
						byte valAtLevel = GetValAtLevel((InputManager.TouchPanelArea)data, _levelWrite);
						num++;
						if (list[num] != valAtLevel)
						{
							_retryWrite++;
							_waitComTimer.Reset();
							_waitComTimer.Start();
							StateWrite = TouchpanelLevelState.WriteIdle;
						}
						else
						{
							byte switchDataCharIndex = GetSwitchDataCharIndex(b);
							tsSet.FarmSensitivity[switchDataCharIndex] = list[num];
							_countRecv = (uint)(b - 65 + 1);
						}
					}
					_queRecv.RemoveAt(0);
					if (StateWrite != TouchpanelLevelState.WriteIdle)
					{
						if (34 <= _countRecv)
						{
							StateWrite = TouchpanelLevelState.WriteAllOver;
						}
						else
						{
							StateWrite = TouchpanelLevelState.WriteSend;
						}
					}
				}
				else if (3000 <= _waitComTimer.ElapsedMilliseconds)
				{
					_retryWrite++;
					_waitComTimer.Reset();
					_waitComTimer.Start();
					StateWrite = TouchpanelLevelState.WriteIdle;
				}
				break;
			case TouchpanelLevelState.WriteAllOver:
				return true;
			case TouchpanelLevelState.WriteError:
				return false;
			}
			return false;
		}

		private bool WriteTouchPanelRatio()
		{
			switch (StateWrite)
			{
			case TouchpanelLevelState.WriteInit:
				_countRecv = 0u;
				_retryWrite = 0u;
				StateWrite = TouchpanelLevelState.WriteSend;
				break;
			case TouchpanelLevelState.WriteIdle:
				if (3000 <= _waitComTimer.ElapsedMilliseconds)
				{
					StateWrite = TouchpanelLevelState.WriteRecv;
				}
				if (_retryWrite >= 3)
				{
					_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_OpenError : ConstParameter.ErrorID.TouchPanel_Right_OpenError);
					_errorWrite = true;
					StateWrite = TouchpanelLevelState.WriteError;
					Status = StatusEnum.GotoError;
					_isRunning = false;
				}
				break;
			case TouchpanelLevelState.WriteSend:
			{
				_waitComTimer.Reset();
				_waitComTimer.Start();
				uint data = _countRecv % 34u;
				SetSenserRatio((InputManager.TouchPanelArea)data, 50);
				StateWrite = TouchpanelLevelState.WriteRecv;
				break;
			}
			case TouchpanelLevelState.WriteRecv:
				if (_queRecv.Count != 0)
				{
					int num = 0;
					List<byte> list = _queRecv[0];
					num++;
					num++;
					byte b = list[num++];
					if (b == 64)
					{
						_retryWrite++;
						_waitComTimer.Reset();
						_waitComTimer.Start();
						StateWrite = TouchpanelLevelState.WriteIdle;
					}
					else
					{
						num++;
						if (list[num] != 50)
						{
							_retryWrite++;
							_waitComTimer.Reset();
							_waitComTimer.Start();
							StateWrite = TouchpanelLevelState.WriteIdle;
						}
						else
						{
							_countRecv = (uint)(b - 65 + 1);
						}
					}
					_queRecv.RemoveAt(0);
					if (StateWrite != TouchpanelLevelState.WriteIdle)
					{
						if (34 <= _countRecv)
						{
							StateWrite = TouchpanelLevelState.WriteAllOver;
						}
						else
						{
							StateWrite = TouchpanelLevelState.WriteSend;
						}
					}
				}
				else if (3000 <= _waitComTimer.ElapsedMilliseconds)
				{
					_retryWrite++;
					_waitComTimer.Reset();
					_waitComTimer.Start();
					StateWrite = TouchpanelLevelState.WriteIdle;
				}
				break;
			case TouchpanelLevelState.WriteAllOver:
				return true;
			case TouchpanelLevelState.WriteError:
				return false;
			}
			return false;
		}

		private bool IsWriteError()
		{
			return _errorWrite;
		}

		public ConstParameter.ErrorID GetLastErrorPs()
		{
			return _psError;
		}

		private void SendResetCommand()
		{
			_queCommand.Add(commandRSET);
		}

		private void StartConditioningMode()
		{
			_queCommand.Add(commandHALT);
			_isModeConditioning = true;
		}

		private void EndConditioningMode()
		{
			_queCommand.Add(commandSTAT);
			_isModeConditioning = false;
		}

		private byte GetSideChar()
		{
			if (_monitorIndex == 0)
			{
				return 76;
			}
			return 82;
		}

		private byte GetSwitchDataChar(InputManager.TouchPanelArea data)
		{
			byte result = 0;
			if (isBlank(data))
			{
				return result;
			}
			return _switchAsciiTbl[(int)data];
		}

		private byte GetSwitchDataCharIndex(byte c)
		{
			for (byte b = 0; b < 35; b = (byte)(b + 1))
			{
				if (_switchAsciiTbl[b] == c)
				{
					return b;
				}
			}
			_psError = ((_monitorIndex == 0) ? ConstParameter.ErrorID.TouchPanel_Left_InitError : ConstParameter.ErrorID.TouchPanel_Right_InitError);
			Status = StatusEnum.GotoError;
			_isRunning = false;
			return 34;
		}

		private bool IsConditioningMode()
		{
			return _isModeConditioning;
		}

		private void SetSenserRatio(InputManager.TouchPanelArea data, byte val)
		{
			if (!isBlank(data))
			{
				commandRatio[1] = GetSideChar();
				commandRatio[2] = GetSwitchDataChar(data);
				commandRatio[4] = val;
				_queCommand.Add(commandRatio);
			}
		}

		private void SetSenserSensitivity(InputManager.TouchPanelArea data, byte val)
		{
			if (!isBlank(data))
			{
				commandSens[1] = GetSideChar();
				commandSens[2] = GetSwitchDataChar(data);
				commandSens[4] = val;
				SetOffsetSenssitiveVal(data, val);
				_queCommand.Add(commandSens);
			}
		}

		private void CheckSenserSensitivity(InputManager.TouchPanelArea data)
		{
			if (!isBlank(data))
			{
				commandSensCheck[1] = GetSideChar();
				commandSensCheck[2] = GetSwitchDataChar(data);
				_queCommand.Add(commandSensCheck);
			}
		}

		private void ResetSenserSensitivit(InputManager.TouchPanelArea data)
		{
			if (!isBlank(data))
			{
				GetSideChar();
				GetSwitchDataChar(data);
				byte defaultVal = GetDefaultVal(data);
				SetOffsetSenssitiveVal(data, defaultVal);
			}
		}

		private byte GetDefaultVal(InputManager.TouchPanelArea data)
		{
			return GetValAtLevel(data, 5);
		}

		private byte GetValAtLevel(InputManager.TouchPanelArea data, int level)
		{
			if (data < InputManager.TouchPanelArea.A1)
			{
				return 0;
			}
			byte b = ((level >= 12) ? _valSensitivity[(int)data] : getValAtLevel_Index((byte)data, level));
			if (b < 1)
			{
				b = 1;
			}
			return b;
		}

		public byte getValAtLevel_Index(byte index, int level)
		{
			return GetDefSensitiveLevel(index, level);
		}

		private byte GetDefSensitiveLevel(byte index, int level)
		{
			if ((uint)index >= 34u)
			{
				return 0;
			}
			if (level >= 12)
			{
				return 0;
			}
			return _defSensTblRaw[index, level];
		}

		private void SetOffsetSenssitiveVal(InputManager.TouchPanelArea data, byte val)
		{
			if (!isBlank(data) && val <= byte.MaxValue && val >= 1)
			{
				_valSensitivity[(int)data] = val;
			}
		}

		public void ReqSenssivity()
		{
			_testmodeSettingReq = true;
			Status = StatusEnum.Init;
		}

		public bool IsSenssivity()
		{
			if (Status == StatusEnum.Drive)
			{
				_testmodeSettingReq = false;
			}
			return _testmodeSettingReq;
		}

		public uint GetRecvCount()
		{
			return _countRecv;
		}

		public byte GetPanelSensitive(InputManager.TouchPanelArea panelArea)
		{
			return tsSet.FarmSensitivity[(int)panelArea];
		}
	}
}
