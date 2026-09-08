using System;

namespace Microsoft.VisualBasic;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ComClassAttribute : Attribute
{
	public string ClassID { get; }

	public string InterfaceID { get; }

	public string EventID { get; }

	public bool InterfaceShadows { get; set; }

	public ComClassAttribute()
	{
	}

	public ComClassAttribute(string _ClassID)
	{
		this.ClassID = _ClassID;
	}

	public ComClassAttribute(string _ClassID, string _InterfaceID)
	{
		this.ClassID = _ClassID;
		this.InterfaceID = _InterfaceID;
	}

	public ComClassAttribute(string _ClassID, string _InterfaceID, string _EventId)
	{
		this.ClassID = _ClassID;
		this.InterfaceID = _InterfaceID;
		EventID = _EventId;
	}
}
