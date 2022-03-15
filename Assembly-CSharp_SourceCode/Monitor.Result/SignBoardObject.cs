using Timeline;
using UI;
using UnityEngine;

namespace Monitor.Result
{
	public class SignBoardObject : MonoBehaviour
	{
		[SerializeField]
		[Header("コンボ")]
		private GameObject _comboObject;

		[SerializeField]
		private CounterObject _comboCounter;

		[SerializeField]
		private GuageObject _comboGuage;

		[SerializeField]
		[Header("シンク")]
		private GameObject _syncObject;

		[SerializeField]
		private CounterObject _syncCounter;

		[SerializeField]
		private GuageObject _sync1Guage;

		[SerializeField]
		private GuageObject _sync2Guage;

		[SerializeField]
		private MultipleImage _maxTextImage;

		[SerializeField]
		private MultipleImage _maxEffectImage;

		[SerializeField]
		private Transform _centerCounterParent;

		public CounterObject ComboCounter => _comboCounter;

		public CounterObject SyncCounter => _syncCounter;

		public GuageObject ComboGuage => _comboGuage;

		public GuageObject SyncGuage1 => _sync1Guage;

		public GuageObject SyncGuage2 => _sync2Guage;

		public Transform CenterCounterParent => _centerCounterParent;

		public void SetCombo(uint combo, float guageRate)
		{
			_comboObject.SetActive(value: true);
			_syncObject.SetActive(value: false);
			_comboCounter.SetCountData(0u, combo);
			_comboGuage.SetCountData(0f, guageRate);
		}

		public void SetSync(uint sync, float guageRate1, float guageRate2)
		{
			_comboObject.SetActive(value: false);
			_syncObject.SetActive(value: true);
			if (guageRate1 > 0.5f)
			{
				guageRate1 = 0.5f;
			}
			if (guageRate2 > 0.5f)
			{
				guageRate2 = 0.5f;
			}
			_syncCounter.SetCountData(0u, sync);
			_sync1Guage.SetCountData(0f, guageRate1);
			_sync2Guage.SetCountData(0f, guageRate2);
		}

		public void Skip()
		{
			_comboCounter.Skip();
			_syncCounter.Skip();
		}
	}
}
