using TMPro;
using UnityEngine;

public class BaloonMessage : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _serifText;

	[SerializeField]
	private Animator _animator;

	public void SetSerif(string serif)
	{
		_serifText.text = serif;
	}

	public void Play(string serif)
	{
		SetSerif(serif);
		int stateNameHash = Animator.StringToHash("Base Layer.Loop");
		_animator.Play(stateNameHash, 0, 0f);
	}

	public void PlayNonActive()
	{
		_animator.Play("NonActive");
	}
}
