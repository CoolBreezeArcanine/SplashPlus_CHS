using UnityEngine;

public class CollectionOtherParts : MonoBehaviour
{
	[SerializeField]
	[Header("その他のパーツ")]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private GameObject _lockObj;

	[SerializeField]
	[Header("装備フレーム")]
	private GameObject _setFrameObj;

	[SerializeField]
	[Header("キラ属性")]
	private GameObject _setSpecialObj;

	private StateAnimController _controller;

	private bool _isActive;

	public void SetOtherParts(bool isHave, bool isActive, bool isEquipment, bool isRandom, bool isSpecial, int monitorIndex)
	{
		_isActive = isActive;
		_lockObj.SetActive(!_isActive);
		if (_setFrameObj != null)
		{
			_setFrameObj?.SetActive(_isActive);
		}
		if (_setFrameObj != null && isSpecial)
		{
			_setSpecialObj?.SetActive(value: true);
		}
		else
		{
			_setSpecialObj?.SetActive(value: false);
		}
		SetVisibleSetIcon(isEquipment);
	}

	public void UpdateView(float syncTimer)
	{
		_ = _isActive;
	}

	public void SetVisibleSetIcon(bool isEquipment)
	{
		if (_isActive)
		{
			_setFrameObj?.SetActive(isEquipment);
		}
	}

	public bool IsActive()
	{
		return _canvasGroup.alpha != 0f;
	}

	public void SetVisible(bool isVisible)
	{
		_canvasGroup.alpha = (isVisible ? 1 : 0);
	}
}
