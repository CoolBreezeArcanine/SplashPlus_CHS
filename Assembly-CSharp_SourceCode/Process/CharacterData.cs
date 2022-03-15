using Manager.MaiStudio;
using Manager.UserDatas;
using UnityEngine;

namespace Process
{
	public class CharacterData
	{
		public int ID;

		public CharaData Data;

		public UserChara UserChara;

		public Texture2D Texture;

		public Color Color;

		public Color ShadowColor;

		public static CharacterData Empty()
		{
			UserChara userChara = new UserChara(0)
			{
				Level = 0u
			};
			return new CharacterData
			{
				UserChara = userChara,
				Color = Color.white,
				ShadowColor = Color.clear
			};
		}
	}
}
