using System;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class SAT : INarrowPhaseCollider
{
	private static SAT _instance;

	public static SAT Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SAT();
			}
			return _instance;
		}
	}

	private SAT()
	{
	}

	public void Collide(Geom geomA, Geom geomB, ContactList contactList)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		PolygonCollisionResult polygonCollisionResult = PolygonCollision(geomA.WorldVertices, geomB.WorldVertices);
		float num = ((Vector2)(ref polygonCollisionResult.MinimumTranslationVector)).Length();
		int num2 = 0;
		Vector2 normal = Vector2.Normalize(-polygonCollisionResult.MinimumTranslationVector);
		int num3 = 0;
		if (!polygonCollisionResult.Intersect || !(num > 0.001f))
		{
			return;
		}
		for (int i = 0; i < geomA.WorldVertices.Count; i++)
		{
			if (num2 > PhysicsSimulator.MaxContactsToDetect)
			{
				break;
			}
			if (InsidePolygon(geomB.WorldVertices, geomA.WorldVertices[i]))
			{
				Contact item = new Contact(geomA.WorldVertices[i], normal, 0f - num, new ContactId(geomA.id, i, geomB.id));
				contactList.Add(item);
				num2++;
				num3++;
			}
		}
		num2 = 0;
		for (int j = 0; j < geomB.WorldVertices.Count; j++)
		{
			if (num2 > PhysicsSimulator.MaxContactsToDetect)
			{
				break;
			}
			if (InsidePolygon(geomA.WorldVertices, geomB.WorldVertices[j]))
			{
				Contact item2 = new Contact(geomB.WorldVertices[j], normal, 0f - num, new ContactId(geomB.id, j, geomA.id));
				contactList.Add(item2);
				num2++;
				num3++;
			}
		}
		if (num3 != 0)
		{
			return;
		}
		int num4 = polygonCollisionResult.bestEdgeIndex;
		Geom geom = geomA;
		Geom geom2 = geomB;
		if (polygonCollisionResult.bestEdgeIndex >= geomA.WorldVertices.Count)
		{
			num4 -= geomA.WorldVertices.Count;
			geom = geomB;
			geom2 = geomA;
		}
		Vector2 edge = geom.WorldVertices.GetEdge(num4);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(0f - edge.Y, edge.X);
		((Vector2)(ref val)).Normalize();
		int num5 = 0;
		float num6 = Vector2.Dot(val, geom2.WorldVertices[0]);
		for (int k = 1; k < geom2.WorldVertices.Count; k++)
		{
			float num7 = Vector2.Dot(val, geom2.WorldVertices[k]);
			if (num7 < num6)
			{
				num6 = num7;
				num5 = k;
			}
		}
		Contact item3 = new Contact(geom2.WorldVertices[num5], normal, 0f - num, new ContactId(geom2.id, num5, geom.id));
		contactList.Add(item3);
	}

	public bool Intersect(Geom geom, ref Vector2 position)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return InsidePolygon(geom.LocalVertices, position);
	}

	private bool InsidePolygon(Vertices polygon, Vector2 position)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Vector2 val = polygon[0];
		for (int i = 1; i <= polygon.Count; i++)
		{
			Vector2 val2 = polygon[i % polygon.Count];
			if (position.Y > Math.Min(val.Y, val2.Y) && position.Y <= Math.Max(val.Y, val2.Y) && position.X <= Math.Max(val.X, val2.X) && val.Y != val2.Y)
			{
				double num2 = (position.Y - val.Y) * (val2.X - val.X) / (val2.Y - val.Y) + val.X;
				if (val.X == val2.X || (double)position.X <= num2)
				{
					num++;
				}
			}
			val = val2;
		}
		if (num % 2 == 0)
		{
			return false;
		}
		return true;
	}

	private void ProjectPolygon(Vector2 axis, Vertices polygon, out float min, out float max)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		max = (min = Vector2.Dot(axis, polygon[0]));
		for (int i = 0; i < polygon.Count; i++)
		{
			float num = Vector2.Dot(polygon[i], axis);
			if (num < min)
			{
				min = num;
			}
			else if (num > max)
			{
				max = num;
			}
		}
	}

	private float IntervalDistance(float minA, float maxA, float minB, float maxB)
	{
		if (minA < minB)
		{
			return minB - maxA;
		}
		return minA - maxB;
	}

	private PolygonCollisionResult PolygonCollision(Vertices polygonA, Vertices polygonB)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		PolygonCollisionResult result = new PolygonCollisionResult
		{
			Intersect = true
		};
		int count = polygonA.Count;
		int count2 = polygonB.Count;
		float num = float.PositiveInfinity;
		Vector2 val = default(Vector2);
		Vector2 val3 = default(Vector2);
		for (int i = 0; i < count + count2; i++)
		{
			Vector2 val2 = ((i >= count) ? polygonB.GetEdge(i - count) : polygonA.GetEdge(i));
			((Vector2)(ref val3))._002Ector(0f - val2.Y, val2.X);
			((Vector2)(ref val3)).Normalize();
			ProjectPolygon(val3, polygonA, out var min, out var max);
			ProjectPolygon(val3, polygonB, out var min2, out var max2);
			float num2 = IntervalDistance(min, max, min2, max2);
			if (num2 > 0f)
			{
				result.Intersect = false;
			}
			if (!result.Intersect)
			{
				break;
			}
			num2 = Math.Abs(num2);
			if (num2 < num)
			{
				num = num2;
				val = val3;
				Vector2 val4 = polygonA.GetCentroid() - polygonB.GetCentroid();
				if (Vector2.Dot(val4, val) < 0f)
				{
					val = -val;
				}
				result.bestEdgeIndex = i;
			}
		}
		result.MinimumTranslationVector = val * num;
		return result;
	}
}
