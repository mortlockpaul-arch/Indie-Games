using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _0001;
using E;
using J;
using Microsoft.Xna.Framework;
using W;
using Y;
using Z;
using d;
using g;
using l;
using m;
using n;
using p;
using s;
using u;

namespace r;

internal class v : a, IDisposable
{
	private B a5h;

	private d.b a5b;

	private Y.b a56;

	private bool a5a;

	[CompilerGenerated]
	private W.v a57;

	[CompilerGenerated]
	private n._7 a5_0006;

	[CompilerGenerated]
	private Z._6 a5v;

	[CompilerGenerated]
	private W._0006 a5B;

	[CompilerGenerated]
	private W.a a5X;

	[CompilerGenerated]
	private J.b a5_0018;

	[CompilerGenerated]
	private u.b a5W;

	[CompilerGenerated]
	private m._6 a5_0002;

	[CompilerGenerated]
	private n.b a5_000E;

	[CompilerGenerated]
	private W._7 a5y;

	[CompilerGenerated]
	private _0001.W a5r;

	[CompilerGenerated]
	private _0001._0002 a5_0001;

	[CompilerGenerated]
	private _0001._000E a5_000F;

	[CompilerGenerated]
	private _0001.y a5Z;

	[CompilerGenerated]
	private _0001.r a5u;

	[CompilerGenerated]
	private global::_0001._0001 a5L;

	public B TimeStepSettings
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
			DeactivationManager.TimeStepSettings = value;
			ForceUpdater.TimeStepSettings = value;
			BoundingBoxUpdater.TimeStepSettings = value;
			Solver.TimeStepSettings = value;
			PositionUpdater.TimeStepSettings = value;
		}
	}

	public d.b ThreadManager
	{
		get
		{
			return a5b;
		}
		set
		{
			a5b = value;
			DeactivationManager.ThreadManager = value;
			ForceUpdater.ThreadManager = value;
			BoundingBoxUpdater.ThreadManager = value;
			BroadPhase.ThreadManager = value;
			NarrowPhase.ThreadManager = value;
			Solver.ThreadManager = value;
			PositionUpdater.ThreadManager = value;
			DuringForcesUpdateables.ThreadManager = value;
			BeforeNarrowPhaseUpdateables.ThreadManager = value;
			EndOfTimeStepUpdateables.ThreadManager = value;
			EndOfFrameUpdateables.ThreadManager = value;
		}
	}

	public W.v SpaceObjectBuffer
	{
		[CompilerGenerated]
		get
		{
			return a57;
		}
		[CompilerGenerated]
		set
		{
			a57 = value;
		}
	}

	public n._7 EntityStateWriteBuffer
	{
		[CompilerGenerated]
		get
		{
			return a5_0006;
		}
		[CompilerGenerated]
		set
		{
			a5_0006 = value;
		}
	}

	public Z._6 DeactivationManager
	{
		[CompilerGenerated]
		get
		{
			return a5v;
		}
		[CompilerGenerated]
		set
		{
			a5v = value;
		}
	}

	public W._0006 ForceUpdater
	{
		[CompilerGenerated]
		get
		{
			return a5B;
		}
		[CompilerGenerated]
		set
		{
			a5B = value;
		}
	}

	public W.a BoundingBoxUpdater
	{
		[CompilerGenerated]
		get
		{
			return a5X;
		}
		[CompilerGenerated]
		set
		{
			a5X = value;
		}
	}

	public Y.b BroadPhase
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = value;
			if (NarrowPhase != null)
			{
				if (value != null)
				{
					NarrowPhase.BroadPhaseOverlaps = a56.Overlaps;
				}
				else
				{
					NarrowPhase.BroadPhaseOverlaps = null;
				}
			}
		}
	}

	public J.b NarrowPhase
	{
		[CompilerGenerated]
		get
		{
			return a5_0018;
		}
		[CompilerGenerated]
		set
		{
			a5_0018 = value;
		}
	}

	public u.b Solver
	{
		[CompilerGenerated]
		get
		{
			return a5W;
		}
		[CompilerGenerated]
		set
		{
			a5W = value;
		}
	}

	public m._6 PositionUpdater
	{
		[CompilerGenerated]
		get
		{
			return a5_0002;
		}
		[CompilerGenerated]
		set
		{
			a5_0002 = value;
		}
	}

	public n.b BufferedStates
	{
		[CompilerGenerated]
		get
		{
			return a5_000E;
		}
		[CompilerGenerated]
		set
		{
			a5_000E = value;
		}
	}

	public W._7 DeferredEventDispatcher
	{
		[CompilerGenerated]
		get
		{
			return a5y;
		}
		[CompilerGenerated]
		set
		{
			a5y = value;
		}
	}

	public _0001.W DuringForcesUpdateables
	{
		[CompilerGenerated]
		get
		{
			return a5r;
		}
		[CompilerGenerated]
		set
		{
			a5r = value;
		}
	}

	public _0001._0002 BeforeNarrowPhaseUpdateables
	{
		[CompilerGenerated]
		get
		{
			return a5_0001;
		}
		[CompilerGenerated]
		set
		{
			a5_0001 = value;
		}
	}

	public _0001._000E BeforeSolverUpdateables
	{
		[CompilerGenerated]
		get
		{
			return a5_000F;
		}
		[CompilerGenerated]
		set
		{
			a5_000F = value;
		}
	}

	public _0001.y BeforePositionUpdateUpdateables
	{
		[CompilerGenerated]
		get
		{
			return a5Z;
		}
		[CompilerGenerated]
		set
		{
			a5Z = value;
		}
	}

	public _0001.r EndOfTimeStepUpdateables
	{
		[CompilerGenerated]
		get
		{
			return a5u;
		}
		[CompilerGenerated]
		set
		{
			a5u = value;
		}
	}

	public global::_0001._0001 EndOfFrameUpdateables
	{
		[CompilerGenerated]
		get
		{
			return a5L;
		}
		[CompilerGenerated]
		set
		{
			a5L = value;
		}
	}

	public l.X<E.h> Entities => BufferedStates.Entities;

	public v()
	{
		J.a.CollisionManagers = J.a.CollisionManagers;
		a5h = new B();
		a5b = new d._7();
		SpaceObjectBuffer = new W.v(this);
		EntityStateWriteBuffer = new n._7();
		DeactivationManager = new Z._6(TimeStepSettings, ThreadManager);
		ForceUpdater = new W._0006(TimeStepSettings, ThreadManager);
		BoundingBoxUpdater = new W.a(TimeStepSettings, ThreadManager);
		BroadPhase = new g.h(ThreadManager);
		NarrowPhase = new J.b(TimeStepSettings, BroadPhase.Overlaps, ThreadManager);
		Solver = new u.b(TimeStepSettings, DeactivationManager, ThreadManager);
		NarrowPhase.Solver = Solver;
		PositionUpdater = new m.a(TimeStepSettings, ThreadManager);
		BufferedStates = new n.b(ThreadManager);
		DeferredEventDispatcher = new W._7();
		DuringForcesUpdateables = new _0001.W(a5h, ThreadManager);
		BeforeNarrowPhaseUpdateables = new _0001._0002(a5h, ThreadManager);
		BeforeSolverUpdateables = new _0001._000E(a5h, ThreadManager);
		BeforePositionUpdateUpdateables = new _0001.y(a5h, ThreadManager);
		EndOfTimeStepUpdateables = new _0001.r(a5h, ThreadManager);
		EndOfFrameUpdateables = new global::_0001._0001(a5h, ThreadManager);
	}

	public void Add(h spaceObject)
	{
		if (spaceObject.Space != null)
		{
			throw new ArgumentException("The object belongs to some Space already; cannot add it again.");
		}
		spaceObject.Space = this;
		if (spaceObject is Z._0006 simulationIslandMember)
		{
			DeactivationManager.Add(simulationIslandMember);
		}
		if (spaceObject is Z.b b2)
		{
			DeactivationManager.Add(b2.ActivityInformation);
		}
		if (spaceObject is W._6 forceUpdateable)
		{
			ForceUpdater.Add(forceUpdateable);
		}
		if (spaceObject is s.h entry)
		{
			BoundingBoxUpdater.Add(entry);
		}
		if (spaceObject is Y.a entry2)
		{
			BroadPhase.Add(entry2);
		}
		if (spaceObject is Y.h h2)
		{
			BroadPhase.Add(h2.Entry);
			if (h2.Entry is s.h entry3)
			{
				BoundingBoxUpdater.Add(entry3);
			}
		}
		if (spaceObject is u.h item)
		{
			Solver.Add(item);
		}
		if (spaceObject is m.h updateable)
		{
			PositionUpdater.Add(updateable);
		}
		if (spaceObject is E.h e)
		{
			BufferedStates.Add(e);
		}
		if (spaceObject is W.h creator)
		{
			DeferredEventDispatcher.AddEventCreator(creator);
		}
		if (spaceObject is W.b b3)
		{
			DeferredEventDispatcher.AddEventCreator(b3.EventCreator);
		}
		if (spaceObject is _0001.v updateable2)
		{
			DuringForcesUpdateables.Add(updateable2);
		}
		if (spaceObject is _0001._7 updateable3)
		{
			BeforeNarrowPhaseUpdateables.Add(updateable3);
		}
		if (spaceObject is _0001._6 updateable4)
		{
			BeforeSolverUpdateables.Add(updateable4);
		}
		if (spaceObject is _0001._0006 updateable5)
		{
			BeforePositionUpdateUpdateables.Add(updateable5);
		}
		if (spaceObject is _0001.B updateable6)
		{
			EndOfTimeStepUpdateables.Add(updateable6);
		}
		if (spaceObject is _0001.a updateable7)
		{
			EndOfFrameUpdateables.Add(updateable7);
		}
		spaceObject.OnAdditionToSpace(this);
	}

	public void Remove(h spaceObject)
	{
		if (spaceObject.Space != this)
		{
			throw new ArgumentException("The object does not belong to this space; cannot remove it.");
		}
		if (spaceObject is Z._0006 simulationIslandMember)
		{
			DeactivationManager.Remove(simulationIslandMember);
		}
		if (spaceObject is Z.b b2)
		{
			DeactivationManager.Remove(b2.ActivityInformation);
		}
		if (spaceObject is W._6 forceUpdateable)
		{
			ForceUpdater.Remove(forceUpdateable);
		}
		if (spaceObject is s.h entry)
		{
			BoundingBoxUpdater.Remove(entry);
		}
		if (spaceObject is Y.a entry2)
		{
			BroadPhase.Remove(entry2);
		}
		if (spaceObject is Y.h h2)
		{
			BroadPhase.Remove(h2.Entry);
			if (h2.Entry is s.h entry3)
			{
				BoundingBoxUpdater.Remove(entry3);
			}
		}
		if (spaceObject is u.h item)
		{
			Solver.Remove(item);
		}
		if (spaceObject is m.h updateable)
		{
			PositionUpdater.Remove(updateable);
		}
		if (spaceObject is E.h e)
		{
			BufferedStates.Remove(e);
		}
		if (spaceObject is W.h creator)
		{
			DeferredEventDispatcher.RemoveEventCreator(creator);
		}
		if (spaceObject is W.b b3)
		{
			DeferredEventDispatcher.RemoveEventCreator(b3.EventCreator);
		}
		if (spaceObject is _0001.v updateable2)
		{
			DuringForcesUpdateables.Remove(updateable2);
		}
		if (spaceObject is _0001._7 updateable3)
		{
			BeforeNarrowPhaseUpdateables.Remove(updateable3);
		}
		if (spaceObject is _0001._6 updateable4)
		{
			BeforeSolverUpdateables.Remove(updateable4);
		}
		if (spaceObject is _0001._0006 updateable5)
		{
			BeforePositionUpdateUpdateables.Remove(updateable5);
		}
		if (spaceObject is _0001.B updateable6)
		{
			EndOfTimeStepUpdateables.Remove(updateable6);
		}
		if (spaceObject is _0001.a updateable7)
		{
			EndOfFrameUpdateables.Remove(updateable7);
		}
		spaceObject.Space = null;
		spaceObject.OnRemovalFromSpace(this);
	}

	private void a_0002()
	{
		SpaceObjectBuffer.Update();
		EntityStateWriteBuffer.Update();
		DeactivationManager.Update();
		ForceUpdater.Update();
		DuringForcesUpdateables.Update();
		BoundingBoxUpdater.Update();
		BroadPhase.Update();
		BeforeNarrowPhaseUpdateables.Update();
		NarrowPhase.Update();
		BeforeSolverUpdateables.Update();
		NarrowPhase.FlushGeneratedSolverUpdateables();
		Solver.Update();
		BeforePositionUpdateUpdateables.Update();
		PositionUpdater.Update();
		BufferedStates.ReadBuffers.Update();
		DeferredEventDispatcher.Update();
		EndOfTimeStepUpdateables.Update();
	}

	public void Update()
	{
		a_0002();
		EndOfFrameUpdateables.Update();
	}

	public void Update(float dt)
	{
		TimeStepSettings.AccumulatedTime += dt;
		for (int i = 0; i < TimeStepSettings.MaximumTimeStepsPerFrame; i++)
		{
			if (!(TimeStepSettings.AccumulatedTime >= TimeStepSettings.TimeStepDuration))
			{
				break;
			}
			TimeStepSettings.AccumulatedTime -= TimeStepSettings.TimeStepDuration;
			a_0002();
		}
		BufferedStates.InterpolatedStates.BlendAmount = TimeStepSettings.AccumulatedTime / TimeStepSettings.TimeStepDuration;
		BufferedStates.InterpolatedStates.Update();
		EndOfFrameUpdateables.Update();
	}

	public bool RayCast(Ray ray, out _7 result)
	{
		return RayCast(ray, float.MaxValue, out result);
	}

	public bool RayCast(Ray ray, Func<Y.a, bool> filter, out _7 result)
	{
		return RayCast(ray, float.MaxValue, filter, out result);
	}

	public bool RayCast(Ray ray, float maximumLength, out _7 result)
	{
		l._7<_7> rayCastResultList = p._6.GetRayCastResultList();
		bool result2 = RayCast(ray, maximumLength, rayCastResultList);
		result = rayCastResultList.Elements[0];
		for (int i = 1; i < rayCastResultList.a5h; i++)
		{
			_7 obj = rayCastResultList.Elements[i];
			if (obj.HitData.T < result.HitData.T)
			{
				result = obj;
			}
		}
		p._6.GiveBack(rayCastResultList);
		return result2;
	}

	public bool RayCast(Ray ray, float maximumLength, Func<Y.a, bool> filter, out _7 result)
	{
		l._7<_7> rayCastResultList = p._6.GetRayCastResultList();
		bool result2 = RayCast(ray, maximumLength, filter, rayCastResultList);
		result = rayCastResultList.Elements[0];
		for (int i = 1; i < rayCastResultList.a5h; i++)
		{
			_7 obj = rayCastResultList.Elements[i];
			if (obj.HitData.T < result.HitData.T)
			{
				result = obj;
			}
		}
		p._6.GiveBack(rayCastResultList);
		return result2;
	}

	public bool RayCast(Ray ray, float maximumLength, IList<_7> outputRayCastResults)
	{
		l._7<Y.a> collisionEntryList = p._6.GetCollisionEntryList();
		if (BroadPhase.QueryAccelerator.RayCast(ray, maximumLength, collisionEntryList))
		{
			for (int i = 0; i < collisionEntryList.a5h; i++)
			{
				Y.a a2 = collisionEntryList.Elements[i];
				if (a2.RayCast(ray, maximumLength, out var rayHit))
				{
					outputRayCastResults.Add(new _7(rayHit, a2));
				}
			}
		}
		p._6.GiveBack(collisionEntryList);
		return outputRayCastResults.Count > 0;
	}

	public bool RayCast(Ray ray, float maximumLength, Func<Y.a, bool> filter, IList<_7> outputRayCastResults)
	{
		l._7<Y.a> collisionEntryList = p._6.GetCollisionEntryList();
		if (BroadPhase.QueryAccelerator.RayCast(ray, maximumLength, collisionEntryList))
		{
			for (int i = 0; i < collisionEntryList.a5h; i++)
			{
				Y.a a2 = collisionEntryList.Elements[i];
				if (a2.RayCast(ray, maximumLength, filter, out var rayHit))
				{
					outputRayCastResults.Add(new _7(rayHit, a2));
				}
			}
		}
		p._6.GiveBack(collisionEntryList);
		return outputRayCastResults.Count > 0;
	}

	public void Dispose()
	{
		if (!a5a)
		{
			a5a = true;
			ThreadManager.Dispose();
		}
	}
}
