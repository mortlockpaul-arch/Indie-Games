namespace System.Reflection;

internal sealed class CustomAttributeCtorParameter(CustomAttributeType type)
{
	public CustomAttributeType CustomAttributeType => type;

	public CustomAttributeEncodedArgument EncodedArgument { get; set; }
}
