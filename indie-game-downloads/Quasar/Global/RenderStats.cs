using System.Diagnostics;

namespace Quasar.Global;

public static class RenderStats
{
	private static int renderCalls;

	private static int culledObjects;

	public static int RenderCalls => renderCalls;

	public static int CulledObjects => culledObjects;

	[Conditional("SHOWFPS")]
	public static void IncRenderCalls()
	{
		renderCalls++;
	}

	[Conditional("SHOWFPS")]
	public static void IncCulledObjects()
	{
		culledObjects++;
	}

	[Conditional("SHOWFPS")]
	public static void ResetStats()
	{
		renderCalls = 0;
		culledObjects = 0;
	}
}
