using DB;
using UnityEngine;

public class FixedWindow : CommonWindow
{
	public void Prepare(WindowMessageID id, Vector3 position)
	{
		CommonPrepare(id, id.GetPosition());
		SetPosition(position);
	}
}
