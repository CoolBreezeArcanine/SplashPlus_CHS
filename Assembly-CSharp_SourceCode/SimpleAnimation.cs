using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class SimpleAnimation : MonoBehaviour
{
	private PlayableGraph graph;

	private AnimationMixerPlayable mixer;

	private AnimationClipPlayable prePlayable;

	private AnimationClipPlayable currentPlayable;

	[SerializeField]
	private AnimationClip animationClip;

	private void Awake()
	{
		graph = GetComponent<Animator>().playableGraph;
	}

	private void Start()
	{
		mixer = AnimationMixerPlayable.Create(graph, 2, normalizeWeights: true);
		graph.GetOutput(0).SetSourcePlayable(mixer);
		Play();
	}

	private void Update()
	{
	}

	public void Play()
	{
		if (currentPlayable.IsValid())
		{
			currentPlayable.Destroy();
		}
		graph.Disconnect(mixer, 0);
		graph.Disconnect(mixer, 1);
		currentPlayable = AnimationClipPlayable.Create(graph, animationClip);
		mixer.ConnectInput(0, currentPlayable, 0);
		mixer.SetInputWeight(1, 0f);
		mixer.SetInputWeight(0, 1f);
	}
}
