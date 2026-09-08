using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoMod.Utils;

public static class DynDll
{
	public static Dictionary<string, string> DllMap = new Dictionary<string, string>();

	private const int RTLD_NOW = 2;

	[DllImport("kernel32")]
	private static extern IntPtr GetModuleHandle(string lpModuleName);

	[DllImport("kernel32")]
	private static extern IntPtr LoadLibrary(string lpFileName);

	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

	[DllImport("dl", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPTStr)] string filename, int flags);

	[DllImport("dl", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPTStr)] string symbol);

	[DllImport("dl", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr dlerror();

	public static IntPtr OpenLibrary(string name)
	{
		if (name != null && DllMap.TryGetValue(name, out var value))
		{
			name = value;
		}
		IntPtr intPtr;
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			intPtr = GetModuleHandle(name);
			if (intPtr == IntPtr.Zero)
			{
				intPtr = LoadLibrary(name);
			}
			return intPtr;
		}
		IntPtr zero = IntPtr.Zero;
		intPtr = dlopen(name, 2);
		if ((zero = dlerror()) != IntPtr.Zero)
		{
			Console.WriteLine(string.Format("DynDll can't access {0}!", name ?? "entry point"));
			Console.WriteLine("dlerror: " + Marshal.PtrToStringAnsi(zero));
			return IntPtr.Zero;
		}
		return intPtr;
	}

	public static IntPtr GetFunction(this IntPtr lib, string name)
	{
		if (lib == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			return GetProcAddress(lib, name);
		}
		IntPtr result = dlsym(lib, name);
		IntPtr ptr;
		if ((ptr = dlerror()) != IntPtr.Zero)
		{
			Console.WriteLine("DynDll can't access " + name + "!");
			Console.WriteLine("dlerror: " + Marshal.PtrToStringAnsi(ptr));
			return IntPtr.Zero;
		}
		return result;
	}

	public static T AsDelegate<T>(this IntPtr s) where T : class
	{
		return Marshal.GetDelegateForFunctionPointer(s, typeof(T)) as T;
	}

	public static void ResolveDynDllImports(this Type type)
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			bool flag = true;
			object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DynDllImportAttribute), inherit: true);
			for (int j = 0; j < customAttributes.Length; j++)
			{
				DynDllImportAttribute dynDllImportAttribute = (DynDllImportAttribute)customAttributes[j];
				flag = false;
				IntPtr intPtr = OpenLibrary(dynDllImportAttribute.DLL);
				if (intPtr == IntPtr.Zero)
				{
					continue;
				}
				string[] entryPoints = dynDllImportAttribute.EntryPoints;
				foreach (string name in entryPoints)
				{
					IntPtr function = intPtr.GetFunction(name);
					if (!(function == IntPtr.Zero))
					{
						fieldInfo.SetValue(null, Marshal.GetDelegateForFunctionPointer(function, fieldInfo.FieldType));
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				throw new EntryPointNotFoundException($"No matching entry point found for {fieldInfo.Name} in {fieldInfo.DeclaringType.FullName}");
			}
		}
	}
}
