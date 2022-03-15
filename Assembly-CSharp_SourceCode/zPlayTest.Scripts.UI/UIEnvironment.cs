using System;
using PartyLink;
using UnityEngine;
using UnityEngine.UI;

namespace zPlayTest.Scripts.UI
{
	public class UIEnvironment : MonoBehaviour
	{
		[SerializeField]
		private Text _machine;

		[SerializeField]
		private Text _ip;

		private bool _inited;

		private void Update()
		{
			if (!_inited && TestCore.IsReady && PartyLink.Util.MyIpAddress().ToString() != "0.0.0.0")
			{
				Text machine = _machine;
				machine.text = machine.text + " " + Environment.MachineName;
				_ip.text += $" {PartyLink.Util.MyIpAddress()}";
				_inited = true;
			}
		}
	}
}
