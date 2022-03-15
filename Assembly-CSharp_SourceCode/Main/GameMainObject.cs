using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using AMDaemon;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.Party.Party;
using Net;
using PartyLink;
using UnityEngine;
using UnityEngine.Analytics;

namespace Main
{
	[ProjectPrefs("ScreenHeight", "", "Screen", typeof(int))]
	[ProjectPrefs("ScreenWidth", "", "Screen", typeof(int))]
	public class GameMainObject : MonoBehaviour
	{
		public const string SCREEN_WIDTH_KEY = "ScreenWidth";

		public const string SCREEN_HEGITH_KEY = "ScreenHeight";

		[SerializeField]
		private Transform leftMonitor;

		[SerializeField]
		private Transform rightMonitor;

		[SerializeField]
		private GameObject _prefabCriWareLibraryInitializer;

		private Transform _criParent;

		private GameMain main;

		private GUIStyle debugStyle;

		private double prevTime;

		private int frameCounter;

		private int minMsec = 1000;

		private int maxMsec;

		private volatile bool isQuit;

		[DllImport("PreLoad")]
		private static extern void datasec_preload();

		private void Awake()
		{
			datasec_preload();
			UnityEngine.Debug.unityLogger.logEnabled = false;
			Singleton<SystemConfig>.Instance.initialize();
			InitializeCriGameObject();
			SingletonStateMachine<AmManager, AmManager.EState>.Instance.Initialize();
			Time.fixedDeltaTime = 1f / 60f;
			QualitySettings.maxQueuedFrames = 0;
			Analytics.enabled = false;
			Analytics.deviceStatsEnabled = false;
			Camera.main.backgroundColor = Color.black;
			if (!Singleton<SystemConfig>.Instance.config.IsDummyTouchPanel)
			{
				Cursor.visible = false;
			}
			main = new GameMain();
			debugStyle = new GUIStyle();
			debugStyle.normal.textColor = Color.black;
			debugStyle.fontSize = 16;
			CommonPrefab.CreatePrefab();
			CommonScriptable.CreateScriptable();
			ButtonControllerBase.LoadDefaultResources();
			GameManager.Initialize();
			Singleton<NetDataManager>.Instance.Initialize();
			Singleton<SeManager>.Instance.Initialize();
			Packet.createAes();
			Setting.createManager(new Setting.Parameter());
			Advertise.createManager(new Advertise.Parameter());
			DeliveryChecker.createManager(new DeliveryChecker.InitParam());
			Setting.get().initialize();
			DeliveryChecker.get().initialize();
			Manager.Party.Party.Party.CreateManager(new PartyLink.Party.InitParam());
			Core.ExceptionHook = DaemonException;
		}

		private void DaemonException(DaemonException ex)
		{
		}

		private void Start()
		{
			if (main != null)
			{
				main.Initialize(this, leftMonitor, rightMonitor);
			}
		}

		private void Update()
		{
			frameCounter++;
			prevTime += GameManager.GetGameMSecAddD();
			double gameMSecAddD = GameManager.GetGameMSecAddD();
			if ((double)minMsec > gameMSecAddD || minMsec == 0)
			{
				minMsec = (int)gameMSecAddD;
			}
			if ((double)maxMsec < gameMSecAddD)
			{
				maxMsec = (int)gameMSecAddD;
			}
			if (prevTime >= 1000.0)
			{
				frameCounter = 0;
				prevTime = 0.0;
				minMsec = 0;
				maxMsec = 0;
			}
			if (main == null)
			{
				return;
			}
			if (!isQuit)
			{
				Core.Execute();
			}
			GameManager.UpdateGameTimer();
			main.Update();
			if (GameManager.IsException)
			{
				isQuit = true;
				Application.Quit();
			}
			else if (DebugInput.GetKeyDown(KeyCode.Escape) || GameManager.IsGotoSystemTest)
			{
				isQuit = true;
				Core.Kill(NextProcess.SystemTest);
				Application.Quit();
			}
			else if (GameManager.IsGotoReboot)
			{
				isQuit = true;
				if (Singleton<OperationManager>.Instance.IsAutoRebootNeeded() || Singleton<OperationManager>.Instance.IsSegaBootNeeded())
				{
					Core.Reboot();
					return;
				}
				Core.Kill(NextProcess.SegaBoot);
				Application.Quit();
			}
			else if (GameManager.IsGotoSystemError)
			{
				isQuit = true;
				Core.Kill(NextProcess.SegaBootError);
				Application.Quit();
			}
		}

		private void LateUpdate()
		{
			if (main != null)
			{
				main.LateUpdate();
			}
		}

		private void OnApplicationQuit()
		{
			isQuit = true;
			if (main != null)
			{
				main.OnApplicationQuit();
			}
			DeliveryChecker.get()?.terminate();
			Setting.get()?.terminate();
			DeliveryChecker.destroyManager();
			Advertise.destroyManager();
			Setting.destroyManager();
			Packet.destroyAes();
			Manager.Party.Party.Party.DestroyManager();
			CommonPrefab.DestroyPrefab();
			CommonScriptable.DestroyScriptable();
			if (null != _criParent)
			{
				SoundManager.StopAll();
				SoundManager.Terminate();
				Object.Destroy(_criParent);
				_criParent = null;
			}
			NetPacketUtil.ForcedUserLogout();
		}

		public static IEnumerator takeScreenShot(string finename, bool waitNextFrame)
		{
			if (waitNextFrame)
			{
				yield return null;
			}
			yield return new WaitForEndOfFrame();
			string text = Application.dataPath + "/../ScreenShot/";
			string filename = text + finename;
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			ScreenCapture.CaptureScreenshot(filename);
		}

		private void InitializeCriGameObject()
		{
			GameObject gameObject = new GameObject("CRIWARE");
			Object.DontDestroyOnLoad(gameObject);
			_criParent = gameObject.transform;
			WasapiExclusive.Intialize();
			Object.Instantiate(_prefabCriWareLibraryInitializer, _criParent);
		}
	}
}
