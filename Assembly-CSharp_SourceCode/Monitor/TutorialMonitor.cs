using MAI2.Util;
using Manager;
using UnityEngine;
using UnityEngine.Playables;

namespace Monitor
{
	public class TutorialMonitor : GameMonitor
	{
		[SerializeField]
		[Header("チュートリアルパラメータ")]
		private PlayableDirector _PlayableDirector;

		[SerializeField]
		[Header("新要素チュートリアル再生リスト")]
		private PlayableAsset[] _stageAssets;

		[SerializeField]
		[Header("新要素チュートリアルフェーズ進行表(ミリ秒)")]
		private float[] _stageTimes;

		[SerializeField]
		[Header("きほんチュートリアル再生リスト")]
		private PlayableAsset[] _stageAssetsLong;

		[SerializeField]
		[Header("きほんチュートリアルフェーズ進行表(ミリ秒)")]
		private float[] _stageTimesLong;

		[SerializeField]
		[Header("強制終了インフォメーションディレクター")]
		private PlayableDirector _ForceQuitPlayableDirector;

		[SerializeField]
		[Header("デモプレイ判定用オブジェクト")]
		private GameObject _DemoPlay;

		private PlayableAsset[] _stageAssetsPlay;

		private float[] _stageTimesPlay;

		private int _stageIndex;

		public TutorialMonitor()
		{
			GameMode = GameModeEnum.Tutorial;
		}

		public override void Initialize(int monIndex, bool isActive)
		{
			_stageIndex = 0;
			GameMode = GameModeEnum.Tutorial;
			base.Initialize(monIndex, isActive);
			if (GameManager.TutorialPlayed == GameManager.TutorialEnum.BasicPlay)
			{
				_stageAssetsPlay = _stageAssetsLong;
				_stageTimesPlay = _stageTimesLong;
			}
			else
			{
				_stageAssetsPlay = _stageAssets;
				_stageTimesPlay = _stageTimes;
			}
			_PlayableDirector.Play();
			_PlayableDirector.Pause();
			_PlayableDirector.time = 0.0;
			_ForceQuitPlayableDirector.Play();
			_ForceQuitPlayableDirector.Pause();
			_ForceQuitPlayableDirector.time = 0.0;
		}

		public override void PlayStart()
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				_PlayableDirector.Play();
				_ForceQuitPlayableDirector.Play();
			}
		}

		public override void ViewUpdate()
		{
			base.ViewUpdate();
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry && _stageIndex < _stageTimesPlay.Length && _stageTimesPlay[_stageIndex] <= NotesManager.GetCurrentMsec())
			{
				_PlayableDirector.Stop();
				_PlayableDirector.playableAsset = _stageAssetsPlay[_stageIndex];
				_PlayableDirector.Play();
				_stageIndex++;
			}
		}

		public bool IsDemoPlaying()
		{
			if (Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return _DemoPlay.activeSelf;
			}
			return false;
		}
	}
}
