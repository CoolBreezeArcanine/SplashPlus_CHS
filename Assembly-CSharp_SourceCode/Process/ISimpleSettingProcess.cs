using UnityEngine;

namespace Process
{
	public interface ISimpleSettingProcess
	{
		Sprite GetFrameSpriteByAdjustIndex(int monitorIndex, int diff);

		void SetTimerVisible(int playerIndex, bool isVisible);
	}
}
