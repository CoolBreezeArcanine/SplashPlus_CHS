using UnityEngine;

namespace Monitor.Game
{
	public class GameObjectCtrl : MonoBehaviour
	{
		[SerializeField]
		[Header("画面中央表示")]
		private GameAchiveNum AchiveNumObj;

		[SerializeField]
		[Header("画面上部表示")]
		private GameUpperGaugeCtrl GaugeNumObj;

		[SerializeField]
		[Header("結果表示")]
		private GameResultEffectCtrl ResultEffectObj;

		[SerializeField]
		[Header("写真撮影クマ")]
		private GamePhotoInfoCtrl PhotoKumaObj;

		public GameAchiveNum GetAchiveObj()
		{
			return AchiveNumObj;
		}

		public GameUpperGaugeCtrl GetUpperGaugeNumObj()
		{
			return GaugeNumObj;
		}

		public GameResultEffectCtrl GetResultEffectObj()
		{
			return ResultEffectObj;
		}

		public GamePhotoInfoCtrl GetPhotoKumaObj()
		{
			return PhotoKumaObj;
		}

		public void Initialize(int monitorIndex, int mybest)
		{
			GetAchiveObj().Initialize(monitorIndex, mybest);
			GetUpperGaugeNumObj().Initialize(monitorIndex);
			GetResultEffectObj().Initialize(monitorIndex);
			GetPhotoKumaObj().Initialize();
		}

		public void Execute()
		{
			GetAchiveObj().Execute();
			GetUpperGaugeNumObj().Execute();
			GetResultEffectObj().Execute();
			GetPhotoKumaObj().Execute();
		}

		public void SetForceShutter()
		{
			GetAchiveObj().SetForceShutter();
		}
	}
}
