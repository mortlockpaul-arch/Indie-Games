using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Media;

public sealed class MediaSource
{
	public MediaSourceType MediaSourceType
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public string Name
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal MediaSource()
	{
		throw new NotImplementedException();
	}

	public static IList<MediaSource> GetAvailableMediaSources()
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		throw new NotImplementedException();
	}
}
