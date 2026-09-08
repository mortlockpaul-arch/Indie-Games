using System;
using System.Collections.Generic;
using E;
using Microsoft.Xna.Framework;
using Y;
using l;

namespace r;

internal interface a
{
	l.X<E.h> Entities { get; }

	void Add(h spaceObject);

	void Remove(h spaceObject);

	void Update();

	void Update(float dt);

	bool RayCast(Ray ray, out _7 result);

	bool RayCast(Ray ray, Func<Y.a, bool> filter, out _7 result);

	bool RayCast(Ray ray, float maximumLength, out _7 result);

	bool RayCast(Ray ray, float maximumLength, Func<Y.a, bool> filter, out _7 result);

	bool RayCast(Ray ray, float maximumLength, IList<_7> outputRayCastResults);

	bool RayCast(Ray ray, float maximumLength, Func<Y.a, bool> filter, IList<_7> outputRayCastResults);
}
