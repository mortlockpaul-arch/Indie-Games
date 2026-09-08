using Microsoft.Xna.Framework;

namespace ProjectMercury.Modifiers;

public sealed class HueShiftModifier : Modifier
{
	private static Matrix YIQTransformMatrix;

	private static Matrix RGBTransformMatrix;

	public float HueShift;

	static HueShiftModifier()
	{
		YIQTransformMatrix = new Matrix(0.299f, 0.587f, 0.114f, 0f, 0.596f, -0.274f, -0.321f, 0f, 0.211f, -0.523f, 0.311f, 0f, 0f, 0f, 0f, 1f);
		Matrix.Invert(ref YIQTransformMatrix, out RGBTransformMatrix);
	}

	public override Modifier DeepCopy()
	{
		HueShiftModifier hueShiftModifier = new HueShiftModifier();
		hueShiftModifier.HueShift = HueShift;
		return hueShiftModifier;
	}

	protected internal unsafe override void Process(float dt, Particle* particleArray, int count)
	{
		float value = HueShift * dt * 3.141593f / 180f;
		float num = Calculator.Cos(value);
		float num2 = Calculator.Sin(value);
		Matrix matrix = new Matrix(1f, 0f, 0f, 0f, 0f, num, 0f - num2, 0f, 0f, num2, num, 0f, 0f, 0f, 0f, 1f);
		for (int i = 0; i < count; i++)
		{
			Particle* ptr = particleArray + i;
			Vector4.Transform(ref ptr->Colour, ref YIQTransformMatrix, out var result);
			Vector4.Transform(ref result, ref matrix, out result);
			Vector4.Transform(ref result, ref RGBTransformMatrix, out result);
			ptr->Colour.X = result.X;
			ptr->Colour.Y = result.Y;
			ptr->Colour.Z = result.Z;
		}
	}
}
