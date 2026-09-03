using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class PlaneEffect : IEffect
{
	public List<TreeNode> treeNodes;

	public VertexBuffer vBuffer;

	public int numLines;

	public Vector3[] cornerNodes = (Vector3[])(object)new Vector3[4];

	public PlaneEffect()
	{
		treeNodes = new List<TreeNode>();
	}

	public void iteratePlane()
	{
		int i = 0;
		Random random = new Random();
		for (; i < treeNodes.Count; i++)
		{
			if (treeNodes[i].calculated)
			{
				continue;
			}
			if (treeNodes[i].shiftSide)
			{
				if (treeNodes[i].gensLeft <= 0)
				{
					continue;
				}
				double num = random.NextDouble();
				float num2 = (float)random.NextDouble();
				TreeNode treeNode = new TreeNode(treeNodes[i]);
				if (num < 0.5)
				{
					if (1f < treeNode.curNode.X + num2 * treeNodes[i].ssRand + treeNodes[i].sideStep)
					{
						ref Vector3 curNode = ref treeNode.curNode;
						curNode.X -= num2 * treeNodes[i].ssRand + treeNodes[i].sideStep;
					}
					else
					{
						ref Vector3 curNode2 = ref treeNode.curNode;
						curNode2.X += num2 * treeNodes[i].ssRand + treeNodes[i].sideStep;
					}
				}
				else if (0f > treeNode.curNode.X - num2 * treeNodes[i].ssRand - treeNodes[i].sideStep)
				{
					ref Vector3 curNode3 = ref treeNode.curNode;
					curNode3.X += num2 * treeNodes[i].ssRand + treeNodes[i].sideStep;
				}
				else
				{
					ref Vector3 curNode4 = ref treeNode.curNode;
					curNode4.X -= num2 * treeNodes[i].ssRand + treeNodes[i].sideStep;
				}
				ref Vector3 curNode5 = ref treeNode.curNode;
				curNode5.Y *= 0f;
				ref Vector3 curNode6 = ref treeNode.curNode;
				curNode6.Y += treeNode.curNode.X * treeNode.curNode.Z;
				treeNode.parent = i;
				treeNode.nextNode = -1;
				treeNodes[i].nextNode = treeNodes.Count;
				treeNode.shiftSide = false;
				treeNodes[i].calculated = true;
				addNode(treeNode);
			}
			else
			{
				float num2 = (float)(random.NextDouble() - 0.5);
				TreeNode treeNode = new TreeNode(treeNodes[i]);
				ref Vector3 curNode7 = ref treeNode.curNode;
				curNode7.Z += num2 * treeNodes[i].vRand + treeNodes[i].vel;
				if (treeNode.curNode.Z > 1f)
				{
					treeNode.curNode.Z = 1f;
					treeNode.gensLeft = 0;
				}
				treeNode.shiftSide = true;
				treeNode.calculated = false;
				ref Vector3 curNode8 = ref treeNode.curNode;
				curNode8.Y *= 0f;
				ref Vector3 curNode9 = ref treeNode.curNode;
				curNode9.Y += treeNode.curNode.X * treeNode.curNode.Z;
				treeNode.parent = i;
				treeNodes[i].nextNode = treeNodes.Count;
				addNode(treeNode);
			}
		}
	}

	public void FinalizeEffect()
	{
		FinalizeEffect(centerTransform: false);
	}

	public void FinalizeEffect(bool centerTransform)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		int i = 0;
		List<VertexPositionColor> list = new List<VertexPositionColor>();
		for (; i < treeNodes.Count; i++)
		{
			if (treeNodes[i].parent != -1)
			{
				continue;
			}
			TreeNode treeNode = treeNodes[i];
			list.Add(calculatePoint(treeNode.curNode.Z, treeNode.curNode.X, treeNode.color, centerTransform));
			while (treeNode.nextNode > 0)
			{
				treeNode = treeNodes[treeNode.nextNode];
				list.Add(calculatePoint(treeNode.curNode.Z, treeNode.curNode.X, treeNode.color, centerTransform));
				if (treeNode.nextNode > 0)
				{
					list.Add(calculatePoint(treeNode.curNode.Z, treeNode.curNode.X, treeNode.color, centerTransform));
				}
			}
		}
		numLines = list.Count / 2;
		vBuffer = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, list.Count * VertexPositionColor.SizeInBytes, (BufferUsage)8);
		vBuffer.SetData<VertexPositionColor>(list.ToArray());
		list.Clear();
	}

	public void addNode(TreeNode toAdd)
	{
		treeNodes.Add(toAdd);
	}

	public override void draw()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(cornerNodes[0]));
		BaseGame.Get().fogEffect.Parameters["xWorld"].SetValue(BaseGame.Get().world);
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().fogEffect.Parameters["usePalette"].SetValue(false);
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(vBuffer, 0, VertexPositionColor.SizeInBytes);
		BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)2, 0, numLines);
		BaseGame.Get().matStack.PopMatrix();
	}

	private VertexPositionColor calculatePoint(float deepPoint, float widePoint, Color _col)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return calculatePoint(deepPoint, widePoint, _col, centerTransform: false);
	}

	private VertexPositionColor calculatePoint(float deepPoint, float widePoint, Color _col, bool centerTransform)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = cornerNodes[2] - cornerNodes[0];
		Vector3 val2 = cornerNodes[3] - cornerNodes[1];
		val *= deepPoint;
		val2 *= deepPoint;
		Vector3 val3 = val2 + cornerNodes[1] - (val + cornerNodes[0]);
		val3 *= widePoint;
		val3 += val;
		if (centerTransform)
		{
			val3.X -= (cornerNodes[3].X - cornerNodes[0].X) / 2f;
			val3.Z -= (cornerNodes[3].Z - cornerNodes[0].Z) / 2f;
		}
		return new VertexPositionColor(val3, _col);
	}

	public VertexPositionColor ConvertTreeNodeToVertPosColor(TreeNode v)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return new VertexPositionColor(v.curNode, Color.White);
	}

	public VertexPositionColor ConvertVect3ToVertPosColor(Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return new VertexPositionColor(v, Color.White);
	}
}
