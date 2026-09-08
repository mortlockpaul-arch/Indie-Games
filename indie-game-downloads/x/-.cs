using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace x
{
	internal class _0018
	{
		internal const string _3A_0018 = "UniqueId";

		internal const string _3AL = "Name";

		internal const string _3A_0019 = "Enabled";

		internal const string _3A3 = "UpdateType";

		internal const string _3A6 = "LightingType";

		internal const string _3AD = "DiffuseColor";

		internal const string _3A_0017 = "Intensity";

		internal const string _3A_0003 = "FillLight";

		internal const string _3Al = "FalloffStrength";

		internal const string _3At = "ShadowSource";

		internal const string _3AF = "ShadowType";

		internal const string _3Ac = "Position";

		internal const string _3Ag = "Radius";

		internal const string _3AI = "Direction";

		internal const string _3A8 = "Angle";

		internal const string _3AZ = "Volume";

		internal const string _3Ax = "Depth";

		internal const string _3Aq = "ShadowQuality";

		internal const string _3Ab = "ShadowPrimaryBias";

		internal const string _3AT = "ShadowSecondaryBias";

		internal const string _3Ay = "ShadowPerSurfaceLOD";

		internal const string _3A_0015 = "ShadowRenderLightsTogether";
	}
}
namespace X
{
	internal class _0018<TManager, TObject> where TManager : ISubmit<TObject>
	{
		private TManager _3A_0018;

		private List<TObject> _3AL = new List<TObject>(16);

		public TManager ContainingManager => _3A_0018;

		public void SetManager(TManager manager)
		{
			if (_3A_0018 != null)
			{
				if (manager != null)
				{
					throw new Exception("Unable to assign scene to more than one " + typeof(TManager).Name + ". Remove from the previous manager first.");
				}
				RemoveSubmittedObjects();
			}
			_3A_0018 = manager;
		}

		public void Submit(TObject obj)
		{
			if (_3A_0018 != null)
			{
				_3A_0018.Submit(obj);
				_3AL.Add(obj);
			}
		}

		public void RemoveSubmittedObjects()
		{
			if (_3A_0018 == null)
			{
				return;
			}
			foreach (TObject item in _3AL)
			{
				_3A_0018.Remove(item);
			}
			_3AL.Clear();
		}

		public void Optimize()
		{
			if (_3A_0018 is IWorldRenderableManager)
			{
				(_3A_0018 as IWorldRenderableManager).Optimize();
			}
		}
	}
	[Flags]
	internal enum _0019
	{
		LightingEffect = 1,
		BasicEffect_Lighting = 2,
		BasicEffect_NonLighting = 4,
		MiscEffect = 8
	}
	internal class _0017
	{
		internal bool _3A_0018 = true;

		internal bool _3AL;

		internal bool _3A_0019;

		internal bool _3A3;

		internal bool _3A6;

		internal Effect _3AD;

		private List<RenderableMesh> _3A_0017 = new List<RenderableMesh>(32);

		internal List<RenderableMesh> Objects => _3A_0017;

		internal void U()
		{
			_3A_0018 = true;
			_3AL = false;
			_3A_0019 = false;
			_3A3 = false;
			_3A6 = false;
			_3AD = null;
			_3A_0017.Clear();
		}
	}
	internal class _0003
	{
		internal static bool _6_0017(List<RenderableMesh> P_0, BoundingBox P_1)
		{
			bool flag = false;
			foreach (RenderableMesh item in P_0)
			{
				ISceneObject sceneObject = item._3AL;
				item._3Ax = P_1.Contains(sceneObject.WorldBoundingBox) != ContainmentType.Disjoint;
				flag |= item._3Ax;
			}
			return flag;
		}

		internal static bool _6_0003(List<RenderableMesh> P_0, BoundingFrustum P_1)
		{
			bool flag = false;
			foreach (RenderableMesh item in P_0)
			{
				ISceneObject sceneObject = item._3AL;
				item._3Ax = sceneObject.CastShadows && P_1.Contains(sceneObject.WorldBoundingBox) != ContainmentType.Disjoint;
				flag |= item._3Ax;
			}
			return flag;
		}
	}
}
