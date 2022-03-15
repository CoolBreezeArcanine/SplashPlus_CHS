using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline
{
	public class TimeControlAsset : PlayableAsset, ITimelineClipAsset, IPropertyPreview
	{
		public ExposedReference<TimeControlBaseObject> Target;

		[HideInInspector]
		public TimeControlBaseObject TargetObject;

		public ClipCaps clipCaps => ClipCaps.None;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<TimeControlPlayableBehaviour> scriptPlayable = ScriptPlayable<TimeControlPlayableBehaviour>.Create(graph);
			scriptPlayable.GetBehaviour().Target = ((TargetObject == null) ? Target.Resolve(graph.GetResolver()) : TargetObject);
			return scriptPlayable;
		}

		public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
		}
	}
}
