using AMDaemon;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.Party.Party;
using PartyLink;
using UnityEngine;

namespace zPlayTest.Scripts
{
	public class TestCore : MonoBehaviour
	{
		public static bool IsReady { get; private set; }

		private void Start()
		{
			Singleton<SystemConfig>.Instance.initialize();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Initialize();
			Packet.createAes();
			PartyLink.Party.createManager(new PartyLink.Party.InitParam());
			Setting.createManager(new Setting.Parameter());
			Advertise.createManager(new Advertise.Parameter());
			DeliveryChecker.createManager(new DeliveryChecker.InitParam());
			Setting.get().initialize();
			DeliveryChecker.get().initialize();
			Manager.Party.Party.Party.CreateManager(new PartyLink.Party.InitParam());
		}

		private void Update()
		{
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Execute();
			if (!IsReady)
			{
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsReady)
				{
					MachineGroupID machineGroupID = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID;
					Advertise.get().initialize(machineGroupID);
					Manager.Party.Party.Party.Get().Start(machineGroupID);
					IsReady = true;
				}
			}
			else
			{
				PartyLink.Party.get().execute();
				Manager.Party.Party.Party.Get().Execute();
			}
		}

		private void FixedUpdate()
		{
			Core.Execute();
		}

		private void OnApplicationQuit()
		{
			DeliveryChecker.get().terminate();
			Setting.get().terminate();
			DeliveryChecker.destroyManager();
			Advertise.destroyManager();
			Setting.destroyManager();
			PartyLink.Party.destroyManager();
			Packet.destroyAes();
			Manager.Party.Party.Party.DestroyManager();
		}
	}
}
