namespace Monitor
{
	public class JudgeTouchGrade : JudgeGrade
	{
		protected override void Awake()
		{
			base.Awake();
			SpriteRenderAdd = null;
			base.gameObject.SetActive(value: false);
		}

		public override void SetLedSetting(int buttonIndex, int monitorIndex)
		{
		}

		protected override bool IsDispPosition()
		{
			return _dispPos != 0;
		}
	}
}
