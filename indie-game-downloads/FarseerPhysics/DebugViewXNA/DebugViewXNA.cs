using System;
using System.Collections.Generic;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerPhysics.DebugViewXNA;

public class DebugViewXNA : DebugView, IDisposable
{
	private struct ContactPoint
	{
		public Vector2 Normal;

		public Vector2 Position;

		public PointState State;
	}

	private struct StringData
	{
		public object[] Args;

		public Color Color;

		public string S;

		public int X;

		public int Y;

		public StringData(int x, int y, string s, object[] args)
		{
			X = x;
			Y = y;
			S = s;
			Args = args;
			Color = new Color(0.9f, 0.6f, 0.6f);
		}

		public StringData(int x, int y, string s, object[] args, Color color)
		{
			X = x;
			Y = y;
			S = s;
			Args = args;
			Color = color;
		}
	}

	private const int MaxContactPoints = 2048;

	private static VertexPositionColor[] _vertsLines;

	private static VertexPositionColor[] _vertsFill;

	private static int _lineCount;

	private static int _fillCount;

	private static SpriteBatch _batch;

	private static SpriteFont _font;

	private static GraphicsDevice _device;

	private static Vector2[] _tempVertices;

	private static List<StringData> _stringData;

	private static BasicEffect _effect;

	public Color DefaultShapeColor = new Color(0.9f, 0.7f, 0.7f);

	public Color InactiveShapeColor = new Color(0.5f, 0.5f, 0.3f);

	public Color KinematicShapeColor = new Color(0.5f, 0.5f, 0.9f);

	public Color SleepingShapeColor = new Color(0.6f, 0.6f, 0.6f);

	public Color StaticShapeColor = new Color(0.5f, 0.9f, 0.5f);

	public Color TextColor = Color.White;

	private int _pointCount;

	private ContactPoint[] _points = new ContactPoint[2048];

	public DebugViewXNA(World world)
		: base(world)
	{
		_vertsLines = new VertexPositionColor[1000000];
		_vertsFill = new VertexPositionColor[1000000];
		_tempVertices = new Vector2[Settings.MaxPolygonVertices];
		ContactManager contactManager = world.ContactManager;
		contactManager.PreSolve = (PreSolveDelegate)Delegate.Combine(contactManager.PreSolve, new PreSolveDelegate(PreSolve));
		AppendFlags(DebugViewFlags.Shape);
		AppendFlags(DebugViewFlags.Joint);
	}

	public void Dispose()
	{
		ContactManager contactManager = base.World.ContactManager;
		contactManager.PreSolve = (PreSolveDelegate)Delegate.Remove(contactManager.PreSolve, new PreSolveDelegate(PreSolve));
	}

	private void PreSolve(Contact contact, ref Manifold oldManifold)
	{
		if ((base.Flags & DebugViewFlags.ContactPoints) != DebugViewFlags.ContactPoints)
		{
			return;
		}
		Manifold manifold = contact.Manifold;
		if (manifold.PointCount == 0)
		{
			return;
		}
		Fixture fixtureA = contact.FixtureA;
		FarseerPhysics.Collision.Collision.GetPointStates(out var _, out var state2, ref oldManifold, ref manifold);
		contact.GetWorldManifold(out var worldManifold);
		for (int i = 0; i < manifold.PointCount; i++)
		{
			if (_pointCount >= 2048)
			{
				break;
			}
			if (fixtureA == null)
			{
				_points[i] = default(ContactPoint);
			}
			ContactPoint contactPoint = _points[_pointCount];
			contactPoint.Position = worldManifold.Points[i];
			contactPoint.Normal = worldManifold.Normal;
			contactPoint.State = state2[i];
			_points[_pointCount] = contactPoint;
			_pointCount++;
		}
	}

	private void DrawDebugData()
	{
		if ((base.Flags & DebugViewFlags.ContactPoints) == DebugViewFlags.ContactPoints)
		{
			for (int i = 0; i < _pointCount; i++)
			{
				ContactPoint contactPoint = _points[i];
				if (contactPoint.State == PointState.Add)
				{
					DrawPoint(contactPoint.Position, 0.1f, new Color(0.3f, 0.95f, 0.3f));
				}
				else if (contactPoint.State == PointState.Persist)
				{
					DrawPoint(contactPoint.Position, 0.1f, new Color(0.3f, 0.3f, 0.95f));
				}
				if ((base.Flags & DebugViewFlags.ContactNormals) == DebugViewFlags.ContactNormals)
				{
					Vector2 position = contactPoint.Position;
					Vector2 end = position + 0.3f * contactPoint.Normal;
					DrawSegment(position, end, new Color(0.4f, 0.9f, 0.4f));
				}
			}
			_pointCount = 0;
		}
		if ((base.Flags & DebugViewFlags.PolygonPoints) == DebugViewFlags.PolygonPoints)
		{
			foreach (Body body in base.World.BodyList)
			{
				foreach (Fixture fixture in body.FixtureList)
				{
					if (fixture.Shape is PolygonShape polygonShape)
					{
						body.GetTransform(out var transform);
						for (int j = 0; j < polygonShape.Vertices.Count; j++)
						{
							Vector2 p = MathUtils.Multiply(ref transform, polygonShape.Vertices[j]);
							DrawPoint(p, 0.1f, Color.Red);
						}
					}
				}
			}
		}
		if ((base.Flags & DebugViewFlags.DebugPanel) == DebugViewFlags.DebugPanel)
		{
			DrawDebugPanel();
		}
		if ((base.Flags & DebugViewFlags.Shape) == DebugViewFlags.Shape)
		{
			foreach (Body body2 in base.World.BodyList)
			{
				body2.GetTransform(out var transform2);
				foreach (Fixture fixture2 in body2.FixtureList)
				{
					if (!body2.Active)
					{
						DrawShape(fixture2, transform2, InactiveShapeColor);
					}
					else if (body2.BodyType == BodyType.Static)
					{
						DrawShape(fixture2, transform2, StaticShapeColor);
					}
					else if (body2.BodyType == BodyType.Kinematic)
					{
						DrawShape(fixture2, transform2, KinematicShapeColor);
					}
					else if (!body2.Awake)
					{
						DrawShape(fixture2, transform2, SleepingShapeColor);
					}
					else
					{
						DrawShape(fixture2, transform2, DefaultShapeColor);
					}
				}
			}
		}
		if ((base.Flags & DebugViewFlags.Joint) == DebugViewFlags.Joint)
		{
			foreach (Joint joint in base.World.JointList)
			{
				DrawJoint(joint);
			}
		}
		if ((base.Flags & DebugViewFlags.Pair) == DebugViewFlags.Pair)
		{
			Color color = new Color(0.3f, 0.9f, 0.9f);
			for (Contact contact = base.World.ContactManager.ContactList; contact != null; contact = contact.Next)
			{
				Fixture fixtureA = contact.FixtureA;
				Fixture fixtureB = contact.FixtureB;
				fixtureA.GetAABB(out var aabb, 0);
				fixtureB.GetAABB(out var aabb2, 0);
				Vector2 center = aabb.Center;
				Vector2 center2 = aabb2.Center;
				DrawSegment(center, center2, color);
			}
		}
		if ((base.Flags & DebugViewFlags.AABB) == DebugViewFlags.AABB)
		{
			Color color2 = new Color(0.9f, 0.3f, 0.9f);
			BroadPhase broadPhase = base.World.ContactManager.BroadPhase;
			foreach (Body body3 in base.World.BodyList)
			{
				if (!body3.Active)
				{
					continue;
				}
				foreach (Fixture fixture3 in body3.FixtureList)
				{
					for (int k = 0; k < fixture3.ProxyCount; k++)
					{
						FixtureProxy fixtureProxy = fixture3.Proxies[k];
						broadPhase.GetFatAABB(fixtureProxy.ProxyId, out var aabb3);
						DrawPolygon(new Vector2[4]
						{
							new Vector2(aabb3.LowerBound.X, aabb3.LowerBound.Y),
							new Vector2(aabb3.UpperBound.X, aabb3.LowerBound.Y),
							new Vector2(aabb3.UpperBound.X, aabb3.UpperBound.Y),
							new Vector2(aabb3.LowerBound.X, aabb3.UpperBound.Y)
						}, 4, color2);
					}
				}
			}
		}
		if ((base.Flags & DebugViewFlags.CenterOfMass) != DebugViewFlags.CenterOfMass)
		{
			return;
		}
		foreach (Body body4 in base.World.BodyList)
		{
			body4.GetTransform(out var transform3);
			transform3.Position = body4.WorldCenter;
			DrawTransform(ref transform3);
		}
	}

	private void DrawDebugPanel()
	{
		int num = 0;
		for (int i = 0; i < base.World.BodyList.Count; i++)
		{
			num += base.World.BodyList[i].FixtureList.Count;
		}
		DrawString(50, 100, "Bodies: " + base.World.BodyList.Count);
		DrawString(50, 115, "Fixtures: " + num);
		DrawString(50, 130, "Contacts: " + base.World.ContactCount);
		DrawString(50, 145, "Joints: " + base.World.JointList.Count);
		DrawString(50, 160, "Proxies: " + base.World.ProxyCount);
		DrawString(50, 175, "Breakable: " + base.World.BreakableBodyList.Count);
		DrawString(50, 190, "Controllers: " + base.World.Controllers.Count);
		DrawString(160, 100, "New contacts: " + base.World.NewContactsTime);
		DrawString(160, 115, "Controllers: " + base.World.ControllersUpdateTime);
		DrawString(160, 130, "Breakable: " + base.World.BreakableBodyTime);
		DrawString(160, 145, "Contacts: " + base.World.ContactsUpdateTime);
		DrawString(160, 160, "Solve: " + base.World.SolveUpdateTime);
		DrawString(160, 175, "CCD: " + base.World.ContinuousPhysicsTime);
		DrawString(160, 190, "Total: " + base.World.UpdateTime);
	}

	private void DrawJoint(Joint joint)
	{
		Body bodyA = joint.BodyA;
		Body bodyB = joint.BodyB;
		bodyA.GetTransform(out var transform);
		Vector2 vector = default(Vector2);
		if (!joint.IsFixedType())
		{
			bodyB.GetTransform(out var transform2);
			vector = transform2.Position;
		}
		Vector2 worldAnchorB = joint.WorldAnchorB;
		Vector2 position = transform.Position;
		Vector2 worldAnchorA = joint.WorldAnchorA;
		Color color = new Color(0.5f, 0.8f, 0.8f);
		switch (joint.JointType)
		{
		case JointType.Distance:
			DrawSegment(worldAnchorA, worldAnchorB, color);
			break;
		case JointType.Pulley:
		{
			PulleyJoint pulleyJoint = (PulleyJoint)joint;
			Vector2 groundAnchorA = pulleyJoint.GroundAnchorA;
			Vector2 groundAnchorB = pulleyJoint.GroundAnchorB;
			DrawSegment(groundAnchorA, worldAnchorA, color);
			DrawSegment(groundAnchorB, worldAnchorB, color);
			DrawSegment(groundAnchorA, groundAnchorB, color);
			break;
		}
		case JointType.FixedMouse:
		{
			FixedMouseJoint fixedMouseJoint = (FixedMouseJoint)joint;
			worldAnchorA = fixedMouseJoint.Target;
			DrawPoint(worldAnchorB, 0.5f, new Color(0f, 1f, 0f));
			DrawSegment(worldAnchorA, worldAnchorB, new Color(0.8f, 0.8f, 0.8f));
			break;
		}
		case JointType.Revolute:
			DrawSegment(worldAnchorB, worldAnchorA, color);
			DrawSolidCircle(worldAnchorB, 0.1f, Vector2.Zero, Color.Red);
			DrawSolidCircle(worldAnchorA, 0.1f, Vector2.Zero, Color.Blue);
			break;
		case JointType.FixedRevolute:
			DrawSegment(position, worldAnchorA, color);
			DrawSolidCircle(worldAnchorA, 0.1f, Vector2.Zero, Color.Pink);
			break;
		case JointType.FixedLine:
			DrawSegment(position, worldAnchorA, color);
			DrawSegment(worldAnchorA, worldAnchorB, color);
			break;
		case JointType.FixedDistance:
			DrawSegment(position, worldAnchorA, color);
			DrawSegment(worldAnchorA, worldAnchorB, color);
			break;
		case JointType.FixedPrismatic:
			DrawSegment(position, worldAnchorA, color);
			DrawSegment(worldAnchorA, worldAnchorB, color);
			break;
		case JointType.Gear:
			DrawSegment(position, vector, color);
			break;
		default:
			DrawSegment(position, worldAnchorA, color);
			DrawSegment(worldAnchorA, worldAnchorB, color);
			DrawSegment(vector, worldAnchorB, color);
			break;
		}
	}

	private void DrawShape(Fixture fixture, Transform xf, Color color)
	{
		switch (fixture.ShapeType)
		{
		case ShapeType.Circle:
		{
			CircleShape circleShape = (CircleShape)fixture.Shape;
			Vector2 center = MathUtils.Multiply(ref xf, circleShape.Position);
			float radius = circleShape.Radius;
			Vector2 col = xf.R.Col1;
			DrawSolidCircle(center, radius, col, color);
			break;
		}
		case ShapeType.Polygon:
		{
			PolygonShape polygonShape = (PolygonShape)fixture.Shape;
			int count2 = polygonShape.Vertices.Count;
			for (int j = 0; j < count2; j++)
			{
				ref Vector2 reference = ref _tempVertices[j];
				reference = MathUtils.Multiply(ref xf, polygonShape.Vertices[j]);
			}
			DrawSolidPolygon(_tempVertices, count2, color);
			break;
		}
		case ShapeType.Edge:
		{
			EdgeShape edgeShape = (EdgeShape)fixture.Shape;
			Vector2 start2 = MathUtils.Multiply(ref xf, edgeShape.Vertex1);
			Vector2 end = MathUtils.Multiply(ref xf, edgeShape.Vertex2);
			DrawSegment(start2, end, color);
			break;
		}
		case ShapeType.Loop:
		{
			LoopShape loopShape = (LoopShape)fixture.Shape;
			int count = loopShape.Vertices.Count;
			Vector2 start = MathUtils.Multiply(ref xf, loopShape.Vertices[count - 1]);
			for (int i = 0; i < count; i++)
			{
				Vector2 vector = MathUtils.Multiply(ref xf, loopShape.Vertices[i]);
				DrawSegment(start, vector, color);
				start = vector;
			}
			break;
		}
		}
	}

	public override void DrawPolygon(Vector2[] vertices, int count, float red, float green, float blue)
	{
		DrawPolygon(vertices, count, new Color(red, green, blue));
	}

	public void DrawPolygon(Vector2[] vertices, int count, Color color)
	{
		for (int i = 0; i < count - 1; i++)
		{
			_vertsLines[_lineCount * 2].Position = new Vector3(vertices[i], 0f);
			_vertsLines[_lineCount * 2].Color = color;
			_vertsLines[_lineCount * 2 + 1].Position = new Vector3(vertices[i + 1], 0f);
			_vertsLines[_lineCount * 2 + 1].Color = color;
			_lineCount++;
		}
		_vertsLines[_lineCount * 2].Position = new Vector3(vertices[count - 1], 0f);
		_vertsLines[_lineCount * 2].Color = color;
		_vertsLines[_lineCount * 2 + 1].Position = new Vector3(vertices[0], 0f);
		_vertsLines[_lineCount * 2 + 1].Color = color;
		_lineCount++;
	}

	public override void DrawSolidPolygon(Vector2[] vertices, int count, float red, float green, float blue)
	{
		DrawSolidPolygon(vertices, count, new Color(red, green, blue), outline: true);
	}

	public void DrawSolidPolygon(Vector2[] vertices, int count, Color color)
	{
		DrawSolidPolygon(vertices, count, color, outline: true);
	}

	public void DrawSolidPolygon(Vector2[] vertices, int count, Color color, bool outline)
	{
		if (count == 2)
		{
			DrawPolygon(vertices, count, color);
			return;
		}
		Color color2 = color * (outline ? 0.5f : 1f);
		for (int i = 1; i < count - 1; i++)
		{
			_vertsFill[_fillCount * 3].Position = new Vector3(vertices[0], 0f);
			_vertsFill[_fillCount * 3].Color = color2;
			_vertsFill[_fillCount * 3 + 1].Position = new Vector3(vertices[i], 0f);
			_vertsFill[_fillCount * 3 + 1].Color = color2;
			_vertsFill[_fillCount * 3 + 2].Position = new Vector3(vertices[i + 1], 0f);
			_vertsFill[_fillCount * 3 + 2].Color = color2;
			_fillCount++;
		}
		if (outline)
		{
			DrawPolygon(vertices, count, color);
		}
	}

	public override void DrawCircle(Vector2 center, float radius, float red, float green, float blue)
	{
		DrawCircle(center, radius, new Color(red, green, blue));
	}

	public void DrawCircle(Vector2 center, float radius, Color color)
	{
		double num = 0.0;
		for (int i = 0; i < 32; i++)
		{
			Vector2 value = center + radius * new Vector2((float)Math.Cos(num), (float)Math.Sin(num));
			Vector2 value2 = center + radius * new Vector2((float)Math.Cos(num + Math.PI / 16.0), (float)Math.Sin(num + Math.PI / 16.0));
			if (_lineCount * 2 < _vertsLines.Length)
			{
				_vertsLines[_lineCount * 2].Position = new Vector3(value, 0f);
				_vertsLines[_lineCount * 2].Color = color;
				_vertsLines[_lineCount * 2 + 1].Position = new Vector3(value2, 0f);
				_vertsLines[_lineCount * 2 + 1].Color = color;
				_lineCount++;
			}
			num += Math.PI / 16.0;
		}
	}

	public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, float red, float green, float blue)
	{
		DrawSolidCircle(center, radius, axis, new Color(red, green, blue));
	}

	public void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
	{
		double num = 0.0;
		Color color2 = color * 0.5f;
		Vector2 value = center + radius * new Vector2((float)Math.Cos(num), (float)Math.Sin(num));
		num += Math.PI / 16.0;
		for (int i = 1; i < 31; i++)
		{
			Vector2 value2 = center + radius * new Vector2((float)Math.Cos(num), (float)Math.Sin(num));
			Vector2 value3 = center + radius * new Vector2((float)Math.Cos(num + Math.PI / 16.0), (float)Math.Sin(num + Math.PI / 16.0));
			_vertsFill[_fillCount * 3].Position = new Vector3(value, 0f);
			_vertsFill[_fillCount * 3].Color = color2;
			_vertsFill[_fillCount * 3 + 1].Position = new Vector3(value2, 0f);
			_vertsFill[_fillCount * 3 + 1].Color = color2;
			_vertsFill[_fillCount * 3 + 2].Position = new Vector3(value3, 0f);
			_vertsFill[_fillCount * 3 + 2].Color = color2;
			_fillCount++;
			num += Math.PI / 16.0;
		}
		DrawCircle(center, radius, color);
		DrawSegment(center, center + axis * radius, color);
	}

	public override void DrawSegment(Vector2 start, Vector2 end, float red, float green, float blue)
	{
		DrawSegment(start, end, new Color(red, green, blue));
	}

	public void DrawSegment(Vector2 start, Vector2 end, Color color)
	{
		_vertsLines[_lineCount * 2].Position = new Vector3(start, 0f);
		_vertsLines[_lineCount * 2 + 1].Position = new Vector3(end, 0f);
		_vertsLines[_lineCount * 2].Color = (_vertsLines[_lineCount * 2 + 1].Color = color);
		_lineCount++;
	}

	public override void DrawTransform(ref Transform transform)
	{
		Vector2 position = transform.Position;
		Vector2 end = position + 0.4f * transform.R.Col1;
		DrawSegment(position, end, Color.Red);
		end = position + 0.4f * transform.R.Col2;
		DrawSegment(position, end, Color.Green);
	}

	public void DrawPoint(Vector2 p, float size, Color color)
	{
		Vector2[] array = new Vector2[4];
		float num = size / 2f;
		ref Vector2 reference = ref array[0];
		reference = p + new Vector2(0f - num, 0f - num);
		ref Vector2 reference2 = ref array[1];
		reference2 = p + new Vector2(num, 0f - num);
		ref Vector2 reference3 = ref array[2];
		reference3 = p + new Vector2(num, num);
		ref Vector2 reference4 = ref array[3];
		reference4 = p + new Vector2(0f - num, num);
		DrawSolidPolygon(array, 4, color, outline: true);
	}

	public void DrawString(int x, int y, string s, params object[] args)
	{
		_stringData.Add(new StringData(x, y, s, args, TextColor));
	}

	public void RenderDebugData(ref Matrix projection)
	{
		DrawDebugData();
		_device.RasterizerState = RasterizerState.CullNone;
		_effect.Projection = projection;
		_effect.Techniques[0].Passes[0].Apply();
		if (_fillCount > 0)
		{
			_device.DrawUserPrimitives(PrimitiveType.TriangleList, _vertsFill, 0, _fillCount);
		}
		if (_lineCount > 0)
		{
			_device.DrawUserPrimitives(PrimitiveType.LineList, _vertsLines, 0, _lineCount);
		}
		_batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Matrix.Identity);
		for (int i = 0; i < _stringData.Count; i++)
		{
			_batch.DrawString(_font, string.Format(_stringData[i].S, _stringData[i].Args), new Vector2(_stringData[i].X, _stringData[i].Y), _stringData[i].Color);
		}
		_batch.End();
		_stringData.Clear();
		_lineCount = (_fillCount = 0);
	}

	public void RenderDebugData(ref Matrix projection, ref Matrix view)
	{
		_effect.View = view;
		RenderDebugData(ref projection);
	}

	public void DrawAABB(ref AABB aabb, Color color)
	{
		DrawPolygon(new Vector2[4]
		{
			new Vector2(aabb.LowerBound.X, aabb.LowerBound.Y),
			new Vector2(aabb.UpperBound.X, aabb.LowerBound.Y),
			new Vector2(aabb.UpperBound.X, aabb.UpperBound.Y),
			new Vector2(aabb.LowerBound.X, aabb.UpperBound.Y)
		}, 4, color);
	}

	public static void LoadContent(GraphicsDevice device, ContentManager content)
	{
		_batch = new SpriteBatch(device);
		_font = content.Load<SpriteFont>("Font/Genericfont");
		_device = device;
		_effect = new BasicEffect(device);
		_effect.VertexColorEnabled = true;
		_stringData = new List<StringData>();
	}
}
