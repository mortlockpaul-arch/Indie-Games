using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Game.Data;

public class DataSky
{
	public string name;

	public int type;

	public Vector3 primaryLightPosition;

	public Vector3 primaryLightColor;

	public Vector3 secondaryLightPosition;

	public Vector3 secondaryLightColor;

	public Vector3 ambientLightColor;

	public List<DataSkyItem> items;

	public List<DataSkyRing> rings;

	public DataSky()
	{
	}

	public DataSky(string xName, int xType, Vector3 vPrimaryLightPosition, Vector3 vPrimaryLightColor, Vector3 vSecondaryLightPosition, Vector3 vSecondaryLightColor, Vector3 vAmbientLightColor, List<DataSkyItem> aItems, List<DataSkyRing> aRings)
	{
		name = xName;
		type = xType;
		primaryLightPosition = vPrimaryLightPosition;
		primaryLightColor = vPrimaryLightColor;
		secondaryLightPosition = vPrimaryLightPosition;
		secondaryLightColor = vPrimaryLightPosition;
		ambientLightColor = vAmbientLightColor;
		items = aItems;
		rings = aRings;
	}
}
