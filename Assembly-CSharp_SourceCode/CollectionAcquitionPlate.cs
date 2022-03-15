using DB;
using TMPro;
using UI;
using UnityEngine;

public class CollectionAcquitionPlate : MonoBehaviour
{
	[SerializeField]
	private MultipleImage _titleImage;

	[SerializeField]
	private TextMeshProUGUI _acquitionTitleText;

	[SerializeField]
	private TextMeshProUGUI _numText;

	public void Prepare(CollectionProcess.SubSequence sequence, int num = 0)
	{
		base.gameObject.SetActive(value: true);
		_titleImage.ChangeSprite((int)sequence);
		ChangeAcuitionTitle((int)sequence);
		_numText.text = num.ToString();
	}

	private void ChangeAcuitionTitle(int index)
	{
		_acquitionTitleText.text = ((index == 0) ? CommonMessageID.CollectionTotalNum.GetName() : CommonMessageID.CollectionNum.GetName());
	}
}
