using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.Versioning;

namespace Internal.Runtime.InteropServices;

[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("maccatalyst")]
[UnsupportedOSPlatform("tvos")]
[RequiresUnreferencedCode("The trimmer might remove assemblies that are loaded by this class", Url = "https://aka.ms/dotnet-illink/nativehost")]
internal sealed class IsolatedComponentLoadContext : AssemblyLoadContext
{
	private readonly AssemblyDependencyResolver _resolver;

	public IsolatedComponentLoadContext(string componentAssemblyPath)
		: base("IsolatedComponentLoadContext(" + componentAssemblyPath + ")")
	{
		_resolver = new AssemblyDependencyResolver(componentAssemblyPath);
	}

	protected override Assembly Load(AssemblyName assemblyName)
	{
		string text = _resolver.ResolveAssemblyToPath(assemblyName);
		if (text != null)
		{
			return LoadFromAssemblyPath(text);
		}
		return null;
	}

	protected override nint LoadUnmanagedDll(string unmanagedDllName)
	{
		string text = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
		if (text != null)
		{
			return LoadUnmanagedDllFromPath(text);
		}
		return IntPtr.Zero;
	}
}
