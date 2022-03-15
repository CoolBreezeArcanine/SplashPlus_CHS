using UnityEngine;

namespace Comio.BD15070_4
{
	public class Board15070_4 : BoardBase
	{
		public class InitParam
		{
			public byte BoardNodeId;

			public byte FirmVersion;

			public ushort FirmSum;

			public byte Timeout;

			public bool WithoutResponse;

			public ushort LedInteval;

			public InitParam()
			{
				BoardNodeId = 17;
				FirmVersion = 16;
				FirmSum = 0;
				Timeout = 10;
				WithoutResponse = true;
				LedInteval = 33;
			}
		}

		private BoardCtrl15070_4 _ctrl;

		public Board15070_4(InitParam initParam)
		{
			_construct(initParam);
		}

		public override BoardCtrlBase GetCtrlBase()
		{
			return _ctrl;
		}

		public override void Reset()
		{
			_ctrl.Reset();
		}

		public bool IsError()
		{
			return _ctrl.IsError();
		}

		public void ClearError()
		{
			_ctrl.ClearError();
		}

		public ErrorNo GetErrorNo()
		{
			return _ctrl.GetErrorNo();
		}

		public void SetLedData(byte ledPos, LedData data)
		{
			_ctrl.SetLedData(ledPos, data);
		}

		public void SetLedDataMulti(Color32 data, byte speed)
		{
			_ctrl.SetLedDataMulti(data, speed);
		}

		public void SetLedDataMultiFade(Color32 data, byte speed)
		{
			_ctrl.SetLedDataMultiFade(data, speed);
		}

		public void SetLedDataMultiFet(Color32 data)
		{
			_ctrl.SetLedDataMultiFet(data);
		}

		public void SetLedDataUpdate()
		{
			_ctrl.SetLedDataUpdate();
		}

		public void SetLedDataAllOff()
		{
			_ctrl.SetLedDataAllOff();
		}

		public static BoardNo GetDefBoardNo()
		{
			return BoardCtrl15070_4.GetDefBoardNo();
		}

		private void _construct(InitParam initParam)
		{
			_ctrl = new BoardCtrl15070_4(initParam);
		}
	}
}
