using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using d;
using l;
using p;
using r;

namespace z
{
	internal class _6 : b
	{
		private readonly List<a> a5h = new List<a>();

		private readonly List<IDisposable> a5b = new List<IDisposable>();

		private readonly BasicEffect a56;

		private VertexDeclaration a5a;

		public _6()
		{
			GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
			a56 = new BasicEffect(graphicsDevice);
			a56.PreferPerPixelLighting = false;
			a56.LightingEnabled = false;
			a56.TextureEnabled = false;
			a56.FogEnabled = false;
			a56.VertexColorEnabled = false;
			a56.AmbientLightColor = Vector3.One;
			a56.DiffuseColor = new Vector3(0.85f, 0.5f, 1f);
			a56.Alpha = 0.5f;
			a5a = VertexPositionNormalTexture.VertexDeclaration;
		}

		protected override void Add(_7 displayObject)
		{
			a item = a.b_0004(this, displayObject);
			a5b.Add(item);
			a5h.Add(item);
		}

		protected override void Remove(_7 displayObject)
		{
			for (int i = 0; i < a5h.Count; i++)
			{
				a a2 = a5h[i];
				if (a2.a5h == displayObject)
				{
					a5h.RemoveAt(i);
					a2.Dispose();
					a5b.Remove(a2);
					break;
				}
			}
		}

		protected override void ClearManagedModels()
		{
			a5h.Clear();
		}

		protected override void UpdateManagedModels()
		{
			foreach (a item in a5h)
			{
				item.a5h.Update();
			}
		}

		public override void Unload()
		{
			foreach (IDisposable item in a5b)
			{
				item.Dispose();
			}
		}

		protected override void DrawManagedModels(Matrix viewMatrix, Matrix projectionMatrix, RasterizerState fillmode, RasterizerState wireframemode)
		{
			GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
			a56.View = viewMatrix;
			a56.Projection = projectionMatrix;
			for (int i = 0; i < a56.CurrentTechnique.Passes.Count; i++)
			{
				foreach (a item in a5h)
				{
					if (item.a5h.a5b)
					{
						item.bi(graphicsDevice, a56, a56.CurrentTechnique.Passes[i], fillmode, wireframemode);
					}
				}
			}
		}
	}
}
namespace Z
{
	internal class _6 : r.b
	{
		private int a5h = 100;

		private int a5b;

		internal float a56 = 0.07f;

		internal float a5a = 1f;

		internal bool a57 = true;

		private Queue<_0006> a5_0006 = new Queue<_0006>();

		private Queue<_0006> a5v = new Queue<_0006>();

		private List<_0006> a5B = new List<_0006>();

		private List<_0006> a5X = new List<_0006>();

		private r.B a5_0018;

		private l._7<_0006> a5W = new l._7<_0006>();

		private l._7<a> a5_0002 = new l._7<a>();

		private p.a<a> a5_000E = new p.a<a>();

		private Action<int> a5y;

		private Queue<_7> a5r = new Queue<_7>();

		private static float a5_0001 = 0.01f;

		private static int a5_000F = 3;

		public float VelocityLowerLimit
		{
			get
			{
				return (float)Math.Sqrt(a56);
			}
			set
			{
				a56 = value * value;
			}
		}

		public float LowVelocityTimeMinimum
		{
			get
			{
				return a5a;
			}
			set
			{
				if (value <= 0f)
				{
					throw new Exception("Must use a positive, non-zero value for deactivation time minimum.");
				}
				a5a = value;
			}
		}

		public bool UseStabilization
		{
			get
			{
				return a57;
			}
			set
			{
				a57 = value;
			}
		}

		public int MaximumDeactivationAttemptsPerFrame
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
			}
		}

		public r.B TimeStepSettings
		{
			get
			{
				return a5_0018;
			}
			set
			{
				a5_0018 = value;
			}
		}

		public l.X<a> SimulationIslands => new l.X<a>(a5_0002);

		public static float MaximumSplitAttemptsFraction
		{
			get
			{
				return a5_0001;
			}
			set
			{
				if (value > 1f || value < 0f)
				{
					throw new Exception("Value must be from zero to one.");
				}
				a5_0001 = value;
			}
		}

		public static int MinimumSplitAttempts
		{
			get
			{
				return a5_000F;
			}
			set
			{
				if (value >= 0)
				{
					throw new Exception("Minimum split count must be nonnegative.");
				}
				a5_000F = value;
			}
		}

		public _6(r.B timeStepSettings)
		{
			Enabled = true;
			a5y = _6_0012;
			a5_0018 = timeStepSettings;
		}

		public _6(r.B timeStepSettings, d.b threadManager)
			: this(timeStepSettings)
		{
			base.ThreadManager = threadManager;
			base.AllowMultithreading = true;
		}

		private void _6Q(a P_0)
		{
			P_0.bV();
			a5_000E.GiveBack(P_0);
		}

		public void Add(_0006 simulationIslandMember)
		{
			if (simulationIslandMember.DeactivationManager == null)
			{
				simulationIslandMember.Activate();
				simulationIslandMember.DeactivationManager = this;
				a5W.Add(simulationIslandMember);
				if (simulationIslandMember.IsDynamic)
				{
					AddSimulationIslandToMember(simulationIslandMember);
				}
				else
				{
					RemoveSimulationIslandFromMember(simulationIslandMember);
				}
				return;
			}
			throw new Exception("Cannot add that member to this DeactivationManager; it already belongs to a manager.");
		}

		public void Remove(_0006 simulationIslandMember)
		{
			if (simulationIslandMember.DeactivationManager == this)
			{
				if (simulationIslandMember.IsDynamic)
				{
					simulationIslandMember.Activate();
				}
				else
				{
					foreach (_7 item in simulationIslandMember.a57)
					{
						foreach (_0006 item2 in item.a5h)
						{
							if (item2 != simulationIslandMember)
							{
								item2.Activate();
							}
						}
					}
				}
				simulationIslandMember.DeactivationManager = null;
				a5W.Remove(simulationIslandMember);
				RemoveSimulationIslandFromMember(simulationIslandMember);
				return;
			}
			throw new Exception("Cannot remove that member from this DeactivationManager; it belongs to a different or no manager.");
		}

		private void _6_0012(int P_0)
		{
			a5W.Elements[P_0].UpdateDeactivationCandidacy(a5_0018.TimeStepDuration);
		}

		protected override void UpdateMultithreaded()
		{
			_6g();
			base.ThreadManager.ForLoop(0, a5W.a5h, a5y);
			_6P();
		}

		protected override void UpdateSingleThreaded()
		{
			_6g();
			for (int i = 0; i < a5W.a5h; i++)
			{
				a5W.Elements[i].UpdateDeactivationCandidacy(a5_0018.TimeStepDuration);
			}
			_6P();
		}

		private void _6g()
		{
			int num = Math.Max(a5_000F, (int)((float)a5r.Count * a5_0001));
			int num2 = 0;
			while (num2 < num && a5r.Count > 0)
			{
				_7 obj = a5r.Dequeue();
				if (!obj.SlatedForRemoval)
				{
					continue;
				}
				obj.SlatedForRemoval = false;
				obj.RemoveReferencesFromConnectedMembers();
				bool flag = false;
				for (int i = 0; i < obj.a5h.a5h; i++)
				{
					for (int j = i + 1; j < obj.a5h.a5h; j++)
					{
						flag |= _6I(obj.a5h.Elements[i], obj.a5h.Elements[j]);
					}
				}
				if (flag)
				{
					num2++;
				}
				if (obj.Owner == null)
				{
					p._6.GiveBack(obj);
				}
			}
		}

		private void _6P()
		{
			int num = 0;
			int num2 = 0;
			int num3 = a5_0002.a5h;
			while (num < a5h && a5_0002.a5h > 0 && num2 < num3)
			{
				a5b = (a5b + 1) % a5_0002.a5h;
				a a2 = a5_0002.Elements[a5b];
				if (a2.a5a == 0)
				{
					a5_0002.FastRemoveAt(a5b);
					_6Q(a2);
				}
				else
				{
					a2.TryToDeactivate();
					num += a2.a5a;
				}
				num2++;
			}
		}

		public void Add(_7 connection)
		{
			if (connection.DeactivationManager == null)
			{
				connection.DeactivationManager = this;
				if (connection.a5h.a5h <= 0)
				{
					return;
				}
				a a2 = connection.a5h.Elements[0].SimulationIsland;
				for (int i = 1; i < connection.a5h.a5h; i++)
				{
					a simulationIsland;
					if (a2 != (simulationIsland = connection.a5h.Elements[i].SimulationIsland))
					{
						a2 = _6s(a2, simulationIsland);
					}
				}
				if (connection.SlatedForRemoval)
				{
					connection.SlatedForRemoval = false;
				}
				else
				{
					connection.AddReferencesToConnectedMembers();
				}
				return;
			}
			throw new ArgumentException("Cannot add connection to deactivation manager; it already belongs to one.");
		}

		private a _6s(a P_0, a P_1)
		{
			if (P_0 == null)
			{
				P_1.Activate();
				return P_1;
			}
			if (P_1 == null)
			{
				P_0.Activate();
				return P_0;
			}
			if (P_0.a5a < P_1.a5a)
			{
				a a2 = P_1;
				P_1 = P_0;
				P_0 = a2;
			}
			P_0.Activate();
			P_1.a5h = P_0;
			return P_0;
		}

		public void Remove(_7 connection)
		{
			if (connection.DeactivationManager == this)
			{
				connection.DeactivationManager = null;
				connection.SlatedForRemoval = true;
				a5r.Enqueue(connection);
				return;
			}
			throw new ArgumentException("Cannot remove connection from activity manager; it is owned by a different or no activity manager.");
		}

		private bool _6I(_0006 P_0, _0006 P_1)
		{
			if (P_0.SimulationIsland != P_1.SimulationIsland || P_0.SimulationIsland == null || P_1.SimulationIsland == null)
			{
				return false;
			}
			a5_0006.Enqueue(P_0);
			a5v.Enqueue(P_1);
			a5B.Add(P_0);
			a5X.Add(P_1);
			P_0.a5r = v.OwnedByFirst;
			P_1.a5r = v.OwnedBySecond;
			while (true)
			{
				if (a5_0006.Count > 0 && a5v.Count > 0)
				{
					_0006 obj = a5_0006.Dequeue();
					for (int i = 0; i < obj.a57.a5h; i++)
					{
						for (int j = 0; j < obj.a57.Elements[i].a5h.a5h; j++)
						{
							_0006 obj2;
							if ((obj2 = obj.a57.Elements[i].a5h.Elements[j]) == obj || obj2.SimulationIsland == null)
							{
								continue;
							}
							switch (obj2.a5r)
							{
							case v.Unclaimed:
								a5_0006.Enqueue(obj2);
								obj2.a5r = v.OwnedByFirst;
								a5B.Add(obj2);
								continue;
							case v.OwnedBySecond:
								break;
							default:
								continue;
							}
							goto IL_00e1;
						}
					}
					obj = a5v.Dequeue();
					for (int k = 0; k < obj.a57.a5h; k++)
					{
						for (int l = 0; l < obj.a57.Elements[k].a5h.a5h; l++)
						{
							_0006 obj3;
							if ((obj3 = obj.a57.Elements[k].a5h.Elements[l]) == obj || obj3.SimulationIsland == null)
							{
								continue;
							}
							switch (obj3.a5r)
							{
							case v.Unclaimed:
								a5v.Enqueue(obj3);
								obj3.a5r = v.OwnedBySecond;
								a5X.Add(obj3);
								continue;
							case v.OwnedByFirst:
								break;
							default:
								continue;
							}
							goto IL_01b6;
						}
					}
					continue;
				}
				a a2 = a5_000E.Take();
				a5_0002.Add(a2);
				if (a5_0006.Count == 0)
				{
					for (int m = 0; m < a5B.Count; m++)
					{
						a5B[m].a5y.Remove(a5B[m]);
						a2.Add(a5B[m]);
					}
					a5v.Clear();
				}
				else if (a5v.Count == 0)
				{
					for (int n = 0; n < a5X.Count; n++)
					{
						a5X[n].a5y.Remove(a5X[n]);
						a2.Add(a5X[n]);
					}
					a5_0006.Clear();
				}
				P_0.Activate();
				P_1.Activate();
				break;
				IL_00e1:
				a5_0006.Clear();
				a5v.Clear();
				break;
				IL_01b6:
				a5_0006.Clear();
				a5v.Clear();
				break;
			}
			for (int num = 0; num < a5B.Count; num++)
			{
				a5B[num].a5r = v.Unclaimed;
			}
			for (int num2 = 0; num2 < a5X.Count; num2++)
			{
				a5X[num2].a5r = v.Unclaimed;
			}
			a5B.Clear();
			a5X.Clear();
			return true;
		}

		public void RemoveSimulationIslandFromMember(_0006 member)
		{
			if (member.a5y != null)
			{
				a a2 = member.a5y;
				a2.Remove(member);
				if (a2.a5a == 0)
				{
					a5_0002.Remove(a2);
					_6Q(a2);
					return;
				}
			}
			if (member.a57.a5h <= 0)
			{
				return;
			}
			for (int i = 0; i < member.Connections.Count; i++)
			{
				_0006 obj = null;
				for (int j = 0; j < member.a57.Elements[i].a5h.a5h; j++)
				{
					if (member.a57.Elements[i].a5h.Elements[j].SimulationIsland != null)
					{
						obj = member;
						break;
					}
				}
				if (obj == null)
				{
					continue;
				}
				for (int k = i + 1; k < member.Connections.Count; k++)
				{
					_0006 obj2 = null;
					for (int l = 0; l < member.a57.Elements[k].a5h.a5h; l++)
					{
						if (member.a57.Elements[k].a5h.Elements[l].SimulationIsland != null)
						{
							obj2 = member;
							break;
						}
					}
					if (obj2 != null)
					{
						_6I(obj, obj2);
					}
				}
			}
		}

		public void AddSimulationIslandToMember(_0006 member)
		{
			if (member.SimulationIsland != null)
			{
				throw new Exception("Cannot initialize member's simulation island; it already has one.");
			}
			if (member.Connections.Count > 0)
			{
				a a2 = null;
				for (int i = 0; i < member.Connections.Count; i++)
				{
					for (int j = 0; j < member.a57.Elements[i].a5h.a5h; j++)
					{
						a2 = member.a57.Elements[i].a5h.Elements[j].SimulationIsland;
						if (a2 != null)
						{
							a2.Add(member);
							break;
						}
					}
					if (a2 != null)
					{
						break;
					}
				}
				if (member.SimulationIsland == null)
				{
					a a3 = a5_000E.Take();
					a5_0002.Add(a3);
					a3.Add(member);
					return;
				}
				for (int k = 0; k < member.a57.a5h; k++)
				{
					for (int l = 0; l < member.a57.Elements[k].a5h.a5h; l++)
					{
						if (member.a57.Elements[k].a5h.Elements[l] == member)
						{
							continue;
						}
						a simulationIsland = member.a57.Elements[k].a5h.Elements[l].SimulationIsland;
						if (simulationIsland != null)
						{
							if (a2 != simulationIsland)
							{
								a2 = _6s(a2, simulationIsland);
							}
							break;
						}
					}
				}
			}
			else
			{
				a a4 = a5_000E.Take();
				a5_0002.Add(a4);
				a4.Add(member);
			}
		}
	}
}
