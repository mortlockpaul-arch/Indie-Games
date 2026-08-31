using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace u;

internal class _6
{
	protected class _0001CB
	{
		public SystemStatistic BatchVertexBufferChanges = SystemConsole.GetStatistic("Renderer_BatchVertexBufferChanges", SystemStatisticCategory.Rendering);
	}

	protected _0001CB Statistics = new _0001CB();

	private RenderableMesh HCB;

	internal void _76()
	{
		HCB = null;
	}

	internal void R(GraphicsDevice P_0, RenderableMesh P_1)
	{
		if (HCB == null || HCB.HC_0003 != P_1.HC_0003 || HCB.HCk != P_1.HCk)
		{
			P_0.SetVertexBuffer(P_1.HC_0003, P_1.HCk);
			Statistics.BatchVertexBufferChanges.AccumulationValue++;
		}
		if (HCB == null || HCB.HCK != P_1.HCK)
		{
			P_0.Indices = P_1.HCK;
		}
		HCB = P_1;
	}
}
