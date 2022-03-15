using System;
using Manager.Party.Party;
using UnityEngine;
using UnityEngine.UI;

namespace zPlayTest.Scripts.UI
{
	public class UIClient : MonoBehaviour
	{
		[SerializeField]
		private GameObject _rig;

		[SerializeField]
		private Button _ready;

		private TestPartyProxy _proxy;

		private Action _state;

		private float _time;

		private void Start()
		{
			_proxy = GetComponentInParent<TestPartyProxy>();
			_state = StateNormal;
		}

		private void Update()
		{
			if (TestCore.IsReady)
			{
				_state();
			}
		}

		private void StateNormal()
		{
			if (Party.Get().IsClient() && !Party.Get().IsPlay() && !Party.Get().IsNews())
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
			if (Party.Get().IsClientDisconnected())
			{
				_state = StateDelay;
				_time = 0f;
			}
		}

		private void StateDelay()
		{
			_time += Time.deltaTime;
			if (_time >= 1f)
			{
				_state = StateNormal;
				Party.Get().Wait();
			}
		}

		public void OnFinishSetting(Button btn)
		{
			btn.interactable = false;
			_proxy.FinishSetting();
		}

		public void OnLeave()
		{
			_proxy.Leave();
		}
	}
}
