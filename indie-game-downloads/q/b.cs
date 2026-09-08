using System.Collections.Generic;
using _0001;
using _000F;
using E;
using L;
using Microsoft.Xna.Framework;
using l;
using r;

namespace Q
{
	internal class b : L.h
	{
		internal class _00065h : _0001.b, _0001.a, _0001.h, r.h
		{
			private b a5h;

			public _00065h(b rotationalmotionconstraint)
			{
				a5h = rotationalmotionconstraint;
			}

			public void Update(float notcorrecttimefromBEPU)
			{
				a5h.a5h = 0f;
			}
		}

		private new float a5h;

		private float a5b;

		private _000F.h a56;

		private _00065h a5a;

		public b(_000F.h controller)
		{
			a56 = controller;
			a5a = new _00065h(this);
			CollectInvolvedEntities();
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			outputInvolvedEntities.Add(a56.Body);
		}

		public override void OnAdditionToSpace(r.a newSpace)
		{
			base.OnAdditionToSpace(newSpace);
			newSpace.Add(a5a);
		}

		public override void OnRemovalFromSpace(r.a oldSpace)
		{
			base.OnRemovalFromSpace(oldSpace);
			oldSpace.Remove(a5a);
		}

		public override float SolveIteration()
		{
			return 0f;
		}

		public void Turn(float amount)
		{
			a5h = amount;
		}

		public override void ExclusiveUpdate()
		{
			a56.Body.Orientation *= Quaternion.CreateFromAxisAngle(Vector3.Up, a5b);
			a5b = 0f;
		}

		public override void Update(float dt)
		{
			a5b = a5h * dt;
		}
	}
}
namespace q
{
	internal class b
	{
		private readonly int a5h;

		public b()
		{
			ulong num = (ulong)base.GetHashCode();
			num = num * num * num * num * num * 3625334849u;
			a5h = (int)num;
		}

		public static void DefineCollisionRule(b groupA, b groupB, a rule)
		{
			_6 key = new _6(groupA, groupB);
			if (_7.CollisionGroupRules.ContainsKey(key))
			{
				_7.CollisionGroupRules[key] = rule;
			}
			else
			{
				_7.CollisionGroupRules.Add(key, rule);
			}
		}

		public static void DefineCollisionRulesBetweenSets(List<b> aGroups, List<b> bGroups, a rule)
		{
			foreach (b aGroup in aGroups)
			{
				DefineCollisionRulesWithSet(aGroup, bGroups, rule);
			}
		}

		public static void DefineCollisionRulesInSet(List<b> groups, a self, a other)
		{
			for (int i = 0; i < groups.Count; i++)
			{
				DefineCollisionRule(groups[i], groups[i], self);
			}
			for (int j = 0; j < groups.Count - 1; j++)
			{
				for (int k = j + 1; k < groups.Count; k++)
				{
					DefineCollisionRule(groups[j], groups[k], other);
				}
			}
		}

		public static void DefineCollisionRulesWithSet(b group, List<b> groups, a rule)
		{
			foreach (b group2 in groups)
			{
				DefineCollisionRule(group, group2, rule);
			}
		}

		public static void RemoveCollisionRule(b groupA, b groupB)
		{
			Dictionary<_6, a> collisionGroupRules = _7.CollisionGroupRules;
			_6 key = new _6(groupA, groupB);
			if (collisionGroupRules.ContainsKey(key))
			{
				collisionGroupRules.Remove(key);
			}
		}

		public static void RemoveCollisionRulesBetweenSets(List<b> aGroups, List<b> bGroups)
		{
			foreach (b aGroup in aGroups)
			{
				RemoveCollisionRulesWithSet(aGroup, bGroups);
			}
		}

		public static void RemoveCollisionRulesInSet(List<b> groups)
		{
			for (int i = 0; i < groups.Count; i++)
			{
				RemoveCollisionRule(groups[i], groups[i]);
			}
			for (int j = 0; j < groups.Count - 1; j++)
			{
				for (int k = j + 1; k < groups.Count; k++)
				{
					RemoveCollisionRule(groups[j], groups[k]);
				}
			}
		}

		public static void RemoveCollisionRulesWithSet(b group, List<b> groups)
		{
			foreach (b group2 in groups)
			{
				RemoveCollisionRule(group, group2);
			}
		}

		public override int GetHashCode()
		{
			return a5h;
		}
	}
}
