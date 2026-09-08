using System;
using System.Runtime.CompilerServices;
using _0004;
using _0013;
using D;
using Microsoft.Xna.Framework;
using P;
using d;
using l;
using q;
using r;

namespace _000F
{
	internal class X
	{
		internal static float a5h = 0.01f;

		internal l._7<W> a5b = new l._7<W>();

		internal l._7<_0002> a56 = new l._7<_0002>();

		internal l._7<_0002> a5a = new l._7<_0002>();

		private float a57;

		private h a5_0006;

		internal float a5v = (float)Math.Sin(0.7953981850296259);

		internal float a5B = (float)Math.Cos(0.7953981850296259);

		[CompilerGenerated]
		private bool a5X;

		[CompilerGenerated]
		private bool a5_0018;

		[CompilerGenerated]
		private _000E? a5W;

		public float RayLengthToBottom => a57;

		public y? SupportData
		{
			get
			{
				if (a5b.Count > 0)
				{
					y value = new y
					{
						Position = a5b.Elements[0].Contact.Position,
						Normal = a5b.Elements[0].Contact.Normal
					};
					for (int i = 1; i < a5b.Count; i++)
					{
						Vector3.Add(ref value.Position, ref a5b.Elements[i].Contact.Position, out value.Position);
						Vector3.Add(ref value.Normal, ref a5b.Elements[i].Contact.Normal, out value.Normal);
					}
					if (a5b.Count > 1)
					{
						Vector3.Multiply(ref value.Position, 1f / (float)a5b.Count, out value.Position);
						float num = value.Normal.LengthSquared();
						if (num < 1E-07f)
						{
							value.Normal = a5b.Elements[0].Contact.Normal;
						}
						else
						{
							Vector3.Multiply(ref value.Normal, 1f / (float)Math.Sqrt(num), out value.Normal);
						}
					}
					float num2 = float.MinValue;
					P.h supportObject = null;
					for (int j = 0; j < a5b.Count; j++)
					{
						Vector3.Dot(ref a5b.Elements[j].Contact.Normal, ref value.Normal, out var result);
						result *= a5b.Elements[j].Contact.PenetrationDepth;
						if (result > num2)
						{
							num2 = result;
							supportObject = a5b.Elements[j].Support;
						}
					}
					value.Depth = num2;
					value.SupportObject = supportObject;
					return value;
				}
				if (SupportRayData.HasValue)
				{
					return new y
					{
						Position = SupportRayData.Value.HitData.Location,
						Normal = SupportRayData.Value.HitData.Normal,
						HasTraction = SupportRayData.Value.HasTraction,
						Depth = Vector3.Dot(a5_0006.Body.OrientationMatrix.Down, SupportRayData.Value.HitData.Normal) * (a57 - SupportRayData.Value.HitData.T),
						SupportObject = SupportRayData.Value.HitObject
					};
				}
				return null;
			}
		}

		public y? TractionData
		{
			get
			{
				if (a5b.Count > 0)
				{
					y value = default(y);
					int num = 0;
					for (int i = 0; i < a5b.Count; i++)
					{
						if (a5b.Elements[i].HasTraction)
						{
							num++;
							Vector3.Add(ref value.Position, ref a5b.Elements[i].Contact.Position, out value.Position);
							Vector3.Add(ref value.Normal, ref a5b.Elements[i].Contact.Normal, out value.Normal);
						}
					}
					if (num > 1)
					{
						Vector3.Multiply(ref value.Position, 1f / (float)num, out value.Position);
						float num2 = value.Normal.LengthSquared();
						if (num2 < 1E-05f)
						{
							value.Normal = a5b.Elements[0].Contact.Normal;
						}
						else
						{
							Vector3.Multiply(ref value.Normal, 1f / (float)Math.Sqrt(num2), out value.Normal);
						}
					}
					if (num > 0)
					{
						float num3 = float.MinValue;
						P.h supportObject = null;
						for (int j = 0; j < a5b.Count; j++)
						{
							if (a5b.Elements[j].HasTraction)
							{
								Vector3.Dot(ref a5b.Elements[j].Contact.Normal, ref value.Normal, out var result);
								result *= a5b.Elements[j].Contact.PenetrationDepth;
								if (result > num3)
								{
									num3 = result;
									supportObject = a5b.Elements[j].Support;
								}
							}
						}
						value.Depth = num3;
						value.SupportObject = supportObject;
						value.HasTraction = true;
						return value;
					}
				}
				if (SupportRayData.HasValue && SupportRayData.Value.HasTraction)
				{
					return new y
					{
						Position = SupportRayData.Value.HitData.Location,
						Normal = SupportRayData.Value.HitData.Normal,
						HasTraction = true,
						Depth = Vector3.Dot(a5_0006.Body.OrientationMatrix.Down, SupportRayData.Value.HitData.Normal) * (a57 - SupportRayData.Value.HitData.T),
						SupportObject = SupportRayData.Value.HitObject
					};
				}
				return null;
			}
		}

		public bool HasSupport
		{
			[CompilerGenerated]
			get
			{
				return a5X;
			}
			[CompilerGenerated]
			private set
			{
				a5X = flag;
			}
		}

		public bool HasTraction
		{
			[CompilerGenerated]
			get
			{
				return a5_0018;
			}
			[CompilerGenerated]
			private set
			{
				a5_0018 = flag;
			}
		}

		public _000E? SupportRayData
		{
			[CompilerGenerated]
			get
			{
				return a5W;
			}
			[CompilerGenerated]
			private set
			{
				a5W = obj;
			}
		}

		public l.X<W> Supports => new l.X<W>(a5b);

		public l.X<_0002> SideContacts => new l.X<_0002>(a56);

		public l.X<_0002> HeadContacts => new l.X<_0002>(a5a);

		public _0018 TractionSupports => new _0018(a5b);

		public float MaximumSlope
		{
			get
			{
				return (float)Math.Acos(MathHelper.Clamp(a5B, -1f, 1f));
			}
			set
			{
				a5B = (float)Math.Cos(value);
				a5v = (float)Math.Sin(value);
			}
		}

		public bool GetTractionInDirection(ref Vector3 movementDirection, out y supportData)
		{
			if (HasTraction)
			{
				int num = -1;
				float num2 = float.MinValue;
				for (int i = 0; i < a5b.Count; i++)
				{
					if (a5b.Elements[i].HasTraction)
					{
						Vector3.Dot(ref movementDirection, ref a5b.Elements[i].Contact.Normal, out var result);
						if (result > num2)
						{
							num2 = result;
							num = i;
						}
					}
				}
				if (num != -1)
				{
					supportData.Position = a5b.Elements[num].Contact.Position;
					supportData.Normal = a5b.Elements[num].Contact.Normal;
					supportData.SupportObject = a5b.Elements[num].Support;
					supportData.HasTraction = true;
					float num3 = float.MinValue;
					for (int j = 0; j < a5b.Count; j++)
					{
						if (a5b.Elements[j].HasTraction)
						{
							Vector3.Dot(ref a5b.Elements[j].Contact.Normal, ref supportData.Normal, out var result2);
							result2 *= a5b.Elements[j].Contact.PenetrationDepth;
							if (result2 > num3)
							{
								num3 = result2;
							}
						}
					}
					supportData.Depth = num3;
					return true;
				}
				if (SupportRayData.HasValue && SupportRayData.Value.HasTraction)
				{
					supportData.Position = SupportRayData.Value.HitData.Location;
					supportData.Normal = SupportRayData.Value.HitData.Normal;
					supportData.Depth = Vector3.Dot(a5_0006.Body.OrientationMatrix.Down, SupportRayData.Value.HitData.Normal) * (a57 - SupportRayData.Value.HitData.T);
					supportData.SupportObject = SupportRayData.Value.HitObject;
					supportData.HasTraction = true;
					return true;
				}
				supportData = default(y);
				return false;
			}
			supportData = default(y);
			return false;
		}

		public X(h character)
		{
			a5_0006 = character;
		}

		public void UpdateSupports()
		{
			bool hasTraction = HasTraction;
			HasTraction = false;
			HasSupport = false;
			_0013.h body = a5_0006.Body;
			Vector3 vector = a5_0006.Body.OrientationMatrix.Down;
			a5b.Clear();
			a56.Clear();
			a5a.Clear();
			Vector3 value = a5_0006.Body.Position;
			_0002 item2 = default(_0002);
			_0002 item3 = default(_0002);
			foreach (D.b pair in a5_0006.Body.CollisionInformation.Pairs)
			{
				if (pair.CollisionRule != q.a.Normal)
				{
					continue;
				}
				foreach (D._0001 contact in pair.Contacts)
				{
					if (contact.Pair.CollisionRule != q.a.Normal)
					{
						continue;
					}
					Vector3.Subtract(ref contact.Contact.Position, ref value, out var result);
					Vector3.Dot(ref result, ref contact.Contact.Normal, out var result2);
					Vector3 value2 = contact.Contact.Normal;
					if (result2 < 0f)
					{
						Vector3.Negate(ref value2, out value2);
						result2 = 0f - result2;
					}
					Vector3.Dot(ref value2, ref vector, out result2);
					if (result2 > a5h)
					{
						HasSupport = true;
						W item = new W
						{
							Contact = new _0004.b
							{
								Position = contact.Contact.Position,
								Normal = value2,
								PenetrationDepth = contact.Contact.PenetrationDepth,
								Id = contact.Contact.Id
							},
							Support = ((pair.BroadPhaseOverlap.EntryA != body.CollisionInformation) ? ((P.h)pair.BroadPhaseOverlap.EntryA) : ((P.h)pair.BroadPhaseOverlap.EntryB))
						};
						if (result2 > a5B)
						{
							item.HasTraction = true;
							HasTraction = true;
						}
						else
						{
							a56.Add(new _0002
							{
								Collidable = item.Support,
								Contact = item.Contact
							});
						}
						a5b.Add(item);
					}
					else if (result2 < 0f - a5h)
					{
						item2.Collidable = ((pair.BroadPhaseOverlap.EntryA != body.CollisionInformation) ? ((P.h)pair.BroadPhaseOverlap.EntryA) : ((P.h)pair.BroadPhaseOverlap.EntryB));
						item2.Contact.Position = contact.Contact.Position;
						item2.Contact.Normal = value2;
						item2.Contact.PenetrationDepth = contact.Contact.PenetrationDepth;
						item2.Contact.Id = contact.Contact.Id;
						a5a.Add(item2);
					}
					else
					{
						item3.Collidable = ((pair.BroadPhaseOverlap.EntryA != body.CollisionInformation) ? ((P.h)pair.BroadPhaseOverlap.EntryA) : ((P.h)pair.BroadPhaseOverlap.EntryB));
						item3.Contact.Position = contact.Contact.Position;
						item3.Contact.Normal = value2;
						item3.Contact.PenetrationDepth = contact.Contact.PenetrationDepth;
						item3.Contact.Id = contact.Contact.Id;
						a56.Add(item3);
					}
				}
			}
			SupportRayData = null;
			a57 = body.Height * 0.25f;
			if (!HasTraction && hasTraction)
			{
				float num = (hasTraction ? (a57 + a5_0006.StepManager.MaximumStepHeight) : a57);
				Ray ray = new Ray(body.Position + vector * body.Height * 0.25f, vector);
				if (bg(ref ray, num, out var _, out var value3))
				{
					SupportRayData = value3;
					HasTraction = value3.HasTraction;
					HasSupport = true;
				}
			}
			bool flag2 = a5_0006.HorizontalMotionConstraint.MovementDirection.LengthSquared() > 0f;
			if (!HasTraction && hasTraction && flag2)
			{
				Ray ray2 = new Ray(body.Position + new Vector3(a5_0006.HorizontalMotionConstraint.MovementDirection.X, 0f, a5_0006.HorizontalMotionConstraint.MovementDirection.Y) * (a5_0006.Body.Radius - a5_0006.Body.CollisionInformation.Shape.CollisionMargin) + vector * body.Height * 0.25f, vector);
				Ray ray3 = default(Ray);
				ray3.Position = body.Position + vector * body.Height * 0.25f;
				ray3.Direction = ray2.Position - ray3.Position;
				if (!a5_0006.QueryManager.RayCastHitAnything(ray3, 1f))
				{
					float num2 = (hasTraction ? (a57 + a5_0006.StepManager.MaximumStepHeight) : a57);
					if (bg(ref ray2, num2, out var flag3, out var value4) && (!SupportRayData.HasValue || value4.HitData.T < SupportRayData.Value.HitData.T))
					{
						if (flag3)
						{
							SupportRayData = value4;
							HasTraction = true;
						}
						else if (!SupportRayData.HasValue)
						{
							SupportRayData = value4;
						}
						HasSupport = true;
					}
				}
			}
			if (!HasTraction && hasTraction && flag2)
			{
				Vector3 vector2 = new Vector3(a5_0006.HorizontalMotionConstraint.MovementDirection.X, 0f, a5_0006.HorizontalMotionConstraint.MovementDirection.Y);
				Vector3.Cross(ref vector2, ref vector, out vector2);
				Vector3.Multiply(ref vector2, a5_0006.Body.Radius - a5_0006.Body.CollisionInformation.Shape.CollisionMargin, out vector2);
				Ray ray4 = new Ray(body.Position + vector2 + vector * body.Height * 0.25f, vector);
				Ray ray5 = default(Ray);
				ray5.Position = body.Position + vector * body.Height * 0.25f;
				ray5.Direction = ray4.Position - ray5.Position;
				if (!a5_0006.QueryManager.RayCastHitAnything(ray5, 1f))
				{
					float num3 = (hasTraction ? (a57 + a5_0006.StepManager.MaximumStepHeight) : a57);
					if (bg(ref ray4, num3, out var flag4, out var value5) && (!SupportRayData.HasValue || value5.HitData.T < SupportRayData.Value.HitData.T))
					{
						if (flag4)
						{
							SupportRayData = value5;
							HasTraction = true;
						}
						else if (!SupportRayData.HasValue)
						{
							SupportRayData = value5;
						}
						HasSupport = true;
					}
				}
			}
			if (HasTraction || !hasTraction || !flag2)
			{
				return;
			}
			Vector3 vector3 = new Vector3(a5_0006.HorizontalMotionConstraint.MovementDirection.X, 0f, a5_0006.HorizontalMotionConstraint.MovementDirection.Y);
			Vector3.Cross(ref vector, ref vector3, out vector3);
			Vector3.Multiply(ref vector3, a5_0006.Body.Radius - a5_0006.Body.CollisionInformation.Shape.CollisionMargin, out vector3);
			Ray ray6 = new Ray(body.Position + vector3 + vector * body.Height * 0.25f, vector);
			Ray ray7 = default(Ray);
			ray7.Position = body.Position + vector * body.Height * 0.25f;
			ray7.Direction = ray6.Position - ray7.Position;
			if (a5_0006.QueryManager.RayCastHitAnything(ray7, 1f))
			{
				return;
			}
			float num4 = (hasTraction ? (a57 + a5_0006.StepManager.MaximumStepHeight) : a57);
			if (bg(ref ray6, num4, out var flag5, out var value6) && (!SupportRayData.HasValue || value6.HitData.T < SupportRayData.Value.HitData.T))
			{
				if (flag5)
				{
					SupportRayData = value6;
					HasTraction = true;
				}
				else if (!SupportRayData.HasValue)
				{
					SupportRayData = value6;
				}
				HasSupport = true;
			}
		}

		private bool bg(ref Ray P_0, float P_1, out bool P_2, out _000E P_3)
		{
			P_3 = default(_000E);
			P_2 = false;
			if (a5_0006.QueryManager.RayCast(P_0, P_1, out var earliestHit, out var hitObject))
			{
				float num = earliestHit.Normal.LengthSquared();
				if (num < 1E-07f)
				{
					return false;
				}
				Vector3.Divide(ref earliestHit.Normal, (float)Math.Sqrt(num), out earliestHit.Normal);
				earliestHit.Normal.Normalize();
				Vector3.Dot(ref P_0.Direction, ref earliestHit.Normal, out var result);
				if (result < 0f)
				{
					Vector3.Negate(ref earliestHit.Normal, out earliestHit.Normal);
					result = 0f - result;
				}
				if (result > a5B)
				{
					P_2 = true;
					P_3 = new _000E
					{
						HitData = earliestHit,
						HitObject = hitObject,
						HasTraction = true
					};
				}
				else
				{
					P_3 = new _000E
					{
						HitData = earliestHit,
						HitObject = hitObject
					};
				}
				return true;
			}
			return false;
		}

		internal void bP()
		{
			HasSupport = false;
			HasTraction = false;
			a5b.Clear();
			SupportRayData = null;
		}
	}
}
namespace _0002
{
	internal struct X
	{
		internal D.b a5h;

		internal _0004.b a5b;

		internal P.h a56;

		internal X(P.h P_0, D.b P_1, ref _0004.b P_2)
		{
			a56 = P_0;
			a5h = P_1;
			a5b = P_2;
		}
	}
}
namespace _0001
{
	internal abstract class X : global::r.b
	{
		protected Action<int> multithreadedUpdateDelegate;

		protected global::r.B timeStepSettings;

		[CompilerGenerated]
		private global::r.a a5h;

		public global::r.B TimeStepSettings => timeStepSettings;

		public global::r.a Space
		{
			[CompilerGenerated]
			get
			{
				return a5h;
			}
			[CompilerGenerated]
			set
			{
				a5h = value;
			}
		}

		protected X(global::r.B P_0)
		{
			timeStepSettings = P_0;
			Enabled = true;
		}

		protected X(global::r.B P_0, d.b P_1)
			: this(P_0)
		{
			base.ThreadManager = P_1;
			base.AllowMultithreading = true;
		}

		public abstract void SequentialUpdatingStateChanged(h updateable);
	}
}
