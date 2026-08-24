using GKEngine;
using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Particles;

public class ParticleEmitterSchema
{
	private Vector3 _temp_unit = default(Vector3);

	public Vector3[] positions;

	public Vector3[] deltas;

	public Vector3[] data;

	public ParticleEmitter.Mode mode;

	public int count;

	public int tween;

	public float scaleStart;

	public float scaleEnd;

	public int scaleTween;

	public float rotationStart;

	public float rotationEnd;

	public int rotationTween;

	public Color tintStart;

	public Color tintEnd;

	public int tintTween;

	public Texture2D textureStart;

	public Texture2D textureEnd;

	public int textureTween;

	public ParticleEmitterSchema(int xCount)
	{
		count = xCount;
		positions = new Vector3[count];
		deltas = new Vector3[count];
		data = new Vector3[count];
		for (int i = 0; i < count; i++)
		{
			ref Vector3 reference = ref positions[i];
			reference = Vector3.Zero;
			ref Vector3 reference2 = ref deltas[i];
			reference2 = Vector3.Zero;
			ref Vector3 reference3 = ref data[i];
			reference3 = new Vector3(-1f, 1f, 1f);
		}
	}

	public void Dispose()
	{
		positions = null;
		deltas = null;
		data = null;
		textureStart = null;
		textureEnd = null;
	}

	public void Float_Spread(ref Vector3[] aStack, uint xSlot, float xMin, float xMax)
	{
		Range range = new Range(xMin, xMax);
		float num = 0f;
		for (int i = 0; i < count; i++)
		{
			num = range.Lerp((float)i / (float)count);
			switch (xSlot)
			{
			case 0u:
				aStack[i].X = num;
				break;
			case 1u:
				aStack[i].Y = num;
				break;
			case 2u:
				aStack[i].Z = num;
				break;
			}
		}
	}

	public void Float_Random(ref Vector3[] aStack, uint xSlot, float xMin, float xMax)
	{
		float num = 0f;
		for (int i = 0; i < count; i++)
		{
			num = xMin + (float)(GameEngine.random.NextDouble() * (double)(xMax - xMin));
			switch (xSlot)
			{
			case 0u:
				aStack[i].X = num;
				break;
			case 1u:
				aStack[i].Y = num;
				break;
			case 2u:
				aStack[i].Z = num;
				break;
			}
		}
	}

	public void Float_Constant(ref Vector3[] aStack, uint xSlot, float xValue)
	{
		for (int i = 0; i < count; i++)
		{
			switch (xSlot)
			{
			case 0u:
				aStack[i].X = xValue;
				break;
			case 1u:
				aStack[i].Y = xValue;
				break;
			case 2u:
				aStack[i].Z = xValue;
				break;
			}
		}
	}

	public void Vector_Random(ref Vector3[] aStack, Vector3 vCenter, float xMin, float xMax)
	{
		for (int i = 0; i < count; i++)
		{
			_temp_unit.X = (float)(GameEngine.random.NextDouble() * 2.0) - 1f;
			_temp_unit.Y = (float)(GameEngine.random.NextDouble() * 2.0) - 1f;
			_temp_unit.Z = (float)(GameEngine.random.NextDouble() * 2.0) - 1f;
			_temp_unit.Normalize();
			float num = (float)(GameEngine.random.NextDouble() * (double)(xMax - xMin)) + xMin;
			aStack[i].X = vCenter.X + _temp_unit.X * num;
			aStack[i].Y = vCenter.Y + _temp_unit.Y * num;
			aStack[i].Z = vCenter.Z + _temp_unit.Z * num;
		}
	}

	public void Vector_Constant(ref Vector3[] aStack, Vector3 vValue)
	{
		for (int i = 0; i < count; i++)
		{
			aStack[i] = vValue;
		}
	}

	public void Vector_RandomRay(ref Vector3[] aStack, Vector3 vStart, Vector3 vUnit, float xDistance)
	{
		float num = 0f;
		for (int i = 0; i < count; i++)
		{
			num = (float)(GameEngine.random.NextDouble() * (double)xDistance);
			aStack[i].X = vStart.X + vUnit.X * num;
			aStack[i].Y = vStart.Y + vUnit.Y * num;
			aStack[i].Z = vStart.Z + vUnit.Z * num;
		}
	}

	public void Vector_Focus(ref Vector3[] aPositions, ref Vector3[] aStack, Vector3 vCenter, float xMin, float xMax)
	{
		for (int i = 0; i < count; i++)
		{
			Vector3.Normalize(ref aPositions[i], out aStack[i]);
			float num = (float)(GameEngine.random.NextDouble() * (double)(xMax - xMin)) + xMin;
			aStack[i].X *= num;
			aStack[i].Y *= num;
			aStack[i].Z *= num;
			aStack[i] += vCenter;
		}
	}

	public void Vector_Helix(ref Vector3[] aStack, Vector3 vStart, Vector3 vUnit, float xDistance, Range oAngle, Vector3 vRadius)
	{
		for (int i = 0; i < count; i++)
		{
			float num = (float)i / (float)count;
			Vector3 vector = Vector3.Transform(vRadius, Quaternion.CreateFromAxisAngle(vUnit, oAngle.Lerp(num)));
			aStack[i].X = vStart.X + vUnit.X * xDistance * num + vector.X;
			aStack[i].Y = vStart.Y + vUnit.Y * xDistance * num + vector.Y;
			aStack[i].Z = vStart.Z + vUnit.Z * xDistance * num + vector.Z;
		}
	}

	public void Vector_Random_XZ(ref Vector3[] aStack, Vector3 vCenter, Range oSpreadX, Range oSpreadZ)
	{
		for (int i = 0; i < count; i++)
		{
			aStack[i].X = vCenter.X + oSpreadX.random;
			aStack[i].Y = vCenter.Y;
			aStack[i].Z = vCenter.Z + oSpreadZ.random;
		}
	}
}
