using System.Collections.Generic;
using BEPUphysics.Collidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;

namespace BEPUphysics.UpdateableSystems.ForceFields;

/// <summary>
/// Defines the area in which a force field works using an entity's shape.
/// </summary>
public class VolumeForceFieldShape : ForceFieldShape
{
	private readonly RawList<Entity> affectedEntities = new RawList<Entity>();

	/// <summary>
	/// Gets or sets the volume used by the shape.
	/// </summary>
	public DetectorVolume Volume { get; set; }

	/// <summary>
	/// Constructs a new force field shape using a detector volume.
	/// </summary>
	/// <param name="volume">Volume to use.</param>
	public VolumeForceFieldShape(DetectorVolume volume)
	{
		Volume = volume;
	}

	/// <summary>
	/// Determines the possibly involved entities.
	/// </summary>
	/// <returns>Possibly involved entities.</returns>
	public override IList<Entity> GetPossiblyAffectedEntities()
	{
		affectedEntities.Clear();
		foreach (Entity key in Volume.pairs.Keys)
		{
			affectedEntities.Add(key);
		}
		return affectedEntities;
	}

	/// <summary>
	/// Determines if the entity is affected by the force field.
	/// </summary>
	/// <param name="testEntity">Entity to test.</param>
	/// <returns>Whether the entity is affected.</returns>
	public override bool IsEntityAffected(Entity testEntity)
	{
		return Volume.pairs[testEntity].Touching;
	}
}
