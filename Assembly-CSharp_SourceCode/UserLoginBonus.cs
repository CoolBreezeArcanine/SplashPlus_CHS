public class UserLoginBonus
{
	public int ID { get; set; }

	public string Name { get; set; }

	public uint Point { get; set; }

	public bool IsCurrent { get; set; }

	public bool IsComplete { get; set; }

	public UserLoginBonus()
	{
		Clear();
	}

	public UserLoginBonus(int id)
	{
		ID = id;
		Clear();
	}

	public UserLoginBonus(int id, uint point, bool isCurrent, bool isComplete)
	{
		ID = id;
		Point = point;
		IsCurrent = isCurrent;
		IsComplete = isComplete;
	}

	public void Clear()
	{
		Point = 0u;
		IsCurrent = false;
		IsComplete = false;
	}
}
