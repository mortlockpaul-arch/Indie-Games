using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Billard3;

public class BandeObject
{
	public enum Type
	{
		ORTHO_X,
		ORTHO_Z,
		TROU_CORNER,
		TROU_CENTRAL,
		COLLISION_AVEC_TROU,
		CUSTOM
	}

	public enum Id
	{
		ORTHO_X_ZP,
		ORTHO_X_ZM,
		ORTHO_Z_XP,
		ORTHO_Z_XM,
		CENTRAL_XP_ZP,
		CENTRAL_XP_ZM,
		CENTRAL_XM_ZP,
		CENTRAL_XM_ZM,
		CORNER_XP_ZP_LARGEUR,
		CORNER_XP_ZP_LONGUEUR,
		CORNER_XP_ZM_LARGEUR,
		CORNER_XP_ZM_LONGUEUR,
		CORNER_XM_ZP_LARGEUR,
		CORNER_XM_ZP_LONGUEUR,
		CORNER_XM_ZM_LARGEUR,
		CORNER_XM_ZM_LONGUEUR,
		CUSTOM,
		FUNKY_BANDE_REJET_XP,
		FUNKY_BANDE_REJET_XM,
		FUNKY_BANDE_REJET_ZP,
		FUNKY_BANDE_REJET_ZM
	}

	public Id id;

	public Type type;

	public Vector2 p1;

	public Vector2 p2;

	public string name;

	public Vector2 trou;

	public List<Vector2> points;

	public BandeObject(Id id, Type type, Vector2 p1, Vector2 p2, Vector2 trou, string name)
	{
		this.id = id;
		this.type = type;
		this.p1 = p1;
		this.p2 = p2;
		this.trou = trou;
		this.name = name;
		points = new List<Vector2>();
		points.Add(p1);
		points.Add(p2);
	}

	public BandeObject()
	{
	}

	public Vector2 getVectorSameYSignAs(Vector2 velocity2)
	{
		Vector2 result = p1 - p2;
		if (Math.Sign(result.Y) != Math.Sign(velocity2.Y))
		{
			result *= -1f;
		}
		return result;
	}

	public Vector2 getVectorUnitaireRejet()
	{
		Vector2 v = p1 - p2;
		v.Normalize();
		v = OldMath.vector2Normal(v);
		Vector2 value = p1 + v;
		Vector2 value2 = p1 - v;
		if (Vector2.Distance(value, trou) > Vector2.Distance(value2, trou))
		{
			v *= -1f;
		}
		return v;
	}
}
