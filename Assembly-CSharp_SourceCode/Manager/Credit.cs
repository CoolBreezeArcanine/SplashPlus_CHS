using AMDaemon;
using Mai2.Mai2Cue;
using MAI2.Util;

namespace Manager
{
	public class Credit
	{
		public delegate void OnUpdate(int oldValue, int nowValue);

		public delegate void OnCoinInDelegate();

		public enum GameCost
		{
			Credit1,
			Credit2,
			Freedom,
			End
		}

		private int _prevCredit;

		private bool _addCoinNow;

		private readonly GameCost[] _paydCost = new GameCost[2];

		public int AddableCoin => (int)AMDaemon.Credit.Players[0].AddableCoin;

		public int NowCredit => (int)AMDaemon.Credit.Players[0].Credit;

		public int Remain => (int)AMDaemon.Credit.Players[0].Remain;

		public int CoinToCredit => (int)AMDaemon.Credit.Players[0].CoinToCredit;

		public GameCost NowCoinCost { get; set; }

		public OnUpdate OnUpdateCredit { get; set; }

		public OnCoinInDelegate OnCoinIn { get; set; }

		public void Initialize()
		{
			_prevCredit = NowCredit;
			AMDaemon.Credit.CoinInHook = OnCoinInHook;
		}

		public void Execute()
		{
			CheckUpdate();
		}

		public void Terminate()
		{
			AMDaemon.Credit.CoinInHook = null;
		}

		public void ClearPaydCost()
		{
			for (int i = 0; i < _paydCost.Length; i++)
			{
				_paydCost[i] = GameCost.End;
			}
		}

		public GameCost GetPaydCost(int i)
		{
			return _paydCost[i];
		}

		public void SendAimeLog(AimeId aimeId, AimeLogStatus logStatus, int gameCostIndex)
		{
			Aime.SendLog(aimeId, logStatus, gameCostIndex);
		}

		public void SendAimeLog(AimeId aimeId, AimeLogStatus logStatus)
		{
			Aime.SendLog(aimeId, logStatus);
		}

		public bool IsGameCostEnough()
		{
			return AMDaemon.Credit.Players[0].IsGameCostEnough((int)NowCoinCost);
		}

		public bool IsGameCostEnoughFreedom()
		{
			return AMDaemon.Credit.Players[0].IsGameCostEnough(2);
		}

		public bool PayGameCost(int index)
		{
			if (!IsFreePlay())
			{
				bool num = AMDaemon.Credit.Players[0].PayGameCost((int)NowCoinCost);
				CheckUpdate();
				if (num)
				{
					if (NowCoinCost == GameCost.Credit1)
					{
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCredit1P((uint)GameCostEnoughPlay());
					}
					else
					{
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCredit2P((uint)GameCostEnoughPlay());
					}
					_paydCost[index] = NowCoinCost;
					NowCoinCost++;
				}
				return num;
			}
			return true;
		}

		public bool PayGameCostFreedom(int index, bool bookkeep = true)
		{
			if (!IsFreePlay())
			{
				bool num = AMDaemon.Credit.Players[0].PayGameCost(2);
				CheckUpdate();
				if (num)
				{
					if (bookkeep)
					{
						SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.addCreditFreedom((uint)GameCostEnoughFreedom());
					}
					_paydCost[index] = GameCost.Freedom;
				}
				return num;
			}
			return true;
		}

		public bool PayItemCost(int index, bool bookkeep = true)
		{
			if (!IsFreePlay())
			{
				bool flag = index switch
				{
					0 => AMDaemon.Credit.Players[0].PayGameCost(0), 
					1 => AMDaemon.Credit.Players[0].PayGameCost(1), 
					_ => AMDaemon.Credit.Players[0].PayGameCost(0), 
				};
				CheckUpdate();
				if (flag && bookkeep)
				{
					switch (index)
					{
					case 0:
						_paydCost[index] = GameCost.Credit1;
						break;
					case 1:
						_paydCost[index] = GameCost.Credit2;
						break;
					default:
						_paydCost[index] = GameCost.Credit1;
						break;
					}
				}
				return flag;
			}
			return true;
		}

		public void SetCoinCostInit()
		{
			NowCoinCost = GameCost.Credit1;
		}

		public bool IsFreePlay()
		{
			return AMDaemon.Credit.Players[0].IsFreePlay;
		}

		public int GameCostEnoughFreedom()
		{
			return (int)AMDaemon.Credit.Players[0].GameCosts[2];
		}

		public int GameCostEnoughPlay()
		{
			return (int)AMDaemon.Credit.Players[0].GameCosts[(int)NowCoinCost];
		}

		public bool IsZero()
		{
			return AMDaemon.Credit.Players[0].IsZero;
		}

		public bool IsAddCoinNow()
		{
			return _addCoinNow;
		}

		private void CheckUpdate()
		{
			if (_prevCredit != NowCredit)
			{
				OnUpdateCredit?.Invoke(_prevCredit, NowCredit);
				_prevCredit = NowCredit;
				_addCoinNow = true;
			}
			else
			{
				_addCoinNow = false;
			}
		}

		private bool OnCoinInHook(CreditSound creditSound)
		{
			OnCoinIn?.Invoke();
			if (SoundManager.IsInitialized())
			{
				switch (creditSound)
				{
				case CreditSound.Coin:
					SoundManager.PlaySystemSE(Cue.SE_SYS_COIN);
					break;
				case CreditSound.Credit:
					SoundManager.PlaySystemSE(Cue.SE_SYS_CREDIT);
					break;
				}
			}
			return true;
		}
	}
}
