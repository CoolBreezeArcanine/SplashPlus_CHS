using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentInfo : MonoBehaviour
{
	[SerializeField]
	[Header("ジャケット画像")]
	private RawImage _jacketImage;

	[SerializeField]
	[Header("テキスト")]
	private TextMeshProUGUI _infoText;

	public void SetInit()
	{
		List<ScoreRankingForAdvertiseSeq> advertise = Singleton<ScoreRankingManager>.Instance.Advertise;
		if (advertise.Count != 0)
		{
			base.gameObject.SetActive(value: true);
			_infoText.text = CommonMessageID.TournamentInfo_1.GetName();
			if (_jacketImage != null)
			{
				string fileName = advertise[0].FileName;
				Sprite sprite = Resources.Load<Sprite>("Common/Sprites/Tab/Title/" + fileName);
				_jacketImage.texture = sprite.texture;
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
