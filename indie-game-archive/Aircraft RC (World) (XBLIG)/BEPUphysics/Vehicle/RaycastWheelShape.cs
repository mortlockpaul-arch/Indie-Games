using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.Entities;
using BEPUphysics.Materials;
using BEPUphysics.MathExtensions;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Vehicle;

/// <summary>
/// Uses a raycast as the shape of a wheel.
/// </summary>
public class RaycastWheelShape : WheelShape
{
	private float graphicalRadius;

	/// <summary>
	/// Gets or sets the graphical radius of the wheel.
	/// This is not used for simulation.  It is only used in
	/// determining aesthetic properties of a vehicle wheel,
	/// like position and orientation.
	/// </summary>
	public sealed override float Radius
	{
		get
		{
			return graphicalRadius;
		}
		set
		{
			graphicalRadius = MathHelper.Max(value, 0f);
		}
	}

	/// <summary>
	/// Creates a new raycast based wheel shape.
	/// </summary>
	/// <param name="graphicalRadius">Graphical radius of the wheel.
	/// This is not used for simulation.  It is only used in
	/// determining aesthetic properties of a vehicle wheel,
	/// like position and orientation.</param>
	/// <param name="localGraphicTransform">Local graphic transform of the wheel shape.
	/// This transform is applied first when creating the shape's worldTransform.</param>
	public RaycastWheelShape(float graphicalRadius, Matrix localGraphicTransform)
	{
		Radius = graphicalRadius;
		base.LocalGraphicTransform = localGraphicTransform;
	}

	/// <summary>
	/// Updates the wheel's world transform for graphics.
	/// Called automatically by the owning wheel at the end of each frame.
	/// If the engine is updating asynchronously, you can call this inside of a space read buffer lock
	/// and update the wheel transforms safely.
	/// </summary>
	public override void UpdateWorldTransform()
	{
		Vector3 translation = default(Vector3);
		Vector3.Add(ref wheel.suspension.localAttachmentPoint, ref wheel.vehicle.Body.CollisionInformation.localPosition, out var result);
		worldTransform = Matrix3X3.ToMatrix4X4(wheel.vehicle.Body.BufferedStates.InterpolatedStates.OrientationMatrix);
		Vector3.TransformNormal(ref result, ref worldTransform, out var result2);
		result2 += wheel.vehicle.Body.BufferedStates.InterpolatedStates.Position;
		Vector3.Transform(ref wheel.suspension.localDirection, ref worldTransform, out var result3);
		float num = wheel.suspension.currentLength - graphicalRadius;
		translation.X = result2.X + result3.X * num;
		translation.Y = result2.Y + result3.Y * num;
		translation.Z = result2.Z + result3.Z * num;
		Vector3 axis = Vector3.Cross(wheel.localForwardDirection, wheel.suspension.localDirection);
		Matrix.CreateFromAxisAngle(ref axis, spinAngle, out var result4);
		Matrix.Multiply(ref localGraphicTransform, ref result4, out var result5);
		Matrix.Multiply(ref result5, ref steeringTransform, out result5);
		Matrix.Multiply(ref result5, ref worldTransform, out worldTransform);
		worldTransform.Translation = translation;
	}

	/// <summary>
	/// Finds a supporting entity, the contact location, and the contact normal.
	/// </summary>
	/// <param name="location">Contact point between the wheel and the support.</param>
	/// <param name="normal">Contact normal between the wheel and the support.</param>
	/// <param name="suspensionLength">Length of the suspension at the contact.</param>
	/// <param name="supportingCollidable">Collidable supporting the wheel, if any.</param>
	/// <param name="entity">Supporting object.</param>
	/// <param name="material">Material of the wheel.</param>
	/// <returns>Whether or not any support was found.</returns>
	protected internal override bool FindSupport(out Vector3 location, out Vector3 normal, out float suspensionLength, out Collidable supportingCollidable, out Entity entity, out Material material)
	{
		suspensionLength = float.MaxValue;
		location = Toolbox.NoVector;
		supportingCollidable = null;
		entity = null;
		normal = Toolbox.NoVector;
		material = null;
		bool flag = false;
		for (int i = 0; i < detector.CollisionInformation.pairs.Count; i++)
		{
			CollidablePairHandler collidablePairHandler = detector.CollisionInformation.pairs[i];
			if (!(((collidablePairHandler.BroadPhaseOverlap.entryA == detector.CollisionInformation) ? collidablePairHandler.BroadPhaseOverlap.entryB : collidablePairHandler.BroadPhaseOverlap.entryA) is Collidable collidable) || CollisionRules.CollisionRuleCalculator(this, collidable) != CollisionRule.Normal || !collidable.RayCast(new Ray(wheel.suspension.worldAttachmentPoint, wheel.suspension.worldDirection), wheel.suspension.restLength, out var rayHit) || !(rayHit.T < suspensionLength))
			{
				continue;
			}
			suspensionLength = rayHit.T;
			if (collidable is EntityCollidable entityCollidable)
			{
				entity = entityCollidable.Entity;
				material = entityCollidable.Entity.Material;
			}
			else
			{
				entity = null;
				supportingCollidable = collidable;
				if (collidable is IMaterialOwner materialOwner)
				{
					material = materialOwner.Material;
				}
			}
			location = rayHit.Location;
			normal = rayHit.Normal;
			flag = true;
		}
		if (flag)
		{
			if (suspensionLength > 0f)
			{
				normal.Normalize();
			}
			else
			{
				Vector3.Negate(ref wheel.suspension.worldDirection, out normal);
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Initializes the detector entity and any other necessary logic.
	/// </summary>
	protected internal override void Initialize()
	{
		Vector3 value = wheel.suspension.localAttachmentPoint;
		Vector3 value2 = value + wheel.suspension.localDirection * wheel.suspension.restLength;
		Vector3.Min(ref value, ref value2, out var result);
		Vector3.Max(ref value, ref value2, out var result2);
		detector.Width = result2.X - result.X;
		detector.Height = result2.Y - result.Y;
		detector.Length = result2.Z - result.Z;
	}

	/// <summary>
	/// Updates the position of the detector before each step.
	/// </summary>
	protected internal override void UpdateDetectorPosition()
	{
		Vector3 value = new Vector3
		{
			X = wheel.suspension.worldAttachmentPoint.X + wheel.suspension.worldDirection.X * wheel.suspension.restLength * 0.5f,
			Y = wheel.suspension.worldAttachmentPoint.Y + wheel.suspension.worldDirection.Y * wheel.suspension.restLength * 0.5f,
			Z = wheel.suspension.worldAttachmentPoint.Z + wheel.suspension.worldDirection.Z * wheel.suspension.restLength * 0.5f
		};
		detector.Position = value;
		detector.OrientationMatrix = wheel.Vehicle.Body.orientationMatrix;
		Vector3.Subtract(ref value, ref wheel.vehicle.Body.position, out var result);
		Vector3.Cross(ref result, ref wheel.vehicle.Body.angularVelocity, out result);
		Vector3.Add(ref result, ref wheel.vehicle.Body.linearVelocity, out result);
		detector.LinearVelocity = result;
		detector.AngularVelocity = wheel.vehicle.Body.angularVelocity;
	}
}
