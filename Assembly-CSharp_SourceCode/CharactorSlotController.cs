using UnityEngine;

public class CharactorSlotController : MonoBehaviour
{
	[SerializeField]
	[Header("キャラスロットプレハブ")]
	private GameObject charaSlotPrefab;

	[SerializeField]
	[Header("リーダーキャラプレハブ")]
	private GameObject leaderCharaSlotPrefab;

	[SerializeField]
	private GameObject[] characterObjects = new GameObject[5];

	private CharactorSlotObject[] characters = new CharactorSlotObject[5];

	private RectTransform _rectTransform;

	public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

	public void Awake()
	{
		for (int i = 0; i < characterObjects.Length; i++)
		{
			GameObject gameObject = Object.Instantiate((i == 0) ? leaderCharaSlotPrefab : charaSlotPrefab, characterObjects[i].transform);
			characters[i] = gameObject.GetComponent<CharactorSlotObject>();
		}
	}

	public void Initialize()
	{
		for (int i = 0; i < characters.Length; i++)
		{
			SetVisibleCharacterSet(i, isActive: false);
		}
	}

	public void SetSlotData(MessageCharactorInfomationData data, CharacterSlotData slotData)
	{
		characters[data.Index].SetData(data, slotData);
	}

	public void SetVisibleCharacterSet(int index, bool isActive)
	{
		characters[index].SetVisibleCharacterSet(isActive);
	}

	public void SetTeamSlotData(MessageCharactorInfomationData data)
	{
	}

	public void ResetCharactor()
	{
		for (int i = 0; i < characters.Length; i++)
		{
			characters[i].ResetData();
		}
	}

	public void ResetCharacter(int index)
	{
		characters[index].ResetData();
	}

	public void SetClassicMode(bool isActive)
	{
	}
}
