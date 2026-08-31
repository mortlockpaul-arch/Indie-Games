using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests;

/// <summary>
///  Helper class that refreshes manifolds to keep them recent.
/// </summary>
public class ContactRefresher
{
	/// <summary>
	/// Refreshes the contact manifold, removing any out of date contacts
	/// and updating others.
	/// </summary>
	public static void ContactRefresh(RawList<Contact> contacts, RawValueList<ContactSupplementData> supplementData, ref RigidTransform transformA, ref RigidTransform transformB, RawList<int> toRemove)
	{
		for (int i = 0; i < contacts.count; i++)
		{
			ContactSupplementData contactSupplementData = supplementData.Elements[i];
			RigidTransform.Transform(ref contactSupplementData.LocalOffsetA, ref transformA, out var result);
			RigidTransform.Transform(ref contactSupplementData.LocalOffsetB, ref transformB, out var result2);
			Vector3.Subtract(ref result2, ref result, out var result3);
			Vector3.Dot(ref result3, ref contacts.Elements[i].Normal, out var result4);
			Vector3.Multiply(ref contacts.Elements[i].Normal, result4, out var result5);
			Vector3.Subtract(ref result3, ref result5, out result5);
			result4 = result5.LengthSquared();
			if (result4 > CollisionDetectionSettings.ContactInvalidationLengthSquared)
			{
				toRemove.Add(i);
				continue;
			}
			Vector3.Dot(ref result3, ref contacts.Elements[i].Normal, out result4);
			contacts.Elements[i].PenetrationDepth = contactSupplementData.BasePenetrationDepth - result4;
			if (contacts.Elements[i].PenetrationDepth < 0f - CollisionDetectionSettings.maximumContactDistance)
			{
				toRemove.Add(i);
				continue;
			}
			Vector3.Add(ref result2, ref result, out var result6);
			Vector3.Multiply(ref result6, 0.5f, out result6);
			contacts.Elements[i].Position = result6;
		}
	}
}
