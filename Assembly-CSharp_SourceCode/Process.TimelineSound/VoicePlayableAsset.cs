using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	[Serializable]
	public class VoicePlayableAsset : PlayableAsset
	{
		public ExposedReference<MonitorBase> Monitor;

		[HideInInspector]
		public int MonitorIndex;

		public string CueCode;

		public bool IsPartnerVoice;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			VoicePlayableBehaviour voicePlayableBehaviour = new VoicePlayableBehaviour();
			voicePlayableBehaviour.CueCode = CueCode;
			voicePlayableBehaviour.IsPartnerVoice = IsPartnerVoice;
			if (Monitor.Resolve(graph.GetResolver()) != null)
			{
				voicePlayableBehaviour.PlayerIndex = Monitor.Resolve(graph.GetResolver()).MonitorIndex;
			}
			else
			{
				voicePlayableBehaviour.PlayerIndex = MonitorIndex;
			}
			return ScriptPlayable<VoicePlayableBehaviour>.Create(graph, voicePlayableBehaviour);
		}
	}
}
