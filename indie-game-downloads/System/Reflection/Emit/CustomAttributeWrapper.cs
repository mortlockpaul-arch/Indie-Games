namespace System.Reflection.Emit;

internal readonly struct CustomAttributeWrapper(ConstructorInfo constructorInfo, ReadOnlySpan<byte> binaryAttribute)
{
	private readonly ConstructorInfo _constructorInfo = constructorInfo;

	private readonly byte[] _binaryAttribute = binaryAttribute.ToArray();

	public ConstructorInfo Ctor => _constructorInfo;

	public byte[] Data => _binaryAttribute;
}
