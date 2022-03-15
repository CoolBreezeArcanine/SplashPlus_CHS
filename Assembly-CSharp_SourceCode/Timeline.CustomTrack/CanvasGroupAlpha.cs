using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.CustomTrack
{
	public class CanvasGroupAlpha : PlayableAsset, ITimelineClipAsset
	{
		[SerializeField]
		public CanvasGroupAlphaBehaviour Template;

		public ClipCaps clipCaps => ClipCaps.Blending;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<CanvasGroupAlphaBehaviour>.Create(graph, Template);
		}
	}
}
