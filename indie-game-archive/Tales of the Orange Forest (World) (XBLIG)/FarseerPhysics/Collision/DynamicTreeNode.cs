namespace FarseerPhysics.Collision;

internal struct DynamicTreeNode
{
	internal AABB AABB;

	internal int Child1;

	internal int Child2;

	internal int LeafCount;

	internal int ParentOrNext;

	internal object UserData;

	internal bool IsLeaf()
	{
		return Child1 == -1;
	}
}
