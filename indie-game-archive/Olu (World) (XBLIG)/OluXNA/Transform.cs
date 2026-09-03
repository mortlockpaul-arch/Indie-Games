using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class Transform
{
	private int start;

	private int end;

	private List<ITransform> transforms;

	public Transform(Dictionary<string, string> attributes, XmlNode node)
	{
		LevelLoader.GetAttributeDictionary(node);
		transforms = new List<ITransform>();
		start = LevelLoader.GetIntFromAtt(attributes, "beatstart", -1);
		end = LevelLoader.GetIntFromAtt(attributes, "beatend", -1);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name != "#comment")
			{
				transforms.Add((ITransform)LevelLoader.MakeObj(childNode));
				transforms[transforms.Count - 1].Initialize(start, end);
			}
		}
	}

	public bool Update(double gametime)
	{
		if (BaseGame.Get().elaspedEndTime >= start && BaseGame.Get().elaspedEndTime < end)
		{
			foreach (ITransform transform in transforms)
			{
				transform.Update(gametime);
			}
			return true;
		}
		return false;
	}

	public Matrix GetAllMatrix()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.Identity;
		foreach (ITransform transform in transforms)
		{
			val *= transform.GetMatrix();
		}
		return val;
	}

	public Matrix GetAllMatrix(float progress)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.Identity;
		foreach (ITransform transform in transforms)
		{
			val *= transform.GetMatrix(progress);
		}
		return val;
	}

	public Matrix GetMatrix()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.Identity;
		foreach (ITransform transform in transforms)
		{
			if (!(transform is TScale))
			{
				val *= transform.GetMatrix();
			}
		}
		return val;
	}

	public Matrix GetMatrix(float progress)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.Identity;
		foreach (ITransform transform in transforms)
		{
			if (!(transform is TScale))
			{
				val *= transform.GetMatrix(progress);
			}
		}
		return val;
	}

	public Matrix GetScaleMatrix()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.Identity;
		foreach (ITransform transform in transforms)
		{
			if (transform is TScale)
			{
				val *= transform.GetMatrix();
			}
		}
		return val;
	}

	public Matrix GetScaleMatrix(float progress)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.Identity;
		foreach (ITransform transform in transforms)
		{
			if (transform is TScale)
			{
				val *= transform.GetMatrix(progress);
			}
		}
		return val;
	}
}
