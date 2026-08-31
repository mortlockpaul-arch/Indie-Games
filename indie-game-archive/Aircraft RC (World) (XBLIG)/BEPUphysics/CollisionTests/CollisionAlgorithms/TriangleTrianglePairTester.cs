using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
/// Generates candidates between two triangles and manages the persistent state of the pair.
/// </summary>
public class TriangleTrianglePairTester : TriangleConvexPairTester
{
	public override bool GenerateContactCandidate(out TinyStructList<ContactData> contactList)
	{
		if (base.GenerateContactCandidate(out contactList))
		{
			TriangleShape triangleShape = (TriangleShape)convex;
			Vector3.Subtract(ref triangleShape.vB, ref triangleShape.vA, out var result);
			Vector3.Subtract(ref triangleShape.vC, ref triangleShape.vA, out var result2);
			Vector3.Cross(ref result, ref result2, out var result3);
			TriangleSidedness sidedness = triangleShape.sidedness;
			if (sidedness != TriangleSidedness.DoubleSided)
			{
				for (int num = contactList.count - 1; num >= 0; num--)
				{
					contactList.Get(num, out var item);
					Vector3.Dot(ref item.Normal, ref result3, out var result4);
					if (sidedness == TriangleSidedness.Clockwise)
					{
						if (result4 < 0f)
						{
							contactList.RemoveAt(num);
						}
					}
					else if (result4 > 0f)
					{
						contactList.RemoveAt(num);
					}
				}
			}
			return contactList.count > 0;
		}
		return false;
	}
}
