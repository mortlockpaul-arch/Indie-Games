namespace System.Net.Http.Headers;

internal struct HeaderEntry(HeaderDescriptor key, object value)
{
	public HeaderDescriptor Key = key;

	public object Value = value;
}
