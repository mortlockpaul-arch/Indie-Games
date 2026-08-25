using Microsoft.Xna.Framework;
using RenegadeEngine.Threading;

namespace RenegadeEngine.Gameplay;

public class TinyUpdateAsync1 : UpdateAsyncTask
{
	private float m1m2 = DataMgr.tinyMass * DataMgr.giantMass;

	private float giantMassSq = DataMgr.giantMass * DataMgr.giantMass;

	private float[] weights = new float[3];

	private float[] gConsts = new float[DataMgr.numGiants];

	private float[] distanceBetweenGiants = new float[DataMgr.numGiants];

	private float[] distanceToGiants = new float[DataMgr.numGiants];

	protected override void DoWork(bool isCancelled)
	{
		if (isCancelled)
		{
			return;
		}
		float[] array = new float[DataMgr.numGiants];
		_ = DataMgr.numGiants;
		float[] array2 = new float[DataMgr.numGiants];
		_ = Vector3.Zero;
		for (int i = 0; i < 500; i++)
		{
			for (int j = 0; j < DataMgr.numGiants; j++)
			{
				Vector3.Distance(ref DataMgr.smallBodies[i].Position, ref DataMgr.giantBodies[j].Position, out array2[j]);
				array[j] = m1m2 / array2[j] * DataMgr.movementRate;
				weights[j] = 12f / array2[j];
				BoundingSphere boundingSphere = new BoundingSphere(DataMgr.giantBodies[j].Position, DataMgr.giantSize);
				BoundingSphere sphere = new BoundingSphere(DataMgr.smallBodies[i].Position, DataMgr.smallSize);
				if (boundingSphere.Intersects(sphere))
				{
					float num = Vector3.Distance(DataMgr.giantBodies[j].Position, DataMgr.smallBodies[i].Position);
					float num2 = DataMgr.giantSize + DataMgr.smallSize - num + 0.0001f;
					Vector3 vector = Vector3.Zero;
					if (DataMgr.giantBodies[j].Velocity.Length() > 0f)
					{
						vector = Vector3.Normalize(DataMgr.giantBodies[j].Velocity) * num2;
					}
					DataMgr.smallBodies[i].Position -= vector;
					Vector3 value = DataMgr.smallBodies[i].Position - DataMgr.giantBodies[j].Position;
					value = Vector3.Normalize(value);
					if (DataMgr.giantBodies[j].Velocity.Length() > 0f)
					{
						DataMgr.smallBodies[i].AddForce(value * DataMgr.giantBodies[j].Velocity.Length());
					}
					else
					{
						DataMgr.smallBodies[i].AddForce(value);
					}
				}
				DataMgr.smallBodies[i].AddForce(Vector3.Normalize(DataMgr.giantBodies[j].Position - DataMgr.smallBodies[i].Position) * array[j]);
			}
			DataMgr.smallColors[i].Diffuse = new Vector3(MathHelper.Lerp(0.2f, 1f, weights[0]), MathHelper.Lerp(0.2f, 1f, weights[1]), MathHelper.Lerp(0.2f, 1f, weights[2]));
			BoundingSphere sphere2 = new BoundingSphere(DataMgr.smallBodies[i].Position, DataMgr.smallSize);
			if (DataMgr.frust.Left.Intersects(sphere2) == PlaneIntersectionType.Intersecting)
			{
				if (DataMgr.smallBodies[i].Velocity.X < 0f)
				{
					DataMgr.smallBodies[i].Velocity.X = 0f - DataMgr.smallBodies[i].Velocity.X;
				}
			}
			else if (DataMgr.frust.Right.Intersects(sphere2) == PlaneIntersectionType.Intersecting && DataMgr.smallBodies[i].Velocity.X > 0f)
			{
				DataMgr.smallBodies[i].Velocity.X = 0f - DataMgr.smallBodies[i].Velocity.X;
			}
			if (DataMgr.frust.Bottom.Intersects(sphere2) == PlaneIntersectionType.Intersecting)
			{
				if (DataMgr.smallBodies[i].Velocity.Y < 0f)
				{
					DataMgr.smallBodies[i].Velocity.Y = 0f - DataMgr.smallBodies[i].Velocity.Y;
				}
			}
			else if (DataMgr.frust.Top.Intersects(sphere2) == PlaneIntersectionType.Intersecting && DataMgr.smallBodies[i].Velocity.Y > 0f)
			{
				DataMgr.smallBodies[i].Velocity.Y = 0f - DataMgr.smallBodies[i].Velocity.Y;
			}
			if (DataMgr.smallBodies[i].Position.Z >= 25f)
			{
				if (DataMgr.smallBodies[i].Velocity.Z > 0f)
				{
					DataMgr.smallBodies[i].Velocity.Z = 0f - DataMgr.smallBodies[i].Velocity.Z;
				}
			}
			else if (DataMgr.smallBodies[i].Position.Z <= -20f && DataMgr.smallBodies[i].Velocity.Z < 0f)
			{
				DataMgr.smallBodies[i].Velocity.Z = 0f - DataMgr.smallBodies[i].Velocity.Z;
			}
			DataMgr.smallBodies[i].Integrate((float)gameTimeAsync.ElapsedGameTime.TotalSeconds);
			DataMgr.starTransforms[i].DiffuseColor = new Vector4(DataMgr.smallColors[i].Diffuse, 1f);
			DataMgr.starTransforms[i].World = Matrix.CreateScale(DataMgr.smallSize) * Matrix.CreateTranslation(DataMgr.smallBodies[i].Position);
		}
	}
}
