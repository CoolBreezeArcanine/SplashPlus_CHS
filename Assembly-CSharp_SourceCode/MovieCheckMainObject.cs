using System;
using AMDaemon;
using Datas.DebugData;
using MAI2.Util;
using MAI2System;
using Main;
using Manager;
using Process;
using UnityEngine;
using UnityEngine.Analytics;

public class MovieCheckMainObject : MonoBehaviour
{
	[SerializeField]
	private Transform leftMonitor;

	[SerializeField]
	private Transform rightMonitor;

	[SerializeField]
	private GameObject _prefabCriWareLibraryInitializer;

	private Transform _criParent;

	[SerializeField]
	[Header("デバッグユーザーデータ")]
	private DebugUserData _player01;

	private GameMain main;

	private void Awake()
	{
		Singleton<SystemConfig>.Instance.initialize();
		InitializeCriGameObject();
		SingletonStateMachine<AmManager, AmManager.EState>.Instance.Initialize();
		QualitySettings.maxQueuedFrames = 0;
		Time.fixedDeltaTime = 1f / 120f;
		Analytics.enabled = false;
		Analytics.deviceStatsEnabled = false;
		Camera.main.backgroundColor = Color.black;
		main = new GameMain();
		GameManager.Initialize();
	}

	private void Start()
	{
		main?.Initialize(this, leftMonitor, rightMonitor);
	}

	private void FixedUpdate()
	{
		Core.Execute();
	}

	private void Update()
	{
		if (main != null)
		{
			FixedUpdate();
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
		main?.OnApplicationQuit();
		if (null != _criParent)
		{
			UnityEngine.Object.Destroy(_criParent);
			_criParent = null;
		}
	}

	public void DebugAddProcess(ProcessBase prevProcess)
	{
		if (_player01 != null)
		{
			Singleton<UserDataManager>.Instance.SetDebugUserData(0L, _player01);
		}
		Type t = null;
		main.DebugAddProcess(prevProcess, t);
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
