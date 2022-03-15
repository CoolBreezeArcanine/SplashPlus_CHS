using UI;
using UnityEngine;

namespace Monitor.MusicSelect.UI
{
	public class MultipleColors : MonoBehaviour
	{
		[SerializeField]
		private int _index;

		[SerializeField]
		private Color _basecolor = Color.white;

		[SerializeField]
		private MultipleImage[] _images;

		[SerializeField]
		private MultipleImage[] _addImages;

		public void ChangeStar(int type)
		{
			if (type >= 0 && 2 >= type && _images != null)
			{
				MultipleImage[] images = _images;
				for (int i = 0; i < images.Length; i++)
				{
					images[i].ChangeSprite(type);
				}
				images = _addImages;
				for (int i = 0; i < images.Length; i++)
				{
					images[i].ChangeSprite(type);
				}
			}
		}

		private void UpdateImages()
		{
			if (_index < 0 || 2 < _index)
			{
				return;
			}
			MultipleImage[] images;
			if (_images != null)
			{
				images = _images;
				foreach (MultipleImage multipleImage in images)
				{
					if (multipleImage != null)
					{
						multipleImage.color = _basecolor;
						multipleImage.ChangeSprite(_index);
					}
				}
			}
			if (_addImages == null)
			{
				return;
			}
			images = _addImages;
			foreach (MultipleImage multipleImage2 in images)
			{
				if (multipleImage2 != null)
				{
					multipleImage2.ChangeSprite(_index);
				}
			}
		}
	}
}
