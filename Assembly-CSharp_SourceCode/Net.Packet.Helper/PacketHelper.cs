using UnityEngine;

namespace Net.Packet.Helper
{
	public class PacketHelper
	{
		public static GameObject StartPacket(Packet packet)
		{
			GameObject gameObject = new GameObject(packet.GetType().Name);
			gameObject.AddComponent<PacketComponent>().StartPacket(packet);
			return gameObject;
		}
	}
}
