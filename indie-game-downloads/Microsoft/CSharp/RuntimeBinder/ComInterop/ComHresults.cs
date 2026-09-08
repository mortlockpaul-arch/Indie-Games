namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class ComHresults
{
	internal static bool IsSuccess(int hresult)
	{
		return hresult >= 0;
	}
}
