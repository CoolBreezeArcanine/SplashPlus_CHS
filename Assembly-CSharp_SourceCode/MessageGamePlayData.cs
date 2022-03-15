public class MessageGamePlayData
{
	public uint Score { get; private set; }

	public uint Rank { get; private set; }

	public uint Sync { get; private set; }

	public int Diff { get; private set; }

	public MessageGamePlayData(uint score, uint rank, uint sync, int diff)
	{
		SetData(score, rank, sync, diff);
	}

	public void SetData(uint score, uint rank, uint sync, int diff)
	{
		Score = score;
		Rank = rank;
		Sync = sync;
		Diff = diff;
	}

	public void SetData(MessageGamePlayData data)
	{
		Score = data.Score;
		Rank = data.Rank;
		Sync = data.Sync;
		Diff = data.Diff;
	}

	public static bool operator ==(MessageGamePlayData a, MessageGamePlayData b)
	{
		if (a.Score == b.Score && a.Rank == b.Rank && a.Sync == b.Sync)
		{
			return a.Diff == b.Diff;
		}
		return false;
	}

	public static bool operator !=(MessageGamePlayData a, MessageGamePlayData b)
	{
		if (a.Score == b.Score && a.Rank == b.Rank && a.Sync == b.Sync)
		{
			return a.Diff != b.Diff;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
