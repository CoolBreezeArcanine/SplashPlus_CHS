using Manager.MaiStudio;
using TMPro;
using UnityEngine;

namespace Monitor.MovieCheck
{
	public class MovieCheckMonitor : MonitorBase
	{
		[SerializeField]
		private TMP_Dropdown dropDown;

		[SerializeField]
		private TextMeshProUGUI time;

		[SerializeField]
		private TextMeshProUGUI stat;

		[SerializeField]
		private TextMeshProUGUI mTime;

		[SerializeField]
		private TextMeshProUGUI sTime;

		[SerializeField]
		[Header("ムービーマスク")]
		private SpriteRenderer _movieSprite;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			if (monIndex == 0)
			{
				dropDown.gameObject.SetActive(value: false);
			}
			else
			{
				_movieSprite.gameObject.SetActive(value: false);
			}
		}

		public override void ViewUpdate()
		{
		}

		public void AddDropDown(int musicID, MusicData music)
		{
			dropDown.options.Add(new TMP_Dropdown.OptionData(musicID.ToString().PadLeft(5, '0') + ":" + music?.name.str));
		}

		public int GetSelectDropDown()
		{
			return int.Parse(dropDown.options[dropDown.value].text.Substring(0, 5));
		}

		public void SetMovieSize(uint height, uint width)
		{
			_movieSprite.size = new Vector2(width, height);
		}

		public void SetStat(string text)
		{
			stat.text = text;
		}

		public void SetTime(string text)
		{
			time.text = text;
		}

		public void SetDebugTime(long m, ulong s)
		{
			mTime.text = m.ToString();
			sTime.text = s.ToString();
		}
	}
}
