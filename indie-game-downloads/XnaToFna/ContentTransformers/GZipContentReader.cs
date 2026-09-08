using System.IO.Compression;
using Microsoft.Xna.Framework.Content;

namespace XnaToFna.ContentTransformers;

public class GZipContentReader<ContentType> : ContentTypeReader<ContentType>
{
	private ForcedStreamContentManager WrappedContentManager;

	protected override ContentType Read(ContentReader input, ContentType existing)
	{
		if (WrappedContentManager == null)
		{
			WrappedContentManager = new ForcedStreamContentManager(input.ContentManager.ServiceProvider);
		}
		WrappedContentManager.RootDirectory = input.ContentManager.RootDirectory;
		WrappedContentManager.Stream = new GZipStream(input.BaseStream, CompressionMode.Decompress, leaveOpen: true);
		bool enabled = FNAHooks.Enabled;
		FNAHooks.Enabled = false;
		ContentType result = WrappedContentManager.Load<ContentType>(input.AssetName);
		FNAHooks.Enabled = enabled;
		return result;
	}
}
