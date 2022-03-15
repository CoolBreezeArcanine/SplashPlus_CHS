using UnityEngine;
using UnityEngine.UI;

public class ClassMedalObject : MonoBehaviour
{
	private Animator _medalAnimator;

	[SerializeField]
	private Image _medalImage;

	[SerializeField]
	private Image _effectLoopMedalImage;

	[SerializeField]
	private Image _nextMedalImage;

	[SerializeField]
	private Image _nextEffectMedalImage;

	[SerializeField]
	private Image _nextEffectLoopImage;

	public Animator MedalAnimator
	{
		get
		{
			if (_medalAnimator == null)
			{
				_medalAnimator = GetComponent<Animator>();
			}
			return _medalAnimator;
		}
	}

	public void SetCurrentMedal(Sprite medal)
	{
		_medalImage.sprite = medal;
		_effectLoopMedalImage.sprite = medal;
	}

	public void SetNextMedal(Sprite medal)
	{
		_nextMedalImage.sprite = medal;
		_nextEffectMedalImage.sprite = medal;
		_nextEffectLoopImage.sprite = medal;
	}

	public void Loop()
	{
		MedalAnimator.Play("Loop", 0, 0f);
	}

	public void ClassUp()
	{
		MedalAnimator.Play("ClassUp", 0, 0f);
	}

	public void ClassDown()
	{
		MedalAnimator.Play("ClassDown", 0, 0f);
	}
}
