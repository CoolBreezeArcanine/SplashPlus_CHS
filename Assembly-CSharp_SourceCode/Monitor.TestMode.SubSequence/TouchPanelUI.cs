using System;
using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class TouchPanelUI : MonoBehaviour
	{
		private int _playerID;

		private List<CustomGraphic> _buttonList;

		private readonly Color _offTouchCol = new Color(0f, 0f, 1f, 0.4f);

		private readonly Color _onTouchCol = new Color(1f, 0f, 0f, 0.4f);

		private void Start()
		{
			_playerID = ((!(base.transform.position.x < 0f)) ? 1 : 0);
			_buttonList = new List<CustomGraphic>();
			foreach (Transform item in base.transform)
			{
				CustomGraphic component = item.GetComponent<CustomGraphic>();
				_buttonList.Add(component);
			}
		}

		private void OnGUI()
		{
			foreach (CustomGraphic button2 in _buttonList)
			{
				InputManager.TouchPanelArea button = InputManager.TouchPanelArea.A1;
				foreach (InputManager.TouchPanelArea value in Enum.GetValues(typeof(InputManager.TouchPanelArea)))
				{
					if (value.ToString() == button2.name)
					{
						button = value;
						break;
					}
				}
				button2.color = (InputManager.GetTouchPanelAreaPush(_playerID, button) ? _onTouchCol : _offTouchCol);
				TextMeshProUGUI component = button2.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
				if ((bool)component)
				{
					component.text = "";
				}
			}
		}
	}
}
