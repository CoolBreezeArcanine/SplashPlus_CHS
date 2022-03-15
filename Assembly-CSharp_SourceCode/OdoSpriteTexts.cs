using Monitor.MapResult.Parts;
using UnityEngine;

public class OdoSpriteTexts : MonoBehaviour
{
	[SerializeField]
	private OdoSpriteText[] _odoSprites;

	public void SetDistance(int distance)
	{
		if (_odoSprites == null)
		{
			return;
		}
		OdoSpriteText[] odoSprites = _odoSprites;
		foreach (OdoSpriteText odoSpriteText in odoSprites)
		{
			if (odoSpriteText != null)
			{
				odoSpriteText.SetOdoText(distance);
			}
		}
	}
}
