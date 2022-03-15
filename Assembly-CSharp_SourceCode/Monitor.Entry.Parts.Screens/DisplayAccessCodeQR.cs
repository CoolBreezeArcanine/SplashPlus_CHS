using System.Collections.Generic;
using System.Linq;
using DB;
using IO;
using Mai2.Mai2Cue;
using Monitor.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Entry.Parts.Screens
{
	public class DisplayAccessCodeQR : EntryScreen
	{
		[SerializeField]
		private RawImage _rawImage;

		[SerializeField]
		private TextMeshProUGUI _accessCode;

		[SerializeField]
		private TextMeshProUGUI _message;

		[SerializeField]
		private TextMeshProUGUI _message1;

		public override void Open(params object[] args)
		{
			string text = (string)args[0];
			_message.text = CommonMessageID.Entry_AccessCodeMessage.GetName();
			_message1.text = CommonMessageID.Entry_AccessCodeMessage1.GetName();
			base.Open(args);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			PlaySE(Cue.SE_ENTRY_AIME_CODE);
			if (_rawImage.texture != null)
			{
				Object.Destroy(_rawImage.texture);
			}
			_rawImage.texture = QRImage.encode("https://my-aime.net/aime/i/registq.html?ac=" + text, 5, 4, 1, 4);
			IEnumerable<string> values = from e in text.Select((char code, int index) => new { code, index })
				group e.code by e.index / 4 into e
				select string.Join("", e);
			_accessCode.text = string.Join("-", values);
			CreateButton(ButtonType.Return, delegate
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
