using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	[Serializable]
	public class BgmPlayableAsset : PlayableAsset
	{
		public ExposedReference<MonitorBase> Monitor;

		public string CueCode;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			BgmPlayableBehaviour bgmPlayableBehaviour = new BgmPlayableBehaviour();
			bgmPlayableBehaviour.CueCode = CueCode;
			if (Monitor.Resolve(graph.GetResolver()) != null)
			{
				bgmPlayableBehaviour.PlayerIndex = Monitor.Resolve(graph.GetResolver()).MonitorIndex;
			}
			return ScriptPlayable<BgmPlayableBehaviour>.Create(graph, bgmPlayableBehaviour);
		}
	}
}
