using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class ImageColorGroup : MonoBehaviour
	{
		[SerializeField]
		private uint _index;

		[SerializeField]
		private Color[] _colors;

		[SerializeField]
		private Image[] _images;

		private void Start()
		{
			SetImageComponents();
		}

		[ContextMenu("SetImageComponents")]
		public void SetImageComponents()
		{
			_images = GetComponentsInChildren<Image>(includeInactive: true);
			SetColor();
		}

		public void SetColor(int index)
		{
			if (_images != null && _colors != null && _colors.Length != 0 && _index < _colors.Length)
			{
				_index = (uint)index;
				Image[] images = _images;
				foreach (Image obj in images)
				{
					obj.color = _colors[index];
					obj.SetAllDirty();
				}
			}
		}

		public void SetColor(Color color)
		{
			Image[] images = _images;
			foreach (Image obj in images)
			{
				obj.color = color;
				obj.SetAllDirty();
			}
		}

		private void SetColor()
		{
			if (_images != null && _colors != null && _colors.Length != 0 && _index < _colors.Length)
			{
				Image[] images = _images;
				foreach (Image obj in images)
				{
					obj.color = _colors[_index];
					obj.SetAllDirty();
				}
			}
		}
	}
}
