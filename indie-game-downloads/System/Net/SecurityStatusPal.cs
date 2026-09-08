namespace System.Net;

internal readonly struct SecurityStatusPal(SecurityStatusPalErrorCode errorCode, Exception exception = null)
{
	public readonly SecurityStatusPalErrorCode ErrorCode = errorCode;

	public readonly Exception Exception = exception;

	public override string ToString()
	{
		if (Exception == null)
		{
			return $"{"ErrorCode"}={ErrorCode}";
		}
		return $"{"ErrorCode"}={ErrorCode}, {"Exception"}={Exception}";
	}
}
