using Mai2.Mai2Cue;
using Monitor.Common;
using Net.Packet;
using TMPro;
using UnityEngine;

namespace Monitor.Entry.Parts.Screens
{
	public class ErrorGeneral : EntryScreen
	{
		[SerializeField]
		private TextMeshProUGUI _text;

		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			if (args[0] is string text)
			{
				_text.text = "Error: " + text;
			}
			else
			{
				PacketStatus packetStatus = (PacketStatus)args[0];
				_text.text = "NetError: " + packetStatus;
			}
			PlaySE(Cue.SE_ENTRY_AIME_ERROR);
			Delay.StartDelay(3f, delegate
			{
				EntryMonitor.ResponseYes();
			});
		}
	}
}
