using DB;
using TMPro;
using UI;
using UnityEngine;

namespace Monitor
{
	public class ChallengeRule : MonoBehaviour
	{
		[SerializeField]
		[Header("パラメータ")]
		private SpriteCounter _restLife;

		[SerializeField]
		private MultipleImage _unlockDifficulty;

		[SerializeField]
		private TextMeshProUGUI _relaxInformation;

		public void SetChallengeRule(int restLife, int difficulty, int relaxDay, bool infoEnable)
		{
			string text = restLife.ToString("D000");
			if (restLife > 999)
			{
				text = "999";
			}
			if (_restLife != null)
			{
				_restLife.ChangeText(text);
				if (restLife < 100)
				{
					_restLife.FrameList[2].Scale = 0f;
					_restLife.FrameList[0].RelativePosition.x = 23f;
					_restLife.FrameList[1].RelativePosition.x = 8f;
					if (restLife < 10)
					{
						_restLife.FrameList[1].Scale = 0f;
						_restLife.FrameList[0].RelativePosition.x = 34f;
					}
					else
					{
						_restLife.FrameList[1].Scale = 1f;
					}
				}
				else
				{
					_restLife.FrameList[1].Scale = 1f;
					_restLife.FrameList[2].Scale = 1f;
					_restLife.FrameList[0].RelativePosition.x = 17f;
					_restLife.FrameList[1].RelativePosition.x = 0f;
					_restLife.FrameList[2].RelativePosition.x = -17f;
				}
				_restLife.ChangeText(restLife.ToString());
			}
			_unlockDifficulty?.ChangeSprite(difficulty);
			if (_relaxInformation != null)
			{
				if (infoEnable)
				{
					_relaxInformation.text = CommonMessageID.MapResultInfoChallenge02.GetName().Replace(CommonMessageID.MapResultInfoChallenge02Replace.GetName(), relaxDay.ToString());
				}
				else
				{
					_relaxInformation.text = "";
				}
			}
		}
	}
}
