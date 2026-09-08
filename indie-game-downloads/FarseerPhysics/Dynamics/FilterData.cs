namespace FarseerPhysics.Dynamics;

public abstract class FilterData
{
	public Category DisabledOnCategories;

	public int DisabledOnGroup;

	public Category EnabledOnCategories = Category.All;

	public int EnabledOnGroup;

	public virtual bool IsActiveOn(Body body)
	{
		if (body == null || !body.Enabled || body.IsStatic)
		{
			return false;
		}
		if (body.FixtureList == null)
		{
			return false;
		}
		foreach (Fixture fixture in body.FixtureList)
		{
			if (fixture.CollisionFilter.CollisionGroup == DisabledOnGroup && fixture.CollisionFilter.CollisionGroup != 0 && DisabledOnGroup != 0)
			{
				return false;
			}
			if ((fixture.CollisionFilter.CollisionCategories & DisabledOnCategories) != Category.None)
			{
				return false;
			}
			if (EnabledOnGroup != 0 || EnabledOnCategories != Category.All)
			{
				if (fixture.CollisionFilter.CollisionGroup == EnabledOnGroup && fixture.CollisionFilter.CollisionGroup != 0 && EnabledOnGroup != 0)
				{
					return true;
				}
				if ((fixture.CollisionFilter.CollisionCategories & EnabledOnCategories) != Category.None && EnabledOnCategories != Category.All)
				{
					return true;
				}
				continue;
			}
			return true;
		}
		return false;
	}

	public void AddDisabledCategory(Category category)
	{
		DisabledOnCategories |= category;
	}

	public void RemoveDisabledCategory(Category category)
	{
		DisabledOnCategories &= ~category;
	}

	public bool IsInDisabledCategory(Category category)
	{
		return (DisabledOnCategories & category) == category;
	}

	public void AddEnabledCategory(Category category)
	{
		EnabledOnCategories |= category;
	}

	public void RemoveEnabledCategory(Category category)
	{
		EnabledOnCategories &= ~category;
	}

	public bool IsInEnbledCategory(Category category)
	{
		return (EnabledOnCategories & category) == category;
	}
}
