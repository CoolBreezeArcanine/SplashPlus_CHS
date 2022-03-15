using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	[Serializable]
	public class JinglePlayableAsset : PlayableAsset
	{
		public ExposedReference<MonitorBase> Monitor;

		public string CueCode;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			JinglePlayableBehaviour jinglePlayableBehaviour = new JinglePlayableBehaviour();
			jinglePlayableBehaviour.CueCode = CueCode;
			if (Monitor.Resolve(graph.GetResolver()) != null)
			{
				jinglePlayableBehaviour.PlayerIndex = Monitor.Resolve(graph.GetResolver()).MonitorIndex;
			}
			return ScriptPlayable<JinglePlayableBehaviour>.Create(graph, jinglePlayableBehaviour);
		}
	}
}
