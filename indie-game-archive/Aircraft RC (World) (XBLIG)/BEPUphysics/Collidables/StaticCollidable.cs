using System;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Materials;
using BEPUphysics.OtherSpaceStages;

namespace BEPUphysics.Collidables;

/// <summary>
///  Superclass of static collidable objects which can be added directly to a space.  Static objects cannot move.
/// </summary>
public abstract class StaticCollidable : Collidable, ISpaceObject, IMaterialOwner, IDeferredEventCreatorOwner
{
	internal Material material;

	private Action<Material> materialChangedDelegate;

	private ISpace space;

	/// <summary>
	///  Gets or sets the material used by the collidable.
	/// </summary>
	public Material Material
	{
		get
		{
			return material;
		}
		set
		{
			if (material != null)
			{
				material.MaterialChanged -= materialChangedDelegate;
			}
			material = value;
			if (material != null)
			{
				material.MaterialChanged += materialChangedDelegate;
			}
			OnMaterialChanged(material);
		}
	}

	protected internal override bool IsActive => false;

	ISpace ISpaceObject.Space
	{
		get
		{
			return space;
		}
		set
		{
			space = value;
		}
	}

	/// <summary>
	///  Gets the space that owns the mesh.
	/// </summary>
	public ISpace Space => space;

	IDeferredEventCreator IDeferredEventCreatorOwner.EventCreator => EventCreator;

	/// <summary>
	/// Gets the event creator associated with this collidable.
	/// </summary>
	protected abstract IDeferredEventCreator EventCreator { get; }

	/// <summary>
	///  Performs common initialization.
	/// </summary>
	protected StaticCollidable()
	{
		collisionRules.group = CollisionRules.DefaultKinematicCollisionGroup;
		material = new Material();
		materialChangedDelegate = OnMaterialChanged;
		material.MaterialChanged += materialChangedDelegate;
	}

	protected override void OnShapeChanged(CollisionShape collisionShape)
	{
		if (!base.IgnoreShapeChanges)
		{
			UpdateBoundingBox();
		}
	}

	protected virtual void OnMaterialChanged(Material newMaterial)
	{
		for (int i = 0; i < pairs.Count; i++)
		{
			pairs[i].UpdateMaterialProperties();
		}
	}

	void ISpaceObject.OnAdditionToSpace(ISpace newSpace)
	{
	}

	void ISpaceObject.OnRemovalFromSpace(ISpace oldSpace)
	{
	}
}
