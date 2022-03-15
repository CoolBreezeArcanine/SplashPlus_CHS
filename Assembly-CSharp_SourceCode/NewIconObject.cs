using UnityEngine;

public class NewIconObject : MonoBehaviour
{
	[SerializeField]
	[Header("本体")]
	private GameObject _baseObj;

	[SerializeField]
	[Header("アニメーション")]
	private Animator _mainAnim;

	public void SetView(bool isActive)
	{
		if (isActive)
		{
			_baseObj.SetActive(value: true);
			_mainAnim.Play("Loop");
		}
		else
		{
			_baseObj.SetActive(value: false);
		}
	}
}
