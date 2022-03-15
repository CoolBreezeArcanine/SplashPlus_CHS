using Timeline;
using UnityEngine;

public class CustomTypewriterObject : TimeControlBaseObject
{
	[SerializeField]
	private CustomTypewriter _typewriter;

	[SerializeField]
	private string[] _messages;

	public void SetMessages(string[] messages)
	{
		_messages = messages;
	}

	public override void OnBehaviourPlay()
	{
		_typewriter.SetMessage(GetRandomMessage());
		UpdateTypewriter(0f);
	}

	public override void OnClipPlay()
	{
		UpdateTypewriter(0f);
	}

	public override void OnClipTailEnd()
	{
		UpdateTypewriter(1f);
	}

	public override void OnClipHeadEnd()
	{
		UpdateTypewriter(0f);
	}

	public override void OnGraphStop()
	{
		UpdateTypewriter(0f);
	}

	public override void PrepareFrame(double normalizeTime)
	{
		UpdateTypewriter((float)normalizeTime);
	}

	private string GetRandomMessage()
	{
		int num = Random.Range(0, _messages.Length);
		if (num < 0 || _messages.Length <= num)
		{
			return "";
		}
		return _messages[num];
	}

	private void UpdateTypewriter(float rate)
	{
		_typewriter.UpdateTypewriter(rate);
	}
}
