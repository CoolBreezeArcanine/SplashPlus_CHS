using UnityEngine;
using UnityEngine.UI;

public class CopyButtons : MonoBehaviour
{
	[SerializeField]
	private Button _copyTButton;

	[SerializeField]
	private Button _copyMButton;

	public void Prepare(IInputField inputField)
	{
		_copyTButton.onClick.RemoveAllListeners();
		_copyTButton.onClick.AddListener(delegate
		{
			GUIUtility.systemCopyBuffer = inputField.Title();
		});
		_copyMButton.onClick.RemoveAllListeners();
		_copyMButton.onClick.AddListener(delegate
		{
			GUIUtility.systemCopyBuffer = inputField.Message();
		});
	}
}
