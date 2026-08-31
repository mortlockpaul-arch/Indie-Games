using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Rendering;

namespace u;

internal class w
{
	internal bool HCB = true;

	internal bool HC_0002;

	internal bool HC_0012;

	internal bool HCH;

	internal bool HC7;

	internal Effect HC_0001;

	private List<RenderableMesh> HCw = new List<RenderableMesh>(32);

	internal List<RenderableMesh> Objects => HCw;

	internal void G()
	{
		HCB = true;
		HC_0002 = false;
		HC_0012 = false;
		HCH = false;
		HC7 = false;
		HC_0001 = null;
		HCw.Clear();
	}
}
