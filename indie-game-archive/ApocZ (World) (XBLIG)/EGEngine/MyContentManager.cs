using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace EGEngine;

public class MyContentManager : ContentManager
{
	public static bool LoadingTexture;

	public static bool CanLoadTexture;

	public static long TotalTextureBytes;

	public static long TotalVertexBytes;

	public static long TotalIndexBytes;

	private bool generateMipmaps;

	private Dictionary<string, object> loadedAssets = new Dictionary<string, object>();

	private List<IDisposable> disposableAssets = new List<IDisposable>();

	public bool GenerateMipmaps
	{
		get
		{
			return generateMipmaps;
		}
		set
		{
			generateMipmaps = value;
		}
	}

	public MyContentManager(IServiceProvider serviceProvider)
		: base(serviceProvider)
	{
	}

	public MyContentManager(IServiceProvider services, string rootDirectory)
		: base(services, rootDirectory)
	{
	}

	public void OutputMemoryUse()
	{
	}
}
