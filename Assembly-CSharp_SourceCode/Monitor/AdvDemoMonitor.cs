using System.Collections.Generic;
using System.Linq;
using IO;
using Manager;
using UnityEngine;
using UnityEngine.Playables;

namespace Monitor
{
	public class AdvDemoMonitor : GameMonitor
	{
		private List<Jvs.LedPwmFadeParam> _fadeList = new List<Jvs.LedPwmFadeParam>();

		private AdvRankingParts _musicRanking;

		[SerializeField]
		[Header("アドバタイズパラメータ")]
		private PlayableDirector _musicRankingDirector;

		[SerializeField]
		private PlayableDirector _scrollRankingDirector;

		[SerializeField]
		private AdvRankingParts _rankingPlateParts;

		[SerializeField]
		private AdvRankingParts _rankingParts;

		[SerializeField]
		private AdvRankingParts _rankingTopParts;

		[SerializeField]
		private GameObject _rankingPlayingRoot;

		[SerializeField]
		private GameObject _rankingScrollRoot;

		private List<GameObject> _rankingScrollList = new List<GameObject>();

		[SerializeField]
		private GameObject _rankingTop3Root;

		private List<GameObject> _rankingTop3List = new List<GameObject>();

		[SerializeField]
		private GameObject _aimeInfoObj;

		[SerializeField]
		[Header("ゲームノーツ表示時間制御用")]
		private GameObject _notesFieldActiveObject;

		private const string RankingPlateAnimTrackName = "RankingPlateAnimTrack";

		private const string RankingPlateActiveTrackName = "RankingPlateActiveTrack";

		private bool _killSwitch;

		public AdvDemoMonitor()
		{
			GameMode = GameModeEnum.Advertise;
		}

		public override void Initialize(int monIndex, bool isActive)
		{
			MechaManager.LedIf[monIndex].ButtonLedReset();
			_fadeList.Clear();
			_fadeList.Add(new Jvs.LedPwmFadeParam
			{
				StartFadeColor = CommonScriptable.GetLedSetting().ButtonMainColor,
				EndFadeColor = Color.black,
				FadeTime = 100L,
				NextIndex = -1
			});
			base.Initialize(monIndex, isActive);
			for (int i = 0; i < _rankingScrollRoot.transform.childCount; i++)
			{
				_rankingScrollList.Add(_rankingScrollRoot.transform.GetChild(i).gameObject);
			}
			for (int j = 0; j < _rankingTop3Root.transform.childCount; j++)
			{
				_rankingTop3List.Add(_rankingTop3Root.transform.GetChild(j).gameObject);
			}
			_musicRanking = Object.Instantiate(_rankingPlateParts.gameObject, _rankingPlayingRoot.transform).GetComponent<AdvRankingParts>();
			_musicRanking.gameObject.SetActive(value: true);
			Animator component = _musicRanking.GetComponent<Animator>();
			PlayableBinding playableBinding = _musicRankingDirector.playableAsset.outputs.First((PlayableBinding c) => c.streamName == "RankingPlateAnimTrack");
			_musicRankingDirector.SetGenericBinding(playableBinding.sourceObject, component);
			playableBinding = _musicRankingDirector.playableAsset.outputs.First((PlayableBinding c) => c.streamName == "RankingPlateActiveTrack");
			_musicRankingDirector.SetGenericBinding(playableBinding.sourceObject, _musicRanking.gameObject);
			_musicRankingDirector.Play();
			_musicRankingDirector.Pause();
			_musicRankingDirector.time = 0.0;
			_scrollRankingDirector.Play();
			_scrollRankingDirector.Pause();
			_scrollRankingDirector.time = 0.0;
			Main.gameObject.SetActive(value: false);
			Sub.gameObject.SetActive(value: false);
		}

		public void SetMusicRanking(Texture2D tex, string musicName, int rank, bool isNew)
		{
			AdvRankingParts advRankingParts = null;
			advRankingParts = ((rank >= 3) ? Object.Instantiate(_rankingParts.gameObject, _rankingScrollList[rank - 3].transform).GetComponent<AdvRankingParts>() : Object.Instantiate(_rankingPlateParts.gameObject, _rankingTop3List[rank].transform).GetComponent<AdvRankingParts>());
			advRankingParts.gameObject.SetActive(value: true);
			advRankingParts.SetMusic(tex, musicName, rank, isNew);
		}

		public void SetMusicInfo(Texture2D tex, string musicName, int rank, bool isNew)
		{
			_musicRanking.SetMusic(tex, musicName, rank, isNew);
		}

		public override void PlayStart()
		{
			Main.gameObject.SetActive(value: true);
			Sub.gameObject.SetActive(value: true);
			_musicRankingDirector.Play();
			_scrollRankingDirector.Play();
		}

		protected override void BeatUpdate()
		{
			BarData barNextData = NoteMng.getReader().getBarNextData(BarType.GAMEBEAT, _barTime);
			if (barNextData != null)
			{
				Jvs.LedPwmFadeParam ledPwmFadeParam = _fadeList[0];
				ledPwmFadeParam.FadeTime = (long)(barNextData.time.msec - _barTime);
				MechaManager.LedIf[monitorIndex].SetColorMultiAutoFade(_fadeList);
				_barTime = barNextData.time.msec;
			}
		}

		public override void ViewUpdate()
		{
			base.ViewUpdate();
			_musicRanking.ViewUpdate();
			GameController.SetActiveNotesField(IsActiveNotesField());
		}

		public bool IsActiveNotesField()
		{
			if (_notesFieldActiveObject.activeSelf)
			{
				return !_killSwitch;
			}
			return false;
		}

		public override void PlayStop()
		{
			_killSwitch = true;
			GameController.SetActiveNotesField(flg: false);
			_musicRankingDirector.time = _musicRankingDirector.duration;
			_scrollRankingDirector.time = _scrollRankingDirector.duration;
			_musicRankingDirector.Evaluate();
			_scrollRankingDirector.Evaluate();
		}
	}
}
