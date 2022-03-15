using System.Collections.Generic;
using UnityEngine;

namespace Monitor.Game
{
	public class GameSpritePreLoader : MonoBehaviour
	{
		private bool _renderOnce;

		private int _counter;

		private void Awake()
		{
			List<string> list = new List<string> { "Process/Game/Sprites", "Textures/hy", "Textures/me" };
			Vector2 pivot = new Vector2(0.5f, 0.5f);
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				Object[] array = Resources.LoadAll(list[i], typeof(Texture2D));
				for (int j = 0; j < array.Length; j++)
				{
					Texture2D texture2D = (Texture2D)array[j];
					GameObject obj = new GameObject();
					Rect rect = new Rect(0f, 0f, 8f, 8f);
					Sprite sprite = Sprite.Create(texture2D, rect, pivot, 1f);
					sprite.name = texture2D.name;
					obj.AddComponent<SpriteRenderer>();
					obj.GetComponent<SpriteRenderer>().sprite = sprite;
					obj.transform.SetParent(base.transform);
					int num2 = num * 8 / 1008;
					obj.transform.localPosition = new Vector3(num * 8 - 504 - 1008 * num2, -8 * num2, 0f);
					obj.name = sprite.name;
					num++;
				}
			}
			base.transform.localPosition = new Vector3(0f, 900f, 0f);
		}

		private void OnGUI()
		{
			if (!_renderOnce)
			{
				_counter++;
				if (_counter >= 2)
				{
					_renderOnce = true;
					base.gameObject.SetActive(value: false);
				}
			}
		}
	}
}
