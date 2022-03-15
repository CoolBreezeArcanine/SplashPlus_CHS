using System;
using UnityEngine;

[Serializable]
public class SpriteSheet
{
	[SerializeField]
	private Sprite[] _sheet;

	public Sprite[] Sheet => _sheet;
}
