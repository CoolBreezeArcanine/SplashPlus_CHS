using DB;
using UnityEngine;

namespace Process
{
	public interface IPhotoEditProcess
	{
		PhotoeditSettingID GetCurrentSettingIndex(int playerIndex);

		Rect[] GetCurrentFaceRects(int playerIndex);

		Sprite GetSettingNameSprite(int playerIndex, int diff);

		int GetSettingSwitchValue(int playerIndex, int diff, out string switchName);

		void IsCheckCategory(int playerIndex, int diff, out bool isLeftActive, out bool isRightActive);

		bool GetUploaded(int playerIndex);
	}
}
