namespace Process.SubSequence
{
	public class CharacterSelectSequence : SequenceBase
	{
		public CharacterSelectSequence(int index, bool isValidity, IMusicSelectProcessProcessing processing)
			: base(index, isValidity, processing)
		{
		}

		public override void Initialize()
		{
		}

		public override void OnStartSequence()
		{
			ProcessProcessing.CharacterSelectReset(PlayerIndex);
		}
	}
}
