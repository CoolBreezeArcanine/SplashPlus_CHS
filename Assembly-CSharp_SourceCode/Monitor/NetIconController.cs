using AMDaemon;
using MAI2.Util;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor
{
	public class NetIconController : MonoBehaviour
	{
		[SerializeField]
		private Sprite _onSprite;

		[SerializeField]
		private Sprite _offSprite;

		[SerializeField]
		private Sprite _warrnSprite;

		[SerializeField]
		private Image _netIcon;

		private OperationManager.NetStatus _iconStatus;

		private readonly Color _dispColor = new Color(1f, 1f, 1f, 1f);

		private readonly Color _hideColor = new Color(1f, 1f, 1f, 0f);

		public void Initialize(int monitorIndex, bool isActive)
		{
		}

		public void ViewUpdate()
		{
			if (Sequence.IsTest)
			{
				DispOn(flag: false);
				return;
			}
			DispOn(flag: true);
			SetIconStatus(Singleton<OperationManager>.Instance.NetIconStatus);
		}

		public void SetIconStatus(OperationManager.NetStatus status)
		{
			_iconStatus = status;
			_netIcon.color = _dispColor;
			switch (_iconStatus)
			{
			case OperationManager.NetStatus.Online:
				_netIcon.sprite = _onSprite;
				break;
			case OperationManager.NetStatus.Offline:
				_netIcon.sprite = _offSprite;
				break;
			case OperationManager.NetStatus.LimitedOnline:
				_netIcon.sprite = _warrnSprite;
				break;
			default:
				_netIcon.color = _hideColor;
				break;
			}
		}

		public void DispOn(bool flag)
		{
			_netIcon.gameObject.SetActive(flag);
		}
	}
}
