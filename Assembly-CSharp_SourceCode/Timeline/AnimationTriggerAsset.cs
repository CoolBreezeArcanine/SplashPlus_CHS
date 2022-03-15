using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline
{
	public class AnimationTriggerAsset : PlayableAsset, ITimelineClipAsset, IPropertyPreview
	{
		public ExposedReference<Animator> TargetAnimatoReference;

		[SerializeField]
		public string TriggerName;

		public ClipCaps clipCaps => ClipCaps.None;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<AnimationTriggerBehaviour> scriptPlayable = ScriptPlayable<AnimationTriggerBehaviour>.Create(graph);
			scriptPlayable.GetBehaviour().Target = TargetAnimatoReference.Resolve(graph.GetResolver());
			scriptPlayable.GetBehaviour().Trigger = TriggerName;
			return scriptPlayable;
		}

		public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
		{
		}
	}
}
