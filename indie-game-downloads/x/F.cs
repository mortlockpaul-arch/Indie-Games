using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace X;

internal class F
{
	protected class DA_0018
	{
		public SystemStatistic BatchVertexBufferChanges = SystemConsole.GetStatistic("Renderer_BatchVertexBufferChanges", SystemStatisticCategory.Rendering);
	}

	protected DA_0018 Statistics = new DA_0018();

	private RenderableMesh _3A_0018;

	internal void _6F()
	{
		_3A_0018 = null;
	}

	internal void _0016(GraphicsDevice P_0, RenderableMesh P_1)
	{
		if (_3A_0018 == null || _3A_0018.Index.BufferIndex._3AL != P_1.Index.BufferIndex._3AL || _3A_0018.Index._3A3 != P_1.Index._3A3)
		{
			P_0.SetVertexBuffer(P_1.Index.BufferIndex._3AL, P_1.Index._3A3);
			Statistics.BatchVertexBufferChanges.AccumulationValue++;
		}
		if (_3A_0018 == null || _3A_0018.Index.BufferIndex._3A_0018 != P_1.Index.BufferIndex._3A_0018)
		{
			P_0.Indices = P_1.Index.BufferIndex._3A_0018;
		}
		_3A_0018 = P_1;
	}
}
