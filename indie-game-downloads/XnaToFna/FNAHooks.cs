using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using XnaToFna.ContentTransformers;

namespace XnaToFna;

public static class FNAHooks
{
	public delegate ContentReader d_GetContentReaderFromXnb(ContentManager self, string originalAssetName, ref Stream stream, BinaryReader xnbReader, char platform, Action<IDisposable> recordDisposableObject);

	public delegate void d_ctor_ContentReader(ContentReader self, ContentManager manager, Stream stream, GraphicsDevice graphicsDevice, string assetName, int version, char platform, Action<IDisposable> recordDisposableObject);

	public static bool Enabled = true;

	private static bool Hooked = false;

	public static d_GetContentReaderFromXnb orig_GetContentReaderFromXnb;

	public static Detour h_ctor_ContentReader;

	public static d_ctor_ContentReader orig_ctor_ContentReader;

	public static void Hook()
	{
		if (Hooked)
		{
			Hooked = true;
			Dictionary<string, Func<ContentTypeReader>> obj = (Dictionary<string, Func<ContentTypeReader>>)typeof(ContentTypeReaderManager).GetField("typeCreators", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			obj["Microsoft.Xna.Framework.Content.EffectReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553"] = () => new EffectTransformer();
			obj["Microsoft.Xna.Framework.Content.SoundEffectReader"] = () => new SoundEffectTransformer();
			Hook<d_GetContentReaderFromXnb>(typeof(ContentManager), out orig_GetContentReaderFromXnb);
			Hook<d_ctor_ContentReader>(typeof(ContentReader), out orig_ctor_ContentReader);
		}
	}

	internal static MethodBase Find(Type type, string name, List<Type> argTypes, bool hasSelf = true)
	{
		Type[] types = argTypes.ToArray();
		MethodBase method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
		if (method != null)
		{
			return method;
		}
		if (name.StartsWith("ctor_"))
		{
			method = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
			if (method != null)
			{
				return method;
			}
		}
		if (name.StartsWith("get_") || name.StartsWith("set_"))
		{
			PropertyInfo property = type.GetProperty(name.Substring(4), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			method = ((name[0] != 'g') ? property.GetSetMethod(nonPublic: true) : property.GetGetMethod(nonPublic: true));
			if (method != null)
			{
				return method;
			}
		}
		argTypes.RemoveAt(0);
		return Find(type, name, argTypes, hasSelf: false);
	}

	internal static Detour Hook<T>(Type type, out T trampoline) where T : class
	{
		string name = typeof(T).Name.Substring(2);
		List<Type> argTypes = (from arg in typeof(T).GetMethod("Invoke").GetParameters()
			select arg.ParameterType).ToList();
		MethodBase to = Find(typeof(T).DeclaringType, name, argTypes);
		Detour detour = new Detour(Find(type, name, argTypes), to);
		trampoline = detour.GenerateTrampoline<T>();
		return detour;
	}

	public static ContentReader GetContentReaderFromXnb(ContentManager self, string originalAssetName, ref Stream stream, BinaryReader xnbReader, char platform, Action<IDisposable> recordDisposableObject)
	{
		Stream output = File.OpenWrite(originalAssetName + ".tmp");
		long position = xnbReader.BaseStream.Position;
		xnbReader.BaseStream.Seek(0L, SeekOrigin.Begin);
		using (BinaryWriter binaryWriter = new BinaryWriter(output, Encoding.ASCII, leaveOpen: true))
		{
			binaryWriter.Write(xnbReader.ReadBytes(5));
			byte b = xnbReader.ReadByte();
			b = (byte)(b & -129);
			binaryWriter.Write(b);
			binaryWriter.Write(0);
		}
		xnbReader.BaseStream.Seek(position, SeekOrigin.Begin);
		ContentReader contentReader = orig_GetContentReaderFromXnb(self, originalAssetName, ref stream, xnbReader, platform, recordDisposableObject);
		((CopyingStream)contentReader.BaseStream).Output = output;
		return contentReader;
	}

	public static void ctor_ContentReader(ContentReader self, ContentManager manager, Stream stream, GraphicsDevice graphicsDevice, string assetName, int version, char platform, Action<IDisposable> recordDisposableObject)
	{
		stream = new CopyingStream(stream, null);
		orig_ctor_ContentReader(self, manager, stream, graphicsDevice, assetName, version, platform, recordDisposableObject);
	}
}
