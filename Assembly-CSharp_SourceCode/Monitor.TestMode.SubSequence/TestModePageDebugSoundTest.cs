using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.MaiStudio;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageDebugSoundTest : TestModePage
	{
		private enum ItemEnum
		{
			Se,
			Se_Back,
			Se_Play,
			Se_Next,
			Voice,
			Voice_Back,
			Voice_Play,
			Voice_Next,
			Partner_Voice,
			Partner_Voice_Back,
			Partner_Voice_Play,
			Partner_Voice_Next,
			Partner,
			Music,
			Music_Back,
			Music_Play,
			Music_Next,
			Finish
		}

		private int _seIndex;

		private List<string> _seList = new List<string>();

		private int _voiceIndex;

		private List<string> _voiceList = new List<string>();

		private int _partnerVoiceIndex;

		private List<string> _partnerVoiceList = new List<string>();

		private int _partnerIndex;

		private List<string> _partnerList = new List<string>();

		private List<int> _partnerIDList = new List<int>();

		private int _musicIndex;

		private bool _pv;

		private List<StringID> _musicList = new List<StringID>();

		protected override void OnCreate()
		{
			_seList.Clear();
			foreach (Mai2.Mai2Cue.Cue value in Enum.GetValues(typeof(Mai2.Mai2Cue.Cue)))
			{
				string item = Enum.GetName(typeof(Mai2.Mai2Cue.Cue), value);
				_seList.Add(item);
			}
			_seList.Sort();
			_seIndex = 0;
			_voiceList.Clear();
			foreach (Mai2.Voice_000001.Cue value2 in Enum.GetValues(typeof(Mai2.Voice_000001.Cue)))
			{
				string item2 = Enum.GetName(typeof(Mai2.Voice_000001.Cue), value2);
				_voiceList.Add(item2);
			}
			_voiceIndex = 0;
			_partnerVoiceList.Clear();
			foreach (Mai2.Voice_Partner_000001.Cue value3 in Enum.GetValues(typeof(Mai2.Voice_Partner_000001.Cue)))
			{
				string item3 = Enum.GetName(typeof(Mai2.Voice_Partner_000001.Cue), value3);
				_partnerVoiceList.Add(item3);
			}
			_partnerVoiceIndex = 0;
			_partnerList.Clear();
			_partnerIDList.Clear();
			foreach (KeyValuePair<int, PartnerData> partner in Singleton<DataManager>.Instance.GetPartners())
			{
				_partnerList.Add(partner.Value.name.str);
				_partnerIDList.Add(partner.Value.name.id);
			}
			_partnerIndex = 0;
			_pv = false;
			_musicList.Clear();
			foreach (KeyValuePair<int, MusicData> Value in Singleton<DataManager>.Instance.GetMusics())
			{
				if (_musicList.FirstOrDefault((StringID c) => c.id == Value.Value.cueName.id) == null)
				{
					_musicList.Add(Value.Value.cueName);
				}
			}
			_musicList.Sort((StringID a, StringID b) => a.id - b.id);
			_musicIndex = 0;
			SoundManager.SetMasterVolume(AdvertiseVolumeID._100.GetVolume());
		}

		protected override void Destroy()
		{
			base.OnCreate();
			SoundManager.StopAll();
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				item.ValueText.text = _seList[_seIndex];
				break;
			case 4:
				item.ValueText.text = _voiceList[_voiceIndex];
				break;
			case 8:
				item.ValueText.text = _partnerVoiceList[_partnerVoiceIndex];
				break;
			case 12:
				item.ValueText.text = _partnerList[_partnerIndex];
				break;
			case 13:
				item.ValueText.text = _musicList[_musicIndex].id + ":" + _musicList[_musicIndex].str + (_pv ? "(PV)" : "");
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			base.OnSelectItem(item, index);
			switch (index)
			{
			case 3:
				_seIndex++;
				_seIndex %= _seList.Count;
				SoundManager.StopAll();
				SoundManager.PlaySE((Mai2.Mai2Cue.Cue)Enum.Parse(typeof(Mai2.Mai2Cue.Cue), _seList[_seIndex], ignoreCase: true), 0);
				break;
			case 1:
				_seIndex--;
				_seIndex = ((_seIndex < 0) ? (_seList.Count - 1) : _seIndex);
				SoundManager.StopAll();
				SoundManager.PlaySE((Mai2.Mai2Cue.Cue)Enum.Parse(typeof(Mai2.Mai2Cue.Cue), _seList[_seIndex], ignoreCase: true), 0);
				break;
			case 2:
				SoundManager.StopAll();
				SoundManager.PlaySE((Mai2.Mai2Cue.Cue)Enum.Parse(typeof(Mai2.Mai2Cue.Cue), _seList[_seIndex], ignoreCase: true), 0);
				break;
			case 7:
				_voiceIndex++;
				_voiceIndex %= _voiceList.Count;
				SoundManager.StopAll();
				SoundManager.PlayVoice((Mai2.Voice_000001.Cue)Enum.Parse(typeof(Mai2.Voice_000001.Cue), _voiceList[_voiceIndex], ignoreCase: true), 0);
				break;
			case 5:
				_voiceIndex--;
				_voiceIndex = ((_voiceIndex < 0) ? (_voiceList.Count - 1) : _voiceIndex);
				SoundManager.StopAll();
				SoundManager.PlayVoice((Mai2.Voice_000001.Cue)Enum.Parse(typeof(Mai2.Voice_000001.Cue), _voiceList[_voiceIndex], ignoreCase: true), 0);
				break;
			case 6:
				SoundManager.StopAll();
				SoundManager.PlayVoice((Mai2.Voice_000001.Cue)Enum.Parse(typeof(Mai2.Voice_000001.Cue), _voiceList[_voiceIndex], ignoreCase: true), 0);
				break;
			case 11:
				_partnerVoiceIndex++;
				_partnerVoiceIndex %= _partnerVoiceList.Count;
				SoundManager.StopAll();
				SoundManager.PlayPartnerVoice((Mai2.Voice_Partner_000001.Cue)Enum.Parse(typeof(Mai2.Voice_Partner_000001.Cue), _partnerVoiceList[_partnerVoiceIndex], ignoreCase: true), 0);
				break;
			case 9:
				_partnerVoiceIndex--;
				_partnerVoiceIndex = ((_partnerVoiceIndex < 0) ? (_partnerVoiceList.Count - 1) : _partnerVoiceIndex);
				SoundManager.StopAll();
				SoundManager.PlayPartnerVoice((Mai2.Voice_Partner_000001.Cue)Enum.Parse(typeof(Mai2.Voice_Partner_000001.Cue), _partnerVoiceList[_partnerVoiceIndex], ignoreCase: true), 0);
				break;
			case 10:
				SoundManager.StopAll();
				SoundManager.PlayPartnerVoice((Mai2.Voice_Partner_000001.Cue)Enum.Parse(typeof(Mai2.Voice_Partner_000001.Cue), _partnerVoiceList[_partnerVoiceIndex], ignoreCase: true), 0);
				break;
			case 12:
				_partnerIndex++;
				_partnerIndex %= _partnerList.Count;
				SoundManager.StopAll();
				SoundManager.SetPartnerVoiceCue(0, _partnerIDList[_partnerIndex]);
				break;
			case 13:
				_pv = !_pv;
				break;
			case 16:
				_musicIndex++;
				_musicIndex %= _musicList.Count;
				SoundManager.StopAll();
				if (_pv)
				{
					SoundManager.PreviewPlay(_musicList[_musicIndex].id);
				}
				else
				{
					SoundManager.MusicPrepare(_musicList[_musicIndex].id, prepare: false);
				}
				break;
			case 14:
				_musicIndex--;
				_musicIndex = ((_musicIndex < 0) ? (_musicList.Count - 1) : _musicIndex);
				SoundManager.StopAll();
				if (_pv)
				{
					SoundManager.PreviewPlay(_musicList[_musicIndex].id);
				}
				else
				{
					SoundManager.MusicPrepare(_musicList[_musicIndex].id, prepare: false);
				}
				break;
			case 15:
				SoundManager.StopAll();
				if (_pv)
				{
					SoundManager.PreviewPlay(_musicList[_musicIndex].id);
				}
				else
				{
					SoundManager.MusicPrepare(_musicList[_musicIndex].id, prepare: false);
				}
				break;
			case 4:
			case 8:
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeDebugSoundtestID)Enum.Parse(typeof(TestmodeDebugSoundtestID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeDebugSoundtestID)Enum.Parse(typeof(TestmodeDebugSoundtestID), GetTitleName(index))).GetName();
		}
	}
}
