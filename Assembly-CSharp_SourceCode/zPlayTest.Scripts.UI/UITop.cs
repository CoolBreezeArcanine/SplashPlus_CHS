using Manager.Party.Party;
using UnityEngine;

namespace zPlayTest.Scripts.UI
{
	public class UITop : MonoBehaviour
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
				_rig.SetActive(!Party.Get().IsHost() && !Party.Get().IsClient());
			}
		}

		public void OnStartRecruit()
		{
			_proxy.StartRecruit();
		}
	}
}
