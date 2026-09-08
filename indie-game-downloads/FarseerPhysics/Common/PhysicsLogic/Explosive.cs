using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PhysicsLogic;

public sealed class Explosive
{
	private const int MaxShapes = 100;

	private Dictionary<Fixture, List<Vector2>> _exploded;

	private RayDataComparer _rdc;

	private List<ShapeData> _data = new List<ShapeData>();

	private World _world;

	public Explosive(World world)
	{
		_exploded = new Dictionary<Fixture, List<Vector2>>();
		_rdc = new RayDataComparer();
		_data = new List<ShapeData>();
		_world = world;
	}

	public Dictionary<Fixture, List<Vector2>> Explode(Vector2 pos, float radius, float maxForce)
	{
		_exploded.Clear();
		AABB aabb = default(AABB);
		aabb.LowerBound = pos + new Vector2(0f - radius, 0f - radius);
		aabb.UpperBound = pos + new Vector2(radius, radius);
		Fixture[] shapes = new Fixture[100];
		int shapeCount = 0;
		_world.QueryAABB(delegate(Fixture fixture2)
		{
			shapes[shapeCount++] = fixture2;
			return true;
		}, ref aabb);
		bool flag = false;
		for (int num = 0; num < shapeCount; num++)
		{
			if (shapes[num].TestPoint(ref pos))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			RayData[] array = new RayData[shapeCount * 2];
			int num2 = 0;
			for (int num3 = 0; num3 < shapeCount; num3++)
			{
				PolygonShape polygonShape;
				if (shapes[num3].Shape is CircleShape circleShape)
				{
					Vertices vertices = new Vertices();
					Vector2 item = Vector2.Zero + new Vector2(circleShape.Radius, 0f);
					vertices.Add(item);
					item = Vector2.Zero + new Vector2(0f, circleShape.Radius);
					vertices.Add(item);
					item = Vector2.Zero + new Vector2(0f - circleShape.Radius, circleShape.Radius);
					vertices.Add(item);
					item = Vector2.Zero + new Vector2(0f, 0f - circleShape.Radius);
					vertices.Add(item);
					polygonShape = new PolygonShape(vertices);
				}
				else
				{
					polygonShape = shapes[num3].Shape as PolygonShape;
				}
				if (shapes[num3].Body.BodyType != BodyType.Dynamic || polygonShape == null)
				{
					continue;
				}
				Vector2 vector = shapes[num3].Body.GetWorldPoint(polygonShape.Centroid) - pos;
				float num4 = (float)Math.Atan2(vector.Y, vector.X);
				float num5 = float.MaxValue;
				float num6 = float.MinValue;
				float angle = 0f;
				float angle2 = 0f;
				for (int num7 = 0; num7 < polygonShape.Vertices.Count(); num7++)
				{
					Vector2 vector2 = shapes[num3].Body.GetWorldPoint(polygonShape.Vertices[num7]) - pos;
					float num8 = (float)Math.Atan2(vector2.Y, vector2.X);
					float num9 = num8 - num4;
					num9 = (num9 - (float)Math.PI) % ((float)Math.PI * 2f);
					if (num9 < 0f)
					{
						num9 += (float)Math.PI * 2f;
					}
					num9 -= (float)Math.PI;
					if (Math.Abs(num9) > (float)Math.PI)
					{
						throw new ArgumentException("OMG!");
					}
					if (num9 > num6)
					{
						num6 = num9;
						angle2 = num8;
					}
					if (num9 < num5)
					{
						num5 = num9;
						angle = num8;
					}
				}
				array[num2].Angle = angle;
				num2++;
				array[num2].Angle = angle2;
				num2++;
			}
			Array.Sort(array, 0, num2, _rdc);
			_data.Clear();
			bool flag2 = true;
			ShapeData item2 = default(ShapeData);
			for (int num10 = 0; num10 < num2; num10++)
			{
				Fixture shape = null;
				int num11 = ((num10 != num2 - 1) ? (num10 + 1) : 0);
				if (array[num10].Angle == array[num11].Angle)
				{
					continue;
				}
				float num12 = ((num10 != num2 - 1) ? (array[num10 + 1].Angle + array[num10].Angle) : (array[0].Angle + (float)Math.PI * 2f + array[num10].Angle));
				num12 /= 2f;
				Vector2 point = pos;
				Vector2 point2 = radius * new Vector2((float)Math.Cos(num12), (float)Math.Sin(num12)) + pos;
				bool hitClosest = false;
				_world.RayCast(delegate(Fixture f, Vector2 p, Vector2 n, float fr)
				{
					Body body = f.Body;
					if (body.UserData != null && (int)body.UserData == 0)
					{
						return -1f;
					}
					hitClosest = true;
					shape = f;
					return fr;
				}, point, point2);
				if (hitClosest && shape.Body.BodyType == BodyType.Dynamic)
				{
					if (_data.Count() > 0 && _data.Last().Body == shape.Body && !flag2)
					{
						int index = _data.Count - 1;
						ShapeData value = _data[index];
						value.Max = array[num11].Angle;
						_data[index] = value;
					}
					else
					{
						item2.Body = shape.Body;
						item2.Min = array[num10].Angle;
						item2.Max = array[num11].Angle;
						_data.Add(item2);
					}
					if (_data.Count() > 1 && num10 == num2 - 1 && _data.Last().Body == _data.First().Body && _data.Last().Max == _data.First().Min)
					{
						ShapeData value2 = _data[0];
						value2.Min = _data.Last().Min;
						_data.RemoveAt(_data.Count() - 1);
						_data[0] = value2;
						while (_data.First().Min >= _data.First().Max)
						{
							value2.Min -= (float)Math.PI * 2f;
							_data[0] = value2;
						}
					}
					int index2 = _data.Count - 1;
					ShapeData value3 = _data[index2];
					while (_data.Count() > 0 && _data.Last().Min >= _data.Last().Max)
					{
						value3.Min = _data.Last().Min - (float)Math.PI * 2f;
						_data[index2] = value3;
					}
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
			}
			RayCastInput input = default(RayCastInput);
			for (int num13 = 0; num13 < _data.Count(); num13++)
			{
				float num14 = _data[num13].Max - _data[num13].Min;
				if (_data[num13].Body == null)
				{
					for (float num15 = _data[num13].Min; num15 <= _data[num13].Max; num15 += (float)Math.PI / 15f)
					{
					}
					continue;
				}
				float num16 = MathHelper.Min((float)Math.PI / 90f, 0.025f * num14);
				int num17 = (int)Math.Ceiling((num14 - 2f * num16 - (float)Math.PI * 4f / 15f) / ((float)Math.PI / 15f));
				if (num17 < 0)
				{
					num17 = 0;
				}
				float num18 = (num14 - num16 * 2f) / (5f + (float)num17 - 1f);
				for (float num19 = _data[num13].Min + num16; num19 <= _data[num13].Max; num19 += num18)
				{
					Vector2 vector3 = pos;
					Vector2 vector4 = pos + radius * new Vector2((float)Math.Cos(num19), (float)Math.Sin(num19));
					Vector2 point3 = Vector2.Zero;
					float num20 = float.MaxValue;
					List<Fixture> fixtureList = _data[num13].Body.FixtureList;
					for (int num21 = 0; num21 < fixtureList.Count; num21++)
					{
						Fixture fixture = fixtureList[num21];
						input.Point1 = vector3;
						input.Point2 = vector4;
						input.MaxFraction = 50f;
						if (fixture.RayCast(out var output, ref input, 0) && num20 > output.Fraction)
						{
							num20 = output.Fraction;
							point3 = output.Fraction * vector4 + (1f - output.Fraction) * vector3;
						}
						float num22 = num14 / (float)(5 + num17) * maxForce * 180f / (float)Math.PI * (1f - Math.Min(1f, num20));
						Vector2 impulse = Vector2.Dot(num22 * new Vector2((float)Math.Cos(num19), (float)Math.Sin(num19)), -output.Normal) * new Vector2((float)Math.Cos(num19), (float)Math.Sin(num19));
						_data[num13].Body.ApplyLinearImpulse(ref impulse, ref point3);
						Vector2 zero = Vector2.Zero;
						if (_exploded.TryGetValue(fixture, out var value4))
						{
							zero.X += Math.Abs(impulse.X);
							zero.Y += Math.Abs(impulse.Y);
							value4.Add(zero);
						}
						else
						{
							value4 = new List<Vector2>();
							zero.X = Math.Abs(impulse.X);
							zero.Y = Math.Abs(impulse.Y);
							value4.Add(zero);
							_exploded.Add(fixture, value4);
						}
						if (num20 > 1f)
						{
							point3 = vector4;
						}
					}
				}
			}
		}
		return _exploded;
	}
}
