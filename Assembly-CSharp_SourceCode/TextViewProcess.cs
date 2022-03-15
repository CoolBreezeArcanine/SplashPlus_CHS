using System.Collections.Generic;
using UnityEngine;

public class TextViewProcess : MonoBehaviour, ITextViewProcess
{
	[SerializeField]
	private Transform _leftMonitor;

	private TagManager _tagManager;

	private TextMonitor _monitor;

	private bool _isFirst;

	private void OnDestroy()
	{
		if (_monitor != null)
		{
			Object.Destroy(_monitor.gameObject);
		}
	}

	public void Awake()
	{
		_tagManager = new TagManager();
		GameObject original = Resources.Load<GameObject>("TextView/TextViewProcess");
		_monitor = Object.Instantiate(original, _leftMonitor).GetComponent<TextMonitor>();
		_monitor.Prepare(this);
	}

	private void Update()
	{
		if (!_isFirst)
		{
			Screen.SetResolution(1728, 800, fullscreen: false, 60);
			_isFirst = true;
		}
		_monitor.UpdateView();
	}

	public List<TagTip> GeTagTipList()
	{
		return _tagManager.GetTagTipList();
	}
}
