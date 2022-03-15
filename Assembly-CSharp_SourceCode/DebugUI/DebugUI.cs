using UnityEngine;

namespace DebugUI
{
	public class DebugUI : MonoBehaviour
	{
		public const int TypeTabPanel = 0;

		public const int TypePanel = 1;

		public const int TypeButton = 2;

		public const int TypeSelectionGrid = 3;

		public const int TypeScrollText = 4;

		public const int TypeSlider = 5;

		public const int TypeToggle = 6;

		public const int TypeUserDraw = 7;

		public const int TypeWindow = 8;

		public const int TypeContextMenu = 9;

		public const int TypeMenuItem = 10;

		public static DebugUI instance => null;
	}
}
