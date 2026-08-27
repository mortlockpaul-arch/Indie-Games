using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;

namespace EGEngine;

public class ePointLightGrid
{
	public Vector2 Min;

	public Vector2 Max;

	public Vector3[] FrustumCorners = new Vector3[4];

	public List<eLevelLight> CurrentLights = new List<eLevelLight>();
}
