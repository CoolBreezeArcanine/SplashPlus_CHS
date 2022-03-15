using System;
using Manager.Party.Party;
using PartyLink;
using UnityEngine;

namespace zPlayTest.Scripts
{
	public class TestGameClient : MonoBehaviour
	{
		private Action _state;

		private float _elapsedTime;

		private int _count;

		private void Start()
		{
			_state = StateBeginPlay;
		}

		private void Update()
		{
			_state?.Invoke();
		}

		private void StateBeginPlay()
		{
			Manager.Party.Party.Party.Get().BeginPlay();
			_state = StateIsJoinAndActive;
		}

		private void StateIsJoinAndActive()
		{
			if (Manager.Party.Party.Party.Get().IsJoinAndActive())
			{
				_state = StateIsAllBeginPlay;
			}
		}

		private void StateIsAllBeginPlay()
		{
			if (Manager.Party.Party.Party.Get().IsAllBeginPlay())
			{
				_state = StateReady;
			}
		}

		private void StateReady()
		{
			Manager.Party.Party.Party.Get().Ready();
			_state = StateGamePlay;
		}

		private void StateGamePlay()
		{
			_elapsedTime += Time.deltaTime;
			_count++;
			if (_elapsedTime >= 5f)
			{
				ClientPlayInfo clientPlayInfo = new ClientPlayInfo();
				clientPlayInfo.IpAddress = PartyLink.Util.MyIpAddress().ToNetworkByteOrderU32();
				clientPlayInfo.Count = _count;
				clientPlayInfo.Achieves = new int[2]
				{
					_count,
					_count * 2
				};
				ClientPlayInfo clientPlayInfo2 = clientPlayInfo;
				if (Manager.Party.Party.Party.Get().IsHost())
				{
					clientPlayInfo2.Achieves[0] *= 2;
					clientPlayInfo2.Achieves[1] *= 2;
				}
				Manager.Party.Party.Party.Get().SendClientPlayInfo(clientPlayInfo2);
				_elapsedTime = 0f;
			}
		}
	}
}
