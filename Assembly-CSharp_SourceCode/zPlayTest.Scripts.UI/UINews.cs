using Manager.Party.Party;
using UnityEngine;

namespace zPlayTest.Scripts.UI
{
	public class UINews : MonoBehaviour
	{
		[SerializeField]
		private GameObject _rig;

		private TestPartyProxy _proxy;

		private void Start()
		{
			_proxy = GetComponentInParent<TestPartyProxy>();
		}

		private void Update()
		{
			if (TestCore.IsReady)
			{
				_rig.SetActive(Party.Get().IsNews());
			}
		}

		public void OnClose()
		{
			_proxy.FinishNews();
		}
	}
}
