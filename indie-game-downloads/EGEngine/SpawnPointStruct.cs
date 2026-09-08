using Microsoft.Xna.Framework;

namespace EGEngine;

public class SpawnPointStruct
{
	public SpawnPointType SpawnType;

	public Vector3 Position;

	public Vector3 Direction;

	public float SpawnRatio;

	public float SpawnTimer;

	public bool OccupiedFlag;
}
