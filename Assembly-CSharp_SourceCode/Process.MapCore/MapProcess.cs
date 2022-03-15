using System;
using System.Collections.Generic;
using Manager;
using Monitor.MapCore;
using UnityEngine;

namespace Process.MapCore
{
	public class MapProcess : ProcessBase
	{
		protected readonly List<MapMonitor> Monitors = new List<MapMonitor>();

		protected Action State;

		public ProcessDataContainer Container => container;

		public ProcessManager ProcessManager => Container.processManager;

		public MapProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public MapProcess(ProcessDataContainer dataContainer, ProcessType type)
			: base(dataContainer, type)
		{
		}

		public override void OnStart()
		{
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			State?.Invoke();
			Monitors.ForEach(delegate(MapMonitor m)
			{
				m.OnUpdate((float)GameManager.GetGameMSecAdd() / 1000f);
			});
		}

		public override void OnLateUpdate()
		{
			Monitors.ForEach(delegate(MapMonitor m)
			{
				m.OnLateUpdate((float)GameManager.GetGameMSecAdd() / 1000f);
			});
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			Monitors.ForEach(delegate(MapMonitor m)
			{
				UnityEngine.Object.Destroy(m.gameObject);
			});
			Resources.UnloadUnusedAssets();
		}

		protected void SetStateTerminate()
		{
			State = null;
		}
	}
}
