using System.Collections.Generic;

namespace System.Diagnostics.Metrics;

public class MeterOptions
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

	public string? Version { get; set; }

	public IEnumerable<KeyValuePair<string, object?>>? Tags { get; set; }

	public object? Scope { get; set; }

	public string? TelemetrySchemaUrl { get; set; }

	public MeterOptions(string name)
	{
		Name = name;
	}
}
