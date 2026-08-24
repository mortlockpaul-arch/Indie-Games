using System;
using GKEngine.Entities;
using GKEngine.Utils;

namespace Game.Grids;

public interface IGridable
{
	GridPoint gridPoint { get; }

	Base3D gridBase3D { get; }

	GridPoint[] gridArea { get; }

	GUID gridGUID { get; }

	Type gridType { get; }
}
