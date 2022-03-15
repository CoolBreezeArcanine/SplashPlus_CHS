using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetTicketWindow : EventWindowBase
{
	[SerializeField]
	private Image _ticketImage;

	[SerializeField]
	private TextMeshProUGUI _ticketName;

	[SerializeField]
	private TextMeshProUGUI _infoText;

	private void Awake()
	{
		_infoText.text = "チケットを獲得しました";
	}

	public void SetTicket(Sprite ticketSprite, string itemName)
	{
		_ticketImage.sprite = ticketSprite;
		_ticketName.text = itemName;
	}

	public void FadeIn()
	{
		_animator.Play("In2");
	}

	public void FadeOut()
	{
		_animator.Play("Out");
	}
}
