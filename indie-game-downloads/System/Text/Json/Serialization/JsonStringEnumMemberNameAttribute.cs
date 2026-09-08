namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class JsonStringEnumMemberNameAttribute : Attribute
{
	public string Name { get; }

	public JsonStringEnumMemberNameAttribute(string name)
	{
		Name = name;
	}
}
