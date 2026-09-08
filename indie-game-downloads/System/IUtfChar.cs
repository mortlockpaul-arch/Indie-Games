namespace System;

internal interface IUtfChar<TSelf> : IEquatable<TSelf> where TSelf : unmanaged, System.IUtfChar<TSelf>
{
	static abstract TSelf CastFrom(byte value);

	static abstract TSelf CastFrom(char value);

	static abstract TSelf CastFrom(int value);

	static abstract TSelf CastFrom(uint value);

	static abstract TSelf CastFrom(ulong value);

	static abstract uint CastToUInt32(TSelf value);
}
