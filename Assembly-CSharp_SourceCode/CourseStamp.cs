using UnityEngine;

public class CourseStamp : MonoBehaviour
{
	public enum BGAnim
	{
		Clear,
		Idle,
		Not_Clear
	}

	[SerializeField]
	private Animator _anim;

	private bool _isClear;

	public void SetClear(bool isClear)
	{
		_isClear = isClear;
	}

	public void PlayClearAnim()
	{
		string stateName = (_isClear ? "Clear" : "NoClear");
		if (_anim != null)
		{
			_anim.Play(stateName);
		}
	}

	public void SetAnim(BGAnim anim)
	{
		string text = "";
		switch (anim)
		{
		case BGAnim.Clear:
			text = "Clear";
			break;
		case BGAnim.Idle:
			text = "Idle";
			break;
		case BGAnim.Not_Clear:
			text = "NoClear";
			break;
		}
		if (text != "" && _anim != null)
		{
			_anim.Play(text);
		}
	}
}
