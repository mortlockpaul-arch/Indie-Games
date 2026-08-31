using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseSystems;

/// <summary>
///  Defines a system that accelerates bounding volume and ray cast queries.
/// </summary>
public interface IQueryAccelerator
{
	/// <summary>
	/// Gets the broad phase associated with this query accelerator, if any.
	/// </summary>
	BroadPhase BroadPhase { get; }

	/// <summary>
	///  Gets the broad phase entries overlapping the ray.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="outputIntersections">Overlapped entries.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	bool RayCast(Ray ray, IList<BroadPhaseEntry> outputIntersections);

	/// <summary>
	///  Gets the broad phase entries overlapping the ray.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	///  <param name="maximumLength">Maximum length of the ray in units of the ray's direction's length.</param>
	/// <param name="outputIntersections">Overlapped entries.</param>
	/// <returns>Whether or not the ray hit anything.</returns>
	bool RayCast(Ray ray, float maximumLength, IList<BroadPhaseEntry> outputIntersections);

	/// <summary>
	/// Gets the entries with bounding boxes which overlap the bounding shape.
	/// </summary>
	/// <param name="boundingShape">Bounding shape to test.</param>
	/// <param name="overlaps">Overlapped entries.</param>
	void GetEntries(BoundingBox boundingShape, IList<BroadPhaseEntry> overlaps);

	/// <summary>
	/// Gets the entries with bounding boxes which overlap the bounding shape.
	/// </summary>
	/// <param name="boundingShape">Bounding shape to test.</param>
	/// <param name="overlaps">Overlapped entries.</param>
	void GetEntries(BoundingSphere boundingShape, IList<BroadPhaseEntry> overlaps);

	/// <summary>
	/// Gets the entries with bounding boxes which overlap the bounding shape.
	/// </summary>
	/// <param name="boundingShape">Bounding shape to test.</param>
	/// <param name="overlaps">Overlapped entries.</param>
	void GetEntries(BoundingFrustum boundingShape, IList<BroadPhaseEntry> overlaps);
}
