using System.Collections.Generic;
using DB;
using IO;
using Mecha;
using Monitor.TakeOver;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Monitor
{
	public class PlInformationMonitor : MonitorBase
	{
		private PlayableDirector _director;

		[SerializeField]
		[Header("In")]
		private PlayableAsset _playableInTrack;

		[SerializeField]
		[Header("Loop")]
		private PlayableAsset _playableLoopTrack;

		[SerializeField]
		private TextMeshProUGUI _AttentionTextLeft;

		[SerializeField]
		private TextMeshProUGUI _AttentionTextRight;

		private bool _isSkip;

		public bool _isDataConversion;

		public bool _isConvertedData;

		public bool _isFinishedDataConversion;

		public uint _rom_version;

		public TakeOverMonitor.MajorRomVersion _major_version;

		public bool _isMajorVersionUp;

		public bool _isMinorVersionUp;

		private void Awake()
		{
			_AttentionTextLeft.text = CommonMessageID.PlInfomationLeft.GetName();
			_AttentionTextRight.text = CommonMessageID.PlInfomationRight.GetName();
		}

		public void Initialize(int monIndex, bool isActive, bool isContinue)
		{
			Initialize(monIndex, isActive);
			if (!isActive)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
				isPlayerActive = false;
				if (!isContinue)
				{
					List<Jvs.LedPwmFadeParam> list = new List<Jvs.LedPwmFadeParam>();
					List<Color> list2 = new List<Color>
					{
						CommonScriptable.GetLedSetting().ButtonMainColor,
						CommonScriptable.GetLedSetting().ButtonSubColor
					};
					for (int i = 0; i < list2.Count; i++)
					{
						list.Add(new Jvs.LedPwmFadeParam
						{
							StartFadeColor = list2[i],
							EndFadeColor = list2[(i + 1) % list2.Count],
							FadeTime = 4000L,
							NextIndex = (i + 1) % list2.Count
						});
					}
					Bd15070_4IF[] ledIf = MechaManager.LedIf;
					for (int j = 0; j < ledIf.Length; j++)
					{
						ledIf[j].SetColorMultiAutoFade(list);
					}
				}
			}
			else
			{
				_director = Main.gameObject.GetComponent<PlayableDirector>();
				_director.extrapolationMode = DirectorWrapMode.Hold;
				_director.playableAsset = _playableInTrack;
				_director.Stop();
			}
		}

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			_isDataConversion = false;
			_isConvertedData = false;
			_isFinishedDataConversion = false;
			MechaManager.LedIf[monIndex].ButtonLedReset();
		}

		public void PlayPlInfo()
		{
			_director.Play();
		}

		public void Skip()
		{
			_isSkip = true;
		}

		public bool IsPlayPlInfoEnd()
		{
			if (_isSkip)
			{
				return true;
			}
			if (!isPlayerActive)
			{
				return true;
			}
			return _director.time >= _director.duration;
		}

		public void PlayPlLoop()
		{
			if (!_isSkip)
			{
				_director.Stop();
				_director.extrapolationMode = DirectorWrapMode.Loop;
				_director.playableAsset = _playableLoopTrack;
				_director.Play();
			}
		}

		public override void ViewUpdate()
		{
		}
	}
}
