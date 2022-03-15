using IO;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.MaiStudio;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor
{
	public class UnlockMonitor : MonitorBase
	{
		[SerializeField]
		public Animator _anim;

		[SerializeField]
		public RawImage _jacket;

		private bool IsUnlockMaster;

		private bool IsUnlockReMaster;

		private AssetManager _assetManager;

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (IsActive())
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			IsUnlockMaster = false;
			IsUnlockReMaster = false;
			if (!isActive || !UnlockCheck())
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
			}
		}

		public void SetAssetManager(AssetManager asset)
		{
			_assetManager = asset;
		}

		public bool IsUnlock()
		{
			return IsUnlockMaster | IsUnlockReMaster;
		}

		private bool UnlockCheck()
		{
			if (!isPlayerActive || GameManager.IsEventMode)
			{
				return false;
			}
			GameScoreList gameScore = Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex);
			int musicId = gameScore.SessionInfo.musicId;
			int difficulty = gameScore.SessionInfo.difficulty;
			bool flag = musicId >= 10000 && musicId < 20000;
			bool flag2 = difficulty >= 2;
			bool flag3 = GameManager.ConvAchiveDecimalToInt(gameScore.GetAchivement()) >= MusicClearrankID.Rank_S.GetAchvement();
			bool flag4 = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).MusicUnlockMasterList.Contains(musicId);
			bool flag5 = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).MusicUnlockReMasterList.Contains(musicId);
			IsUnlockMaster = flag && flag2 && !flag4 && flag3;
			IsUnlockReMaster = flag && flag2 && !flag5 && flag3;
			if (!Singleton<NotesListManager>.Instance.GetNotesList()[musicId].IsEnable[3])
			{
				IsUnlockMaster = false;
			}
			else if (IsUnlockMaster)
			{
				Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).MusicUnlockMasterList.Add(musicId);
			}
			if (!Singleton<NotesListManager>.Instance.GetNotesList()[musicId].IsEnable[4] || Singleton<DataManager>.Instance.GetMusics()[musicId].subLockType != 0)
			{
				IsUnlockReMaster = false;
			}
			else if (IsUnlockReMaster)
			{
				Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).MusicUnlockReMasterList.Add(musicId);
			}
			if (IsUnlockMaster && IsUnlockReMaster)
			{
				_anim.Play("Master_ReMaster");
			}
			else if (IsUnlockMaster)
			{
				_anim.Play("Master");
			}
			else if (IsUnlockReMaster)
			{
				_anim.Play("ReMaster");
			}
			if (IsUnlock())
			{
				MusicData music = Singleton<DataManager>.Instance.GetMusic(musicId);
				_jacket.texture = _assetManager.GetJacketTexture2D(music.jacketFile);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_MASTER_OPEN, monitorIndex);
				SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000133, monitorIndex);
			}
			if (!IsUnlockMaster)
			{
				return IsUnlockReMaster;
			}
			return true;
		}

		public bool IsPlaying()
		{
			if (!isPlayerActive || !IsUnlock())
			{
				return false;
			}
			return _anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
		}

		public override void ViewUpdate()
		{
		}
	}
}
