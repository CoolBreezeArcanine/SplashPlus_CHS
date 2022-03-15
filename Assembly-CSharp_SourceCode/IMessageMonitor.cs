using UnityEngine;

public interface IMessageMonitor
{
	Sprite AttentionWindow { get; }

	Sprite AttentionTitle { get; }

	Sprite DefaultWindow { get; }

	Sprite DefaultTitle { get; }

	int MonitorId { get; }
}
