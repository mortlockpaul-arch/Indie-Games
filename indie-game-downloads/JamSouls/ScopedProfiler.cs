using System;
using System.Runtime.InteropServices;

namespace JamSouls;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ScopedProfiler : IDisposable
{
	public ScopedProfiler(string _name)
	{
		ProfilingManager.Push(_name);
	}

	public void Dispose()
	{
		ProfilingManager.Pop();
	}
}
