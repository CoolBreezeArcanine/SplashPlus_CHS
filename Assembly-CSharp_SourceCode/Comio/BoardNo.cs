namespace Comio
{
	public class BoardNo
	{
		public string Text;

		public BoardNo(string text = "")
		{
			Text = text;
		}

		public void Clear()
		{
			Text = "";
		}

		public bool IsEqual(BoardNo no)
		{
			return Text == no.Text;
		}
	}
}
