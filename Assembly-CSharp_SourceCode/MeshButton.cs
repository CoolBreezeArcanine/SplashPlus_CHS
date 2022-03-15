using System;
using MAI2System;
using Manager;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CustomGraphic))]
public class MeshButton : Button, ICanvasRaycastFilter
{
	private bool isMouseButton;

	private Vector2[] vertexArray;

	private InputManager.TouchPanelArea touchArea;

	private Action<InputManager.TouchPanelArea> onTouchEvent;

	private Action<InputManager.TouchPanelArea> onTouchDownEvent;

	public InputManager.TouchPanelArea GetTouchPanelArea => touchArea;

	protected override void Awake()
	{
		touchArea = (InputManager.TouchPanelArea)Enum.Parse(typeof(InputManager.TouchPanelArea), base.name);
		RectTransform component = GetComponent<RectTransform>();
		Vector3 localScale = component.localScale;
		localScale.x = ((localScale.x < 0f) ? (-1.1f) : 1.1f);
		localScale.y = (base.name.StartsWith("B") ? 1.4f : 1.1f);
		component.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
		CustomGraphic customGraphic = base.targetGraphic as CustomGraphic;
		vertexArray = new Vector2[customGraphic.vertex.Count];
		bool flag = false;
		using (IniFile iniFile = new IniFile("mai2.ini"))
		{
			flag = iniFile.getValue("Player", "SinglePlayer", 0) == 1;
		}
		for (int i = 0; i < customGraphic.vertex.Count; i++)
		{
			vertexArray[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, new Vector2(base.transform.position.x + customGraphic.vertex[i].x + (flag ? 540f : 0f), base.transform.position.y + customGraphic.vertex[i].y));
		}
	}

	private void Update()
	{
		isMouseButton = DebugInput.GetMouseButton(0);
	}

	public void SetTouchCallback(Action<InputManager.TouchPanelArea> OnTouchDown, Action<InputManager.TouchPanelArea> OnTouch)
	{
		onTouchEvent = OnTouch;
		onTouchDownEvent = OnTouchDown;
	}

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		bool flag = IsPointInPolygon(vertexArray, sp);
		if (flag && DebugInput.GetMouseButtonDown(0))
		{
			onTouchDownEvent(touchArea);
			base.onClick.Invoke();
		}
		else if (flag && isMouseButton)
		{
			onTouchEvent(touchArea);
			base.onClick.Invoke();
		}
		if (flag)
		{
			return isMouseButton;
		}
		return false;
	}

	public bool IsPointInPolygon(Vector2 point)
	{
		return IsPointInPolygon(vertexArray, point);
	}

	private bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), point, Camera.main);
	}
}
