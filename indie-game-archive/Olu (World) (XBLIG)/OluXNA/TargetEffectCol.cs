using System.Collections.Generic;

namespace OluXNA;

internal class TargetEffectCol
{
	public List<TargetEffect> fx;

	public TargetEffectCol()
	{
		fx = new List<TargetEffect>();
	}

	public TargetEffectCol(TargetEffectCol other)
	{
		fx = new List<TargetEffect>();
		fx.Clear();
		for (int i = 0; i < other.fx.Count; i++)
		{
			fx.Add(other.fx[i]);
		}
	}

	public void Dispose()
	{
		fx.Clear();
	}
}
