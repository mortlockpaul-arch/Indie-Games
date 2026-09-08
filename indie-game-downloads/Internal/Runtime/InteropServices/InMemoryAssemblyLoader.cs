using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Runtime.Versioning;

namespace Internal.Runtime.InteropServices;

[SupportedOSPlatform("windows")]
internal static class InMemoryAssemblyLoader
{
	[FeatureSwitchDefinition("System.Runtime.InteropServices.EnableCppCLIHostActivation")]
	private static bool IsSupported { get; } = InitializeIsSupported();

	private static bool InitializeIsSupported()
	{
		if (!AppContext.TryGetSwitch("System.Runtime.InteropServices.EnableCppCLIHostActivation", out var isEnabled))
		{
			return true;
		}
		return isEnabled;
	}

	public static void LoadInMemoryAssembly(nint moduleHandle, nint assemblyPath)
	{
		if (!IsSupported)
		{
			throw new NotSupportedException(SR.NotSupported_CppCli);
		}
		LoadInMemoryAssemblyInContextWhenSupported(moduleHandle, assemblyPath);
	}

	private static void LoadInMemoryAssemblyInContextWhenSupported(nint moduleHandle, nint assemblyPath)
	{
		LoadInMemoryAssemblyInContextImpl(moduleHandle, assemblyPath);
	}

	[UnmanagedCallersOnly]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The same C++/CLI feature switch applies to LoadInMemoryAssembly and this function. We rely on the warning from LoadInMemoryAssembly.")]
	public static void LoadInMemoryAssemblyInContext(nint moduleHandle, nint assemblyPath, nint loadContext)
	{
		if (!IsSupported)
		{
			throw new NotSupportedException(SR.NotSupported_CppCli);
		}
		if (loadContext != IntPtr.Zero && loadContext != -1)
		{
			throw new ArgumentOutOfRangeException("loadContext");
		}
		LoadInMemoryAssemblyInContextImpl(moduleHandle, assemblyPath, (loadContext == IntPtr.Zero) ? AssemblyLoadContext.Default : null);
	}

	[RequiresUnreferencedCode("C++/CLI is not trim-compatible", Url = "https://aka.ms/dotnet-illink/nativehost")]
	private static void LoadInMemoryAssemblyInContextImpl(nint moduleHandle, nint assemblyPath, AssemblyLoadContext alc = null)
	{
		string componentAssemblyPath = Marshal.PtrToStringUni(assemblyPath) ?? throw new ArgumentOutOfRangeException("assemblyPath");
		if (alc == null)
		{
			alc = new IsolatedComponentLoadContext(componentAssemblyPath);
		}
		else if (alc == AssemblyLoadContext.Default)
		{
			AssemblyDependencyResolver resolver = new AssemblyDependencyResolver(componentAssemblyPath);
			AssemblyLoadContext.Default.Resolving += [RequiresUnreferencedCode("C++/CLI is not trim-compatible", Url = "https://aka.ms/dotnet-illink/nativehost")] (AssemblyLoadContext context, AssemblyName assemblyName) =>
			{
				string text = resolver.ResolveAssemblyToPath(assemblyName);
				return (text == null) ? null : context.LoadFromAssemblyPath(text);
			};
		}
		alc.LoadFromInMemoryModule(moduleHandle);
	}
}
