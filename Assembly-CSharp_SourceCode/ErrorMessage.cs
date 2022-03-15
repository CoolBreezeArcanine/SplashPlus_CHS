using System.Collections;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour
{
	private static ErrorMessage _instance;

	[SerializeField]
	private CanvasGroup _main;

	[SerializeField]
	private CanvasGroup _sub;

	[SerializeField]
	private TextMeshProUGUI _tmpText;

	[SerializeField]
	private TextMeshProUGUI _tmpMessageText;

	[SerializeField]
	private TextMeshProUGUI _tmpProcessText;

	[SerializeField]
	private ScrollRect scroll;

	public static ErrorMessage Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}
			ErrorMessage component = Object.Instantiate(Resources.Load<GameObject>("System/ErrorExceptionObject")).GetComponent<ErrorMessage>();
			component.transform.localScale = Vector3.one;
			component.transform.localRotation = Quaternion.identity;
			component.transform.localPosition = Vector3.zero;
			component._main.alpha = 0f;
			component._sub.alpha = 0f;
			_instance = component;
			return _instance;
		}
	}

	private IEnumerator corutine()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.2f);
		_main.alpha = 1f;
		_sub.alpha = 1f;
	}

	public void AddMessage(string title, string message, string processDump)
	{
		_tmpText.text = title;
		TextMeshProUGUI tmpMessageText = _tmpMessageText;
		tmpMessageText.text = tmpMessageText.text + title + "\n" + message + "\n";
		_tmpProcessText.text = processDump;
		StartCoroutine(corutine());
	}

	private void Update()
	{
		if (InputManager.GetMonitorButtonDown(InputManager.ButtonSetting.Button01) || DebugInput.GetKey(KeyCode.DownArrow))
		{
			scroll.verticalNormalizedPosition -= 16f / scroll.content.sizeDelta.y;
		}
		else if (InputManager.GetMonitorButtonDown(InputManager.ButtonSetting.Button04) || DebugInput.GetKey(KeyCode.UpArrow))
		{
			scroll.verticalNormalizedPosition += 16f / scroll.content.sizeDelta.y;
		}
	}
}
