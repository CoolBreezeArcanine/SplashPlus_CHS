using System;
using System.Text;

[Serializable]
public class TagTip
{
	public enum TagCategory
	{
		Normal,
		Picto,
		Color,
		Style
	}

	public string Name { get; set; }

	public string OpeningTag { get; set; }

	public string ClosingTag { get; set; }

	public TagCategory Category { get; set; }

	public TagTip()
	{
	}

	public TagTip(string name, string opening, string closing, TagCategory category)
	{
		Name = name;
		OpeningTag = opening;
		ClosingTag = closing;
		Category = category;
	}

	public string ActualTag()
	{
		return OpeningTag + ClosingTag;
	}

	public string ActualTag(string insertString)
	{
		if (Category != TagCategory.Picto)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < OpeningTag.Length; i++)
		{
			stringBuilder.Append(OpeningTag[i]);
			if (OpeningTag[i].ToString() == "\"")
			{
				break;
			}
		}
		for (int j = 0; j < insertString.Length; j++)
		{
			stringBuilder.Append(insertString[j]);
		}
		stringBuilder.Append("\">");
		return stringBuilder.ToString();
	}

	public int CloseTagLength()
	{
		return ClosingTag.Length;
	}
}
