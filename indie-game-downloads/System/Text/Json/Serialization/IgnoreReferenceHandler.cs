namespace System.Text.Json.Serialization;

internal sealed class IgnoreReferenceHandler : ReferenceHandler
{
	public IgnoreReferenceHandler()
	{
		HandlingStrategy = JsonKnownReferenceHandler.IgnoreCycles;
	}

	public override ReferenceResolver CreateResolver()
	{
		return new IgnoreReferenceResolver();
	}
}
