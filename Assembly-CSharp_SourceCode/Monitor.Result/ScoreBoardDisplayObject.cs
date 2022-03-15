using DB;
using Timeline;
using TMPro;
using UI;
using UnityEngine;

namespace Monitor.Result
{
	public class ScoreBoardDisplayObject : MonoBehaviour
	{
		[SerializeField]
		[Header("詳細スコア")]
		private TextMeshProUGUI _criticalText;

		[SerializeField]
		private TextMeshProUGUI _perfectText;

		[SerializeField]
		private TextMeshProUGUI _greatText;

		[SerializeField]
		private TextMeshProUGUI _goodText;

		[SerializeField]
		private TextMeshProUGUI _missText;

		[SerializeField]
		[Header("でらっくスコアスター")]
		private Animator _dxStarsAnimator;

		[SerializeField]
		[Header("でらっくスコア")]
		[Tooltip("DXスコア数値")]
		private TextMeshProCounterObject _dxScoreCounter;

		[SerializeField]
		[Header("でらっくスコア最大数")]
		private TextMeshProUGUI _dxMaxScoreCounter;

		[SerializeField]
		[Header("でらっくスコア増減値")]
		private TextMeshProCounterObject _dxDiffScoreCounter;

		[SerializeField]
		[Header("順位")]
		private MultipleImage _rankOrder;

		[SerializeField]
		[Header("シンク数")]
		private TextMeshProCounterObject _syncText;

		[SerializeField]
		private TextMeshProUGUI _syncMaxText;

		[SerializeField]
		[Header("コンボ数")]
		private TextMeshProCounterObject _comboText;

		[SerializeField]
		private TextMeshProUGUI _comboMaxText;

		public void SetActiveCritical(bool isActive)
		{
			_criticalText.transform.parent.gameObject.SetActive(isActive);
		}

		public void SetScoreData(uint critical, uint perfect, uint great, uint good, uint miss)
		{
			_perfectText.text = perfect.ToString();
			_criticalText.text = critical.ToString();
			_greatText.text = great.ToString();
			_goodText.text = good.ToString();
			_missText.text = miss.ToString();
		}

		public void SetRankOrder(int rankIndex)
		{
			_rankOrder.ChangeSprite(rankIndex);
		}

		public void SetCombo(uint currentCombo, uint maxCombo)
		{
			_comboText.SetCountData(0, (int)currentCombo);
			_comboMaxText.text = $"{maxCombo}";
		}

		public void SetSync(uint sync, uint max)
		{
			_syncText.SetCountData(0, (int)sync);
			_syncMaxText.text = $"{max}";
		}

		public void SetDxScore(uint dxScore, int fluctuation, int dxMax, DeluxcorerankrateID dxRank)
		{
			_dxScoreCounter.SetCountData(0, (int)dxScore);
			_dxMaxScoreCounter.text = dxMax.ToString("0");
			_dxDiffScoreCounter.SetColor((fluctuation >= 0) ? CommonScriptable.GetColorSetting().RiseColor : CommonScriptable.GetColorSetting().DeclineColor);
			_dxDiffScoreCounter.SetCountData(0, fluctuation, isZeroPlus: true);
		}

		public void Switch(bool toDetails)
		{
			if (toDetails)
			{
				_dxStarsAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
			}
			_syncText.OnClipTailEnd();
			_comboText.OnClipTailEnd();
			_dxScoreCounter.OnClipTailEnd();
			_dxDiffScoreCounter.OnClipTailEnd();
		}

		public void Skip()
		{
			_syncText.Skip();
			_comboText.Skip();
			_dxScoreCounter.Skip();
			_dxDiffScoreCounter.Skip();
		}
	}
}
