using System.Linq;
using Manager.Party.Party;
using PartyLink;
using UnityEngine;
using UnityEngine.UI;

namespace zPlayTest.Scripts.UI
{
	public class UIRecruit : MonoBehaviour
	{
		[SerializeField]
		private GameObject _rig;

		[SerializeField]
		private GameObject _button;

		private TestPartyProxy _proxy;

		private Text _text;

		private void Start()
		{
			_proxy = GetComponentInParent<TestPartyProxy>();
			_text = _rig.GetComponentInChildren<Text>();
		}

		private void Update()
		{
			if (TestCore.IsReady)
			{
				RecruitInfo recruitInfo = Manager.Party.Party.Party.Get().GetRecruitListWithoutMe().FirstOrDefault((RecruitInfo i) => i.JoinNumber < 2);
				if (recruitInfo != null)
				{
					_text.text = "募集中: " + recruitInfo.MechaInfo.IpAddress.convIpString();
					_rig.SetActive(value: true);
				}
				else
				{
					_rig.SetActive(value: false);
				}
				_button.SetActive(!Manager.Party.Party.Party.Get().IsJoin());
			}
		}

		public void OnJoin()
		{
			_proxy.Join();
		}
	}
}
