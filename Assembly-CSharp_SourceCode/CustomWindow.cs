using DB;
using UnityEngine;

public class CustomWindow : CommonWindow
{
	public void Prepare(MessageController.MessageInfo info, IMessageController controller)
	{
		SetSprites(info, controller);
		SetPivot(info.PositionId);
		_messageText.text = info.Message;
		_messageText.color = ((info.KindId == WindowKindID.Attention) ? Color.white : Color.black);
		if (info.SizeId == WindowSizeID.LargeHorizontal || info.SizeId == WindowSizeID.LargeVertical)
		{
			SetPadding();
		}
		else
		{
			SetHeight();
		}
		_titleGroup.alpha = ((!(info.Title == "")) ? 1 : 0);
		_titleText.text = info.Title;
		_lifeCounter = 0f;
		LifeTime = -1f;
		_state = WindowState.Prepare;
		_isOpening = true;
		SetPosition(info.Position);
	}

	private void SetSprites(MessageController.MessageInfo info, IMessageController controller)
	{
		if (info.KindId == WindowKindID.Attention)
		{
			_windowImage.sprite = controller.AttentionSp();
			_titleImage.sprite = controller.AttentionTitleSp();
		}
		else if (info.KindId == WindowKindID.Common)
		{
			_windowImage.sprite = controller.DefaultSp();
			_titleImage.sprite = controller.DefaultTitleSp();
		}
		else
		{
			Sprite sprite = Resources.Load<Sprite>("TextView/UI_WND_Type_" + 0.ToString("000"));
			Sprite sprite2 = Resources.Load<Sprite>("TextView/UI_WND_Title_Type_" + 0.ToString("000"));
			_windowImage.sprite = sprite;
			_titleImage.sprite = sprite2;
		}
		if (info.SizeId == WindowSizeID.LargeHorizontal || info.SizeId == WindowSizeID.LargeVertical)
		{
			Sprite iconSprite = Resources.Load<Sprite>("TextView/0");
			SetIconSprite(iconSprite);
		}
	}
}
