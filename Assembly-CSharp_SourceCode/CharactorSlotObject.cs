using UI;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class CharactorSlotObject : MonoBehaviour
{
	[SerializeField]
	private bool _isLeader;

	[SerializeField]
	private GameObject _baseSetObj;

	[SerializeField]
	[Tooltip("キャラクター背景")]
	private Image _baseImage;

	[SerializeField]
	[Tooltip("キャラクターマスク")]
	private GameObject _maskObject;

	[SerializeField]
	[Tooltip("キャラクター画像")]
	private CustomImage _faceImage;

	[SerializeField]
	[Tooltip("キャラクター影用")]
	private MultiImage _faceShadowImage;

	[SerializeField]
	[Tooltip("フレーム")]
	private Image _frameImage;

	[SerializeField]
	[Tooltip("レベル数表示")]
	private SpriteCounter _levelText;

	[SerializeField]
	private AwakeIconController _awakeIconController;

	public virtual void SetSlotActive(bool isActive)
	{
		_levelText.gameObject.SetActive(isActive);
	}

	public void SetCurrentSlotCard(bool isSelect)
	{
		_faceImage.color = (isSelect ? Color.gray : Color.white);
	}

	public void SetData(MessageCharactorInfomationData infoData, CharacterSlotData slotData)
	{
		SetLevel(infoData.Level);
		if (!_maskObject.activeSelf)
		{
			_maskObject.SetActive(value: true);
		}
		SetAwakening(slotData.SmallAwakeStar, slotData.AwakeStarBase, infoData.AwakeRate, (int)infoData.Awakening);
		Sprite sprite = Sprite.Create(infoData.Character, new Rect(0f, 0f, infoData.Character.width, infoData.Character.height), new Vector2(0.5f, 0.5f));
		if (!_isLeader)
		{
			_baseImage.sprite = slotData.Base;
			_faceShadowImage.Image2 = sprite;
			_faceShadowImage.color = Utility.ConvertColor(slotData.SubColor);
			_faceShadowImage.SetAllDirty();
			_frameImage.sprite = slotData.Frame;
		}
		else
		{
			_frameImage.sprite = slotData.LeaderFrame;
		}
		_faceImage.sprite = sprite;
		SetVisibleCharacterSet(isActive: true);
	}

	public void SetVisibleCharacterSet(bool isActive)
	{
		_baseSetObj.gameObject.SetActive(isActive);
	}

	private void SetAwakening(Sprite smallBase, Sprite largeBase, float gaugeAmount, int awakeNum)
	{
		_awakeIconController.Prepare(smallBase, largeBase, gaugeAmount, awakeNum);
	}

	public void SetLevel(uint level)
	{
		if (_levelText != null)
		{
			_levelText.ChangeText(level.ToString().PadRight(4));
		}
	}

	public virtual void ResetData()
	{
		if (_levelText != null)
		{
			_levelText.ChangeText("0   ");
		}
		_maskObject.SetActive(value: false);
		SetVisibleCharacterSet(isActive: false);
	}
}
