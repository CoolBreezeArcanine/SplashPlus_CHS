using AMDaemon;
using UnityEngine;

namespace Net.Packet.Helper
{
	public class PacketComponent : MonoBehaviour
	{
		private Packet _packet;

		public void StartPacket(Packet packet)
		{
			_packet = packet;
		}

		private void Update()
		{
			if (_packet == null)
			{
				return;
			}
			if (Sequence.IsTest)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			PacketState packetState = _packet.Proc();
			if (packetState == PacketState.Done || packetState == PacketState.Error)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
