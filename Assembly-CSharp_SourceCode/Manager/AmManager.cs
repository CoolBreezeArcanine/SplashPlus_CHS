using AMDaemon;
using IO;
using MAI2.Util;
using MAI2System;
using UnityEngine;

namespace Manager
{
	public class AmManager : SingletonStateMachine<AmManager, AmManager.EState>
	{
		public enum EState
		{
			WaitAmDaemonReady,
			Run
		}

		public bool IsEnableEmoneyExecute;

		public bool IsCheckDisableEmoneyBrandID = true;

		public bool IsEmoneyUnconfirm;

		public bool IsReady { get; private set; }

		public VersionNo VersionNo { get; private set; }

		public Backup Backup { get; private set; }

		public Credit Credit { get; private set; }

		public AimeReaderManager AimeReader { get; private set; }

		public EMoney Emoney { get; private set; }

		public Accounting Accounting { get; private set; }

		public Network Network { get; private set; }

		public Delivery Delivery { get; private set; }

		public NewTouchPanel[] NewTouchPanel { get; private set; }

		public void Initialize()
		{
		}

		public void Execute()
		{
			updateState(-1f);
		}

		public void Terminate()
		{
			Backup?.terminate();
			Credit?.Terminate();
			Accounting?.terminate();
			Network?.terminate();
			Delivery?.terminate();
			if (NewTouchPanel != null)
			{
				NewTouchPanel[] newTouchPanel = NewTouchPanel;
				for (int i = 0; i < newTouchPanel.Length; i++)
				{
					newTouchPanel[i]?.Terminate();
				}
			}
		}

		public void StartTouchPanel()
		{
			for (int i = 0; i < NewTouchPanel.Length; i++)
			{
				StartTouchPanel(i);
			}
		}

		public void StartTouchPanel(int index)
		{
			NewTouchPanel[] newTouchPanel = NewTouchPanel;
			if (((newTouchPanel != null) ? newTouchPanel[index] : null) != null)
			{
				NewTouchPanel[index].Initialize((uint)index);
			}
		}

		private void Execute_WaitAmDaemonReady()
		{
			if (!Core.IsReady)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (Sequence.IsPlaying(i))
				{
					Sequence.EndPlay(i);
				}
			}
			if (Sequence.IsTest)
			{
				Sequence.EndTest();
			}
			AMDaemon.Version currentVersion = AppImage.CurrentVersion;
			VersionNo = new VersionNo(currentVersion.Major, currentVersion.Minor, currentVersion.Patch);
			Backup = new Backup();
			Backup.initialize();
			Credit = new Credit();
			Credit.Initialize();
			Emoney = new EMoney();
			Emoney.Initialize();
			Accounting = new Accounting();
			Accounting.initialize();
			AimeReader = new AimeReaderManager();
			AimeReader.Initialize();
			Network = new Network();
			Network.initialize();
			Delivery = new Delivery();
			Delivery.initialize();
			if (NewTouchPanel != null)
			{
				NewTouchPanel[] newTouchPanel = NewTouchPanel;
				for (int j = 0; j < newTouchPanel.Length; j++)
				{
					newTouchPanel[j]?.Terminate();
				}
			}
			NewTouchPanel = new NewTouchPanel[2];
			for (int k = 0; k < NewTouchPanel.Length; k++)
			{
				NewTouchPanel[k] = new NewTouchPanel();
				NewTouchPanel[k].Initialize((uint)k);
			}
			IsReady = true;
			setNextState(EState.Run);
		}

		private void Execute_Run()
		{
			Backup.execute();
			Credit.Execute();
			Emoney.Update();
			Accounting.execute();
			AimeReader.Execute();
			Network.execute();
			Delivery.execute();
			NewTouchPanel[] newTouchPanel = NewTouchPanel;
			for (int i = 0; i < newTouchPanel.Length; i++)
			{
				newTouchPanel[i].Execute();
			}
			if (Singleton<SystemConfig>.Instance.config.IsTarget && (float)Display.main.renderingWidth < 2160f)
			{
				Error.Set(912);
				GameManager.IsGotoSystemError = true;
			}
		}
	}
}
