using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : CommonButtonObject
{
	[SerializeField]
	[Header("メニューボタンパーツ")]
	private Image buttonImage;

	[SerializeField]
	private Image outlineImage;

	[SerializeField]
	private Sprite[] buttonImages;

	private bool _isButtonActive = true;

	private Queue<MenuAnimationData> animationQueue = new Queue<MenuAnimationData>();

	public void SetAnimationData(bool isActive, float relativDistance, float degAngle, AnimationCurve animationCurve, float animationTime, int imageIndex = -1)
	{
		Vector2 anchoredPosition = outlineImage.rectTransform.anchoredPosition;
		float num = Mathf.Sqrt(anchoredPosition.x * anchoredPosition.x + anchoredPosition.y * anchoredPosition.y);
		if (isActive)
		{
			relativDistance *= -1f;
			if (imageIndex >= 0)
			{
				Sprite sprite3 = (buttonImage.sprite = (outlineImage.sprite = buttonImages[imageIndex]));
				imageIndex = -1;
			}
		}
		float x = (num + relativDistance) * Mathf.Cos(degAngle * ((float)Math.PI / 180f));
		float y = (num + relativDistance) * Mathf.Sin(degAngle * ((float)Math.PI / 180f));
		_isButtonActive = relativDistance < 0f;
		animationQueue.Enqueue(new MenuAnimationData
		{
			basePosition = anchoredPosition,
			toPosition = new Vector2(x, y),
			curve = animationCurve,
			duration = animationTime,
			nextImageIndex = imageIndex
		});
	}

	public void SetChangeImageAnimationData(float distance, float degAngle, AnimationCurve animationCurve, float animationTime, int imageIndex)
	{
		if (_isButtonActive)
		{
			Vector2 anchoredPosition = outlineImage.rectTransform.anchoredPosition;
			float num = Mathf.Sqrt(anchoredPosition.x * anchoredPosition.x + anchoredPosition.y * anchoredPosition.y);
			float x = (num + distance) * Mathf.Cos(degAngle * ((float)Math.PI / 180f));
			float y = (num + distance) * Mathf.Sin(degAngle * ((float)Math.PI / 180f));
			animationQueue.Enqueue(new MenuAnimationData
			{
				basePosition = outlineImage.rectTransform.anchoredPosition,
				toPosition = new Vector2(x, y),
				curve = animationCurve,
				duration = animationTime,
				nextImageIndex = imageIndex
			});
			animationQueue.Enqueue(new MenuAnimationData
			{
				basePosition = new Vector2(x, y),
				toPosition = outlineImage.rectTransform.anchoredPosition,
				curve = animationCurve,
				duration = animationTime
			});
		}
		else
		{
			distance *= -1f;
			_ = outlineImage.rectTransform.anchoredPosition;
			animationQueue.Enqueue(new MenuAnimationData
			{
				basePosition = outlineImage.rectTransform.anchoredPosition,
				curve = animationCurve,
				duration = animationTime
			});
			Sprite sprite3 = (buttonImage.sprite = (outlineImage.sprite = buttonImages[imageIndex]));
			_isButtonActive = true;
		}
	}

	public void ViewUpdate()
	{
		if (animationQueue.Count <= 0)
		{
			return;
		}
		Vector2 pos = default(Vector2);
		MenuAnimationData menuAnimationData = animationQueue.Peek();
		if (menuAnimationData.Update(out pos))
		{
			if (menuAnimationData.nextImageIndex >= 0)
			{
				Sprite sprite3 = (buttonImage.sprite = (outlineImage.sprite = buttonImages[menuAnimationData.nextImageIndex]));
			}
			animationQueue.Dequeue();
		}
		outlineImage.rectTransform.anchoredPosition = pos;
	}
}
