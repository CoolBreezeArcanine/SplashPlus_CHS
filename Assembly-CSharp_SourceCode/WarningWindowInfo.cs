public class WarningWindowInfo
{
	private string _title;

	private string _message;

	private float _lifeTime;

	private int _monitorId;

	public string Title => _title;

	public string Message => _message;

	public float LifeTime => _lifeTime;

	public int MonitorId => _monitorId;

	public WarningWindowInfo(string title, string message, float lifeTime, int monitorId)
	{
		_title = title;
		_message = message;
		_lifeTime = lifeTime;
		_monitorId = monitorId;
	}
}
