using System;
using UI;
using UnityEngine;
using UnityEngine.Playables;

namespace Process.Timeline
{
	[Serializable]
	public class MultiImagePlayableAsset : PlayableAsset
	{
		public ExposedReference<MonitorBase> Monitor;

		public ExposedReference<MultipleImage> image;

		public int index;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			MultiImagePlayableBehaviour multiImagePlayableBehaviour = new MultiImagePlayableBehaviour();
			multiImagePlayableBehaviour.image = image.Resolve(graph.GetResolver());
			multiImagePlayableBehaviour.index = index;
			if (Monitor.Resolve(graph.GetResolver()) != null)
			{
				multiImagePlayableBehaviour.PlayerIndex = Monitor.Resolve(graph.GetResolver()).MonitorIndex;
			}
			return ScriptPlayable<MultiImagePlayableBehaviour>.Create(graph, multiImagePlayableBehaviour);
		}
	}
}
