using System.Collections.Generic;
using MAI2.Util;
using MAI2System;
using Manager;
using TMPro;
using UnityEngine;

public class MouseTouchPanel : MonoBehaviour
{
	[SerializeField]
	[Tooltip("起動時にボタンを表示したままにするか")]
	private bool isStartShowButton;

	private int PlayerID;

	private List<MeshButton> buttonList;

	private ulong pushFlag;

	private GUIStyle debugStyle = new GUIStyle();

	public ulong GetPushFlag
	{
		get
		{
			ulong result = pushFlag;
			pushFlag = 0uL;
			return result;
		}
		set
		{
			pushFlag = value;
		}
	}

	private void Awake()
	{
		debugStyle.normal.textColor = Color.black;
		debugStyle.fontSize = 16;
	}

	private void Start()
	{
		PlayerID = ((!(base.transform.position.x < 0f)) ? 1 : 0);
		buttonList = new List<MeshButton>();
		foreach (Transform item in base.transform)
		{
			MeshButton component = item.GetComponent<MeshButton>();
			component.SetTouchCallback(OnClickDown, OnClick);
			if (!isStartShowButton)
			{
				component.targetGraphic.color = Color.clear;
				if (component.transform.childCount != 0)
				{
					TextMeshProUGUI component2 = component.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
					if (null != component2)
					{
						component2.color = Color.clear;
					}
				}
			}
			if (!Singleton<SystemConfig>.Instance.config.IsDummyTouchPanel)
			{
				component.gameObject.GetComponent<CustomGraphic>().raycastTarget = false;
			}
			buttonList.Add(component);
		}
		InputManager.RegisterMouseTouchPanel(PlayerID, this);
	}

	public void LateExecute()
	{
	}

	private void OnClickDown(InputManager.TouchPanelArea area)
	{
	}

	private void OnClick(InputManager.TouchPanelArea area)
	{
		GetPushFlag |= (ulong)(1L << (int)area);
	}

	public void SetVisible(bool isVisible)
	{
		Color color = (isVisible ? new Color(1f, 0f, 0f, 0.5f) : Color.clear);
		for (int i = 0; i < buttonList.Count; i++)
		{
			buttonList[i].targetGraphic.color = color;
			if (buttonList[i].transform.childCount != 0)
			{
				TextMeshProUGUI component = buttonList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
				if (null != component)
				{
					component.color = color;
				}
			}
		}
	}

	public bool IsInside(Vector2 checkScreenPosition, out InputManager.TouchPanelArea area)
	{
		area = InputManager.TouchPanelArea.End;
		bool result = false;
		for (int i = 0; i < buttonList.Count; i++)
		{
			if (buttonList[i].IsPointInPolygon(checkScreenPosition))
			{
				area = buttonList[i].GetTouchPanelArea;
				result = true;
				break;
			}
		}
		return result;
	}

	public Vector3 GetPosition(InputManager.TouchPanelArea area)
	{
		foreach (MeshButton button in buttonList)
		{
			if (button.name == area.ToString())
			{
				return button.transform.localPosition;
			}
		}
		return new Vector3(0f, 0f, 0f);
	}
}
