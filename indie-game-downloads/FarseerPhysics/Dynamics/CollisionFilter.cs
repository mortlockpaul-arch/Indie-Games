using System.Collections.Generic;
using FarseerPhysics.Dynamics.Contacts;

namespace FarseerPhysics.Dynamics;

public class CollisionFilter
{
	private Category _collidesWith;

	private Category _collisionCategories;

	private short _collisionGroup;

	private Dictionary<int, bool> _collisionIgnores = new Dictionary<int, bool>();

	private Fixture _fixture;

	public short CollisionGroup
	{
		get
		{
			return _collisionGroup;
		}
		set
		{
			if (_fixture.Body != null && _collisionGroup != value)
			{
				_collisionGroup = value;
				FilterChanged();
			}
		}
	}

	public Category CollidesWith
	{
		get
		{
			return _collidesWith;
		}
		set
		{
			if (_fixture.Body != null && _collidesWith != value)
			{
				_collidesWith = value;
				FilterChanged();
			}
		}
	}

	public Category CollisionCategories
	{
		get
		{
			return _collisionCategories;
		}
		set
		{
			if (_fixture.Body != null && _collisionCategories != value)
			{
				_collisionCategories = value;
				FilterChanged();
			}
		}
	}

	public CollisionFilter(Fixture fixture)
	{
		_fixture = fixture;
		if (Settings.UseFPECollisionCategories)
		{
			_collisionCategories = Category.All;
		}
		else
		{
			_collisionCategories = Category.Cat1;
		}
		_collidesWith = Category.All;
		_collisionGroup = 0;
	}

	public void AddCollisionCategory(Category category)
	{
		CollisionCategories |= category;
	}

	public void RemoveCollisionCategory(Category category)
	{
		CollisionCategories &= ~category;
	}

	public bool IsInCollisionCategory(Category category)
	{
		return (CollisionCategories & category) == category;
	}

	public void AddCollidesWithCategory(Category category)
	{
		CollidesWith |= category;
	}

	public void RemoveCollidesWithCategory(Category category)
	{
		CollidesWith &= ~category;
	}

	public bool IsInCollidesWithCategory(Category category)
	{
		return (CollidesWith & category) == category;
	}

	public void RestoreCollisionWith(Fixture fixture)
	{
		if (_collisionIgnores.ContainsKey(fixture.FixtureId))
		{
			_collisionIgnores[fixture.FixtureId] = false;
			FilterChanged();
		}
	}

	public void IgnoreCollisionWith(Fixture fixture)
	{
		if (_collisionIgnores.ContainsKey(fixture.FixtureId))
		{
			_collisionIgnores[fixture.FixtureId] = true;
		}
		else
		{
			_collisionIgnores.Add(fixture.FixtureId, value: true);
		}
		FilterChanged();
	}

	public bool IsFixtureIgnored(Fixture fixture)
	{
		if (_collisionIgnores.ContainsKey(fixture.FixtureId))
		{
			return _collisionIgnores[fixture.FixtureId];
		}
		return false;
	}

	private void FilterChanged()
	{
		for (ContactEdge contactEdge = _fixture.Body.ContactList; contactEdge != null; contactEdge = contactEdge.Next)
		{
			Contact contact = contactEdge.Contact;
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			if (fixtureA == _fixture || fixtureB == _fixture)
			{
				contact.FlagForFiltering();
			}
		}
	}
}
