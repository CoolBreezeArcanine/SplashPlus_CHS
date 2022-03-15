using System;
using System.Collections;
using System.Runtime.InteropServices;
using AMDaemon;
using Datas.DebugData;
using Mai2.Mai2Cue;
using MAI2.Util;
using MAI2System;
using Main;
using Manager;
using Manager.Achieve;
using Manager.Party.Party;
using Net;
using Net.Packet.Helper;
using Net.Packet.Mai2;
using Net.VO.Mai2;
using PartyLink;
using Process;
using UnityEngine;
using UnityEngine.Analytics;

public class DebugGameMainObject : MonoBehaviour
{
	[SerializeField]
	private Transform leftMonitor;

	[SerializeField]
	private Transform rightMonitor;

	[SerializeField]
	[Header("フリーダムモードで起動するか")]
	private bool _isFreedomMode;

	[SerializeField]
	[Header("クラシックモードで起動するか")]
	private bool _isClassicMode;

	[SerializeField]
	[Header("デバッグユーザーデータ")]
	private DebugUserData _player01;

	[SerializeField]
	private DebugUserData _player02;

	[SerializeField]
	private DebugUserData _player03;

	[SerializeField]
	private DebugUserData _player04;

	[SerializeField]
	[Header("ゲームリザルトデータ")]
	private DebugGameScoreList _player01GameScore;

	[SerializeField]
	private DebugGameScoreList _player02GameScore;

	[SerializeField]
	private DebugGameScoreList _player03GameScore;

	[SerializeField]
	private DebugGameScoreList _player04GameScore;

	[SerializeField]
	[Header("AimeID")]
	private bool _isUsePlayer01AimeID;

	[SerializeField]
	private uint _player01AimeID;

	[SerializeField]
	[Header("AimeID")]
	private bool _isUsePlayer02AimeID;

	[SerializeField]
	private uint _player02AimeID;

	private bool isQuit;

	[SerializeField]
	private GameObject _prefabCriWareLibraryInitializer;

	private Transform _criParent;

	private GameMain main;

	private float _timeDiff;

	public static bool IsAimeUse;

	[DllImport("PreLoad")]
	private static extern void datasec_preload();

	private void Awake()
	{
		datasec_preload();
		Singleton<SystemConfig>.Instance.initialize();
		InitializeCriGameObject();
		SingletonStateMachine<AmManager, AmManager.EState>.Instance.Initialize();
		QualitySettings.maxQueuedFrames = 0;
		Time.fixedDeltaTime = 1f / 60f;
		Analytics.enabled = false;
		Analytics.deviceStatsEnabled = false;
		Camera.main.backgroundColor = Color.black;
		main = new GameMain();
		CommonPrefab.CreatePrefab();
		CommonScriptable.CreateScriptable();
		ButtonControllerBase.LoadDefaultResources();
		GameManager.Initialize();
		Singleton<NetDataManager>.Instance.Initialize();
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
		main?.Initialize(this, leftMonitor, rightMonitor);
	}

	private void Update()
	{
		if (main != null)
		{
			if (!isQuit)
			{
				Core.Execute();
			}
			GameManager.UpdateGameTimer();
			main.Update();
		}
	}

	private void LateUpdate()
	{
		main?.LateUpdate();
	}

	private void OnApplicationQuit()
	{
		isQuit = true;
		main?.OnApplicationQuit();
		DeliveryChecker.get().terminate();
		Setting.get().terminate();
		DeliveryChecker.destroyManager();
		Advertise.destroyManager();
		Setting.destroyManager();
		Packet.destroyAes();
		Manager.Party.Party.Party.DestroyManager();
		CommonPrefab.DestroyPrefab();
		CommonScriptable.DestroyScriptable();
		if (null != _criParent)
		{
			UnityEngine.Object.Destroy(_criParent);
			_criParent = null;
		}
	}

	public void DebugAddProcess(ProcessBase prevProcess)
	{
		StartCoroutine(ProcessCoroutine(prevProcess));
	}

	private IEnumerator ProcessCoroutine(ProcessBase prevProcess)
	{
		UserLoginResponseVO vo = new UserLoginResponseVO
		{
			loginCount = 1,
			consecutiveLoginCount = 1
		};
		if (_player01 != null)
		{
			Singleton<UserDataManager>.Instance.SetDebugUserData(0L, _player01);
			Singleton<NetDataManager>.Instance.SetLoginVO(0, vo);
		}
		if (_player02 != null)
		{
			Singleton<UserDataManager>.Instance.SetDebugUserData(1L, _player02);
			Singleton<NetDataManager>.Instance.SetLoginVO(0, vo);
		}
		if (_player03 != null)
		{
			Singleton<UserDataManager>.Instance.SetDebugUserData(2L, _player03);
		}
		if (_player04 != null)
		{
			Singleton<UserDataManager>.Instance.SetDebugUserData(3L, _player04);
		}
		if (_player01GameScore != null)
		{
			Singleton<GamePlayManager>.Instance.DebugSetGameScore(0, _player01GameScore);
		}
		if (_player02GameScore != null)
		{
			Singleton<GamePlayManager>.Instance.DebugSetGameScore(1, _player02GameScore);
		}
		if (_player03GameScore != null)
		{
			Singleton<GamePlayManager>.Instance.DebugSetGameScore(2, _player03GameScore);
		}
		if (_player04GameScore != null)
		{
			Singleton<GamePlayManager>.Instance.DebugSetGameScore(3, _player04GameScore);
		}
		if (_isUsePlayer01AimeID || _isUsePlayer02AimeID)
		{
			IsAimeUse = true;
			if (_isUsePlayer01AimeID)
			{
				yield return StartCoroutine(GetUserPreview(0));
			}
			if (_isUsePlayer02AimeID)
			{
				yield return StartCoroutine(GetUserPreview(1));
			}
			yield return StartCoroutine(main.DebugUserDataDownloadCoroutine());
			if (_isUsePlayer02AimeID)
			{
				main.DebugSetData(1);
			}
			if (_isUsePlayer01AimeID)
			{
				main.DebugSetData(0);
			}
		}
		GameManager.SetMaxTrack();
		GameManager.UpdateRandom();
		TimeManager.MarkGameStartTime();
		Singleton<OperationManager>.Instance.UpdateGamePeriod();
		Singleton<EventManager>.Instance.UpdateEvent();
		Singleton<ScoreRankingManager>.Instance.UpdateData();
		Singleton<CollectionAchieve>.Instance.Configure();
		Singleton<CollectionAchieve>.Instance.Initialize();
		Type t = null;
		main.DebugAddProcess(prevProcess, t);
		yield return null;
		GameManager.IsFreedomMode = _isFreedomMode;
	}

	private IEnumerator GetUserPreview(int index)
	{
		bool processing = true;
		PacketHelper.StartPacket(new PacketGetUserPreview(UserID.ToUserID((index == 0) ? _player01AimeID : _player02AimeID), "", delegate(ulong id, UserPreviewResponseVO vo)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(index);
			userData.Initialize();
			userData.IsEntry = true;
			userData.Detail.UserID = vo.userId;
			userData.Detail.UserName = vo.userName;
			userData.Detail.AccessCode = "01234567890123456789";
			userData.Detail.SelectMapID = 3;
			userData.UserType = UserData.UserIDType.Exist;
			processing = false;
			Singleton<SeManager>.Instance.PlaySE(Cue.SE_ENTRY_AIME_OK, 0);
		}, delegate
		{
			processing = false;
		}));
		yield return new WaitWhile(() => processing);
	}

	private void InitializeCriGameObject()
	{
		GameObject gameObject = new GameObject("CRIWARE");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		_criParent = gameObject.transform;
		WasapiExclusive.Intialize();
		UnityEngine.Object.Instantiate(_prefabCriWareLibraryInitializer, _criParent);
	}
}
