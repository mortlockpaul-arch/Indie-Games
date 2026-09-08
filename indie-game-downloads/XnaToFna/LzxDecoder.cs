using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoMod.Utils;

namespace XnaToFna;

public class LzxDecoder
{
	private static readonly Type t_orig;

	private static readonly Type t_proxy;

	private static readonly ConstructorInfo ctor;

	private readonly object _;

	private static FastReflectionDelegate _Decompress;

	static LzxDecoder()
	{
		t_orig = typeof(Game).Assembly.GetType("Microsoft.Xna.Framework.Content.LzxDecoder");
		t_proxy = typeof(LzxDecoder);
		ctor = t_orig.GetConstructor(new Type[1] { typeof(int) });
		_Decompress = t_orig.GetMethod("Decompress", BindingFlags.Instance | BindingFlags.Public).GetFastDelegate();
	}

	public LzxDecoder(int window)
	{
		_ = ctor.Invoke(new object[1] { window });
	}

	public int Decompress(Stream inData, int inLen, Stream outData, int outLen)
	{
		return (int)_Decompress(_, inData, inLen, outData, outLen);
	}
}
