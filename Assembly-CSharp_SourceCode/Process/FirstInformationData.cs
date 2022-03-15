using DB;
using Mai2.Voice_000001;

namespace Process
{
	public class FirstInformationData
	{
		public int SkipButtonTime;

		public WindowMessageID MessageID;

		public Cue VoiceCue;

		public FirstInformationData(WindowMessageID messageID, Cue voiceCue, int skipTime)
		{
			MessageID = messageID;
			VoiceCue = voiceCue;
			SkipButtonTime = skipTime;
		}

		public FirstInformationData(WindowMessageID messageID, int skipTime)
		{
			MessageID = messageID;
			SkipButtonTime = skipTime;
			VoiceCue = Cue.VO_000228;
		}
	}
}
