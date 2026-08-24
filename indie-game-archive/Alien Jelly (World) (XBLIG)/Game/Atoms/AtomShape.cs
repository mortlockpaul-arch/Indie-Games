using System;
using System.Collections.Generic;
using GKEngine.Entities;
using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomShape
{
	public const string TRACER_AREA_PREFIX = "Area";

	public const string TRACER_CONNECTOR_PREFIX = "Connector";

	public MaxModel model;

	public GridPoint[] area;

	public GridPoint[] connectors;

	public AtomShape(MaxModel oModel)
	{
		model = oModel;
		ProcessTracers();
	}

	private void ProcessTracers()
	{
		List<Vector3> list = new List<Vector3>();
		List<Vector3> list2 = new List<Vector3>();
		foreach (KeyValuePair<string, Matrix> tracer in model.tracers)
		{
			if (tracer.Key.Length >= "Area".Length && tracer.Key.Substring(0, "Area".Length).ToLower() == "Area".ToLower())
			{
				list.Add(new Vector3(tracer.Value.M41 / Grid.SPACING.X, tracer.Value.M42 / Grid.SPACING.Y, tracer.Value.M43 / Grid.SPACING.Z));
			}
			if (tracer.Key.Length >= "Connector".Length && tracer.Key.Substring(0, "Connector".Length).ToLower() == "Connector".ToLower())
			{
				list2.Add(new Vector3(tracer.Value.M41 / Grid.SPACING.X, tracer.Value.M42 / Grid.SPACING.Y, tracer.Value.M43 / Grid.SPACING.Z));
			}
		}
		area = new GridPoint[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			area[i] = new GridPoint((int)Math.Round(list[i].X, 0), (int)Math.Round(list[i].Y, 0), (int)Math.Round(list[i].Z, 0));
		}
		connectors = new GridPoint[list2.Count];
		for (int i = 0; i < list2.Count; i++)
		{
			connectors[i] = new GridPoint((int)Math.Round(list2[i].X, 0), (int)Math.Round(list2[i].Y, 0), (int)Math.Round(list2[i].Z, 0));
		}
	}
}
