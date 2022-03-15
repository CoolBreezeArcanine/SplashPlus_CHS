using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using UI;
using UnityEngine;

public class RegionalDiscover : MonoBehaviour
{
	private enum StagingState
	{
		None,
		Initialize,
		Update,
		Wait,
		Release
	}

	[SerializeField]
	private float _skipabbleTime;

	[SerializeField]
	private InstantiateGenerator _discoverWindowGenerator;

	private NewIslandWindow _discoverWindow;

	private Queue<int> _idQueue;

	private StagingState _state;

	private bool _isDone;

	private bool _isSipabble;

	private int _monitorID;

	private int _currentDiscoverID;

	private float _timer;

	private AssetManager _assetManager;

	public bool IsShowSkipButton { get; set; }

	public void Initialize(int monitorID, bool isValid, AssetManager assetManager)
	{
		_monitorID = monitorID;
		if (isValid)
		{
			_discoverWindow = _discoverWindowGenerator.Instantiate<NewIslandWindow>();
		}
		_assetManager = assetManager;
		_state = StagingState.None;
		_timer = 0f;
		_isSipabble = false;
	}

	public void Play(Queue<int> queue)
	{
		_idQueue = queue;
		_isDone = false;
		Next();
	}

	private bool Next()
	{
		if (0 < _idQueue.Count)
		{
			_currentDiscoverID = _idQueue.Dequeue();
			MapData mapData = Singleton<DataManager>.Instance.GetMapData(_currentDiscoverID);
			Sprite mapBgSprite = _assetManager.GetMapBgSprite(_currentDiscoverID, "UI_Island");
			_discoverWindow.Set(mapBgSprite, mapData.name.str);
			_discoverWindow.Play(OnFinish);
			SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000066, _monitorID);
			SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MAP_NEW_OPEN_WINDOW, _monitorID);
			_state = StagingState.Initialize;
			_timer = 0f;
			_isSipabble = false;
			return true;
		}
		return false;
	}

	private void OnFinish()
	{
		if (!Next())
		{
			_state = StagingState.Release;
		}
	}

	public void ViewUpdate(float deltaTime)
	{
		switch (_state)
		{
		case StagingState.Initialize:
			_state = StagingState.Update;
			break;
		case StagingState.Update:
			_isSipabble = _timer >= _skipabbleTime;
			_timer += deltaTime;
			break;
		case StagingState.Release:
			_discoverWindow.gameObject.SetActive(value: false);
			_isDone = true;
			_state = StagingState.None;
			break;
		case StagingState.None:
		case StagingState.Wait:
			break;
		}
	}

	public bool IsDone()
	{
		return _isDone;
	}

	public bool IsSkippable()
	{
		return _isSipabble;
	}

	public void Skip()
	{
		_state = StagingState.Wait;
		_discoverWindow.Skip();
		OnFinish();
		_isSipabble = false;
	}
}
