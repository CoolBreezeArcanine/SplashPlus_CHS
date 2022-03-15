using Process;
using UI;
using UnityEngine;

namespace Monitor.CharacterSelect.Controllers
{
	public class MoveDistanceCounterController : MonoBehaviour
	{
		[SerializeField]
		[Header("距離単位")]
		private MultipleImage _distanceUnitImage;

		[SerializeField]
		[Header("増減矢印")]
		private MultipleImage _arrowImage;

		[SerializeField]
		[Header("桁別数値カウンタ")]
		private SpriteCounter _eighthDigit;

		[SerializeField]
		private SpriteCounter _seventhDigit;

		[SerializeField]
		private SpriteCounter _sixthDigit;

		[SerializeField]
		private SpriteCounter _fifthDigit;

		[SerializeField]
		private SpriteCounter _fourthDigit;

		[SerializeField]
		private SpriteCounter _thirdDigit;

		[SerializeField]
		private SpriteCounter _secondDigit;

		[SerializeField]
		private SpriteCounter _firstDigit;

		public void SetData(int distance)
		{
			float num = distance;
			int num2 = ((distance == 0) ? 1 : ((int)Mathf.Log10(distance) + 1));
			string text;
			if (num >= 1000f)
			{
				num /= 1000f;
				_distanceUnitImage.ChangeSprite(1);
				text = $"{num:.###}";
			}
			else
			{
				_distanceUnitImage.ChangeSprite(0);
				text = $"{num:#,0}";
			}
			ResetColor();
			switch (num2)
			{
			case 1:
				_firstDigit.SetColor(Color.white);
				_firstDigit.ChangeText(text);
				break;
			case 2:
				_secondDigit.SetColor(Color.white);
				_secondDigit.ChangeText(text);
				break;
			case 3:
				_thirdDigit.SetColor(Color.white);
				_thirdDigit.ChangeText(text);
				break;
			case 4:
				_fourthDigit.SetColor(Color.white);
				_fourthDigit.ChangeText(text);
				break;
			case 5:
				_fifthDigit.SetColor(Color.white);
				_fifthDigit.ChangeText(text);
				break;
			case 6:
				_sixthDigit.SetColor(Color.white);
				_sixthDigit.ChangeText(text);
				break;
			case 7:
				_seventhDigit.SetColor(Color.white);
				_seventhDigit.ChangeText(text);
				break;
			case 8:
				_eighthDigit.SetColor(Color.white);
				_eighthDigit.ChangeText(text);
				break;
			}
		}

		public void SetArrow(CharacterSelectProces.ArrowDirection direction)
		{
			if (!(_arrowImage == null))
			{
				switch (direction)
				{
				case CharacterSelectProces.ArrowDirection.Down:
					_arrowImage.ChangeSprite(0);
					_arrowImage.color = Color.white;
					break;
				case CharacterSelectProces.ArrowDirection.Stay:
					_arrowImage.ChangeSprite(2);
					_arrowImage.color = Color.white;
					break;
				case CharacterSelectProces.ArrowDirection.Up:
					_arrowImage.ChangeSprite(1);
					_arrowImage.color = Color.white;
					break;
				}
			}
		}

		public void ResetColor()
		{
			_eighthDigit.SetColor(Color.clear);
			_seventhDigit.SetColor(Color.clear);
			_sixthDigit.SetColor(Color.clear);
			_fifthDigit.SetColor(Color.clear);
			_fourthDigit.SetColor(Color.clear);
			_thirdDigit.SetColor(Color.clear);
			_secondDigit.SetColor(Color.clear);
			_firstDigit.SetColor(Color.clear);
		}
	}
}
