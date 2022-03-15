using UnityEngine;

namespace Monitor.Result
{
	public class ScoreGaugeController : MonoBehaviour
	{
		public enum ScoreGaugeType
		{
			Disable = -1,
			Normal,
			Perfect,
			Critical
		}

		[SerializeField]
		[Header("詳細ゲージスコア")]
		private ScoreGaugeObject _tapGauge;

		[SerializeField]
		private ScoreGaugeObject _holdGauge;

		[SerializeField]
		private ScoreGaugeObject _slideGauge;

		[SerializeField]
		private ScoreGaugeObject _touchGauge;

		[SerializeField]
		private ScoreGaugeObject _breakGauge;

		[SerializeField]
		[Header("ノーマルカラー")]
		private Color _normalBaseColor;

		[SerializeField]
		private Color _normalOutlineColor;

		[SerializeField]
		[Header("パーフェクトカラー")]
		private Color _perfectBaseColor;

		[SerializeField]
		private Color _perfectOutlinColor;

		[SerializeField]
		private Color _perfectDecalColor;

		[SerializeField]
		[Header("クリティカルカラー")]
		private Color _criticalBaseColor;

		[SerializeField]
		private Color _criticalOutlineColor;

		[SerializeField]
		private Color _criticalDecalColor;

		[SerializeField]
		[Header("無効カラー")]
		private Color _disableBaseColor;

		[SerializeField]
		private Color _disableOutlineColor;

		public void SetActiveCriticalScore(bool isActive)
		{
			_tapGauge.SetActiveCriticalPerfectGuage(isActive);
			_holdGauge.SetActiveCriticalPerfectGuage(isActive);
			_slideGauge.SetActiveCriticalPerfectGuage(isActive);
			_touchGauge.SetActiveCriticalPerfectGuage(isActive);
			_breakGauge.SetActiveCriticalPerfectGuage(isActive);
		}

		public void PlayAnimation()
		{
			_tapGauge.Play();
			_holdGauge.Play();
			_slideGauge.Play();
			_touchGauge.Play();
			_breakGauge.Play();
		}

		public void ShowGuages()
		{
			_tapGauge.Show();
			_holdGauge.Show();
			_slideGauge.Show();
			_touchGauge.Show();
			_breakGauge.Show();
		}

		public void ResetGauge()
		{
			_tapGauge.ResetGauge();
			_holdGauge.ResetGauge();
			_slideGauge.ResetGauge();
			_touchGauge.ResetGauge();
			_breakGauge.ResetGauge();
		}

		public void SetScoreGauge(NoteScore.EScoreType type, float perfect, float critical, float great, float good, uint tocalCount)
		{
			ScoreGaugeObject scoreGaugeObject = type switch
			{
				NoteScore.EScoreType.Tap => _tapGauge, 
				NoteScore.EScoreType.Hold => _holdGauge, 
				NoteScore.EScoreType.Slide => _slideGauge, 
				NoteScore.EScoreType.Touch => _touchGauge, 
				NoteScore.EScoreType.Break => _breakGauge, 
				_ => null, 
			};
			if (scoreGaugeObject == null)
			{
				return;
			}
			float num = perfect - critical;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = 1f - (critical + num + great + good) - 0.0001f;
			int[] array = new int[5]
			{
				(int)Mathf.Ceil(critical * 100f),
				(int)Mathf.Ceil(num * 100f),
				(int)Mathf.Ceil(great * 100f),
				(int)Mathf.Ceil(good * 100f),
				(int)Mathf.Ceil(num2 * 100f)
			};
			int num3 = array.Length;
			int num4 = 0;
			for (int i = 0; i < num3; i++)
			{
				if (array[i] < 0)
				{
					array[i] = 0;
				}
				num4 += array[i];
			}
			int num5 = num4 - 100;
			if (num5 < 0)
			{
				num5 = 0;
			}
			int num6 = 0;
			int num7 = -1;
			for (int j = 0; j < num5; j++)
			{
				num6 = 1;
				num7 = -1;
				for (int k = 0; k < num3; k++)
				{
					if (array[k] >= num6)
					{
						num6 = array[k];
						num7 = k;
					}
				}
				if (num7 >= 0)
				{
					array[num7]--;
				}
			}
			critical = (float)array[0] / 100f;
			num = (float)array[1] / 100f;
			great = (float)array[2] / 100f;
			good = (float)array[3] / 100f;
			num2 = (float)array[4] / 100f;
			perfect = num + critical;
			scoreGaugeObject.SetColor(_normalBaseColor, _normalOutlineColor, ScoreGaugeType.Normal);
			if (perfect >= 1f)
			{
				scoreGaugeObject.SetColor(_perfectBaseColor, _perfectOutlinColor, ScoreGaugeType.Perfect);
			}
			if (critical >= 1f)
			{
				scoreGaugeObject.SetColor(_criticalBaseColor, _criticalOutlineColor, ScoreGaugeType.Critical);
			}
			if (tocalCount == 0)
			{
				scoreGaugeObject.SetColor(_disableBaseColor, _disableOutlineColor, ScoreGaugeType.Disable);
			}
			scoreGaugeObject.SetGauge(perfect, critical, great + perfect, good + great + perfect);
			scoreGaugeObject.SetMaxCount(tocalCount.ToString());
		}
	}
}
