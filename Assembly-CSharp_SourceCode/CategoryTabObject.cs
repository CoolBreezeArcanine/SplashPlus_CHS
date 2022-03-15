using System;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTabObject : MonoBehaviour
{
	[SerializeField]
	private Animator _tabAnimator;

	[SerializeField]
	private Text _title;

	[SerializeField]
	private Animator _tabnameAnimator;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private Shadow _tabNameShadow;

	[SerializeField]
	private Outline _tabnameOutline;

	[SerializeField]
	private Image _rightImage;

	[SerializeField]
	private Image _lefetImage;

	[SerializeField]
	private Transform _chainObject;

	[SerializeField]
	private CategoryTabPosition _currentPosition;

	private bool _isAnimation;

	private float _timer;

	public void SetTitle(string title)
	{
		_title.fontSize = 19;
		if (_currentPosition == CategoryTabPosition.Center)
		{
			_title.fontSize = 26;
		}
		_title.text = title;
		_tabnameAnimator.Rebind();
		_tabnameAnimator.Play("Idle");
	}

	public void SetTitle(string title, string element, bool isObjectActive)
	{
		SetTitle(title);
		SetElement(element);
		StartActiveAnimation(isObjectActive);
	}

	public void StartActiveAnimation(bool isObjectActive)
	{
		if (isObjectActive)
		{
			_tabnameAnimator.SetTrigger("ActiveTrigger");
		}
	}

	public void SetData(Sprite background, Color color)
	{
		_background.sprite = background;
		Color color4 = (_tabNameShadow.effectColor = (_tabnameOutline.effectColor = color));
	}

	public void SetElement(string element)
	{
	}

	public void Left(Action OnChangeSide)
	{
		_isAnimation = true;
		_title.fontSize = 19;
		switch (_currentPosition)
		{
		case CategoryTabPosition.Center:
			_tabAnimator.SetTrigger("Center2Left");
			_currentPosition = CategoryTabPosition.Left;
			break;
		case CategoryTabPosition.Left:
			_tabAnimator.SetTrigger("Left2LeftOut");
			_currentPosition = CategoryTabPosition.LeftOut;
			break;
		case CategoryTabPosition.Right:
			_tabAnimator.SetTrigger("Right2Center");
			_currentPosition = CategoryTabPosition.Center;
			_title.fontSize = 26;
			break;
		case CategoryTabPosition.RightOut:
			_tabAnimator.SetTrigger("RightOut2Right");
			_currentPosition = CategoryTabPosition.Right;
			break;
		case CategoryTabPosition.RightOver:
			_tabAnimator.SetTrigger("RightOver2RightOut");
			_currentPosition = CategoryTabPosition.RightOut;
			break;
		case CategoryTabPosition.LeftOut:
			_tabAnimator.SetTrigger("LeftOut2LeftOver");
			_currentPosition = CategoryTabPosition.LeftOver;
			break;
		case CategoryTabPosition.LeftOver:
			_tabAnimator.SetTrigger("LeftOver2RightOver");
			_currentPosition = CategoryTabPosition.RightOver;
			OnChangeSide();
			break;
		}
		_tabnameAnimator.SetTrigger("NormalTrigger");
	}

	public void Right(Action OnChangeSide)
	{
		_isAnimation = true;
		_title.fontSize = 19;
		switch (_currentPosition)
		{
		case CategoryTabPosition.Center:
			_tabAnimator.SetTrigger("Center2Right");
			_currentPosition = CategoryTabPosition.Right;
			break;
		case CategoryTabPosition.Left:
			_tabAnimator.SetTrigger("Left2Center");
			_currentPosition = CategoryTabPosition.Center;
			_title.fontSize = 26;
			break;
		case CategoryTabPosition.Right:
			_tabAnimator.SetTrigger("Right2RightOut");
			_currentPosition = CategoryTabPosition.RightOut;
			break;
		case CategoryTabPosition.LeftOut:
			_tabAnimator.SetTrigger("LeftOut2Left");
			_currentPosition = CategoryTabPosition.Left;
			break;
		case CategoryTabPosition.RightOut:
			_tabAnimator.SetTrigger("RightOut2RightOver");
			_currentPosition = CategoryTabPosition.RightOver;
			break;
		case CategoryTabPosition.LeftOver:
			_tabAnimator.SetTrigger("LeftOver2LeftOut");
			_currentPosition = CategoryTabPosition.LeftOut;
			break;
		case CategoryTabPosition.RightOver:
			_tabAnimator.SetTrigger("RightOver2LeftOver");
			_currentPosition = CategoryTabPosition.LeftOver;
			OnChangeSide();
			break;
		}
		_tabnameAnimator.SetTrigger("NormalTrigger");
	}

	public void ViewUpdate()
	{
		if (_isAnimation)
		{
			_timer += GameManager.GetGameMSecAdd();
			if (_timer >= 150f)
			{
				_timer = 0f;
				_tabnameAnimator.SetTrigger("ActiveTrigger");
				_isAnimation = false;
			}
		}
	}
}
