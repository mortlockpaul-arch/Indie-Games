using System;
using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.QBits;

public class QBitCorner
{
	public float FRICTION_ELASTIC = 0.9f;

	public float GLOOPINESS = 0.1f;

	private Vector3 _worldOrigen;

	public QBit qbit;

	public Vector3 unit;

	public Vector3 origen;

	public Vector3 position;

	public bool simulating;

	private Vector3 velocity;

	public Matrix matrix => Matrix.Multiply(Matrix.CreateTranslation(origen - position), qbit.matrix);

	public QBitCorner(QBit oQBit, Vector3 vUnit)
	{
		vUnit.X = Math.Sign(vUnit.X);
		vUnit.Y = Math.Sign(vUnit.Y);
		vUnit.Z = Math.Sign(vUnit.Z);
		qbit = oQBit;
		unit = vUnit;
		origen = vUnit * (Grid.SPACING / 2f);
		position = vUnit * (Grid.SPACING / 2f);
	}

	public void Update(GameTime oGameTime)
	{
		if (simulating)
		{
			Simulation_Update(oGameTime);
		}
	}

	public bool IsTop()
	{
		bool result = false;
		_worldOrigen = Vector3.Transform(origen, qbit.matrix);
		if (_worldOrigen.Y > qbit.Y)
		{
			result = true;
		}
		return result;
	}

	public void Lean(Vector3 vAmount)
	{
		Simulation_Stop();
		vAmount = Vector3.Transform(vAmount, Quaternion.Inverse(qbit.rotation)) * -1f;
		position.X = origen.X + vAmount.X;
		position.Y = origen.Y + vAmount.Y;
		position.Z = origen.Z + vAmount.Z;
	}

	public void Release()
	{
		velocity.X += (origen.X - position.X) * 0.01f;
		velocity.Y += (origen.Y - position.Y) * 0.01f;
		velocity.Z += (origen.Z - position.Z) * 0.01f;
		simulating = true;
	}

	public void Simulation_Start(Vector3 vVelocity)
	{
		vVelocity = Vector3.Transform(vVelocity, Quaternion.Inverse(qbit.rotation)) * -1f;
		velocity.X += vVelocity.X;
		velocity.Y += vVelocity.Y;
		velocity.Z += vVelocity.Z;
		simulating = true;
	}

	public void Simulation_Stop()
	{
		simulating = false;
		velocity.X = 0f;
		velocity.Y = 0f;
		velocity.Z = 0f;
		position.X = origen.X;
		position.Y = origen.Y;
		position.Z = origen.Z;
	}

	private void Simulation_Update(GameTime oGameTime)
	{
		float num = oGameTime.ElapsedGameTime.Milliseconds;
		Vector3 vector = origen - position;
		Vector3.Normalize(vector);
		float num2 = MathHelper.Clamp(vector.Length() / (Grid.SPACING.Length() / 2f), 0f, 1f);
		if (num2 > 0f)
		{
			velocity += vector / num * GLOOPINESS;
			velocity *= FRICTION_ELASTIC;
		}
		position += velocity * num;
		if (velocity.Length() < 0.001f && num2 < 0.001f)
		{
			Simulation_Stop();
		}
	}
}
