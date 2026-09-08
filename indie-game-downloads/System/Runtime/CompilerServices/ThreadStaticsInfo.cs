namespace System.Runtime.CompilerServices;

internal ref struct ThreadStaticsInfo
{
	internal int _nonGCTlsIndex;

	internal int _gcTlsIndex;

	internal GenericsStaticsInfo _genericStatics;
}
