using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace System.Diagnostics;

public sealed class DiagnosticMethodInfo
{
	private readonly MethodBase _method;

	public string Name => _method.Name;

	public string? DeclaringTypeName
	{
		get
		{
			Type type = _method.DeclaringType;
			if ((object)type != null && type.IsConstructedGenericType)
			{
				type = type.GetGenericTypeDefinition();
			}
			return type?.FullName;
		}
	}

	public string? DeclaringAssemblyName => _method.Module.Assembly.FullName;

	private DiagnosticMethodInfo(MethodBase method)
	{
		_method = method;
	}

	public static DiagnosticMethodInfo? Create(Delegate @delegate)
	{
		ArgumentNullException.ThrowIfNull(@delegate, "@delegate");
		if (!StackTrace.IsSupported)
		{
			return null;
		}
		return new DiagnosticMethodInfo(@delegate.Method);
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "IL-level trimming doesn't remove method name and owning type information; this implementation is not used with native AOT trimming")]
	public static DiagnosticMethodInfo? Create(StackFrame frame)
	{
		ArgumentNullException.ThrowIfNull(frame, "frame");
		if (!StackTrace.IsSupported)
		{
			return null;
		}
		MethodBase method = frame.GetMethod();
		if (method != null)
		{
			return new DiagnosticMethodInfo(method);
		}
		return null;
	}
}
