using UnityEngine;

[CreateAssetMenu(menuName = "Mai2Data/LedColorTable", fileName = "ParameterTable")]
public class LedColorTable : ScriptableObject
{
	[SerializeField]
	[Header("ボタンLED色")]
	private Color btn_main_color;

	[SerializeField]
	private Color btn_sub_color;

	[SerializeField]
	private Color btn_yes_color;

	[SerializeField]
	private Color btn_no_color;

	[SerializeField]
	private Color btn_reaction_color;

	[SerializeField]
	private Color btn_attention_color;

	[SerializeField]
	private Color btn_detailsetting_color;

	[SerializeField]
	private Color btn_timeskip_color;

	[SerializeField]
	private Color btn_upload_color;

	[SerializeField]
	private Color btn_critical_color;

	[SerializeField]
	private Color btn_perfect_color;

	[SerializeField]
	private Color btn_great_color;

	[SerializeField]
	private Color btn_good_color;

	[SerializeField]
	[Header("難易度別ボタンLED色")]
	private Color btn_basic_color;

	[SerializeField]
	private Color btn_advanced_color;

	[SerializeField]
	private Color btn_expert_color;

	[SerializeField]
	private Color btn_master_color;

	[SerializeField]
	private Color btn_remaster_color;

	[SerializeField]
	[Header("ビルボードLED色")]
	private Color bb_main_color;

	[SerializeField]
	private Color bb_photo_color;

	public Color ButtonMainColor
	{
		get
		{
			return btn_main_color;
		}
		private set
		{
		}
	}

	public Color ButtonSubColor
	{
		get
		{
			return btn_sub_color;
		}
		private set
		{
		}
	}

	public Color ButtonYesColor
	{
		get
		{
			return btn_yes_color;
		}
		private set
		{
		}
	}

	public Color ButtonNoColor
	{
		get
		{
			return btn_no_color;
		}
		private set
		{
		}
	}

	public Color ButtonReactionColor
	{
		get
		{
			return btn_reaction_color;
		}
		private set
		{
		}
	}

	public Color ButtonAttentionColor
	{
		get
		{
			return btn_attention_color;
		}
		private set
		{
		}
	}

	public Color ButtonDetailSettingColor
	{
		get
		{
			return btn_detailsetting_color;
		}
		private set
		{
		}
	}

	public Color ButtonTimeSkipColor
	{
		get
		{
			return btn_timeskip_color;
		}
		private set
		{
		}
	}

	public Color ButtonUploadColor
	{
		get
		{
			return btn_upload_color;
		}
		private set
		{
		}
	}

	public Color ButtonCriticalColor
	{
		get
		{
			return btn_critical_color;
		}
		private set
		{
		}
	}

	public Color ButtonPerfectColor
	{
		get
		{
			return btn_perfect_color;
		}
		private set
		{
		}
	}

	public Color ButtonGreatColor
	{
		get
		{
			return btn_great_color;
		}
		private set
		{
		}
	}

	public Color ButtonGoodColor
	{
		get
		{
			return btn_good_color;
		}
		private set
		{
		}
	}

	public Color ButtonBasicColor
	{
		get
		{
			return btn_basic_color;
		}
		private set
		{
		}
	}

	public Color ButtonAdvancesColor
	{
		get
		{
			return btn_advanced_color;
		}
		private set
		{
		}
	}

	public Color ButtonExpertColor
	{
		get
		{
			return btn_expert_color;
		}
		private set
		{
		}
	}

	public Color ButtonMasterColor
	{
		get
		{
			return btn_master_color;
		}
		private set
		{
		}
	}

	public Color ButtonReMasterColor
	{
		get
		{
			return btn_remaster_color;
		}
		private set
		{
		}
	}

	public Color BillboardMainColor
	{
		get
		{
			return bb_main_color;
		}
		private set
		{
		}
	}

	public Color BillboardPhotoColor
	{
		get
		{
			return bb_photo_color;
		}
		private set
		{
		}
	}
}
