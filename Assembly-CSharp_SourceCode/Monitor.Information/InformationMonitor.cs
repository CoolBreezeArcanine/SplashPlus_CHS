using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mai2.Mai2Cue;
using Mai2.Voice_000001;
using Manager;
using Manager.MaiStudio;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Information
{
	public class InformationMonitor : MonitorBase
	{
		public enum DispState
		{
			In,
			Loop,
			DispWait,
			Out,
			EndWait,
			End
		}

		[SerializeField]
		private RawImage _eventImage;

		[SerializeField]
		private Animator _animator;

		[SerializeField]
		[Header("表示時間ms")]
		private int _dispTime;

		[SerializeField]
		private InformationButtonController _buttonController;

		private DispState state = DispState.End;

		private Stopwatch timer = new Stopwatch();

		private List<InformationData> infoList = new List<InformationData>();

		private int infoIndex;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			if (!active)
			{
				state = DispState.End;
			}
		}

		public override void ViewUpdate()
		{
			_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
			switch (state)
			{
			case DispState.In:
				state = DispState.Loop;
				_animator.Play("In");
				_eventImage.texture = GetInformationTexture(infoList[infoIndex].fileName.path.ToString());
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_INFO_NORMAL, monitorIndex);
				switch (infoList[infoIndex].infoKind)
				{
				case InformationKind.Music:
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000059, monitorIndex);
					break;
				case InformationKind.Event:
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000060, monitorIndex);
					break;
				default:
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000061, monitorIndex);
					break;
				}
				break;
			case DispState.Loop:
				if (!IsPlayingAnim())
				{
					state = DispState.DispWait;
					_animator.Play("Loop");
					timer.Restart();
					_buttonController.SetVisible(true, 3);
				}
				break;
			case DispState.DispWait:
				if (timer.ElapsedMilliseconds > _dispTime || InputManager.GetButtonDown(base.MonitorIndex, InputManager.ButtonSetting.Button04))
				{
					state = DispState.Out;
					_animator.Play("Out");
					_buttonController.SetAnimationActive(3);
					_buttonController.SetVisible(false, 3);
				}
				break;
			case DispState.Out:
				if (!IsPlayingAnim())
				{
					infoIndex++;
					if (infoIndex >= infoList.Count)
					{
						state = DispState.EndWait;
					}
					else
					{
						state = DispState.In;
					}
				}
				break;
			case DispState.EndWait:
				state = DispState.End;
				break;
			case DispState.End:
				break;
			}
		}

		public void SetInfoList(List<InformationData> list)
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
			_buttonController.SetVisible(false, 3);
		}

		public bool IsState(DispState _state)
		{
			return state == _state;
		}

		private bool IsPlayingAnim()
		{
			return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
		}

		private Texture2D GetInformationTexture(string filePath)
		{
			Texture2D texture2D = null;
			if (File.Exists(filePath))
			{
				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						byte[] data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
						texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false);
						texture2D.LoadImage(data);
						binaryReader.Close();
					}
					fileStream.Close();
					return texture2D;
				}
			}
			return texture2D;
		}
	}
}
