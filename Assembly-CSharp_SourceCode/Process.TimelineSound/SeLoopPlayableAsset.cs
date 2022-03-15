using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	[Serializable]
	public class SeLoopPlayableAsset : PlayableAsset
	{
		public ExposedReference<MonitorBase> Monitor;

		[HideInInspector]
		public int MonitorIndex;

		public string CueCode;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			SeLoopPlayableBehaviour seLoopPlayableBehaviour = new SeLoopPlayableBehaviour
			{
				CueCode = CueCode
			};
			if (Monitor.Resolve(graph.GetResolver()) != null)
			{
				seLoopPlayableBehaviour.PlayerIndex = Monitor.Resolve(graph.GetResolver()).MonitorIndex;
			}
			else
			{
				seLoopPlayableBehaviour.PlayerIndex = MonitorIndex;
			}
			return ScriptPlayable<SeLoopPlayableBehaviour>.Create(graph, seLoopPlayableBehaviour);
		}
	}
}
