using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TreeNode
{
	public Vector3 curNode;

	public int gensLeft;

	public float vel;

	public float vRand;

	public float sideStep;

	public float ssRand;

	public int nextNode;

	public int parent;

	public bool shiftSide;

	public bool calculated;

	public bool branchTree;

	public Color color;

	public TreeNode()
	{
	}

	public TreeNode(float x, float y, float z, int generationsLeft, float velocity, float vRandom, float sideSpeed, float sideSpeedRandom)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		curNode = new Vector3(x, y, z);
		gensLeft = generationsLeft;
		vel = velocity;
		vRand = vRandom;
		sideStep = sideSpeed;
		ssRand = sideSpeedRandom;
		nextNode = -1;
		parent = -1;
		shiftSide = false;
		calculated = false;
		branchTree = true;
		color = Color.White;
	}

	public TreeNode(TreeNode other)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		curNode = other.curNode;
		gensLeft = other.gensLeft;
		vel = other.vel;
		vRand = other.vRand;
		sideStep = other.sideStep;
		ssRand = other.ssRand;
		nextNode = other.nextNode;
		parent = other.parent;
		shiftSide = other.shiftSide;
		calculated = other.calculated;
		branchTree = other.branchTree;
		color = other.color;
	}

	public void setColor(Color _col)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		color = _col;
	}
}
