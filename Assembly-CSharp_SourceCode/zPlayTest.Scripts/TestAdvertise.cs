using PartyLink;
using UnityEngine;
using UnityEngine.UI;

namespace zPlayTest.Scripts
{
	public class TestAdvertise : MonoBehaviour
	{
		[SerializeField]
		private Button _button;

		private int _state;

		private bool _busy;

		private void Update()
		{
			switch (_state)
			{
			case 16:
				Advertise.get().start(Advertise.Kind.Advertise, 10);
				_state = 17;
				break;
			case 17:
				if (Advertise.get().isGo())
				{
					SetBusy(flag: false);
					_state = 0;
				}
				break;
			}
		}

		private void SetBusy(bool flag)
		{
			_busy = flag;
			_button.GetComponent<Image>().color = (_busy ? Color.red : Color.white);
		}

		public void OnPress()
		{
			if (TestCore.IsReady && !_busy)
			{
				SetBusy(flag: true);
				_state = 16;
			}
		}
	}
}
