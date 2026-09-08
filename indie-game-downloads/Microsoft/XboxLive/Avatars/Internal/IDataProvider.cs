namespace Microsoft.XboxLive.Avatars.Internal;

public interface IDataProvider
{
	void GetDataAsync(DataRequest request, DataProvider dataProvider);

	void CancelAsync();
}
