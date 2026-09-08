namespace System.Reflection;

internal sealed class CustomAttributeNamedParameter(MemberInfo memberInfo, CustomAttributeEncoding fieldOrProperty, CustomAttributeType type)
{
	public MemberInfo MemberInfo => memberInfo;

	public CustomAttributeType CustomAttributeType => type;

	public CustomAttributeEncodedArgument EncodedArgument { get; set; }
}
