using System;
using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contact data between a triangle mesh and a convex.
/// </summary>
public abstract class TriangleMeshConvexContactManifold : ContactManifold
{
	/// <summary>
	/// Edge of a triangle in a mesh in terms of vertex indices.
	/// </summary>
	public struct Edge(int a, int b) : IEquatable<Edge>
	{
		private int A = a;

		private int B = b;

		public override int GetHashCode()
		{
			return A + B;
		}

		public bool Equals(Edge edge)
		{
			if (edge.A != A || edge.B != B)
			{
				if (edge.A == B)
				{
					return edge.B == A;
				}
				return false;
			}
			return true;
		}
	}

	/// <summary>
	///  Stores indices of a triangle.
	/// </summary>
	public struct TriangleIndices : IEquatable<TriangleIndices>
	{
		/// <summary>
		///  First index in the triangle.
		/// </summary>
		public int A;

		/// <summary>
		///  Second index in the triangle.
		/// </summary>
		public int B;

		/// <summary>
		///  Third index in the triangle.
		/// </summary>
		public int C;

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return A + B + C;
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(TriangleIndices other)
		{
			if (A == other.A && B == other.B)
			{
				return C == other.C;
			}
			return false;
		}
	}

	private struct EdgeContact
	{
		public bool ShouldCorrect;

		public Vector3 CorrectedNormal;

		public Edge Edge;

		public ContactData ContactData;
	}

	private struct VertexContact
	{
		public bool ShouldCorrect;

		public Vector3 CorrectedNormal;

		public int Vertex;

		public ContactData ContactData;
	}

	protected RawValueList<ContactSupplementData> supplementData = new RawValueList<ContactSupplementData>(4);

	private Dictionary<TriangleIndices, TrianglePairTester> activePairTesters = new Dictionary<TriangleIndices, TrianglePairTester>(4);

	private RawValueList<ContactData> candidatesToAdd;

	private RawValueList<ContactData> reducedCandidates = new RawValueList<ContactData>();

	protected TriangleShape localTriangleShape = new TriangleShape();

	private BEPUphysics.DataStructures.HashSet<int> blockedVertexRegions = new BEPUphysics.DataStructures.HashSet<int>();

	private BEPUphysics.DataStructures.HashSet<Edge> blockedEdgeRegions = new BEPUphysics.DataStructures.HashSet<Edge>();

	private RawValueList<EdgeContact> edgeContacts = new RawValueList<EdgeContact>();

	private RawValueList<VertexContact> vertexContacts = new RawValueList<VertexContact>();

	protected ConvexCollidable convex;

	/// <summary>
	///  Gets the convex collidable associated with this pair.
	/// </summary>
	public ConvexCollidable ConvexCollidable => convex;

	protected virtual RigidTransform MeshTransform => RigidTransform.Identity;

	protected abstract bool UseImprovedBoundaryHandling { get; }

	protected abstract TrianglePairTester GetTester();

	protected abstract void GiveBackTester(TrianglePairTester tester);

	/// <summary>
	///  Constructs a new contact manifold.
	/// </summary>
	protected TriangleMeshConvexContactManifold()
	{
		contacts = new RawList<Contact>(4);
		unusedContacts = new UnsafeResourcePool<Contact>(4);
		contactIndicesToRemove = new RawList<int>(4);
		candidatesToAdd = new RawValueList<ContactData>(1);
	}

	protected internal abstract int FindOverlappingTriangles(float dt);

	protected abstract bool ConfigureTriangle(int i, out TriangleIndices indices);

	protected internal abstract void CleanUpOverlappingTriangles();

	/// <summary>
	///  Updates the manifold.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void Update(float dt)
	{
		RigidTransform transformB = MeshTransform;
		ContactRefresher.ContactRefresh(contacts, supplementData, ref convex.worldTransform, ref transformB, contactIndicesToRemove);
		RemoveQueuedContacts();
		CleanUpOverlappingTriangles();
		int num = FindOverlappingTriangles(dt);
		Matrix3X3.CreateFromQuaternion(ref convex.worldTransform.Orientation, out var result);
		for (int i = 0; i < num; i++)
		{
			if (!ConfigureTriangle(i, out var indices))
			{
				continue;
			}
			if (!activePairTesters.TryGetValue(indices, out var value))
			{
				value = GetTester();
				value.Initialize(convex.Shape, localTriangleShape);
				activePairTesters.Add(indices, value);
			}
			value.Updated = true;
			Vector3.Subtract(ref localTriangleShape.vA, ref convex.worldTransform.Position, out localTriangleShape.vA);
			Vector3.Subtract(ref localTriangleShape.vB, ref convex.worldTransform.Position, out localTriangleShape.vB);
			Vector3.Subtract(ref localTriangleShape.vC, ref convex.worldTransform.Position, out localTriangleShape.vC);
			Matrix3X3.TransformTranspose(ref localTriangleShape.vA, ref result, out localTriangleShape.vA);
			Matrix3X3.TransformTranspose(ref localTriangleShape.vB, ref result, out localTriangleShape.vB);
			Matrix3X3.TransformTranspose(ref localTriangleShape.vC, ref result, out localTriangleShape.vC);
			if (!value.GenerateContactCandidate(out var contactList))
			{
				continue;
			}
			for (int j = 0; j < contactList.count; j++)
			{
				contactList.Get(j, out var item);
				if (UseImprovedBoundaryHandling)
				{
					if (AnalyzeCandidate(ref indices, value, ref item))
					{
						AddLocalContact(ref item, ref result);
					}
				}
				else
				{
					AddLocalContact(ref item, ref result);
				}
			}
		}
		if (UseImprovedBoundaryHandling)
		{
			int count = candidatesToAdd.count;
			if (vertexContacts.count == 0 && count == 0 && edgeContacts.count > 1)
			{
				bool flag = true;
				bool flag2 = false;
				Vector3 vector = edgeContacts.Elements[0].ContactData.Normal;
				edgeContacts.Elements[0].CorrectedNormal.Normalize();
				Vector3.Dot(ref vector, ref edgeContacts.Elements[0].CorrectedNormal, out var result2);
				if (Math.Abs(result2) > 0.01f)
				{
					flag = false;
				}
				else
				{
					for (int k = 1; k < edgeContacts.count; k++)
					{
						Vector3.Dot(ref edgeContacts.Elements[k].ContactData.Normal, ref vector, out result2);
						if (result2 < 0f)
						{
							flag2 = true;
						}
						Vector3.Dot(ref edgeContacts.Elements[k].ContactData.Normal, ref edgeContacts.Elements[0].CorrectedNormal, out result2);
						if (Math.Abs(result2) > 0.01f)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag && flag2)
				{
					edgeContacts.Elements[0].ContactData.Normal = edgeContacts.Elements[0].CorrectedNormal;
					edgeContacts.Elements[0].ShouldCorrect = true;
					for (int l = 1; l < edgeContacts.count; l++)
					{
						edgeContacts.Elements[l].CorrectedNormal.Normalize();
						Vector3.Dot(ref edgeContacts.Elements[l].CorrectedNormal, ref edgeContacts.Elements[l].ContactData.Normal, out result2);
						if ((double)result2 < 0.01)
						{
							edgeContacts.Elements[l].ContactData.Normal = edgeContacts.Elements[l].CorrectedNormal;
							edgeContacts.Elements[l].ShouldCorrect = true;
						}
					}
				}
			}
			for (int m = 0; m < edgeContacts.count; m++)
			{
				if (!blockedEdgeRegions.Contains(edgeContacts.Elements[m].Edge))
				{
					AddLocalContact(ref edgeContacts.Elements[m].ContactData, ref result);
				}
				else if (edgeContacts.Elements[m].ShouldCorrect || count == 0)
				{
					edgeContacts.Elements[m].CorrectedNormal.Normalize();
					Vector3.Dot(ref edgeContacts.Elements[m].CorrectedNormal, ref edgeContacts.Elements[m].ContactData.Normal, out var result3);
					edgeContacts.Elements[m].ContactData.Normal = edgeContacts.Elements[m].CorrectedNormal;
					edgeContacts.Elements[m].ContactData.PenetrationDepth *= MathHelper.Max(0f, result3);
					AddLocalContact(ref edgeContacts.Elements[m].ContactData, ref result);
				}
			}
			for (int n = 0; n < vertexContacts.count; n++)
			{
				if (!blockedVertexRegions.Contains(vertexContacts.Elements[n].Vertex))
				{
					AddLocalContact(ref vertexContacts.Elements[n].ContactData, ref result);
				}
				else if (vertexContacts.Elements[n].ShouldCorrect || count == 0)
				{
					vertexContacts.Elements[n].CorrectedNormal.Normalize();
					Vector3.Dot(ref vertexContacts.Elements[n].CorrectedNormal, ref vertexContacts.Elements[n].ContactData.Normal, out var result4);
					vertexContacts.Elements[n].ContactData.Normal = vertexContacts.Elements[n].CorrectedNormal;
					vertexContacts.Elements[n].ContactData.PenetrationDepth *= MathHelper.Max(0f, result4);
					AddLocalContact(ref vertexContacts.Elements[n].ContactData, ref result);
				}
			}
			blockedEdgeRegions.Clear();
			blockedVertexRegions.Clear();
			vertexContacts.Clear();
			edgeContacts.Clear();
		}
		TinyList<TriangleIndices> tinyList = default(TinyList<TriangleIndices>);
		foreach (KeyValuePair<TriangleIndices, TrianglePairTester> activePairTester in activePairTesters)
		{
			if (!activePairTester.Value.Updated)
			{
				if (!tinyList.Add(activePairTester.Key))
				{
					break;
				}
			}
			else
			{
				activePairTester.Value.Updated = false;
			}
		}
		for (int num2 = tinyList.count - 1; num2 >= 0; num2--)
		{
			TrianglePairTester trianglePairTester = activePairTesters[tinyList[num2]];
			trianglePairTester.CleanUp();
			GiveBackTester(trianglePairTester);
			activePairTesters.Remove(tinyList[num2]);
		}
		ProcessCandidates(candidatesToAdd);
		if (contacts.count + candidatesToAdd.count > 4)
		{
			ContactReducer.ReduceContacts(contacts, candidatesToAdd, contactIndicesToRemove, reducedCandidates);
			RemoveQueuedContacts();
			for (int num3 = reducedCandidates.count - 1; num3 >= 0; num3--)
			{
				Add(ref reducedCandidates.Elements[num3]);
				reducedCandidates.RemoveAt(num3);
			}
		}
		else if (candidatesToAdd.count > 0)
		{
			for (int num4 = 0; num4 < candidatesToAdd.count; num4++)
			{
				Add(ref candidatesToAdd.Elements[num4]);
			}
		}
		candidatesToAdd.Clear();
	}

	private void AddLocalContact(ref ContactData contact, ref Matrix3X3 orientation)
	{
		Matrix3X3.Transform(ref contact.Position, ref orientation, out contact.Position);
		Vector3.Add(ref contact.Position, ref convex.worldTransform.Position, out contact.Position);
		Matrix3X3.Transform(ref contact.Normal, ref orientation, out contact.Normal);
		if (IsContactUnique(ref contact))
		{
			candidatesToAdd.Add(ref contact);
		}
	}

	protected void GetNormal(ref Vector3 uncorrectedNormal, out Vector3 normal)
	{
		Vector3.Subtract(ref localTriangleShape.vB, ref localTriangleShape.vA, out var result);
		Vector3.Subtract(ref localTriangleShape.vC, ref localTriangleShape.vA, out var result2);
		switch (localTriangleShape.sidedness)
		{
		case TriangleSidedness.DoubleSided:
		{
			Vector3.Cross(ref result, ref result2, out normal);
			Vector3.Dot(ref normal, ref uncorrectedNormal, out var result3);
			if (result3 < 0f)
			{
				Vector3.Negate(ref normal, out normal);
			}
			break;
		}
		case TriangleSidedness.Clockwise:
			Vector3.Cross(ref result2, ref result, out normal);
			break;
		default:
			Vector3.Cross(ref result, ref result2, out normal);
			break;
		}
	}

	private bool AnalyzeCandidate(ref TriangleIndices indices, TrianglePairTester pairTester, ref ContactData contact)
	{
		VertexContact item2 = default(VertexContact);
		EdgeContact item = default(EdgeContact);
		switch (pairTester.GetRegion(ref contact))
		{
		case VoronoiRegion.A:
			item2.ContactData = contact;
			item2.Vertex = indices.A;
			item2.ShouldCorrect = pairTester.ShouldCorrectContactNormal;
			GetNormal(ref contact.Normal, out item2.CorrectedNormal);
			vertexContacts.Add(ref item2);
			blockedEdgeRegions.Add(new Edge(indices.A, indices.B));
			blockedEdgeRegions.Add(new Edge(indices.B, indices.C));
			blockedEdgeRegions.Add(new Edge(indices.A, indices.C));
			blockedVertexRegions.Add(indices.B);
			blockedVertexRegions.Add(indices.C);
			break;
		case VoronoiRegion.B:
			item2.ContactData = contact;
			item2.Vertex = indices.B;
			item2.ShouldCorrect = pairTester.ShouldCorrectContactNormal;
			GetNormal(ref contact.Normal, out item2.CorrectedNormal);
			vertexContacts.Add(ref item2);
			blockedEdgeRegions.Add(new Edge(indices.A, indices.B));
			blockedEdgeRegions.Add(new Edge(indices.B, indices.C));
			blockedEdgeRegions.Add(new Edge(indices.A, indices.C));
			blockedVertexRegions.Add(indices.A);
			blockedVertexRegions.Add(indices.C);
			break;
		case VoronoiRegion.C:
			item2.ContactData = contact;
			item2.Vertex = indices.C;
			item2.ShouldCorrect = pairTester.ShouldCorrectContactNormal;
			GetNormal(ref contact.Normal, out item2.CorrectedNormal);
			vertexContacts.Add(ref item2);
			blockedEdgeRegions.Add(new Edge(indices.A, indices.B));
			blockedEdgeRegions.Add(new Edge(indices.B, indices.C));
			blockedEdgeRegions.Add(new Edge(indices.A, indices.C));
			blockedVertexRegions.Add(indices.A);
			blockedVertexRegions.Add(indices.B);
			break;
		case VoronoiRegion.AB:
			item.Edge = new Edge(indices.A, indices.B);
			item.ContactData = contact;
			item.ShouldCorrect = pairTester.ShouldCorrectContactNormal;
			GetNormal(ref contact.Normal, out item.CorrectedNormal);
			edgeContacts.Add(ref item);
			blockedEdgeRegions.Add(new Edge(indices.B, indices.C));
			blockedEdgeRegions.Add(new Edge(indices.A, indices.C));
			blockedVertexRegions.Add(indices.A);
			blockedVertexRegions.Add(indices.B);
			blockedVertexRegions.Add(indices.C);
			break;
		case VoronoiRegion.AC:
			item.Edge = new Edge(indices.A, indices.C);
			item.ContactData = contact;
			item.ShouldCorrect = pairTester.ShouldCorrectContactNormal;
			GetNormal(ref contact.Normal, out item.CorrectedNormal);
			edgeContacts.Add(ref item);
			blockedEdgeRegions.Add(new Edge(indices.A, indices.B));
			blockedEdgeRegions.Add(new Edge(indices.B, indices.C));
			blockedVertexRegions.Add(indices.A);
			blockedVertexRegions.Add(indices.B);
			blockedVertexRegions.Add(indices.C);
			break;
		case VoronoiRegion.BC:
			item.Edge = new Edge(indices.B, indices.C);
			item.ContactData = contact;
			item.ShouldCorrect = pairTester.ShouldCorrectContactNormal;
			GetNormal(ref contact.Normal, out item.CorrectedNormal);
			edgeContacts.Add(ref item);
			blockedEdgeRegions.Add(new Edge(indices.A, indices.B));
			blockedEdgeRegions.Add(new Edge(indices.A, indices.C));
			blockedVertexRegions.Add(indices.A);
			blockedVertexRegions.Add(indices.B);
			blockedVertexRegions.Add(indices.C);
			break;
		default:
			blockedEdgeRegions.Add(new Edge(indices.A, indices.B));
			blockedEdgeRegions.Add(new Edge(indices.B, indices.C));
			blockedEdgeRegions.Add(new Edge(indices.A, indices.C));
			blockedVertexRegions.Add(indices.A);
			blockedVertexRegions.Add(indices.B);
			blockedVertexRegions.Add(indices.C);
			return true;
		}
		return false;
	}

	protected override void Add(ref ContactData contactCandidate)
	{
		ContactSupplementData item = default(ContactSupplementData);
		item.BasePenetrationDepth = contactCandidate.PenetrationDepth;
		RigidTransform.TransformByInverse(ref contactCandidate.Position, ref convex.worldTransform, out item.LocalOffsetA);
		RigidTransform transform = MeshTransform;
		RigidTransform.TransformByInverse(ref contactCandidate.Position, ref transform, out item.LocalOffsetB);
		supplementData.Add(ref item);
		base.Add(ref contactCandidate);
	}

	protected override void Remove(int contactIndex)
	{
		supplementData.RemoveAt(contactIndex);
		base.Remove(contactIndex);
	}

	private bool IsContactUnique(ref ContactData contactCandidate)
	{
		RigidTransform transform = MeshTransform;
		float result;
		for (int i = 0; i < contacts.count; i++)
		{
			Vector3.DistanceSquared(ref contacts.Elements[i].Position, ref contactCandidate.Position, out result);
			if (result < CollisionDetectionSettings.ContactMinimumSeparationDistanceSquared)
			{
				Vector3.Dot(ref contacts.Elements[i].Normal, ref contactCandidate.Normal, out result);
				if (Math.Abs(result) >= CollisionDetectionSettings.nonconvexNormalDotMinimum)
				{
					contacts.Elements[i].Normal = contactCandidate.Normal;
					contacts.Elements[i].Position = contactCandidate.Position;
					contacts.Elements[i].PenetrationDepth = contactCandidate.PenetrationDepth;
					supplementData.Elements[i].BasePenetrationDepth = contactCandidate.PenetrationDepth;
					RigidTransform.TransformByInverse(ref contactCandidate.Position, ref convex.worldTransform, out supplementData.Elements[i].LocalOffsetA);
					RigidTransform.TransformByInverse(ref contactCandidate.Position, ref transform, out supplementData.Elements[i].LocalOffsetB);
					return false;
				}
			}
		}
		for (int j = 0; j < candidatesToAdd.count; j++)
		{
			Vector3.DistanceSquared(ref candidatesToAdd.Elements[j].Position, ref contactCandidate.Position, out result);
			if (result < CollisionDetectionSettings.ContactMinimumSeparationDistanceSquared)
			{
				Vector3.Dot(ref candidatesToAdd.Elements[j].Normal, ref contactCandidate.Normal, out result);
				if (Math.Abs(result) >= CollisionDetectionSettings.nonconvexNormalDotMinimum)
				{
					return false;
				}
			}
		}
		return true;
	}

	protected virtual void ProcessCandidates(RawValueList<ContactData> candidates)
	{
	}

	/// <summary>
	///  Cleans up the manifold.
	/// </summary>
	public override void CleanUp()
	{
		supplementData.Clear();
		contacts.Clear();
		convex = null;
		foreach (KeyValuePair<TriangleIndices, TrianglePairTester> activePairTester in activePairTesters)
		{
			activePairTester.Value.CleanUp();
			GiveBackTester(activePairTester.Value);
		}
		activePairTesters.Clear();
		CleanUpOverlappingTriangles();
		base.CleanUp();
	}
}
