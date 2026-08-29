using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Cameras;

public class CameraManager
{
	public Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();

	private Camera _camera;

	public Camera camera
	{
		get
		{
			return _camera;
		}
		set
		{
			_camera = value;
		}
	}

	public void Add(Camera oCamera)
	{
		if (!cameras.ContainsKey(oCamera.name))
		{
			cameras.Add(oCamera.name, oCamera);
		}
	}

	public void Remove(Camera oCamera)
	{
		if (cameras.ContainsKey(oCamera.name))
		{
			cameras.Remove(oCamera.name);
			if (camera.name == oCamera.name)
			{
				camera = GetFirst();
			}
		}
	}

	public void SetActive(string xLabel)
	{
		if (cameras.ContainsKey(xLabel))
		{
			_camera = cameras[xLabel];
		}
	}

	public Camera GetFirst()
	{
		Camera result = null;
		using (Dictionary<string, Camera>.ValueCollection.Enumerator enumerator = cameras.Values.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				Camera current = enumerator.Current;
				result = current;
			}
		}
		return result;
	}

	public void UpdateViewports(Viewport newViewport)
	{
		foreach (Camera value in cameras.Values)
		{
			value.viewport = newViewport;
		}
	}
}
