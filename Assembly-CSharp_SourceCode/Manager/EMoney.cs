using AMDaemon;
using Mai2.Mai2Cue;
using MAI2.Util;

namespace Manager
{
	public class EMoney
	{
		public struct EConfig
		{
			public int coins_;

			public EMoneyBrandId brand_;

			public EConfig(int coins, EMoneyBrandId brand)
			{
				coins_ = coins;
				brand_ = brand;
			}
		}

		public enum Operation
		{
			None,
			Pay,
			Balance,
			Cancel,
			CheckDisplay
		}

		public enum Status
		{
			Idle,
			Waiting,
			Operating,
			CheckDisplay
		}

		private enum State
		{
			Init,
			Idle,
			PayToCoin,
			RequestBalance,
			Cancel,
			Busy,
			WaitReporting,
			Error,
			CheckDisplay,
			CheckDisplayProc
		}

		public const float WaitCancelTime = 2f;

		public const float WaitReportingTime = 5f;

		private Mode<EMoney, State> mode_;

		private Operation operation_;

		private int numBrands_;

		public bool IsAvailable => AMDaemon.EMoney.IsAuthCompleted;

		public bool CanOperateDeal => AMDaemon.EMoney.Operation.CanOperateDeal;

		public bool IsDealAvailable => AMDaemon.EMoney.Operation.IsDealAvailable;

		public bool IsCancellable => AMDaemon.EMoney.Operation.IsCancellable;

		public bool IsBusy => AMDaemon.EMoney.Operation.IsBusy;

		public bool IsInOperating
		{
			get
			{
				State state = (State)mode_.get();
				if (state == State.Idle || state == State.Error)
				{
					return false;
				}
				return true;
			}
		}

		public bool IsInCanceling
		{
			get
			{
				State state = (State)mode_.get();
				if (state == State.Cancel)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsInCheckingDisplay
		{
			get
			{
				State state = (State)mode_.get();
				if (state == State.CheckDisplay)
				{
					return true;
				}
				return false;
			}
		}

		public int NumBrands => numBrands_;

		public EConfig Config { get; set; }

		public bool IsAvalilable(EMoneyBrandId brand)
		{
			if (AMDaemon.EMoney.AvailableBrandCount <= 0)
			{
				return false;
			}
			foreach (EMoneyBrand availableBrand in AMDaemon.EMoney.AvailableBrands)
			{
				if (availableBrand.Id == brand)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsAvalilableBalance(EMoneyBrandId brand)
		{
			if (AMDaemon.EMoney.AvailableBrandCount <= 0)
			{
				return false;
			}
			foreach (EMoneyBrand availableBrand in AMDaemon.EMoney.AvailableBrands)
			{
				if (availableBrand.Id == brand && availableBrand.HasBalance)
				{
					return true;
				}
			}
			return false;
		}

		public Status GetStatus()
		{
			State state = (State)mode_.get();
			if ((uint)(state - 2) <= 3u)
			{
				if (!AMDaemon.EMoney.Operation.IsBusy)
				{
					return Status.Waiting;
				}
				return Status.Operating;
			}
			if (operation_ != 0)
			{
				return Status.Waiting;
			}
			return Status.Idle;
		}

		public void Initialize()
		{
			mode_ = new Mode<EMoney, State>(this);
			InitConfig();
			AMDaemon.EMoney.SoundHook = onSound;
		}

		public void ResetDefault()
		{
			RequestCancel();
			InitConfig();
		}

		public void InitConfig()
		{
			numBrands_ = AMDaemon.EMoney.AvailableBrandCount;
			if (numBrands_ <= 0)
			{
				Config = new EConfig(0, EMoneyBrandId.Nanaco);
			}
			else
			{
				Config = new EConfig(0, AMDaemon.EMoney.AvailableBrands[0].Id);
			}
		}

		public void UpdateBrands()
		{
			int availableBrandCount = AMDaemon.EMoney.AvailableBrandCount;
			if (availableBrandCount != numBrands_)
			{
				numBrands_ = availableBrandCount;
			}
		}

		public void Update()
		{
			if (mode_ != null)
			{
				mode_.update();
			}
			UpdateBrands();
		}

		public bool Pay()
		{
			if (!IsAvailable)
			{
				return false;
			}
			if (operation_ != 0)
			{
				return false;
			}
			operation_ = Operation.Pay;
			return true;
		}

		public bool RequestBalance()
		{
			if (!IsAvailable)
			{
				return false;
			}
			if (operation_ != 0)
			{
				return false;
			}
			operation_ = Operation.Balance;
			return true;
		}

		public void RequestCancel()
		{
			if (IsAvailable && IsCancellable)
			{
				operation_ = Operation.Cancel;
				if (4 == mode_.get())
				{
					Cancel_Init();
				}
				else
				{
					mode_.set(State.Cancel);
				}
			}
		}

		public bool Cancel()
		{
			EMoneyOperation operation = AMDaemon.EMoney.Operation;
			if (operation.IsCancellable && operation.Cancel())
			{
				operation_ = Operation.Cancel;
				mode_.set(State.Busy);
				return true;
			}
			return false;
		}

		public bool RequestCheckDisplay()
		{
			if (Operation.CheckDisplay == operation_)
			{
				return false;
			}
			operation_ = Operation.CheckDisplay;
			mode_.set(State.CheckDisplay);
			return true;
		}

		public bool RequestCancelCheckDisplay()
		{
			if (Operation.CheckDisplay != operation_)
			{
				return false;
			}
			operation_ = Operation.Cancel;
			mode_.set(State.Cancel);
			return true;
		}

		private void Init_Init()
		{
			operation_ = Operation.None;
			if (IsAvailable)
			{
				mode_.set(State.Idle);
			}
			else
			{
				mode_.set(State.Error);
			}
		}

		private void ToIdle()
		{
			operation_ = Operation.None;
			mode_.set(State.Idle);
		}

		private void Idle_Init()
		{
		}

		private void Idle_Proc()
		{
			if (AMDaemon.EMoney.IsReporting)
			{
				mode_.set(State.WaitReporting);
				return;
			}
			switch (operation_)
			{
			case Operation.Pay:
				mode_.set(State.PayToCoin);
				break;
			case Operation.Balance:
				mode_.set(State.RequestBalance);
				break;
			case Operation.Cancel:
				mode_.set(State.Cancel);
				break;
			}
		}

		private void PayToCoin_Init()
		{
			EMoneyOperation operation = AMDaemon.EMoney.Operation;
			if (!operation.CanOperateDeal)
			{
				ToIdle();
				return;
			}
			Credit credit = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit;
			int coins_ = Config.coins_;
			if (credit.AddableCoin < coins_)
			{
				ToIdle();
			}
			else if (!operation.PayToCoin(0, Config.brand_, (uint)coins_))
			{
				ToIdle();
			}
		}

		private void PayToCoin_Proc()
		{
			Busy_Proc();
		}

		private void RequestBalance_Init()
		{
			EMoneyOperation operation = AMDaemon.EMoney.Operation;
			if (!operation.CanOperateDeal)
			{
				ToIdle();
			}
			else if (!operation.RequestBalance(Config.brand_))
			{
				ToIdle();
			}
		}

		private void RequestBalance_Proc()
		{
			Busy_Proc();
		}

		private void Cancel_Init()
		{
			EMoneyOperation operation = AMDaemon.EMoney.Operation;
			if (!operation.IsCancellable)
			{
				mode_.set(State.Busy);
			}
			else if (!operation.Cancel())
			{
				mode_.set(State.Busy);
			}
		}

		private void Cancel_Proc()
		{
			if (!AMDaemon.EMoney.Operation.IsBusy)
			{
				ToIdle();
			}
		}

		private void Busy_Proc()
		{
			EMoneyOperation operation = AMDaemon.EMoney.Operation;
			if (!operation.IsBusy)
			{
				if (operation.IsErrorOccurred)
				{
					mode_.set(State.Error);
				}
				else
				{
					ToIdle();
				}
			}
		}

		private void WaitReporting_Proc()
		{
			if (!AMDaemon.EMoney.IsReporting)
			{
				switch (operation_)
				{
				case Operation.None:
					ToIdle();
					break;
				case Operation.Pay:
					mode_.set(State.PayToCoin);
					break;
				case Operation.Balance:
					mode_.set(State.RequestBalance);
					break;
				case Operation.Cancel:
					mode_.set(State.Cancel);
					break;
				}
			}
		}

		private void CheckDisplay_Proc()
		{
			EMoneyOperation operation = AMDaemon.EMoney.Operation;
			if (!operation.IsBusy)
			{
				if (operation.CheckDisplay())
				{
					mode_.set(State.CheckDisplayProc);
				}
			}
			else if (operation.IsCancellable)
			{
				operation.Cancel();
			}
		}

		private void CheckDisplayProc_Proc()
		{
			if (!AMDaemon.EMoney.Operation.IsBusy)
			{
				mode_.set(State.CheckDisplay);
			}
		}

		private void Error_Init()
		{
			_ = AMDaemon.EMoney.Operation.HasResult;
		}

		private void Error_Proc()
		{
			if (IsAvailable)
			{
				ToIdle();
			}
		}

		private void onSound(EMoneySound emoneySound)
		{
			if (!string.IsNullOrEmpty(emoneySound.Id))
			{
				Cue cueIndex;
				switch (emoneySound.Id)
				{
				default:
					return;
				case "0001-02-00":
					cueIndex = Cue.SE_EMONEY_0001_02_00;
					break;
				case "0001-02-01":
					cueIndex = Cue.SE_EMONEY_0001_02_01;
					break;
				case "0001-02-02":
					cueIndex = Cue.SE_EMONEY_0001_02_02;
					break;
				case "0002-02-00":
					cueIndex = Cue.SE_EMONEY_0002_02_00;
					break;
				case "0002-02-01":
					cueIndex = Cue.SE_EMONEY_0002_02_01;
					break;
				case "0002-02-02":
					cueIndex = Cue.SE_EMONEY_0002_02_02;
					break;
				case "0003-02-00":
					cueIndex = Cue.SE_EMONEY_0003_02_00;
					break;
				case "0003-02-01":
					cueIndex = Cue.SE_EMONEY_0003_02_01;
					break;
				case "0003-02-02":
					cueIndex = Cue.SE_EMONEY_0003_02_02;
					break;
				case "0003-02-03":
					cueIndex = Cue.SE_EMONEY_0003_02_03;
					break;
				case "0005-02-00":
					cueIndex = Cue.SE_EMONEY_0005_02_00;
					break;
				case "0005-02-01":
					cueIndex = Cue.SE_EMONEY_0005_02_01;
					break;
				case "0005-02-03":
					cueIndex = Cue.SE_EMONEY_0005_02_03;
					break;
				case "0006-02-00":
					cueIndex = Cue.SE_EMONEY_0006_02_00;
					break;
				case "0006-02-01":
					cueIndex = Cue.SE_EMONEY_0006_02_01;
					break;
				case "0007-02-01":
					cueIndex = Cue.SE_EMONEY_0007_02_01;
					break;
				case "0008-02-00":
					cueIndex = Cue.SE_EMONEY_0008_02_00;
					break;
				case "0008-02-01":
					cueIndex = Cue.SE_EMONEY_0008_02_01;
					break;
				case "9999-02-01":
					cueIndex = Cue.SE_EMONEY_9999_02_01;
					break;
				case "9999-02-99":
					cueIndex = Cue.SE_EMONEY_9999_02_99;
					break;
				}
				SoundManager.PlaySystemSE(cueIndex);
			}
		}
	}
}
