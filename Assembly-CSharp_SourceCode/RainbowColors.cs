using UnityEngine;
using UnityEngine.UI;

public class RainbowColors : MonoBehaviour
{
	[SerializeField]
	private bool _playOnAwake = true;

	[SerializeField]
	private Image[] _rainbowImages = new Image[7];

	[SerializeField]
	private Color[] _rainbowColors = new Color[7];

	private bool _isAnimation;

	private float _timer;

	private int _count;

	public void Play()
	{
		_isAnimation = true;
	}

	public void Stop()
	{
		_isAnimation = false;
	}

	private void Start()
	{
		_isAnimation = _playOnAwake;
	}

	private void Update()
	{
		if (!_isAnimation)
		{
			return;
		}
		_timer += Time.deltaTime;
		if (_timer >= 0.1f)
		{
			for (int i = 0; i < _rainbowImages.Length; i++)
			{
				int num = ((i + _count < 7) ? (i + _count) : (i + _count - 7));
				_rainbowImages[i].color = new Color(_rainbowColors[num].r, _rainbowColors[num].g, _rainbowColors[num].b, _rainbowImages[i].color.a);
			}
			if (_count < 6)
			{
				_count++;
			}
			else
			{
				_count = 0;
			}
			_timer = 0f;
		}
	}
}
