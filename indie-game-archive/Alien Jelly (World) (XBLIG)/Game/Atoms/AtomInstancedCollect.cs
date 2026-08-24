using GKEngine;
using Game.Grids;

namespace Game.Atoms;

public class AtomInstancedCollect : AtomInstanced, IGridable, ICollectable
{
	private bool _collected;

	public bool collected => _collected;

	public int type => 3;

	public Atom atom => this;

	public AtomInstancedCollect(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
		instancer.renderShadows = false;
		instancer.renderDepth = false;
	}

	public override void StateSet(int xState)
	{
		base.StateSet(xState);
		data.Y = 1f + (float)GameEngine.random.NextDouble() * 2000f;
	}

	public void Collect()
	{
		if (!collected)
		{
			_collected = true;
			visible = false;
			data.X = -1f;
			PopulateInstancer();
		}
	}
}
