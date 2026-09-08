namespace System.Net;

internal interface ISafeHandleCachable
{
	bool TryAddRentCount();

	bool TryMarkForDispose();
}
