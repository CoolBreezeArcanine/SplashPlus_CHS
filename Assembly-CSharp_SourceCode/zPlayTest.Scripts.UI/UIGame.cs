using Manager.Party.Party;
using UnityEngine;

namespace zPlayTest.Scripts.UI
{
	public class UIGame : MonoBehaviour
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
				_rig.SetActive(Party.Get().IsPlay());
			}
		}

		public void OnFinishPlay()
		{
			_proxy.FinishPlay();
		}
	}
}
