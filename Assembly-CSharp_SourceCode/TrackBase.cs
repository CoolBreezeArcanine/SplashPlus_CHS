using Manager;
using UnityEngine;
using UnityEngine.UI;

public class TrackBase : MonoBehaviour
{
	[SerializeField]
	private SpriteCounter[] _trackDecimal;

	[SerializeField]
	private SpriteCounter[] _trackNumeric;

	[SerializeField]
	private Image _trackSlash;

	public void SetTrack(uint nextTrack)
	{
		SpriteCounter[] trackDecimal = _trackDecimal;
		for (int i = 0; i < trackDecimal.Length; i++)
		{
			trackDecimal[i].gameObject.SetActive(value: true);
		}
		trackDecimal = _trackNumeric;
		for (int i = 0; i < trackDecimal.Length; i++)
		{
			trackDecimal[i].gameObject.SetActive(value: true);
		}
		_trackSlash.gameObject.SetActive(value: true);
		if (GameManager.IsFreedomMode)
		{
			if (nextTrack >= 10)
			{
				_trackDecimal[0].ChangeText((nextTrack / 10u).ToString());
				_trackDecimal[1].ChangeText((nextTrack % 10u).ToString());
			}
			else
			{
				_trackDecimal[0].ChangeText(nextTrack.ToString());
				_trackDecimal[1].gameObject.SetActive(value: false);
			}
			trackDecimal = _trackNumeric;
			for (int i = 0; i < trackDecimal.Length; i++)
			{
				trackDecimal[i].gameObject.SetActive(value: false);
			}
			_trackSlash.gameObject.SetActive(value: false);
			return;
		}
		if (nextTrack >= 10)
		{
			_trackDecimal[0].ChangeText((nextTrack / 10u).ToString());
			_trackDecimal[1].ChangeText((nextTrack % 10u).ToString());
		}
		else
		{
			_trackDecimal[0].ChangeText(nextTrack.ToString());
			_trackDecimal[1].gameObject.SetActive(value: false);
		}
		uint maxTrackCount = GameManager.GetMaxTrackCount();
		if (maxTrackCount >= 10)
		{
			_trackNumeric[0].ChangeText((maxTrackCount / 10u).ToString());
			_trackNumeric[1].ChangeText((maxTrackCount % 10u).ToString());
		}
		else
		{
			_trackNumeric[0].ChangeText(maxTrackCount.ToString());
			_trackNumeric[1].gameObject.SetActive(value: false);
		}
	}
}
