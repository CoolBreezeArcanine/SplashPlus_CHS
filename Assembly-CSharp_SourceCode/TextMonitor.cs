using UnityEngine;
using UnityEngine.UI;

public class TextMonitor : MonoBehaviour
{
	[SerializeField]
	private MessageController _messageController;

	[SerializeField]
	private InputFieldViewer _inputViewer;

	[SerializeField]
	private DropDownController _dropDownController;

	[SerializeField]
	private FontOutViewer _fontOutViewer;

	[SerializeField]
	private ColorViewer _colorViewer;

	[SerializeField]
	private TagViewer _tagViewer;

	[SerializeField]
	private CopyButtons _copyButtons;

	[SerializeField]
	private Button _adaptateButton;

	private ITextViewProcess _process;

	public void Prepare(ITextViewProcess process)
	{
		_process = process;
		_tagViewer.Prepare(_process.GeTagTipList(), _inputViewer.InsertTitle, _inputViewer.InsertMessage, _fontOutViewer, _colorViewer);
		_copyButtons.Prepare(_inputViewer);
		_adaptateButton.onClick.RemoveAllListeners();
		_adaptateButton.onClick.AddListener(UpdateWindow);
	}

	public void UpdateView()
	{
		_inputViewer.UpdateInputField();
		_messageController.ViewUpdate();
	}

	private void UpdateWindow()
	{
		string title = _inputViewer.Title();
		string message = _inputViewer.Message();
		MessageController.MessageInfo messageInfo = default(MessageController.MessageInfo);
		messageInfo.Title = title;
		messageInfo.Message = message;
		messageInfo.SizeId = _dropDownController.GetSize();
		messageInfo.PositionId = _dropDownController.GetPosition();
		messageInfo.KindId = _dropDownController.GetWindowKind();
		messageInfo.Position = new Vector3(0f, 0f, 0f);
		messageInfo.IsWarning = _dropDownController.IsWarning();
		MessageController.MessageInfo messageWindow = messageInfo;
		_messageController.SetMessageWindow(messageWindow);
	}
}
