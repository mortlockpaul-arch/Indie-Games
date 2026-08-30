using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class PointLight
{
	private EffectParameter positionParameter;

	private EffectParameter instanceParameter;

	private float rangeValue = 30f;

	private float falloffValue = 2f;

	private Vector4 positionValue;

	private Color colorValue = Color.White;

	public Vector4 Position
	{
		get
		{
			return positionValue;
		}
		set
		{
			positionValue = value;
			positionParameter.SetValue(positionValue);
		}
	}

	public Color Color
	{
		get
		{
			return colorValue;
		}
		set
		{
			colorValue = value;
			instanceParameter.StructureMembers["color"].SetValue(colorValue.ToVector4());
		}
	}

	public float Range
	{
		get
		{
			return rangeValue;
		}
		set
		{
			rangeValue = value;
			instanceParameter.StructureMembers["range"].SetValue(rangeValue);
		}
	}

	public float Falloff
	{
		get
		{
			return falloffValue;
		}
		set
		{
			falloffValue = value;
			instanceParameter.StructureMembers["falloff"].SetValue(falloffValue);
		}
	}

	public PointLight(Vector4 initialPosition, EffectParameter lightParameter)
	{
		instanceParameter = lightParameter;
		positionParameter = instanceParameter.StructureMembers["position"];
		Position = initialPosition;
		instanceParameter.StructureMembers["range"].SetValue(rangeValue);
		instanceParameter.StructureMembers["falloff"].SetValue(falloffValue);
		instanceParameter.StructureMembers["color"].SetValue(colorValue.ToVector4());
	}
}
