using UnityEngine;

public class CourseSeparator : MonoBehaviour
{
	[SerializeField]
	private GameObject _daniRight;

	[SerializeField]
	private GameObject _daniLeft;

	[SerializeField]
	private GameObject _sinDaniRight;

	[SerializeField]
	private GameObject _sinDaniLeft;

	[SerializeField]
	private GameObject _randomRight;

	[SerializeField]
	private GameObject _randomLeft;

	public void SetShow(int mode, bool isRight)
	{
		switch (mode)
		{
		case 1:
			_sinDaniRight.SetActive(value: false);
			_sinDaniLeft.SetActive(value: false);
			_randomRight.SetActive(value: false);
			_randomLeft.SetActive(value: false);
			if (isRight)
			{
				_daniRight.SetActive(value: true);
				_daniLeft.SetActive(value: false);
			}
			else
			{
				_daniRight.SetActive(value: false);
				_daniLeft.SetActive(value: true);
			}
			break;
		case 2:
			_daniRight.SetActive(value: false);
			_daniLeft.SetActive(value: false);
			_randomRight.SetActive(value: false);
			_randomLeft.SetActive(value: false);
			if (isRight)
			{
				_sinDaniRight.SetActive(value: true);
				_sinDaniLeft.SetActive(value: false);
			}
			else
			{
				_sinDaniRight.SetActive(value: false);
				_sinDaniLeft.SetActive(value: true);
			}
			break;
		case 3:
			_daniRight.SetActive(value: false);
			_daniLeft.SetActive(value: false);
			_sinDaniRight.SetActive(value: false);
			_sinDaniLeft.SetActive(value: false);
			if (isRight)
			{
				_randomRight.SetActive(value: true);
				_randomLeft.SetActive(value: false);
			}
			else
			{
				_randomRight.SetActive(value: false);
				_randomLeft.SetActive(value: true);
			}
			break;
		}
	}
}
