using System;
using System.Collections.Generic;
using System.Threading;
using l;

namespace q;

internal class _7
{
	private Action a5h;

	private int a5b;

	private Action a56;

	internal b a5a;

	internal a a57;

	internal l.a<_7, a> a5_0006 = new l.a<_7, a>();

	internal static Func<_7, _7, a> a5v;

	public static Dictionary<_6, a> CollisionGroupRules;

	public static a DefaultCollisionRule;

	public static b DefaultDynamicCollisionGroup;

	public static b DefaultKinematicCollisionGroup;

	public b Group
	{
		get
		{
			return a5a;
		}
		set
		{
			a5a = value;
			OnChanged();
		}
	}

	public a Personal
	{
		get
		{
			return a57;
		}
		set
		{
			a57 = value;
			OnChanged();
		}
	}

	public l.a<_7, a> Specific
	{
		get
		{
			return a5_0006;
		}
		set
		{
			if (value != a5_0006)
			{
				if (a5_0006 != null)
				{
					a5_0006.Changed -= a56;
				}
				if (value != null)
				{
					value.Changed += a56;
				}
				a5_0006 = value;
				OnChanged();
			}
		}
	}

	public static Func<_7, _7, a> CollisionRuleCalculator
	{
		get
		{
			return a5v;
		}
		set
		{
			a5v = value;
		}
	}

	public event Action CollisionRulesChanged
	{
		add
		{
			Action action = a5h;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a5h, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = a5h;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a5h, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	public _7()
	{
		a5b = (int)(base.GetHashCode() * 2376512323u);
		a56 = OnChanged;
	}

	public override int GetHashCode()
	{
		return a5b;
	}

	protected void OnChanged()
	{
		if (a5h != null)
		{
			a5h();
		}
	}

	public static void AddRule(h ownerA, h ownerB, a rule)
	{
		ownerA.CollisionRules.a5_0006.Add(ownerB.CollisionRules, rule);
	}

	public static void AddRule(_7 rulesA, h ownerB, a rule)
	{
		rulesA.a5_0006.Add(ownerB.CollisionRules, rule);
	}

	public static void AddRule(h ownerA, _7 rulesB, a rule)
	{
		ownerA.CollisionRules.a5_0006.Add(rulesB, rule);
	}

	public static void RemoveRule(h ownerA, h ownerB)
	{
		if (!ownerA.CollisionRules.a5_0006.Remove(ownerB.CollisionRules))
		{
			ownerB.CollisionRules.a5_0006.Remove(ownerA.CollisionRules);
		}
	}

	public static void RemoveRule(_7 rulesA, h ownerB)
	{
		if (!rulesA.a5_0006.Remove(ownerB.CollisionRules))
		{
			ownerB.CollisionRules.a5_0006.Remove(rulesA);
		}
	}

	public static void RemoveRule(h ownerA, _7 rulesB)
	{
		if (!ownerA.CollisionRules.a5_0006.Remove(rulesB))
		{
			rulesB.a5_0006.Remove(ownerA.CollisionRules);
		}
	}

	static _7()
	{
		a5v = GetCollisionRuleDefault;
		CollisionGroupRules = new Dictionary<_6, a>();
		DefaultCollisionRule = a.Normal;
		DefaultDynamicCollisionGroup = new b();
		DefaultKinematicCollisionGroup = new b();
		CollisionGroupRules.Add(new _6(DefaultKinematicCollisionGroup, DefaultKinematicCollisionGroup), a.NoBroadPhase);
	}

	public static a GetCollisionRule(h ownerA, h ownerB)
	{
		return a5v(ownerA.CollisionRules, ownerB.CollisionRules);
	}

	public static a GetCollisionRuleDefault(_7 a, _7 b)
	{
		a a2 = GetSpecificCollisionRuleDefault(a, b);
		if (a2 == q.a.Defer)
		{
			a2 = GetPersonalCollisionRuleDefault(a, b);
			if (a2 == q.a.Defer)
			{
				a2 = GetGroupCollisionRuleDefault(a, b);
			}
		}
		if (a2 == q.a.Defer)
		{
			a2 = DefaultCollisionRule;
		}
		return a2;
	}

	public static a GetSpecificCollisionRuleDefault(_7 a, _7 b)
	{
		a.a5_0006.a5h.TryGetValue(b, out var value);
		b.a5_0006.a5h.TryGetValue(a, out var value2);
		if (value <= value2)
		{
			return value2;
		}
		return value;
	}

	public static a GetGroupCollisionRuleDefault(_7 a, _7 b)
	{
		if (a.a5a == null || b.a5a == null)
		{
			return q.a.Defer;
		}
		CollisionGroupRules.TryGetValue(new _6(a.a5a, b.a5a), out var value);
		return value;
	}

	public static a GetPersonalCollisionRuleDefault(_7 a, _7 b)
	{
		if (a.a57 <= b.a57)
		{
			return b.a57;
		}
		return a.a57;
	}
}
