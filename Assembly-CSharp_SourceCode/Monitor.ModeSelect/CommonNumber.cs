using UnityEngine;

namespace Monitor.ModeSelect
{
	public class CommonNumber
	{
		public Sprite[] texSign = new Sprite[2];

		public Sprite[] texNumber = new Sprite[10];

		public void Load(string file)
		{
			Sprite[] array = Resources.LoadAll<Sprite>(file);
			texSign[0] = Object.Instantiate(array[10]);
			texSign[1] = Object.Instantiate(array[11]);
			for (int i = 0; i < 10; i++)
			{
				texNumber[i] = Object.Instantiate(array[i]);
			}
		}

		private string GetBasePath()
		{
			return "Common/Sprites/Number";
		}

		public string GetSpritePath(string file)
		{
			return GetBasePath() + "/" + file;
		}
	}
}
