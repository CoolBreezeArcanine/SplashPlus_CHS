using System.Diagnostics;
using UnityEngine;

namespace Comio.BD15070_4
{
	public class IoCtrl
	{
		private BoardCtrl15070_4 _boardCtrl;

		private LedData _ledData;

		public SetLedGs8BitCommand[] SetLedGs8BitCommand;

		public SetLedGs8BitMultiCommand SetLedGs8BitMultiCommand;

		public SetLedGs8BitMultiFadeCommand SetLedGs8BitMultiFadeCommand;

		public SetLedFetCommand SetLedFetCommand;

		public SetLedGsUpdateCommand SetLedGsUpdateCommand;

		private Gs8BitMulti[] _gs8Bit;

		private Gs8BitMulti _gs8BitMulti;

		private Gs8BitMulti _gs8BitMultiFade;

		private Gs8BitMulti _dc8Bit;

		private Stopwatch _modeTimer;

		public IoCtrl(BoardCtrl15070_4 boardCtrl)
		{
			_construct(boardCtrl);
		}

		public void Initialize()
		{
			_initialize();
		}

		public void Execute()
		{
			_sendLed();
		}

		public void SetLedData(byte ledPos, LedData data)
		{
			if (ledPos == 8 || ledPos == 9 || ledPos == 10)
			{
				switch (ledPos)
				{
				case 8:
					_dc8Bit.Color.r = data.GetColor(ledPos).r;
					break;
				case 9:
					_dc8Bit.Color.g = data.GetColor(ledPos).g;
					break;
				case 10:
					_dc8Bit.Color.b = data.GetColor(ledPos).b;
					break;
				}
				SetLedFetCommand.setColor(_dc8Bit.Color);
				_boardCtrl.SendForceCommand(SetLedFetCommand);
			}
			else
			{
				SetLedGs8BitCommand[ledPos].setColor(ledPos, data.GetColor(ledPos));
				_boardCtrl.SendForceCommand(SetLedGs8BitCommand[ledPos]);
			}
		}

		public void SetLedDataMulti(Color32 data, byte speed)
		{
			_gs8BitMulti.Color = data;
			_gs8BitMulti.Start = 0;
			_gs8BitMulti.End = 8;
			_gs8BitMulti.Skip = 0;
			_gs8BitMulti.Speed = speed;
			SetLedGs8BitMultiCommand.setColor(_gs8BitMulti);
			_boardCtrl.SendForceCommand(SetLedGs8BitMultiCommand);
		}

		public void SetLedDataMultiFet(Color32 data)
		{
			_dc8Bit.Color = data;
			SetLedFetCommand.setColor(_dc8Bit.Color);
			_boardCtrl.SendForceCommand(SetLedFetCommand);
		}

		public void SetLedDataMultiFade(Color32 data, byte speed)
		{
			_gs8BitMultiFade.Color = data;
			_gs8BitMultiFade.Start = 0;
			_gs8BitMultiFade.End = 8;
			_gs8BitMultiFade.Skip = 0;
			_gs8BitMultiFade.Speed = speed;
			SetLedGs8BitMultiFadeCommand.setColor(_gs8BitMultiFade);
			_boardCtrl.SendForceCommand(SetLedGs8BitMultiFadeCommand);
		}

		public void SetLedDataAllOff()
		{
			_gs8BitMulti.Color = Color.black;
			_gs8BitMulti.Start = 0;
			_gs8BitMulti.End = 32;
			_gs8BitMulti.Skip = 0;
			_gs8BitMulti.Speed = 0;
			SetLedGs8BitMultiCommand.setColor(_gs8BitMulti);
			_boardCtrl.SendForceCommand(SetLedGs8BitMultiCommand);
			_dc8Bit.Color = Color.black;
			SetLedFetCommand.setColor(_dc8Bit.Color);
			_boardCtrl.SendForceCommand(SetLedFetCommand);
		}

		public void SetUpdateGs()
		{
			_boardCtrl.SendForceCommand(SetLedGsUpdateCommand);
		}

		private void _construct(BoardCtrl15070_4 boardCtrl)
		{
			_boardCtrl = boardCtrl;
			_ledData = new LedData();
			_modeTimer = new Stopwatch();
			_gs8Bit = new Gs8BitMulti[11];
			for (int i = 0; i < _gs8Bit.Length; i++)
			{
				_gs8Bit[i] = default(Gs8BitMulti);
			}
			_gs8BitMulti = default(Gs8BitMulti);
			_gs8BitMultiFade = default(Gs8BitMulti);
			_dc8Bit = default(Gs8BitMulti);
			SetLedGs8BitCommand = new SetLedGs8BitCommand[11];
			for (int j = 0; j < 11; j++)
			{
				SetLedGs8BitCommand[j] = new SetLedGs8BitCommand();
				_boardCtrl.InitCommand(SetLedGs8BitCommand[j], SetLedGs8BitCommand[j].GetCommandNo(), SetLedGs8BitCommand[j].GetLength());
			}
			SetLedGs8BitMultiCommand = new SetLedGs8BitMultiCommand();
			_boardCtrl.InitCommand(SetLedGs8BitMultiCommand, SetLedGs8BitMultiCommand.GetCommandNo(), SetLedGs8BitMultiCommand.GetLength());
			SetLedGs8BitMultiFadeCommand = new SetLedGs8BitMultiFadeCommand();
			_boardCtrl.InitCommand(SetLedGs8BitMultiFadeCommand, SetLedGs8BitMultiFadeCommand.GetCommandNo(), SetLedGs8BitMultiFadeCommand.GetLength());
			SetLedFetCommand = new SetLedFetCommand();
			_boardCtrl.InitCommand(SetLedFetCommand, SetLedFetCommand.GetCommandNo(), SetLedFetCommand.GetLength());
			SetLedGsUpdateCommand = new SetLedGsUpdateCommand();
			_boardCtrl.InitCommand(SetLedGsUpdateCommand, SetLedGsUpdateCommand.GetCommandNo(), SetLedGsUpdateCommand.GetLength());
		}

		private void _initialize()
		{
			_modeTimer.Start();
			_ledData.SetOff();
		}

		private void _sendLed()
		{
			_ = _modeTimer.ElapsedMilliseconds;
			_boardCtrl.GetLedInterval();
		}
	}
}
