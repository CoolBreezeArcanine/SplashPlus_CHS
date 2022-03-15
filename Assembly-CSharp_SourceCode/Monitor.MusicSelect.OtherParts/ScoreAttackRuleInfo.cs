using TMPro;
using UnityEngine;

namespace Monitor.MusicSelect.OtherParts
{
	public class ScoreAttackRuleInfo : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _text1;

		[SerializeField]
		private TextMeshProUGUI _text2;

		[SerializeField]
		private TextMeshProUGUI _text3;

		public void Initialize()
		{
			_text1.SetText("同クレジット内で遊んだ大会対象曲の達成率の合計がスコアになります。");
			_text2.SetText("<color=#c98cf6ff>MASTER</color>以外の難易度を選んだ際は、楽曲の達成率に以下の減算が入ります。");
			_text3.SetText("<color=#ff5a66ff>EXPERT</color>… -1.0000%     <color=#f8df3aff>ADVANCED</color>… -2.0000%     <color=#81d955ff>BASIC</color>… -3.0000%");
		}
	}
}
