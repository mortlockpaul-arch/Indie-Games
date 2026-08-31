using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Entities.Prefabs;

/// <summary>
/// Triangle-shaped object that can collide and move.  After making an entity, add it to a Space so that the engine can manage it.
/// </summary>
public class Triangle : Entity<ConvexCollidable<TriangleShape>>
{
	/// <summary>
	///  Gets or sets the first vertex of the triangle in local space.
	/// </summary>
	public Vector3 LocalVertexA
	{
		get
		{
			return base.CollisionInformation.Shape.VertexA;
		}
		set
		{
			base.CollisionInformation.Shape.VertexA = value;
		}
	}

	/// <summary>
	///  Gets or sets the second vertex of the triangle in local space.
	/// </summary>
	public Vector3 LocalVertexB
	{
		get
		{
			return base.CollisionInformation.Shape.VertexB;
		}
		set
		{
			base.CollisionInformation.Shape.VertexB = value;
		}
	}

	/// <summary>
	///  Gets or sets the third vertex of the triangle in local space.
	/// </summary>
	public Vector3 LocalVertexC
	{
		get
		{
			return base.CollisionInformation.Shape.VertexC;
		}
		set
		{
			base.CollisionInformation.Shape.VertexC = value;
		}
	}

	/// <summary>
	///  Gets or sets the first vertex of the triangle in world space.
	/// </summary>
	public Vector3 VertexA
	{
		get
		{
			return Matrix3X3.Transform(base.CollisionInformation.Shape.VertexA, orientationMatrix) + position;
		}
		set
		{
			base.CollisionInformation.Shape.VertexA = Matrix3X3.TransformTranspose(value - position, orientationMatrix);
		}
	}

	/// <summary>
	///  Gets or sets the second vertex of the triangle in world space.
	/// </summary>
	public Vector3 VertexB
	{
		get
		{
			return Matrix3X3.Transform(base.CollisionInformation.Shape.VertexB, orientationMatrix) + position;
		}
		set
		{
			base.CollisionInformation.Shape.VertexB = Matrix3X3.TransformTranspose(value - position, orientationMatrix);
		}
	}

	/// <summary>
	///  Gets or sets the third vertex of the triangle in world space.
	/// </summary>
	public Vector3 VertexC
	{
		get
		{
			return Matrix3X3.Transform(base.CollisionInformation.Shape.VertexB, orientationMatrix) + position;
		}
		set
		{
			base.CollisionInformation.Shape.VertexC = Matrix3X3.TransformTranspose(value - position, orientationMatrix);
		}
	}

	/// <summary>
	///  Gets or sets the sidedness of the triangle.
	/// </summary>
	public TriangleSidedness Sidedness
	{
		get
		{
			return base.CollisionInformation.Shape.Sidedness;
		}
		set
		{
			base.CollisionInformation.Shape.Sidedness = value;
		}
	}

	/// <summary>
	/// Constructs a dynamic triangle.
	/// </summary>
	/// <param name="v1">Position of the first vertex.</param>
	/// <param name="v2">Position of the second vertex.</param>
	/// <param name="v3">Position of the third vertex.</param>
	/// <param name="mass">Mass of the object.</param>
	public Triangle(Vector3 v1, Vector3 v2, Vector3 v3, float mass)
	{
		TriangleShape shape = new TriangleShape(v1, v2, v3, out var center);
		Initialize(new ConvexCollidable<TriangleShape>(shape), mass);
		base.Position = center;
	}

	/// <summary>
	/// Constructs a nondynamic triangle.
	/// </summary>
	/// <param name="v1">Position of the first vertex.</param>
	/// <param name="v2">Position of the second vertex.</param>
	/// <param name="v3">Position of the third vertex.</param>
	public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		TriangleShape shape = new TriangleShape(v1, v2, v3, out var center);
		Initialize(new ConvexCollidable<TriangleShape>(shape));
		base.Position = center;
	}

	/// <summary>
	/// Constructs a dynamic triangle.
	/// </summary>
	/// <param name="pos">Position where the triangle is initialy centered.</param>
	/// <param name="v1">Position of the first vertex.</param>
	/// <param name="v2">Position of the second vertex.</param>
	/// <param name="v3">Position of the third vertex.</param>
	/// <param name="mass">Mass of the object.</param>
	public Triangle(Vector3 pos, Vector3 v1, Vector3 v2, Vector3 v3, float mass)
		: this(v1, v2, v3, mass)
	{
		base.Position = pos;
	}

	/// <summary>
	/// Constructs a nondynamic triangle.
	/// </summary>
	/// <param name="pos">Position where the triangle is initially centered.</param>
	/// <param name="v1">Position of the first vertex.</param>
	/// <param name="v2">Position of the second vertex.</param>
	/// <param name="v3">Position of the third vertex.</param>
	public Triangle(Vector3 pos, Vector3 v1, Vector3 v2, Vector3 v3)
		: this(v1, v2, v3)
	{
		base.Position = pos;
	}

	/// <summary>
	/// Constructs a dynamic triangle.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="v1">Position of the first vertex.</param>
	/// <param name="v2">Position of the second vertex.</param>
	/// <param name="v3">Position of the third vertex.</param>
	/// <param name="mass">Mass of the object.</param>
	public Triangle(MotionState motionState, Vector3 v1, Vector3 v2, Vector3 v3, float mass)
		: this(v1, v2, v3, mass)
	{
		base.MotionState = motionState;
	}

	/// <summary>
	/// Constructs a nondynamic triangle.
	/// </summary>
	/// <param name="motionState">Motion state specifying the entity's initial state.</param>
	/// <param name="v1">Position of the first vertex.</param>
	/// <param name="v2">Position of the second vertex.</param>
	/// <param name="v3">Position of the third vertex.</param>
	public Triangle(MotionState motionState, Vector3 v1, Vector3 v2, Vector3 v3)
		: this(v1, v2, v3)
	{
		base.MotionState = motionState;
	}
}
