using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.Common
{
	public class MaintenanceInformationController : MonoBehaviour
	{
		[SerializeField]
		private List<MaintenanceInformationLayerObject> _layers;

		private Animator _animator;

		private bool _isActivation;

		private int _hashIn;

		private int _hashOut;

		private bool _isOpen;

		public void Initialize()
		{
			_hashIn = Animator.StringToHash("In");
			_hashOut = Animator.StringToHash("Out");
			_animator = GetComponent<Animator>();
			_animator.speed = 0.5f;
			_animator.PlayInFixedTime("Out", -1, float.MaxValue);
			_isActivation = false;
			_isOpen = false;
			_layers[0].Initialize(MaintenanceInfoID.ShowNotice.GetName());
		}

		public void ViewUpdate()
		{
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Accounting.isReporting())
			{
				OpenWatchWindow(MaintenanceInfoID.Reporting);
			}
			else
			{
				CloseWatchWindow();
			}
		}

		public void Activation(bool flag)
		{
			_isActivation = flag;
			if (!_isActivation)
			{
				CloseWatchWindow();
			}
		}

		private void OpenWatchWindow(MaintenanceInfoID type)
		{
			_animator.Play(_hashIn);
			_isOpen = true;
			switch (type)
			{
			case MaintenanceInfoID.ShowNotice:
				_layers[0].SetText(MaintenanceInfoID.ShowNotice.GetName());
				_layers[0].SetParameter(Singleton<OperationManager>.Instance.GetClosingRemainingMinutes());
				_layers[0].Play(_hashIn);
				break;
			case MaintenanceInfoID.RefreshNotice:
				_layers[0].SetText(MaintenanceInfoID.RefreshNotice.GetName());
				_layers[0].SetParameter(Singleton<OperationManager>.Instance.GetSegaBootRemainingMinutes());
				_layers[0].Play(_hashIn);
				break;
			default:
				_layers[0].SetText(type.GetName());
				_layers[0].Play(_hashIn);
				break;
			}
		}

		private void CloseWatchWindow()
		{
			if (_isOpen)
			{
				_animator.Play(_hashOut);
				_isOpen = false;
			}
		}

		public void ForceKill()
		{
			_isOpen = false;
			_animator.PlayInFixedTime("Out", -1, float.MaxValue);
			_isActivation = false;
			_isOpen = false;
		}
	}
}
