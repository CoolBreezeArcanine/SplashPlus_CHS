using System.Collections.Generic;
using MAI2.Util;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class SelectorTab : MonoBehaviour
{
	public static readonly string CategoryTabImagePath = "Common/Sprites/Tab/Title/";

	[SerializeField]
	private Transform[] _leftTransforms;

	[SerializeField]
	private Transform[] _rightTransforms;

	[SerializeField]
	private MiniTabPanel _miniTabOrigin;

	[SerializeField]
	private MainTabPanel _main;

	[SerializeField]
	private AnimationParts _animation;

	[SerializeField]
	private Transform[] _tabButtonTrans;

	[SerializeField]
	private GameObject _tabButtonObj;

	[SerializeField]
	private Image _leftIcon;

	[SerializeField]
	private Image _rightIcon;

	private MiniTabPanel[] _leftPanels;

	private MiniTabPanel[] _rightPanels;

	private List<TabDataBase> _tabDatas = new List<TabDataBase>();

	private TabButton[] _tabButtons;

	private bool _isAnimation;

	private int _monitorIndex;

	private float _syncTimer;

	private void Awake()
	{
		_leftPanels = new MiniTabPanel[_leftTransforms.Length];
		_rightPanels = new MiniTabPanel[_rightTransforms.Length];
		for (int i = 0; i < _leftTransforms.Length; i++)
		{
			_leftPanels[i] = Object.Instantiate(_miniTabOrigin, _leftTransforms[i]).GetComponent<MiniTabPanel>();
			_rightPanels[i] = Object.Instantiate(_miniTabOrigin, _rightTransforms[i]).GetComponent<MiniTabPanel>();
		}
		if (_main != null)
		{
			_main.ResetSubTitle();
		}
	}

	public virtual void Initialize(int monitorIndex)
	{
		_monitorIndex = monitorIndex;
		if (Singleton<UserDataManager>.Instance.GetUserData(_monitorIndex).IsEntry)
		{
			_tabButtons = new TabButton[_tabButtonTrans.Length];
			for (int i = 0; i < _tabButtons.Length; i++)
			{
				_tabButtons[i] = Object.Instantiate(_tabButtonObj, _tabButtonTrans[i]).GetComponent<TabButton>();
			}
			for (int j = 0; j < _tabButtons.Length; j++)
			{
				_tabButtons[j].Initialize(_monitorIndex);
			}
			_tabButtons[0].UseRightArrow();
			_tabButtons[1].UseLeftArrow();
		}
		_animation.Play("Out");
		SetVisibleParts(isActive: false);
	}

	public void PrepareMainPanel(TabDataBase tabData)
	{
		_main.Prepare(tabData);
	}

	public void UpdateButtonView()
	{
		if (!_isAnimation)
		{
			return;
		}
		_syncTimer += (float)GameManager.GetGameMSecAdd() / 1000f;
		if (_tabButtons != null)
		{
			for (int i = 0; i < _tabButtons.Length; i++)
			{
				_tabButtons[i].ViewUpdate(_syncTimer);
			}
		}
		if (1f < _syncTimer)
		{
			_syncTimer = 0f;
		}
	}

	public void SetData(List<TabDataBase> datas)
	{
		_tabDatas = datas;
		_isAnimation = true;
	}

	public void UpdateTab(int currentCategoryId)
	{
		if (!_isAnimation)
		{
			return;
		}
		int num = currentCategoryId - 1;
		int num2 = _leftPanels.Length - 1;
		while (-1 < num2)
		{
			if (num < 0)
			{
				num = _tabDatas.Count - 1;
			}
			if (0 <= num)
			{
				_leftPanels[num2].Prepare(_tabDatas[num].BaseColor, _tabDatas[num].Title);
				num--;
			}
			num2--;
		}
		num = currentCategoryId + 1;
		for (int i = 0; i < _rightPanels.Length; i++)
		{
			if (_tabDatas.Count <= num)
			{
				num = 0;
			}
			if (num < _tabDatas.Count)
			{
				_rightPanels[i].Prepare(_tabDatas[num].BaseColor, _tabDatas[num].Title);
				num++;
			}
		}
		TabDataBase tabDataBase = _tabDatas[currentCategoryId];
		_main.Prepare(tabDataBase.BaseColor, tabDataBase.TitleSprite, tabDataBase.SubTitle);
	}

	public void PlayInAnimation()
	{
		_animation.Play("In");
		_isAnimation = true;
		SetVisibleParts(isActive: true);
	}

	public void PlayOutAnimation()
	{
		if (_isAnimation)
		{
			_animation.Play("Out");
			_isAnimation = false;
			SetVisibleParts(isActive: false);
			_leftIcon.gameObject.SetActive(value: false);
			_rightIcon.gameObject.SetActive(value: false);
		}
	}

	public void PlayChangeAnimation()
	{
		_animation.Play("In_Change");
	}

	public void PlayMoveRightAnimation()
	{
		_animation.Play("Move_Right");
	}

	public void PlayMoveLeftAnimation()
	{
		_animation.Play("Move_Left");
	}

	public void PressedTabButton(bool isRight)
	{
		if (_tabButtons != null)
		{
			_tabButtons[(!isRight) ? 1 : 0].Pressed();
		}
	}

	public void SetVisibleMiniTab(bool isActive)
	{
		for (int i = 0; i < _leftPanels.Length; i++)
		{
			_leftPanels[i].SetVisible(isActive);
			_rightPanels[i].SetVisible(isActive);
		}
	}

	public void SetVisibleParts(bool isActive)
	{
		SetVisibleMiniTab(isActive);
		if (_tabButtons != null)
		{
			for (int i = 0; i < _tabButtons.Length; i++)
			{
				_tabButtons[i].SetActiveButton(isActive);
			}
		}
	}

	public void SetLeftIcon(Sprite sprite)
	{
		if (_leftIcon != null)
		{
			if (sprite != null)
			{
				_leftIcon.gameObject.SetActive(value: true);
				_leftIcon.sprite = sprite;
			}
			else
			{
				_leftIcon.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetRightIcon(Sprite sprite)
	{
		if (_rightIcon != null)
		{
			if (sprite != null)
			{
				_rightIcon.gameObject.SetActive(value: true);
				_rightIcon.sprite = sprite;
			}
			else
			{
				_rightIcon.gameObject.SetActive(value: false);
			}
		}
	}
}
