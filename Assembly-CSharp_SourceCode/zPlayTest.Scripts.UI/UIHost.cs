using Manager.Party.Party;
using UnityEngine;
using UnityEngine.UI;

namespace zPlayTest.Scripts.UI
{
	public class UIHost : MonoBehaviour
	{
		[SerializeField]
		private GameObject _rig;

		[SerializeField]
		private Button _ready;

		private TestPartyProxy _proxy;

		private void Start()
		{
			_proxy = GetComponentInParent<TestPartyProxy>();
		}

		private void Update()
		{
			if (!TestCore.IsReady)
			{
				return;
			}
			if (Party.Get().IsHost() && !Party.Get().IsPlay() && !Party.Get().IsNews())
			{
				if (!_rig.activeSelf)
				{
					_ready.interactable = true;
					_rig.SetActive(value: true);
				}
			}
			else
			{
				_rig.SetActive(value: false);
			}
		}

		public void OnFinishSetting(Button btn)
		{
			btn.interactable = false;
			_proxy.FinishSetting();
		}

		public void OnFinishRecruit()
		{
			if (Party.Get().GetStartOkNumber() >= 1)
			{
				_proxy.FinishRecruit();
			}
		}

		public void OnCancelRecruit()
		{
			_proxy.CancelRecruit();
		}
	}
}
