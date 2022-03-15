using DB;
using MAI2.Util;
using Manager;

namespace IO
{
	public class CoinBlocker
	{
		public enum Mode
		{
			Initialize,
			Game,
			TestMode,
			Balance
		}

		private Mode _mode;

		private bool _isBlockedInTestMode;

		public Mode mode
		{
			get
			{
				return _mode;
			}
			set
			{
				_mode = value;
			}
		}

		public bool isBlockedInTestMode
		{
			get
			{
				return _isBlockedInTestMode;
			}
			set
			{
				_isBlockedInTestMode = value;
			}
		}

		public void Initialize()
		{
		}

		public void Terminate()
		{
		}

		public void Execute()
		{
			Update();
		}

		private void Update()
		{
			bool flag = true;
			switch (_mode)
			{
			case Mode.Game:
				flag = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.AddableCoin <= 1 || !Singleton<OperationManager>.Instance.IsCoinAcceptable();
				break;
			case Mode.TestMode:
				flag = _isBlockedInTestMode;
				break;
			case Mode.Balance:
			{
				int num = 9;
				int num2 = num;
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit > 1)
				{
					num2 = num * SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit;
				}
				flag = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.AddableCoin <= num2 || !Singleton<OperationManager>.Instance.IsCoinAcceptable();
				break;
			}
			}
			MechaManager.Jvs.SetOutput(JvsOutputID.coin_block, !flag);
		}
	}
}
