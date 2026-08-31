using BEPUphysics.Constraints.Collision;
using Microsoft.Xna.Framework;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Pair handler that manages a pair of two boxes.
/// </summary>
public abstract class ConvexConstraintPairHandler : ConvexPairHandler
{
	private ConvexContactManifoldConstraint contactConstraint = new ConvexContactManifoldConstraint();

	/// <summary>
	/// Gets the contact constraint used by the pair handler.
	/// </summary>
	public override ContactManifoldConstraint ContactConstraint => contactConstraint;

	protected internal override void GetContactInformation(int index, out ContactInformation info)
	{
		info.Contact = ContactManifold.contacts.Elements[index];
		float num = 0f;
		info.NormalImpulse = 0f;
		for (int i = 0; i < contactConstraint.penetrationConstraints.count; i++)
		{
			num += contactConstraint.penetrationConstraints.Elements[i].accumulatedImpulse;
			if (contactConstraint.penetrationConstraints.Elements[i].contact == info.Contact)
			{
				info.NormalImpulse = contactConstraint.penetrationConstraints.Elements[i].accumulatedImpulse;
			}
		}
		Vector3.Distance(ref contactConstraint.slidingFriction.manifoldCenter, ref info.Contact.Position, out var result);
		if (num > 0f)
		{
			info.FrictionImpulse = info.NormalImpulse / num * (contactConstraint.slidingFriction.accumulatedImpulse.Length() + contactConstraint.twistFriction.accumulatedImpulse * result);
		}
		else
		{
			info.FrictionImpulse = 0f;
		}
		Vector3 result2;
		if (EntityA != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref EntityA.position, out result2);
			Vector3.Cross(ref EntityA.angularVelocity, ref result2, out result2);
			Vector3.Add(ref result2, ref EntityA.linearVelocity, out info.RelativeVelocity);
		}
		else
		{
			info.RelativeVelocity = default(Vector3);
		}
		if (EntityB != null)
		{
			Vector3.Subtract(ref info.Contact.Position, ref EntityB.position, out result2);
			Vector3.Cross(ref EntityB.angularVelocity, ref result2, out result2);
			Vector3.Add(ref result2, ref EntityB.linearVelocity, out result2);
			Vector3.Subtract(ref info.RelativeVelocity, ref result2, out info.RelativeVelocity);
		}
		info.Pair = this;
	}
}
