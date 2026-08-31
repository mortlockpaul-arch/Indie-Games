using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Base object for singleton Begin/End statements.  Forces a Begin called on an object
/// to be followed by an End on the same object before Begin can be called on any other
/// object derived from this type.
/// </summary>
public class SafeSingletonBeginableObject
{
	private static bool HCB = false;

	private static object HC_0002;

	/// <summary>
	/// Verifies no other Begin is in process.
	/// </summary>
	public virtual void Begin()
	{
		if (HCB)
		{
			throw new Exception("Cannot call begin within previous begin statement.  Try calling end on the previously begun object.");
		}
		HCB = true;
		HC_0002 = this;
	}

	/// <summary>
	/// Verifies a Begin is in process on this object.
	/// </summary>
	public virtual void End()
	{
		if (!HCB)
		{
			throw new Exception("Cannot call end without first calling begin.");
		}
		if (HC_0002 != this)
		{
			throw new Exception("Cannot call end on this object.  Begin was last called on another object.");
		}
		HCB = false;
		HC_0002 = null;
	}
}
