using UnityEngine;

public class NavigationCharacter : MonoBehaviour
{
	[HideInInspector]
	public int HashWelcom = Animator.StringToHash("Navi_Welcom");

	[HideInInspector]
	public int HashFunStart = Animator.StringToHash("Navi_Fun_Start");

	[HideInInspector]
	public int HashSad01 = Animator.StringToHash("Navi_Sad_01");

	[HideInInspector]
	public int HashFunLoop = Animator.StringToHash("Navi_Fun_Loop_02");

	[SerializeField]
	private Animator[] _characterNaviAnimator;

	[SerializeField]
	private int _animationLayerIndex;

	[SerializeField]
	private Transform _emotionObject;

	[SerializeField]
	[Header("各アニメーション")]
	private AnimationClip _default;

	[SerializeField]
	private AnimationClip _funStart;

	[SerializeField]
	private AnimationClip _funLoop;

	[SerializeField]
	private AnimationClip _funEnd;

	[SerializeField]
	private AnimationClip _sad;

	public Transform EmotionObject => _emotionObject;

	public Animator[] NaviAnimator => _characterNaviAnimator;

	public AnimationClip Default => _default;

	public AnimationClip FunStart => _funStart;

	public AnimationClip FunLoop => _funLoop;

	public AnimationClip FunEnd => _funEnd;

	public AnimationClip Sad => _sad;

	public void Play(NavigationAnime type, float normalizedTime)
	{
		if (_characterNaviAnimator == null)
		{
			return;
		}
		int stateNameHash;
		if (_animationLayerIndex == 0)
		{
			switch (type)
			{
			default:
				return;
			case NavigationAnime.Welcom:
				stateNameHash = HashWelcom;
				break;
			case NavigationAnime.FunStart:
				stateNameHash = HashFunStart;
				break;
			case NavigationAnime.FunStartLoop:
				stateNameHash = HashFunLoop;
				break;
			case NavigationAnime.Sad01:
				stateNameHash = HashSad01;
				break;
			}
		}
		else
		{
			string layerName = _characterNaviAnimator[0].GetLayerName(_animationLayerIndex);
			SetLayer(layerName, 1f);
			switch (type)
			{
			default:
				return;
			case NavigationAnime.Welcom:
				stateNameHash = Animator.StringToHash(layerName + ".Navi_Welcom");
				break;
			case NavigationAnime.FunStart:
				stateNameHash = Animator.StringToHash(layerName + ".Navi_Fun_Start");
				break;
			case NavigationAnime.FunStartLoop:
				stateNameHash = Animator.StringToHash(layerName + ".Navi_Fun_Loop_02");
				break;
			case NavigationAnime.Sad01:
				stateNameHash = Animator.StringToHash(layerName + ".Navi_Sad_01");
				break;
			}
		}
		Animator[] characterNaviAnimator = _characterNaviAnimator;
		foreach (Animator animator in characterNaviAnimator)
		{
			if (animator != null)
			{
				animator.Play(stateNameHash, _animationLayerIndex, normalizedTime);
			}
		}
	}

	public void SetLayer(string index, float weight)
	{
		if (_characterNaviAnimator != null)
		{
			Animator[] characterNaviAnimator = _characterNaviAnimator;
			foreach (Animator obj in characterNaviAnimator)
			{
				obj.SetLayerWeight(obj.GetLayerIndex(index), weight);
			}
		}
	}

	public void SetBool(string flagName, bool flag)
	{
		if (_characterNaviAnimator != null)
		{
			Animator[] characterNaviAnimator = _characterNaviAnimator;
			for (int i = 0; i < characterNaviAnimator.Length; i++)
			{
				characterNaviAnimator[i].SetBool(flagName, flag);
			}
		}
	}

	public AnimatorStateInfo GetCurrentAnimatorStateInfo()
	{
		return _characterNaviAnimator[0].GetCurrentAnimatorStateInfo(_animationLayerIndex);
	}
}
