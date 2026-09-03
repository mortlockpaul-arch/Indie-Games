using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class GraphicsStack
{
	private Effect toUse;

	private Stack<Matrix> matStack;

	private bool isBasicEffect;

	public GraphicsStack(Effect _eff)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		matStack = new Stack<Matrix>();
		matStack.Push(Matrix.Identity);
		toUse = _eff;
		isBasicEffect = false;
		if (_eff is BasicEffect)
		{
			isBasicEffect = true;
		}
	}

	public void ApplyMatrix(Matrix toAdd)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		toAdd *= matStack.Peek();
		matStack.Pop();
		matStack.Push(toAdd);
		if (isBasicEffect)
		{
			((BasicEffect)toUse).World = toAdd;
		}
		else
		{
			BaseGame.Get().world = toAdd;
			toUse.Parameters["xWorld"].SetValue(toAdd);
		}
		toUse.CommitChanges();
	}

	public void ApplyRawMatrix(Matrix toAdd)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		matStack.Pop();
		matStack.Push(toAdd);
		if (isBasicEffect)
		{
			((BasicEffect)toUse).World = toAdd;
		}
		else
		{
			BaseGame.Get().world = toAdd;
			toUse.Parameters["xWorld"].SetValue(toAdd);
		}
		toUse.CommitChanges();
	}

	public void PushMatrix()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		matStack.Push(matStack.Peek());
	}

	public void PopMatrix()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		matStack.Pop();
		if (isBasicEffect)
		{
			((BasicEffect)toUse).World = matStack.Peek();
		}
		else
		{
			BaseGame.Get().world = matStack.Peek();
			toUse.Parameters["xWorld"].SetValue(BaseGame.Get().world);
		}
		toUse.CommitChanges();
	}

	public Matrix Top()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return matStack.Peek();
	}

	public void Clear()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		matStack.Clear();
		matStack.Push(Matrix.Identity);
	}
}
