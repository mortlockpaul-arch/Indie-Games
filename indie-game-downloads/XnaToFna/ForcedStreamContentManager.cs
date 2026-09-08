using System;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace XnaToFna;

public class ForcedStreamContentManager : ContentManager
{
	public Stream Stream;

	public ForcedStreamContentManager(IServiceProvider serviceProvider)
		: base(serviceProvider)
	{
	}

	protected override Stream OpenStream(string assetName)
	{
		Stream result;
		if (Stream != null)
		{
			result = Stream;
			Stream = null;
		}
		else
		{
			result = base.OpenStream(assetName);
		}
		return result;
	}
}
