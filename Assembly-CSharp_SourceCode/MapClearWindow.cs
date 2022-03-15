using System;
using UnityEngine;
using UnityEngine.UI;

public class MapClearWindow : EventWindowBase
{
	[SerializeField]
	private Image _main;

	public void Set(Sprite main)
	{
		_main.sprite = main;
	}

	public override void Play(Action onAction)
	{
		int layerIndex = _animator.GetLayerIndex("Rotation");
		_animator.SetLayerWeight(layerIndex, 1f);
		_animator.Play("Star_Rotation", layerIndex, 0f);
		layerIndex = _animator.GetLayerIndex("Star");
		_animator.SetLayerWeight(layerIndex, 1f);
		_animator.Play("Star", layerIndex, 0f);
		base.Play(onAction);
	}
}
