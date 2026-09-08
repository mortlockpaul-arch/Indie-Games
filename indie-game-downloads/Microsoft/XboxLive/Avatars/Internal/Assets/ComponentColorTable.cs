namespace Microsoft.XboxLive.Avatars.Internal.Assets;

public class ComponentColorTable
{
	public ComponentColors[] Colors;

	public int GetMemoryUsage()
	{
		return Colors.Length * 3 * 16;
	}
}
