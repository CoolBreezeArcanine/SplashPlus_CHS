using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Process.TimelineSound
{
	[Serializable]
	public class SePlayableAsset : PlayableAsset
	{
		public ExposedReference<MonitorBase> Monitor;

		[HideInInspector]
		public int MonitorIndex;

		public string CueCode;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			SePlayableBehaviour sePlayableBehaviour = new SePlayableBehaviour();
			sePlayableBehaviour.CueCode = CueCode;
			if (Monitor.Resolve(graph.GetResolver()) != null)
			{
				sePlayableBehaviour.PlayerIndex = Monitor.Resolve(graph.GetResolver()).MonitorIndex;
			}
			else
			{
				sePlayableBehaviour.PlayerIndex = MonitorIndex;
			}
			return ScriptPlayable<SePlayableBehaviour>.Create(graph, sePlayableBehaviour);
		}
	}
}
