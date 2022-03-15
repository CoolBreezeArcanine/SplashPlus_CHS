using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using UnityEngine;
using UnityEngine.Playables;
using Util;

namespace Monitor.Game
{
	public class GameResultEffectCtrl : MonoBehaviour
	{
		[SerializeField]
		[Header("FullCombo")]
		private GameObject _fullComboPrefub;

		private GameObject _fullComboObj;

		private GameObject _fullComboPlusObj;

		[SerializeField]
		[Header("AllPerfect")]
		private GameObject _allPerfectPrefub;

		private GameObject _allPerfectObj;

		private GameObject _allPerfectPlusObj;

		[SerializeField]
		[Header("FullSync")]
		private GameObject _fullSyncPrefub;

		private GameObject _fullSyncObj;

		private GameObject _fullSyncPlusObj;

		[SerializeField]
		[Header("FullSyncDX")]
		private GameObject _fullsyncDxPrefub;

		private GameObject _fullsyncDxObj;

		private GameObject _fullsyncDxPlusObj;

		[SerializeField]
		[Header("ゲーム中達成率")]
		private GameObject _gameAchiveObj;

		[SerializeField]
		[Header("フルコンボ系Root")]
		private GameObject _fcApRootObj;

		[SerializeField]
		[Header("フルシンク系Root")]
		private GameObject _fullSyncRootObj;

		private const string FcApPlusObjectName = "null_PLUS";

		private const string SyncPlusObjectName = "PLUS";

		private bool _playDirector;

		private PlayableDirector _director;

		private int _monitorIndex;

		public void Initialize(int monIndex)
		{
			_monitorIndex = monIndex;
			_playDirector = false;
			_director = base.gameObject.GetComponent<PlayableDirector>();
			_fullComboObj = Object.Instantiate(_fullComboPrefub, _fcApRootObj.transform);
			_fullComboObj.SetActive(value: false);
			_fullComboPlusObj = Utility.findChildRecursive(_fullComboObj.transform, "null_PLUS").gameObject;
			_allPerfectObj = Object.Instantiate(_allPerfectPrefub, _fcApRootObj.transform);
			_allPerfectObj.SetActive(value: false);
			_allPerfectPlusObj = Utility.findChildRecursive(_allPerfectObj.transform, "null_PLUS").gameObject;
			_fullSyncObj = Object.Instantiate(_fullSyncPrefub, _fullSyncRootObj.transform);
			_fullSyncObj.SetActive(value: false);
			_fullSyncPlusObj = Utility.findChildRecursive(_fullSyncObj.transform, "PLUS").gameObject;
			_fullsyncDxObj = Object.Instantiate(_fullsyncDxPrefub, _fullSyncRootObj.transform);
			_fullsyncDxObj.SetActive(value: false);
			_fullsyncDxPlusObj = Utility.findChildRecursive(_fullsyncDxObj.transform, "PLUS").gameObject;
		}

		public void Execute()
		{
		}

		public bool IsEnd()
		{
			if (!_playDirector)
			{
				return true;
			}
			return _director.time >= _director.duration;
		}

		public bool Play()
		{
			int num = -1;
			PlayComboflagID comboType = Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).ComboType;
			switch (comboType)
			{
			case PlayComboflagID.Silver:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_FULL_COMBO, _monitorIndex);
				num = 62;
				_fullComboObj.SetActive(value: true);
				_fullComboPlusObj.SetActive(value: false);
				break;
			case PlayComboflagID.Gold:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_FULL_COMBO, _monitorIndex);
				num = 11;
				_fullComboObj.SetActive(value: true);
				_fullComboPlusObj.SetActive(value: true);
				break;
			case PlayComboflagID.AllPerfect:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_ALL_PERFECT, _monitorIndex);
				num = 63;
				_allPerfectObj.SetActive(value: true);
				_allPerfectPlusObj.SetActive(value: false);
				break;
			case PlayComboflagID.AllPerfectPlus:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_ALL_PERFECT, _monitorIndex);
				num = 12;
				_allPerfectObj.SetActive(value: true);
				_allPerfectPlusObj.SetActive(value: true);
				break;
			}
			if (comboType != 0)
			{
				if (_gameAchiveObj != null)
				{
					_gameAchiveObj.SetActive(value: false);
				}
				_director.Play();
				_playDirector = true;
			}
			switch (Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).SyncType)
			{
			case PlaySyncflagID.ChainLow:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_FULL_SYNC, _monitorIndex);
				num = 13;
				_fullSyncObj.SetActive(value: true);
				_fullSyncPlusObj.SetActive(value: false);
				break;
			case PlaySyncflagID.ChainHi:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_FULL_SYNC, _monitorIndex);
				num = 14;
				_fullSyncObj.SetActive(value: true);
				_fullSyncPlusObj.SetActive(value: true);
				break;
			case PlaySyncflagID.SyncLow:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_FULL_SYNC_DX, _monitorIndex);
				num = 15;
				_fullsyncDxObj.SetActive(value: true);
				_fullsyncDxPlusObj.SetActive(value: false);
				break;
			case PlaySyncflagID.SyncHi:
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_GAME_FULL_SYNC_DX, _monitorIndex);
				num = 16;
				_fullsyncDxObj.SetActive(value: true);
				_fullsyncDxPlusObj.SetActive(value: true);
				break;
			}
			if (-1 != num)
			{
				SoundManager.PlayPartnerVoice((Mai2.Voice_Partner_000001.Cue)num, _monitorIndex);
			}
			return _playDirector;
		}
	}
}
