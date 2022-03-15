using AMDaemon;
using IO;
using MAI2.Util;
using Manager;
using Monitor;
using Monitor.TestMode.SubSequence;
using UnityEngine;

namespace Process
{
	public class TestModeProcess : ProcessBase
	{
		public enum TestModeSequence
		{
			Disp,
			Release
		}

		private TestModeSequence _state;

		private TestModeMonitor[] _monitors;

		private GameObject _cloneCam;

		private bool _isCallStopBGM;

		private int _count;

		private static RenderTexture _renderTexture;

		public static RenderTexture TestModeRenderTexture => _renderTexture;

		public TestModeProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/TestMode/TestModeProcess");
			_monitors = new TestModeMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<TestModeMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<TestModeMonitor>()
			};
			GameObject original = Resources.Load<GameObject>("Process/TestMode/Materials/LeftCloneCamera");
			_cloneCam = Object.Instantiate(original);
			_renderTexture = new RenderTexture(1080, 1080, 0, RenderTextureFormat.ARGB32);
			_cloneCam.GetComponent<Camera>().targetTexture = _renderTexture;
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
			}
			container.processManager.NotificationFadeIn();
			SoundManager.StopAll();
			Singleton<OperationManager>.Instance.SetCoinBlockerMode(CoinBlocker.Mode.TestMode, flag: true);
			_isCallStopBGM = false;
			_count = 0;
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEmoneyUnconfirm = false;
		}

		public override void OnUpdate()
		{
			if (!_isCallStopBGM)
			{
				SoundManager.StopAll();
				_count++;
				if (_count >= 10)
				{
					_isCallStopBGM = true;
				}
			}
			switch (_state)
			{
			case TestModeSequence.Disp:
				if (_monitors[0].IsExit || _monitors[1].IsExit)
				{
					container.processManager.ReleaseProcess(this);
					if (_monitors[0].Result == TestModePage.Result.GoToSystemTest || _monitors[1].Result == TestModePage.Result.GoToSystemTest)
					{
						GameManager.IsGotoSystemTest = true;
					}
					else
					{
						container.processManager.AddProcess(new StartupProcess(container), 50);
						GameManager.IsGameProcMode = true;
						Sequence.EndTest();
					}
					_state = TestModeSequence.Release;
				}
				break;
			case TestModeSequence.Release:
				return;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].ViewUpdate();
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
			if (null != _cloneCam)
			{
				Object.Destroy(_cloneCam);
			}
			if (null != _renderTexture)
			{
				Object.Destroy(_renderTexture);
			}
		}
	}
}
