using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common;

public class Bone
{
	public int index;

	public int parentIndex = -1;

	public string name;

	public Matrix bind;

	public Matrix inverse;

	public List<Bone> children = new List<Bone>();

	public Bone(int xIndex, int xParentIndex, string xName, Matrix mTransform, Matrix mInverse)
	{
		index = xIndex;
		parentIndex = xParentIndex;
		name = xName;
		bind = mTransform;
		inverse = mInverse;
	}

	public Bone Clone()
	{
		return new Bone(index, parentIndex, name, bind, inverse);
	}
}
