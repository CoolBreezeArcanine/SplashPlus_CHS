using UnityEngine;

public class CourseItemInfo : MonoBehaviour
{
	public enum BGAnim
	{
		Hide,
		Idle,
		In,
		Loop,
		Out
	}

	[SerializeField]
	private Animator _anim;

	[SerializeField]
	private GameObject _ItemGot;

	public void SetAnim(BGAnim anim)
	{
		string text = "";
		switch (anim)
		{
		case BGAnim.Hide:
			text = "Hide";
			break;
		case BGAnim.Idle:
			text = "Idle";
			break;
		case BGAnim.In:
			text = "In";
			break;
		case BGAnim.Loop:
			text = "Loop";
			break;
		case BGAnim.Out:
			text = "Out";
			break;
		}
		if (text != "" && _anim != null)
		{
			_anim.Play(text);
		}
		if (_ItemGot != null)
		{
			_ItemGot.SetActive(value: false);
		}
	}
}
