using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Utils;

public static class Extensions
{
	public static string ToHexadecimalString(this byte[] data)
	{
		return BitConverter.ToString(data).Replace("-", string.Empty);
	}

	public static T InvokePassing<T>(this MulticastDelegate md, T val, params object[] args)
	{
		if ((object)md == null)
		{
			return val;
		}
		object[] array = new object[args.Length + 1];
		array[0] = val;
		Array.Copy(args, 0, array, 1, args.Length);
		Delegate[] invocationList = md.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			array[0] = invocationList[i].DynamicInvoke(array);
		}
		return (T)array[0];
	}

	public static bool InvokeWhileTrue(this MulticastDelegate md, params object[] args)
	{
		if ((object)md == null)
		{
			return true;
		}
		Delegate[] invocationList = md.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (!(bool)invocationList[i].DynamicInvoke(args))
			{
				return false;
			}
		}
		return true;
	}

	public static bool InvokeWhileFalse(this MulticastDelegate md, params object[] args)
	{
		if ((object)md == null)
		{
			return false;
		}
		Delegate[] invocationList = md.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if ((bool)invocationList[i].DynamicInvoke(args))
			{
				return true;
			}
		}
		return false;
	}

	public static T InvokeWhileNull<T>(this MulticastDelegate md, params object[] args) where T : class
	{
		if ((object)md == null)
		{
			return null;
		}
		Delegate[] invocationList = md.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			T val = (T)invocationList[i].DynamicInvoke(args);
			if (val != null)
			{
				return val;
			}
		}
		return null;
	}

	public static string SpacedPascalCase(this string input)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < input.Length; i++)
		{
			char c = input[i];
			if (i > 0 && char.IsUpper(c))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	public static string ReadNullTerminatedString(this BinaryReader stream)
	{
		string text = "";
		char c;
		while ((c = stream.ReadChar()) != 0)
		{
			text += c;
		}
		return text;
	}

	public static void WriteNullTerminatedString(this BinaryWriter stream, string text)
	{
		if (text != null)
		{
			foreach (char ch in text)
			{
				stream.Write(ch);
			}
		}
		stream.Write('\0');
	}

	public static Delegate CastDelegate(this Delegate source, Type type)
	{
		if ((object)source == null)
		{
			return null;
		}
		Delegate[] invocationList = source.GetInvocationList();
		if (invocationList.Length == 1)
		{
			return Delegate.CreateDelegate(type, invocationList[0].Target, invocationList[0].Method);
		}
		Delegate[] array = new Delegate[invocationList.Length];
		for (int i = 0; i < invocationList.Length; i++)
		{
			array[i] = invocationList[i].CastDelegate(type);
		}
		return Delegate.Combine(array);
	}

	public static void LogDetailed(this Exception e, string tag = null)
	{
		if (tag == null)
		{
			Console.WriteLine("--------------------------------");
			Console.WriteLine("Detailed exception log:");
		}
		for (Exception ex = e; ex != null; ex = ex.InnerException)
		{
			Console.WriteLine("--------------------------------");
			Console.WriteLine(ex.GetType().FullName + ": " + ex.Message + "\n" + ex.StackTrace);
			if (ex is ReflectionTypeLoadException)
			{
				ReflectionTypeLoadException ex2 = (ReflectionTypeLoadException)ex;
				for (int i = 0; i < ex2.Types.Length; i++)
				{
					Console.WriteLine("ReflectionTypeLoadException.Types[" + i + "]: " + ex2.Types[i]);
				}
				for (int j = 0; j < ex2.LoaderExceptions.Length; j++)
				{
					ex2.LoaderExceptions[j].LogDetailed(tag + ((tag == null) ? "" : ", ") + "rtle:" + j);
				}
			}
			if (ex is TypeLoadException)
			{
				Console.WriteLine("TypeLoadException.TypeName: " + ((TypeLoadException)ex).TypeName);
			}
			if (ex is BadImageFormatException)
			{
				Console.WriteLine("BadImageFormatException.FileName: " + ((BadImageFormatException)ex).FileName);
			}
		}
	}

	public static Delegate CreateDelegate<T>(this MethodBase method) where T : class
	{
		return method.CreateDelegate(typeof(T), null);
	}

	public static Delegate CreateDelegate<T>(this MethodBase method, object target) where T : class
	{
		return method.CreateDelegate(typeof(T), target);
	}

	public static Delegate CreateDelegate(this MethodBase method, Type delegateType)
	{
		return method.CreateDelegate(delegateType, null);
	}

	public static Delegate CreateDelegate(this MethodBase method, Type delegateType, object target)
	{
		if (!typeof(Delegate).IsAssignableFrom(delegateType))
		{
			throw new ArgumentException("Type argument must be a delegate type!");
		}
		if (method is DynamicMethod)
		{
			return ((DynamicMethod)method).CreateDelegate(delegateType, target);
		}
		RuntimeMethodHandle methodHandle = method.MethodHandle;
		RuntimeHelpers.PrepareMethod(methodHandle);
		IntPtr functionPointer = methodHandle.GetFunctionPointer();
		return (Delegate)Activator.CreateInstance(delegateType, target, functionPointer);
	}
}
