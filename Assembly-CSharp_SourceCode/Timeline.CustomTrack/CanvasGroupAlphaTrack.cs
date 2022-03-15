using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.CustomTrack
{
	[TrackColor(1f, 0f, 0f)]
	[TrackClipType(typeof(CanvasGroupAlpha))]
	[TrackBindingType(typeof(CanvasGroup))]
	public class CanvasGroupAlphaTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			foreach (TimelineClip clip in GetClips())
			{
				CanvasGroupAlpha canvasGroupAlpha = (CanvasGroupAlpha)clip.asset;
				clip.displayName = $"{canvasGroupAlpha.Template.Alpha:0.00}";
			}
			return ScriptPlayable<CanvasGroupAlphaMixerBehaviour>.Create(graph, inputCount);
		}
	}
}
