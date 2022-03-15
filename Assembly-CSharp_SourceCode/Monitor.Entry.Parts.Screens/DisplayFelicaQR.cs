using DB;
using IO;
using Mai2.Mai2Cue;
using Monitor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Entry.Parts.Screens
{
	public class DisplayFelicaQR : EntryScreen
	{
		[SerializeField]
		private RawImage _rawImage;

		[SerializeField]
		private TextMeshProUGUI _message;

		public override void Open(params object[] args)
		{
			string text = (string)args[0];
			_message.text = CommonMessageID.Entry_QRMessage.GetName();
			base.Open(args);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			PlaySE(Cue.SE_ENTRY_AIME_CODE);
			if (_rawImage.texture != null)
			{
				Object.Destroy(_rawImage.texture);
			}
			_rawImage.texture = QRImage.encode("https://my-aime.net/aime/i/registq.html?ac=" + text, 5, 4, 1, 4);
			CreateButton(ButtonType.Skip, delegate
			{
				EntryMonitor.ResponseYes();
			});
			ActivateButtons();
		}

		protected override void OnDestroy()
		{
			if (_rawImage.texture != null)
			{
				Object.Destroy(_rawImage.texture);
			}
			base.OnDestroy();
		}
	}
}
