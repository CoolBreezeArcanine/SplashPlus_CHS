using System;
using DB;
using MAI2.Util;
using Manager;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageGameSetting : TestModePage
	{
		private enum ItemEnum
		{
			PartyGroup,
			DeliverServer,
			EventMode,
			EventTrackNum,
			Continue,
			AdvVol,
			Exit
		}

		private MachineGroupID _machineGroupID;

		private bool _isDeliveryServer;

		private bool _isEventMode;

		private bool _isContinue;

		private EventModeMusicCountID _eventModeTrack;

		private AdvertiseVolumeID _advVol;

		protected override void OnCreate()
		{
			base.OnCreate();
			Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
			_machineGroupID = backup.gameSetting.MachineGroupID;
			_isDeliveryServer = backup.gameSetting.IsStandardSettingMachine;
			_isEventMode = backup.gameSetting.IsEventMode;
			_eventModeTrack = backup.gameSetting.EventModeTrack;
			_isContinue = backup.gameSetting.IsContinue;
			_advVol = backup.gameSetting.AdvVol;
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				item.ValueText.text = _machineGroupID.GetName();
				break;
			case 1:
				if (_machineGroupID == MachineGroupID.OFF)
				{
					item.SetState(Item.State.UnselectableTemp);
				}
				else
				{
					item.SetState(Item.State.Selectable);
				}
				item.ValueText.text = (_isDeliveryServer ? TestmodeGamesettingID.Label00_00.GetName() : TestmodeGamesettingID.Label00_01.GetName());
				break;
			case 2:
				item.SetState((!_isDeliveryServer) ? Item.State.InvisibleTemp : Item.State.Selectable);
				item.SetValueOnOff(_isEventMode);
				break;
			case 3:
				item.SetState((!_isDeliveryServer) ? Item.State.InvisibleTemp : Item.State.Selectable);
				item.ValueText.text = _eventModeTrack.GetName();
				break;
			case 4:
				item.SetValueOnOff(_isContinue);
				break;
			case 5:
				item.ValueText.text = _advVol.GetName();
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			Backup backup = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup;
			switch (index)
			{
			case 0:
				if (!(++_machineGroupID).IsValid())
				{
					_machineGroupID = MachineGroupID.ON;
				}
				backup.gameSetting.MachineGroupID = _machineGroupID;
				if (_machineGroupID == MachineGroupID.OFF)
				{
					_isDeliveryServer = true;
					backup.gameSetting.IsStandardSettingMachine = _isDeliveryServer;
				}
				break;
			case 1:
				_isDeliveryServer = !_isDeliveryServer;
				backup.gameSetting.IsStandardSettingMachine = _isDeliveryServer;
				break;
			case 2:
				_isEventMode = !_isEventMode;
				backup.gameSetting.IsEventMode = _isEventMode;
				break;
			case 3:
				if (!(++_eventModeTrack).IsValid())
				{
					_eventModeTrack = EventModeMusicCountID._1;
				}
				backup.gameSetting.EventModeTrack = _eventModeTrack;
				break;
			case 4:
				_isContinue = !_isContinue;
				backup.gameSetting.IsContinue = _isContinue;
				break;
			case 5:
				if (!(++_advVol).IsValid())
				{
					_advVol = AdvertiseVolumeID._0;
				}
				backup.gameSetting.AdvVol = _advVol;
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeGamesettingID)Enum.Parse(typeof(TestmodeGamesettingID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeGamesettingID)Enum.Parse(typeof(TestmodeGamesettingID), GetTitleName(index))).GetName();
		}
	}
}
