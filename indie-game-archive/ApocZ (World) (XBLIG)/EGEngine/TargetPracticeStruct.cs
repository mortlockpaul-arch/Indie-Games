using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public struct TargetPracticeStruct
{
	public string Name;

	public bool Active;

	public int NumberHits;

	public eOOBB PhysicsBox;

	public float TargetTimer;

	public float CurrentAngle;

	public float TargetAngle;

	public Vector3 TargetOffset;

	public eMesh model;

	public Matrix transform;

	public eTriangleMesh TriMesh;
}
