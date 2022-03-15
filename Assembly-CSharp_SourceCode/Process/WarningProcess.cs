using Monitor;
using UnityEngine;

namespace Process
{
	public class WarningProcess : ProcessBase
	{
		public enum WarningSequence
		{
			Wait,
			Warning,
			Release
		}

		private WarningSequence state;

		private GameObject leftInstance;

		private GameObject rightInstance;

		private WarningMonitor[] monitors;

		private static bool OnceDisp;

		public WarningProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			EndWarning();
		}

		public override void OnUpdate()
		{
			switch (state)
			{
			case WarningSequence.Wait:
			{
				for (int j = 0; j < monitors.Length; j++)
				{
					monitors[j].SetLogoObjectActive(active: true);
					monitors[j].PlayLogo();
					state = WarningSequence.Warning;
				}
				break;
			}
			case WarningSequence.Warning:
				if (monitors[0].IsLogoAnimationEnd())
				{
					for (int i = 0; i < monitors.Length; i++)
					{
						monitors[i].SetLogoObjectActive(active: false);
					}
					EndWarning();
				}
				break;
			case WarningSequence.Release:
				break;
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			Object.Destroy(leftInstance);
			Object.Destroy(rightInstance);
		}

		private void EndWarning()
		{
			state = WarningSequence.Release;
			container.processManager.AddProcess(new CommonProcess(container), 10);
			container.processManager.AddProcess(new AdvertiseProcess(container), 49);
			container.processManager.ReleaseProcess(this);
		}

		static WarningProcess()
		{
		}
	}
}
