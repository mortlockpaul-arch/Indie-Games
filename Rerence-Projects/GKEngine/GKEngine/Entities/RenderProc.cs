using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public struct RenderProc(int xType, string xName, EffectParameter oParam)
{
	public int type = xType;

	public string name = xName;

	public EffectParameter param = oParam;
}
