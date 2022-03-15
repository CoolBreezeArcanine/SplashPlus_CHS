using System.Collections.Generic;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.UnlockMusic
{
	public class UnlockMusicMonitor : MonitorBase
	{
		public enum DispState
		{
			In,
			DispWait,
			Out,
			EndWait,
			End
		}

		public enum InfoKind
		{
			Transmission,
			ScoreRanking,
			Collection
		}

		public struct InfoParam
		{
			public int musicID;

			public InfoKind infoKind;
		}

		[SerializeField]
		[Header("獲得ウィンドウ")]
		private GameObject _commonGetWindow;

		private UnlockMusicWindow _windowController;

		[SerializeField]
		[Header("獲得ウィンドウの親")]
		private GameObject _windowRoot;

		[SerializeField]
		private UnlockMusicButtonController _buttonController;

		private AssetManager assetManager;

		private long InputLockTime;

		private const long SkipLockTime = 1000L;

		private const long NextWaitTime = 20000L;

		private DispState state = DispState.End;

		private List<InfoParam> infoList = new List<InfoParam>();

		private int infoIndex;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			_windowController = Object.Instantiate(_commonGetWindow, _windowRoot.transform).GetComponent<UnlockMusicWindow>();
			_windowController.gameObject.SetActive(value: false);
		}

		public void SetAssetManager(AssetManager manager)
		{
			assetManager = manager;
		}

		public override void ViewUpdate()
		{
			InputLockTime += GameManager.GetGameMSecAdd();
			switch (state)
			{
			case DispState.In:
			{
				state = DispState.DispWait;
				int musicID = infoList[infoIndex].musicID;
				Texture2D jacketTexture2D = assetManager.GetJacketTexture2D(musicID);
				Sprite jacket = Sprite.Create(jacketTexture2D, new Rect(0f, 0f, jacketTexture2D.width, jacketTexture2D.height), new Vector2(0.5f, 0.5f));
				string musicName = Singleton<DataManager>.Instance.GetMusic(musicID)?.name.str;
				PlayUnlockMusic(jacket, musicName, infoList[infoIndex].infoKind);
				InputLockTime = 0L;
				break;
			}
			case DispState.DispWait:
				if ((InputLockTime > 1000 && InputManager.GetButtonDown(base.MonitorIndex, InputManager.ButtonSetting.Button04)) || InputLockTime > 20000)
				{
					_windowController.Skip();
					GetWindowCallback(base.MonitorIndex);
				}
				break;
			case DispState.Out:
				infoIndex++;
				if (infoIndex >= infoList.Count)
				{
					state = DispState.EndWait;
					_buttonController.SetVisible(false, 3);
				}
				else
				{
					state = DispState.In;
				}
				break;
			case DispState.EndWait:
				state = DispState.End;
				break;
			}
			_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
		}

		public bool IsState(DispState _state)
		{
			return state == _state;
		}

		public void SetInfoList(List<InfoParam> list)
		{
			infoList = list;
			if (infoList.Count == 0)
			{
				state = DispState.EndWait;
				return;
			}
			state = DispState.In;
			infoIndex = 0;
			_buttonController.Initialize(base.MonitorIndex);
			_buttonController.SetVisible(true, 3);
		}

		private void GetWindowCallback(int playerIndex)
		{
			state = DispState.Out;
			_buttonController.SetAnimationActive(3);
		}

		public void PlayUnlockMusic(Sprite jacket, string musicName, InfoKind infoKind)
		{
			string infoText = "";
			switch (infoKind)
			{
			case InfoKind.Transmission:
				infoText = CommonMessageID.UnlockMusicTransmission.GetName();
				break;
			case InfoKind.ScoreRanking:
				infoText = CommonMessageID.UnlockMusicScoreRanking.GetName();
				break;
			case InfoKind.Collection:
				infoText = CommonMessageID.UnlockMusicCollection.GetName();
				break;
			}
			_windowController.gameObject.SetActive(value: true);
			_windowController.Set(jacket, musicName);
			_windowController.SetInfoText(infoText);
			_windowController.Play(null);
			SoundManager.PlaySE(Cue.JINGLE_MAP_GET, monitorIndex);
		}
	}
}
