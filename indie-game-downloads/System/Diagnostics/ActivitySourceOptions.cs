using System.Collections.Generic;

namespace System.Diagnostics;

public class ActivitySourceOptions
{
	private string _name;

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value ?? throw new ArgumentNullException("value");
		}
	}

	public string? Version { get; set; } = string.Empty;

	public IEnumerable<KeyValuePair<string, object?>>? Tags { get; set; }

	public string? TelemetrySchemaUrl { get; set; }

	public ActivitySourceOptions(string name)
	{
		_name = name ?? throw new ArgumentNullException("name");
	}
}
