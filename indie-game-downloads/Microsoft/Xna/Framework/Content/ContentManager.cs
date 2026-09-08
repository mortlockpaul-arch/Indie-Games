#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Utilities;

namespace Microsoft.Xna.Framework.Content;

public class ContentManager : IDisposable
{
	private string rootDirectory;

	private GraphicsDevice graphicsDevice;

	private Dictionary<string, object> loadedAssets = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

	private List<IDisposable> disposableAssets = new List<IDisposable>();

	private bool disposed;

	private static object ContentManagerLock = new object();

	private static List<WeakReference> ContentManagers = new List<WeakReference>();

	private static readonly byte[] xnbHeader = new byte[4];

	private static List<char> targetPlatformIdentifiers = new List<char>
	{
		'w', 'x', 'm', 'i', 'a', 'd', 'X', 'W', 'n', 'u',
		'p', 'M', 'r', 'P', 'g', 'l'
	};

	private static readonly string[] effectExtensions = new string[1] { ".fxb" };

	private static readonly string[] texture2DExtensions = new string[10] { ".png", ".jpg", ".jpeg", ".dds", ".qoi", ".bmp", ".gif", ".tga", ".tif", ".tiff" };

	private static readonly string[] textureCubeExtensions = new string[1] { ".dds" };

	private static readonly string[] soundEffectExtensions = new string[1] { ".wav" };

	public IServiceProvider ServiceProvider { get; private set; }

	public string RootDirectory
	{
		get
		{
			return rootDirectory;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (loadedAssets.Count > 0)
			{
				throw new InvalidOperationException("This property cannot be changed after content has been loaded into the ContentManager.");
			}
			rootDirectory = value;
		}
	}

	internal string RootDirectoryFullPath
	{
		get
		{
			if (Path.IsPathRooted(RootDirectory))
			{
				return RootDirectory;
			}
			return Path.Combine(TitleLocation.Path, RootDirectory);
		}
	}

	public ContentManager(IServiceProvider serviceProvider)
	{
		if (serviceProvider == null)
		{
			throw new ArgumentNullException("serviceProvider");
		}
		ServiceProvider = serviceProvider;
		RootDirectory = string.Empty;
		AddContentManager(this);
	}

	public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
	{
		if (serviceProvider == null)
		{
			throw new ArgumentNullException("serviceProvider");
		}
		if (rootDirectory == null)
		{
			throw new ArgumentNullException("rootDirectory");
		}
		ServiceProvider = serviceProvider;
		RootDirectory = rootDirectory;
		AddContentManager(this);
	}

	~ContentManager()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
		RemoveContentManager(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				Unload();
			}
			disposed = true;
		}
	}

	public virtual T Load<T>(string assetName)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(ToString());
		}
		if (string.IsNullOrEmpty(assetName))
		{
			throw new ArgumentNullException("assetName");
		}
		T val = default(T);
		string key = assetName.Replace('\\', '/');
		if (loadedAssets.TryGetValue(key, out var value))
		{
			if (!(value is T result))
			{
				throw new ContentLoadException("Error loading \"" + assetName + "\". File contains " + value.GetType()?.ToString() + " but trying to load as " + typeof(T)?.ToString() + ".");
			}
			return result;
		}
		val = ReadAsset<T>(assetName, null);
		loadedAssets[key] = val;
		return val;
	}

	public virtual void Unload()
	{
		foreach (IDisposable disposableAsset in disposableAssets)
		{
			disposableAsset?.Dispose();
		}
		disposableAssets.Clear();
		loadedAssets.Clear();
	}

	protected virtual Stream OpenStream(string assetName)
	{
		try
		{
			return TitleContainer.OpenStream(Path.Combine(RootDirectory, assetName) + ".xnb");
		}
		catch (FileNotFoundException innerException)
		{
			throw new ContentLoadException("Error loading \"" + assetName + "\". File not found.", innerException);
		}
		catch (DirectoryNotFoundException innerException2)
		{
			throw new ContentLoadException("Error loading \"" + assetName + "\". Directory not found.", innerException2);
		}
		catch (Exception innerException3)
		{
			throw new ContentLoadException("Error loading \"" + assetName + "\". Cannot open file.", innerException3);
		}
	}

	protected T ReadAsset<T>(string assetName, Action<IDisposable> recordDisposableObject)
	{
		if (disposed)
		{
			throw new ObjectDisposedException(ToString());
		}
		if (string.IsNullOrEmpty(assetName))
		{
			throw new ArgumentNullException("assetName");
		}
		object obj = null;
		Stream stream;
		try
		{
			stream = OpenStream(assetName);
		}
		catch (Exception ex)
		{
			stream = OpenStreamRaw<T>(assetName);
			if (stream == null)
			{
				throw new ContentLoadException("Could not load asset " + assetName + "! Error: " + ex.Message, ex);
			}
		}
		stream.Read(xnbHeader, 0, xnbHeader.Length);
		if (xnbHeader[0] == 88 && xnbHeader[1] == 78 && xnbHeader[2] == 66 && targetPlatformIdentifiers.Contains((char)xnbHeader[3]))
		{
			using BinaryReader xnbReader = new BinaryReader(stream);
			using ContentReader contentReader = GetContentReaderFromXnb(assetName, ref stream, xnbReader, (char)xnbHeader[3], recordDisposableObject);
			obj = contentReader.ReadAsset<T>();
			if (obj is GraphicsResource graphicsResource)
			{
				graphicsResource.Name = assetName;
			}
		}
		else
		{
			stream.Seek(0L, SeekOrigin.Begin);
			if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Texture))
			{
				Texture2D texture2D = ((xnbHeader[0] != 68 || xnbHeader[1] != 68 || xnbHeader[2] != 83 || xnbHeader[3] != 32) ? Texture2D.FromStream(GetGraphicsDevice(), stream) : Texture2D.DDSFromStreamEXT(GetGraphicsDevice(), stream));
				texture2D.Name = assetName;
				obj = texture2D;
			}
			else if (typeof(T) == typeof(TextureCube))
			{
				TextureCube textureCube = TextureCube.DDSFromStreamEXT(GetGraphicsDevice(), stream);
				textureCube.Name = assetName;
				obj = textureCube;
			}
			else if (typeof(T) == typeof(SoundEffect))
			{
				SoundEffect soundEffect = SoundEffect.FromStream(stream);
				soundEffect.Name = assetName;
				obj = soundEffect;
			}
			else if (typeof(T) == typeof(Effect))
			{
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, (int)stream.Length);
				obj = new Effect(GetGraphicsDevice(), array)
				{
					Name = assetName
				};
			}
			else if (typeof(T) == typeof(Song))
			{
				string name = (stream as FileStream).Name;
				stream.Close();
				obj = new Song(name);
			}
			else
			{
				if (!(typeof(T) == typeof(Video)))
				{
					stream.Close();
					throw new ContentLoadException("Could not load " + assetName + " asset!");
				}
				string name2 = (stream as FileStream).Name;
				stream.Close();
				obj = new Video(name2, GetGraphicsDevice());
				FNALoggerEXT.LogWarn("Video " + name2 + " does not have an XNB file! Hacking Duration property!");
			}
			if (obj is IDisposable disposable)
			{
				if (recordDisposableObject != null)
				{
					recordDisposableObject(disposable);
				}
				else
				{
					disposableAssets.Add(disposable);
				}
			}
			stream.Close();
		}
		return (T)obj;
	}

	internal void RecordDisposable(IDisposable disposable)
	{
		Debug.Assert(disposable != null, "The disposable is null!");
		if (!disposableAssets.Contains(disposable))
		{
			disposableAssets.Add(disposable);
		}
	}

	internal GraphicsDevice GetGraphicsDevice()
	{
		if (graphicsDevice == null)
		{
			if (!(ServiceProvider.GetService(typeof(IGraphicsDeviceService)) is IGraphicsDeviceService graphicsDeviceService))
			{
				throw new ContentLoadException("No Graphics Device Service");
			}
			graphicsDevice = graphicsDeviceService.GraphicsDevice;
		}
		return graphicsDevice;
	}

	private ContentReader GetContentReaderFromXnb(string originalAssetName, ref Stream stream, BinaryReader xnbReader, char platform, Action<IDisposable> recordDisposableObject)
	{
		byte b = xnbReader.ReadByte();
		byte b2 = xnbReader.ReadByte();
		bool flag = (b2 & 0x80) != 0;
		if (b != 5 && b != 4)
		{
			throw new ContentLoadException("Invalid XNB version");
		}
		int num = xnbReader.ReadInt32();
		if (flag)
		{
			int num2 = num - 14;
			int num3 = xnbReader.ReadInt32();
			MemoryStream memoryStream = new MemoryStream(new byte[num3], 0, num3, writable: true, publiclyVisible: true);
			MemoryStream memoryStream2 = new MemoryStream(new byte[num2], 0, num2, writable: true, publiclyVisible: true);
			stream.Read(memoryStream2.GetBuffer(), 0, num2);
			LzxDecoder lzxDecoder = new LzxDecoder(16);
			int num4 = 0;
			long num5 = 0L;
			while (num5 < num2)
			{
				int num6 = memoryStream2.ReadByte();
				int num7 = memoryStream2.ReadByte();
				int num8 = (num6 << 8) | num7;
				int num9 = 32768;
				if (num6 == 255)
				{
					num6 = num7;
					num7 = (byte)memoryStream2.ReadByte();
					num9 = (num6 << 8) | num7;
					num6 = (byte)memoryStream2.ReadByte();
					num7 = (byte)memoryStream2.ReadByte();
					num8 = (num6 << 8) | num7;
					num5 += 5;
				}
				else
				{
					num5 += 2;
				}
				if (num8 == 0 || num9 == 0)
				{
					break;
				}
				lzxDecoder.Decompress(memoryStream2, num8, memoryStream, num9);
				num5 += num8;
				num4 += num9;
				memoryStream2.Seek(num5, SeekOrigin.Begin);
			}
			if (memoryStream.Position != num3)
			{
				throw new ContentLoadException("Decompression of " + originalAssetName + " failed. ");
			}
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return new ContentReader(this, memoryStream, originalAssetName, b, platform, recordDisposableObject);
		}
		return new ContentReader(this, stream, originalAssetName, b, platform, recordDisposableObject);
	}

	private Stream CheckRawExtensions(string assetName, string[] extensions)
	{
		string text = FileHelpers.NormalizeFilePathSeparators(Path.Combine(RootDirectoryFullPath, assetName));
		if (File.Exists(text))
		{
			return TitleContainer.OpenStream(text);
		}
		foreach (string text2 in extensions)
		{
			string text3 = text + text2;
			if (File.Exists(text3))
			{
				return TitleContainer.OpenStream(text3);
			}
		}
		text = FileHelpers.NormalizeFilePathSeparators(assetName);
		try
		{
			return OpenStream(text);
		}
		catch
		{
			foreach (string text4 in extensions)
			{
				string assetName2 = text + text4;
				try
				{
					return OpenStream(assetName2);
				}
				catch
				{
				}
			}
		}
		return null;
	}

	private Stream OpenStreamRaw<T>(string assetName)
	{
		if (typeof(T) == typeof(Texture2D) || typeof(T) == typeof(Texture))
		{
			return CheckRawExtensions(assetName, texture2DExtensions);
		}
		if (typeof(T) == typeof(TextureCube))
		{
			return CheckRawExtensions(assetName, textureCubeExtensions);
		}
		if (typeof(T) == typeof(SoundEffect))
		{
			return CheckRawExtensions(assetName, soundEffectExtensions);
		}
		if (typeof(T) == typeof(Effect))
		{
			return CheckRawExtensions(assetName, effectExtensions);
		}
		if (typeof(T) == typeof(Song))
		{
			return CheckRawExtensions(assetName, SongReader.supportedExtensions);
		}
		if (typeof(T) == typeof(Video))
		{
			return CheckRawExtensions(assetName, VideoReader.supportedExtensions);
		}
		return null;
	}

	private static void AddContentManager(ContentManager contentManager)
	{
		lock (ContentManagerLock)
		{
			bool flag = false;
			for (int num = ContentManagers.Count - 1; num >= 0; num--)
			{
				WeakReference weakReference = ContentManagers[num];
				if (weakReference.Target == contentManager)
				{
					flag = true;
				}
				if (!weakReference.IsAlive)
				{
					ContentManagers.RemoveAt(num);
				}
			}
			if (!flag)
			{
				ContentManagers.Add(new WeakReference(contentManager));
			}
		}
	}

	private static void RemoveContentManager(ContentManager contentManager)
	{
		lock (ContentManagerLock)
		{
			for (int num = ContentManagers.Count - 1; num >= 0; num--)
			{
				WeakReference weakReference = ContentManagers[num];
				if (!weakReference.IsAlive || weakReference.Target == contentManager)
				{
					ContentManagers.RemoveAt(num);
				}
			}
		}
	}
}
