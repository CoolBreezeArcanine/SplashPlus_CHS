using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.GetMusic
{
	public class GetMusicMonitor : MonitorBase
	{
		public enum DispState
		{
			In,
			DispWait,
			Out,
			EndWait,
			End
		}

		[SerializeField]
		[Header("獲得ウィンドウ")]
		private GameObject _commonGetWindow;

		private MusicWindow _windowController;

		[SerializeField]
		[Header("獲得ウィンドウの親")]
		private GameObject _windowRoot;

		[SerializeField]
		private GetMusicButtonController _buttonController;

		private AssetManager assetManager;

		private long InputLockTime;

		private const long SkipLockTime = 1000L;

		private const long NextWaitTime = 5000L;

		private DispState state = DispState.End;

		private List<int> infoList = new List<int>();

		private int infoIndex;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			_windowController = Object.Instantiate(_commonGetWindow, _windowRoot.transform).GetComponent<MusicWindow>();
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
				SoundManager.PlaySE(Cue.SE_INFO_NORMAL, monitorIndex);
				int id = infoList[infoIndex];
				Texture2D jacketTexture2D = assetManager.GetJacketTexture2D(id);
				Sprite jacket = Sprite.Create(jacketTexture2D, new Rect(0f, 0f, jacketTexture2D.width, jacketTexture2D.height), new Vector2(0.5f, 0.5f));
				string musicName = Singleton<DataManager>.Instance.GetMusic(id)?.name.str;
				PlayGetMusic(jacket, musicName);
				InputLockTime = 0L;
				break;
			}
			case DispState.DispWait:
				if ((InputLockTime > 1000 && InputManager.GetButtonDown(base.MonitorIndex, InputManager.ButtonSetting.Button04)) || InputLockTime > 5000)
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

		public void SetInfoList(List<int> list)
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

		public void PlayGetMusic(Sprite jacket, string musicName)
		{
			_windowController.gameObject.SetActive(value: true);
			_windowController.Set(jacket, musicName);
			_windowController.Play(null);
		}
	}
}
