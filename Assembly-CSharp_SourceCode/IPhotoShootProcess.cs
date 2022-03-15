using UnityEngine;

public interface IPhotoShootProcess
{
	Sprite GetFrameSpriteByAdjustIndex(int playerIndex, int diff);
}
