using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace Deep_waters;

public class ParticleManager : IRenderableManager, IUpdatableManager, IManagerService, IManager, IUnloadable
{
	private CustomCamera upcam;

	private int _ManagerProcessOrder = 100;

	public List<ParticleSystem> ParticleList;

	private IGraphicsDeviceService _GraphicsDeviceManager;

	private GameTime gtime;

	public Type ManagerType => typeof(ParticleManager);

	public int ManagerProcessOrder
	{
		get
		{
			return _ManagerProcessOrder;
		}
		set
		{
			_ManagerProcessOrder = value;
		}
	}

	public IGraphicsDeviceService GraphicsDeviceManager => _GraphicsDeviceManager;

	public IManagerServiceProvider OwnerSceneInterface
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public void submit(ParticleSystem psys)
	{
		ParticleList.Add(psys);
	}

	public ParticleManager(IGraphicsDeviceService graphicsdevicemanager)
	{
		_GraphicsDeviceManager = graphicsdevicemanager;
		ParticleList = new List<ParticleSystem>();
	}

	public void ApplyPreferences(SystemPreferences preferences)
	{
	}

	public void BeginFrameRendering(ISceneState scenestate)
	{
	}

	public void EndFrameRendering()
	{
		foreach (ParticleSystem particle in ParticleList)
		{
			particle.Draw(gtime);
		}
	}

	public void Update(GameTime gametime)
	{
		foreach (ParticleSystem particle in ParticleList)
		{
			if (upcam != null)
			{
				particle.SetCamera(upcam.View, upcam.Projection);
			}
			gtime = gametime;
			if (particle.settings.isUpdatable)
			{
				particle.AddParticle();
				particle.Update(gametime);
			}
			if (particle.ended)
			{
				ParticleList.Remove(particle);
				break;
			}
		}
	}

	public void updatecamera(CustomCamera cam)
	{
		upcam = cam;
	}

	public void Clear()
	{
	}

	public void Unload()
	{
		Clear();
	}

	public void ApplyPreferences(ISystemPreferences preferences)
	{
	}
}
