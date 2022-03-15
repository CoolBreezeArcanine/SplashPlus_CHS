using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ProjectPrefsAttribute : Attribute
{
	public string label { get; set; }

	public string tooltip { get; set; }

	public string group { get; set; }

	public Type type { get; set; }

	public ProjectPrefsAttribute(string label, string tooltip, string group, Type type)
	{
		this.label = label;
		this.tooltip = tooltip;
		this.group = group;
		this.type = type;
	}
}
