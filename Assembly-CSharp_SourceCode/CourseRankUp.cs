using UnityEngine;
using UnityEngine.UI;

public class CourseRankUp : MonoBehaviour
{
	public enum BGAnim
	{
		Idle,
		In_Dani,
		In_SinDani,
		In_Random,
		Out
	}

	[SerializeField]
	private Animator _anim;

	[SerializeField]
	private Image[] _classImage;

	public void SetAnim(BGAnim anim)
	{
		string text = "";
		switch (anim)
		{
		case BGAnim.Idle:
			text = "Idle";
			break;
		case BGAnim.In_Dani:
			text = "In_Dani";
			break;
		case BGAnim.In_SinDani:
			text = "In_ShinDani";
			break;
		case BGAnim.In_Random:
			text = "In_Random";
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

	public void SetClassSprite(Sprite sprite)
	{
		for (int i = 0; i < _classImage.Length; i++)
		{
			_classImage[i].sprite = sprite;
		}
	}
}
