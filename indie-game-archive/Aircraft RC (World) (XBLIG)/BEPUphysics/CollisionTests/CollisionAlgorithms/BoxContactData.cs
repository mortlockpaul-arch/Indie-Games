using System;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

/// <summary>
/// Stores basic data used by some collision systems.
/// </summary>
public struct BoxContactData : IEquatable<BoxContactData>
{
	/// <summary>
	/// Position of the candidate contact.
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// Depth of the candidate contact.
	/// </summary>
	public float Depth;

	/// <summary>
	/// Id of the candidate contact.
	/// </summary>
	public int Id;

	/// <summary>
	/// Returns true if the other data has the same id.
	/// </summary>
	/// <param name="other">Data to compare.</param>
	/// <returns>True if the other data has the same id, false otherwise.</returns>
	public bool Equals(BoxContactData other)
	{
		return Id == other.Id;
	}
}
