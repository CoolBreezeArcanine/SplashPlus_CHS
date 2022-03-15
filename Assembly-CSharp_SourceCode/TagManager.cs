using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class TagManager
{
	private TagList _tagList;

	public TagManager()
	{
		Init();
	}

	private void Init()
	{
		string text = Load("FontOutTagList");
		LoadFromText(text);
	}

	private void LoadTagListData()
	{
	}

	public TagTip[] GetTagTipArray()
	{
		return _tagList.TagTips;
	}

	public List<TagTip> GetTagTipList()
	{
		return _tagList.TagTips.ToList();
	}

	private string Load(string filePath)
	{
		TextAsset textAsset = Resources.Load(filePath) as TextAsset;
		if (textAsset == null)
		{
			return "";
		}
		return textAsset.text;
	}

	private void LoadFromText(string text)
	{
		try
		{
			using StringReader textReader = new StringReader(text);
			TagList tagList = (TagList)new XmlSerializer(typeof(TagList)).Deserialize(textReader);
			if (tagList != null)
			{
				_tagList = tagList;
			}
		}
		catch (Exception)
		{
		}
	}

	private void LoadFromPath(string filePath)
	{
		try
		{
			using FileStream stream = new FileStream(filePath, FileMode.Open);
			TagList tagList = (TagList)new XmlSerializer(typeof(TagList)).Deserialize(stream);
			if (tagList != null)
			{
				_tagList = tagList;
			}
		}
		catch (Exception)
		{
		}
	}

	public void SaveFontOutTagList()
	{
		List<TagTip> list = new List<TagTip>();
		list.Add(new TagTip("AlignCenter(中央表示)", "<align=center>", "</align>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("AlignLeft(左寄せ)", "<align=left>", "</align>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("AlignRight(右寄せ)", "<align=right>", "</align>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("Bold(太字)", "<b>", "</b>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("Italics(斜体)", "<i>", "</i>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("UnderLine(アンダーライン)", "<u>", "</u>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("StrikeThrough(打消し)", "<s>", "</s>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("Superscript(上付き文字)", "<sup>", "</sup>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("Subscript(下付き文字)", "<sub>", "</sub>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("ColorMarker(マーカーを引く)", "<mark=#ffff00aa>", "</mark>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("(文字に色を付ける)", "<color=#ff0000>", "</color>", TagTip.TagCategory.Color));
		list.Add(new TagTip("AllCaps全部大文字表示", "<allcaps>", "</allcaps>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("SmallTips(小文字と同じ高さの大文字)", "<smallcaps>", "</smallcaps>", TagTip.TagCategory.Normal));
		list.Add(new TagTip("Picto-Default(通常絵文字)", "<sprite name=>", "", TagTip.TagCategory.Picto));
		list.Add(new TagTip("H1", "<size=2em><b><#40ff80>*", "*</size></b></color>", TagTip.TagCategory.Style));
		list.Add(new TagTip("Quote", "<i><size=75%><margin=10%>", "</i></size></width></margin>", TagTip.TagCategory.Style));
		list.Add(new TagTip("Title", "<size=125%><b><align=center>", "</size></b></align>", TagTip.TagCategory.Style));
		list.Add(new TagTip("H2", "<size=1.5em><b><#4080FF>", "</size></b></color>", TagTip.TagCategory.Style));
		list.Add(new TagTip("H3", "<size=1.17em><b><#FF8040>", "*</size></b></color>", TagTip.TagCategory.Style));
		list.Add(new TagTip("C1", "<color=#ffff40>", "</color>", TagTip.TagCategory.Style));
		list.Add(new TagTip("C2", "<color=#ff40FF><size=125%>", "</color></size>", TagTip.TagCategory.Style));
		list.Add(new TagTip("C3", "<color=#80A0FF><b>", "</color></b>", TagTip.TagCategory.Style));
		TagList tagList = new TagList();
		tagList.TagTips = list.ToArray();
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(TagList));
		StreamWriter streamWriter = new StreamWriter("FontOutTagList.xml", append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		xmlSerializer.Serialize(streamWriter, tagList);
		streamWriter.Close();
	}
}
