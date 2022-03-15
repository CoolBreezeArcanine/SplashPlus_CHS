using UnityEngine;

public class CourseClearPlate : MonoBehaviour
{
	public enum BGAnim
	{
		Idle,
		In,
		Loop,
		Out
	}

	[SerializeField]
	private Animator _anim;

	[SerializeField]
	private GameObject _clearObj;

	[SerializeField]
	private GameObject _noClearObj;

	public void SetAnim(BGAnim anim)
	{
		string text = "";
		switch (anim)
		{
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
	}

	public void ViewClear(bool isClear)
	{
		_clearObj.SetActive(isClear);
		_noClearObj.SetActive(!isClear);
	}
}
