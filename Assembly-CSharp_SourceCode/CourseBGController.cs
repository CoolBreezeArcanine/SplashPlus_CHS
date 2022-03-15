using UnityEngine;

public class CourseBGController : MonoBehaviour
{
	public enum BGAnim
	{
		Dani_Loop,
		Dani_Track_Loop,
		Idle,
		SinDani_In,
		SinDani_Loop,
		SinDani_Track_Loop
	}

	[SerializeField]
	private Animator _anim;

	public void SetAnim(BGAnim anim)
	{
		string text = "";
		switch (anim)
		{
		case BGAnim.Dani_Loop:
			text = "Dani_Loop";
			break;
		case BGAnim.Dani_Track_Loop:
			text = "Dani_Track_Loop";
			break;
		case BGAnim.Idle:
			text = "Idle";
			break;
		case BGAnim.SinDani_In:
			text = "ShinDani_In";
			break;
		case BGAnim.SinDani_Loop:
			text = "ShinDani_Loop";
			break;
		case BGAnim.SinDani_Track_Loop:
			text = "ShinDani_Track_Loop";
			break;
		}
		if (text != "" && _anim != null)
		{
			_anim.Play(text);
		}
	}
}
