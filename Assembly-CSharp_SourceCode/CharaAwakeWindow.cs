using System;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using UI;
using UnityEngine;

public class CharaAwakeWindow : EventWindowBase
{
	private int HashExitInCode = Animator.StringToHash("Base Layer.In");

	private const float LongInterval = 0.5f;

	private const float ShortInterval = 0.2f;

	[SerializeField]
	private GameObject _awakeStarPrefab;

	[SerializeField]
	private GameObject _awakeRoot;

	[SerializeField]
	private MultiImage _charaImage;

	[SerializeField]
	private MultiImage _shadow;

	private AwakeIconController _awakeIconController;

	private Animator _awakeAnimator;

	[SerializeField]
	[Header("ツアー仲間の距離ゲージ")]
	private OdoSpriteTexts _odoMeters;

	private int _monitorIndex;

	public void Set(Sprite chara, int id)
	{
		int id2 = Singleton<DataManager>.Instance.GetChara(id).color.id;
		Color24 colorDark = Singleton<DataManager>.Instance.GetMapColorData(id2).ColorDark;
		Color color = new Color((float)(int)colorDark.R / 255f, (float)(int)colorDark.G / 255f, (float)(int)colorDark.B / 255f);
		_charaImage.sprite = chara;
		_shadow.Image2 = chara;
		_shadow.color = color;
		if (null == _awakeIconController)
		{
			_awakeIconController = UnityEngine.Object.Instantiate(_awakeStarPrefab, _awakeRoot.transform).GetComponent<AwakeIconController>();
			_awakeIconController.name = _awakeStarPrefab.name;
			_awakeAnimator = _awakeIconController.gameObject.GetComponent<Animator>();
		}
	}

	public override void Play(Action onAction)
	{
		_awakeAnimator?.Play(HashCodeIn);
		base.Play(onAction);
	}

	public void Prepare(Sprite small, Sprite large, int meter, float gaugeAmount, int awakeNum)
	{
		_odoMeters.SetDistance(meter);
		_awakeIconController.AwakePrepare(small, large, gaugeAmount, awakeNum);
	}

	public void SetMeter(int meter)
	{
		_odoMeters.SetDistance(meter);
	}

	public void SetGauge(float gaugeAmount)
	{
		_awakeIconController.PutStar(gaugeAmount);
	}

	public void FadeIn()
	{
		_animator.Play(HashCodeIn);
		IsCanSkip = true;
	}

	public void SparkStar()
	{
		_awakeIconController.AnimCenterSpark();
	}

	public void StarGet(int starIndex)
	{
		_awakeIconController.AnimStarGet(starIndex);
	}

	public override bool Skip()
	{
		if (IsCanSkip)
		{
			_stateController.ResetExitParts();
			_animator.Play("Loop", 0, 0f);
			IsCanSkip = false;
			return true;
		}
		return false;
	}

	private void Close()
	{
		_animator.Play("Out");
	}
}
