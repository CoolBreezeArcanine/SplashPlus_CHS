using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline
{
	public class ParticleControleObject : PlayableAsset, ITimelineClipAsset
	{
		public ExposedReference<ParticleSystem> ParticleSystemReference;

		public ClipCaps clipCaps => ClipCaps.None;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<ParticleControlPlayable> scriptPlayable = ScriptPlayable<ParticleControlPlayable>.Create(graph);
			bool idValid;
			ParticleSystem particleSystem = graph.GetResolver().GetReferenceValue("RankParticle", out idValid) as ParticleSystem;
			if (!idValid)
			{
				particleSystem = ParticleSystemReference.Resolve(graph.GetResolver());
			}
			ParticleControlPlayable behaviour = scriptPlayable.GetBehaviour();
			if (particleSystem != null)
			{
				behaviour.Initialize(particleSystem, 0u);
			}
			return scriptPlayable;
		}
	}
}
