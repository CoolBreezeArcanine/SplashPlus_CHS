using Mai2.Mai2Cue;
using Manager;
using UnityEngine;

public class CourseTransition : MonoBehaviour
{
	public enum BGAnim
	{
		ClearBonus,
		Idle,
		In,
		Loop,
		Out,
		TrackStart_Dani,
		TrackStart_Random,
		TrackStart_SinDani
	}

	[SerializeField]
	private Animator _anim;

	public void SetAnim(BGAnim anim, int playerIndex)
	{
		string text = "";
		switch (anim)
		{
		case BGAnim.ClearBonus:
			text = "ClearBonus";
			break;
		case BGAnim.Idle:
			text = "Idle";
			break;
		case BGAnim.In:
			text = "In";
			SoundManager.PlaySE(Cue.SE_DANI_SHOJI_OPEN, playerIndex);
			break;
		case BGAnim.Loop:
			text = "Loop";
			break;
		case BGAnim.Out:
			text = "Out";
			SoundManager.PlaySE(Cue.SE_DANI_SHOJI_CLOSE, playerIndex);
			break;
		case BGAnim.TrackStart_Dani:
			text = "TrackStart_Dani";
			break;
		case BGAnim.TrackStart_Random:
			text = "TrackStart_Random";
			break;
		case BGAnim.TrackStart_SinDani:
			text = "TrackStart_ShinDani";
			break;
		}
		if (text != "" && _anim != null)
		{
			_anim.Play(text);
		}
	}
}
