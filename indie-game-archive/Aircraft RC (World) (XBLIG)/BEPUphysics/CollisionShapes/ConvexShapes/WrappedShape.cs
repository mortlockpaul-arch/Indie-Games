using System;
using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes.ConvexShapes;

/// <summary>
///  Shape that wraps other convex shapes in a convex hull.
///  One way to think of it is to collect a bunch of items and wrap shrinkwrap around them.
///  That surface is the shape of the WrappedShape.
/// </summary>
public class WrappedShape : ConvexShape
{
	private ObservableList<ConvexShapeEntry> shapes = new ObservableList<ConvexShapeEntry>();

	/// <summary>
	///  Gets the shapes in wrapped shape.
	/// </summary>
	public ObservableList<ConvexShapeEntry> Shapes => shapes;

	private void Recenter(out Vector3 center)
	{
		center = ComputeCenter();
		for (int i = 0; i < shapes.Count; i++)
		{
			shapes.list.Elements[i].Transform.Position -= center;
		}
	}

	/// <summary>
	///  Constructs a wrapped shape.
	///  A constructor is also available which takes a list of objects rather than just a pair.
	///  The shape will be recentered.  If the center is needed, use the other constructor.
	/// </summary>
	/// <param name="firstShape">First shape in the wrapped shape.</param>
	/// <param name="secondShape">Second shape in the wrapped shape.</param>
	public WrappedShape(ConvexShapeEntry firstShape, ConvexShapeEntry secondShape)
	{
		shapes.Add(firstShape);
		shapes.Add(secondShape);
		Recenter(out var _);
		shapes.Changed += ShapesChanged;
	}

	/// <summary>
	///  Constructs a wrapped shape.
	///  A constructor is also available which takes a list of objects rather than just a pair.
	///  The shape will be recentered.
	/// </summary>
	/// <param name="firstShape">First shape in the wrapped shape.</param>
	/// <param name="secondShape">Second shape in the wrapped shape.</param>
	/// <param name="center">Center of the shape before recentering..</param>
	public WrappedShape(ConvexShapeEntry firstShape, ConvexShapeEntry secondShape, out Vector3 center)
	{
		shapes.Add(firstShape);
		shapes.Add(secondShape);
		Recenter(out center);
		shapes.Changed += ShapesChanged;
		OnShapeChanged();
	}

	/// <summary>
	///  Constructs a wrapped shape.
	///  The shape will be recentered; if the center is needed, use the other constructor.
	/// </summary>
	/// <param name="shapeEntries">Shape entries used to construct the shape.</param>
	/// <exception cref="T:System.Exception">Thrown when the shape list is empty.</exception>
	public WrappedShape(IList<ConvexShapeEntry> shapeEntries)
	{
		if (shapeEntries.Count == 0)
		{
			throw new Exception("Cannot create a wrapped shape with no contained shapes.");
		}
		for (int i = 0; i < shapeEntries.Count; i++)
		{
			shapes.Add(shapeEntries[i]);
		}
		Recenter(out var _);
		shapes.Changed += ShapesChanged;
		OnShapeChanged();
	}

	/// <summary>
	///  Constructs a wrapped shape.
	///  The shape will be recentered.
	/// </summary>
	/// <param name="shapeEntries">Shape entries used to construct the shape.</param>
	///  <param name="center">Center of the shape before recentering.</param>
	/// <exception cref="T:System.Exception">Thrown when the shape list is empty.</exception>
	public WrappedShape(IList<ConvexShapeEntry> shapeEntries, out Vector3 center)
	{
		if (shapeEntries.Count == 0)
		{
			throw new Exception("Cannot create a wrapped shape with no contained shapes.");
		}
		for (int i = 0; i < shapeEntries.Count; i++)
		{
			shapes.Add(shapeEntries[i]);
		}
		Recenter(out center);
		shapes.Changed += ShapesChanged;
		OnShapeChanged();
	}

	private void ShapesChanged(ObservableList<ConvexShapeEntry> list)
	{
		OnShapeChanged();
	}

	/// <summary>
	/// Gets the bounding box of the shape given a transform.
	/// </summary>
	/// <param name="shapeTransform">Transform to use.</param>
	/// <param name="boundingBox">Bounding box of the transformed shape.</param>
	public override void GetBoundingBox(ref RigidTransform shapeTransform, out BoundingBox boundingBox)
	{
		RigidTransform.Transform(ref shapes.list.Elements[0].Transform, ref shapeTransform, out var combined);
		shapes.list.Elements[0].CollisionShape.GetBoundingBox(ref combined, out boundingBox);
		for (int i = 1; i < shapes.list.count; i++)
		{
			RigidTransform.Transform(ref shapes.list.Elements[i].Transform, ref shapeTransform, out combined);
			shapes.list.Elements[i].CollisionShape.GetBoundingBox(ref combined, out var boundingBox2);
			BoundingBox.CreateMerged(ref boundingBox, ref boundingBox2, out boundingBox);
		}
		boundingBox.Min.X -= collisionMargin;
		boundingBox.Min.Y -= collisionMargin;
		boundingBox.Min.Z -= collisionMargin;
		boundingBox.Max.X += collisionMargin;
		boundingBox.Max.Y += collisionMargin;
		boundingBox.Max.Z += collisionMargin;
	}

	/// <summary>
	///  Gets the extreme point of the shape in local space in a given direction.
	/// </summary>
	/// <param name="direction">Direction to find the extreme point in.</param>
	/// <param name="extremePoint">Extreme point on the shape.</param>
	public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
	{
		shapes.list.Elements[0].CollisionShape.GetExtremePoint(direction, ref shapes.list.Elements[0].Transform, out extremePoint);
		Vector3.Dot(ref extremePoint, ref direction, out var result);
		for (int i = 1; i < shapes.list.count; i++)
		{
			shapes.list.Elements[i].CollisionShape.GetExtremePoint(direction, ref shapes.list.Elements[i].Transform, out var extremePoint2);
			Vector3.Dot(ref direction, ref extremePoint2, out var result2);
			if (result2 > result)
			{
				extremePoint = extremePoint2;
				result = result2;
			}
		}
	}

	/// <summary>
	/// Computes the maximum radius of the shape.
	/// This is often larger than the actual maximum radius;
	/// it is simply an approximation that avoids underestimating.
	/// </summary>
	/// <returns>Maximum radius of the shape.</returns>
	public override float ComputeMaximumRadius()
	{
		float num = 0f;
		for (int i = 0; i < shapes.Count; i++)
		{
			float num2 = shapes.list.Elements[i].CollisionShape.ComputeMaximumRadius() + shapes.list.Elements[i].Transform.Position.Length();
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num + collisionMargin;
	}

	public override float ComputeMinimumRadius()
	{
		float num = 0f;
		for (int i = 0; i < shapes.Count; i++)
		{
			float num2 = shapes.list.Elements[i].CollisionShape.ComputeMinimumRadius();
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num + collisionMargin;
	}

	/// <summary>
	/// Retrieves an instance of an EntityCollidable that uses this EntityShape.  Mainly used by compound bodies.
	/// </summary>
	/// <returns>EntityCollidable that uses this shape.</returns>
	public override EntityCollidable GetCollidableInstance()
	{
		return new ConvexCollidable<WrappedShape>(this);
	}
}
