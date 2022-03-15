using AMDaemon;
using DB;
using MAI2.Util;
using Manager;
using TMPro;
using UnityEngine;

namespace Monitor
{
	public class CreditController : MonoBehaviour
	{
		private readonly string _freePlayText = CommonMessageID.CommonFreePlay.GetName();

		private readonly string CreditText = CommonMessageID.CommonCredits.GetName();

		[SerializeField]
		private GameObject _creditRootObject;

		[SerializeField]
		private GameObject _creditSubObject;

		[SerializeField]
		private TextMeshProUGUI _creditText;

		private bool _freePlayMode;

		private bool _isDispOn;

		private int creditNum = -1;

		private int moleNum = -1;

		private int denomiNum = -1;

		public void Initialize(int monitorIndex, bool isActive)
		{
			SetFreePlayMode(SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay());
			SetCredits(SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit, SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain, SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit);
		}

		public void ViewUpdate()
		{
			if (Sequence.IsTest)
			{
				DispOn(flag: false);
				return;
			}
			DispOn(flag: true);
			SetCredits(SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit, SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain, SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.CoinToCredit);
		}

		public void SetFreePlayMode(bool flag)
		{
			_freePlayMode = flag;
			if (_freePlayMode)
			{
				_creditText.text = _freePlayText;
			}
		}

		public void SetCredits(int num, int mole, int denomi)
		{
			if ((creditNum == num && moleNum == mole && denomiNum == denomi) || _freePlayMode)
			{
				return;
			}
			creditNum = num;
			denomiNum = denomi;
			moleNum = mole;
			StringEx.ClearString();
			StringEx.AddString(CreditText);
			if (denomiNum > 1 && creditNum == 0)
			{
				StringEx.AddString(mole.ToString());
				StringEx.AddString("/");
				StringEx.AddString(denomi.ToString());
			}
			else
			{
				StringEx.AddString(num.ToString());
				if (mole != 0)
				{
					StringEx.AddString(" ");
					StringEx.AddString(mole.ToString());
					StringEx.AddString("/");
					StringEx.AddString(denomi.ToString());
				}
			}
			_creditText.text = StringEx.GetString();
		}

		public void DispOn(bool flag)
		{
			if (_isDispOn != flag)
			{
				_creditRootObject.SetActive(flag);
				_isDispOn = flag;
			}
		}

		public void SwitchMainMon(bool flag)
		{
			if (_creditRootObject != null && !_creditRootObject.transform.Equals(flag ? base.transform : _creditSubObject.transform))
			{
				_creditRootObject.transform.SetParent(flag ? base.transform : _creditSubObject.transform, worldPositionStays: false);
			}
		}
	}
}
