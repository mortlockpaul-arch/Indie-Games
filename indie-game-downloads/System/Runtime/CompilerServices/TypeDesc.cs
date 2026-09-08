namespace System.Runtime.CompilerServices;

internal struct TypeDesc
{
	private uint _typeAndFlags;

	private nint _exposedClassObject;

	public unsafe RuntimeType ExposedClassObject => Unsafe.Read<RuntimeType>(Unsafe.AsPointer(in _exposedClassObject));
}
