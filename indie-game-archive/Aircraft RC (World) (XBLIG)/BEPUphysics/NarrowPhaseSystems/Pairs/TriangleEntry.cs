using System;
using BEPUphysics.Collidables.MobileCollidables;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
/// Contains a triangle collidable and its index.  Used by mobile mesh-mesh collisions.
/// </summary>
public struct TriangleEntry : IEquatable<TriangleEntry>
{
	/// <summary>
	/// Index of the triangle that was the source of this entry.
	/// </summary>
	public int Index;

	/// <summary>
	/// Collidable for the triangle.
	/// </summary>
	public TriangleCollidable Collidable;

	/// <summary>
	/// Gets the hash code of the object.
	/// </summary>
	/// <returns>Hash code of the object.</returns>
	public override int GetHashCode()
	{
		return Index;
	}

	/// <summary>
	/// Determines if two colliders refer to the same triangle.
	/// </summary>
	/// <param name="other">Object to compare.</param>
	/// <returns>Whether or not the objects are equal.</returns>
	public bool Equals(TriangleEntry other)
	{
		return other.Index == Index;
	}
}
