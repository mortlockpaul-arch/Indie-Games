using System;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace _0003;

internal class D
{
	private bool _3A_0018;

	private IGraphicsDeviceService _3AL;

	private int _3A_0019;

	private int _3A3;

	private int _3A6;

	internal bool Changed
	{
		get
		{
			PresentationParameters presentationParameters = _3AL.GraphicsDevice.PresentationParameters;
			if (_3A_0018 || presentationParameters.BackBufferWidth != _3A_0019 || presentationParameters.BackBufferHeight != _3A3 || presentationParameters.MultiSampleCount != _3A6)
			{
				_3A_0019 = presentationParameters.BackBufferWidth;
				_3A3 = presentationParameters.BackBufferHeight;
				_3A6 = presentationParameters.MultiSampleCount;
				_3A_0018 = false;
				return true;
			}
			return false;
		}
	}

	internal D()
	{
		IGraphicsDeviceService graphicsDeviceManager = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
		_3AL = graphicsDeviceManager;
		_3AL.DeviceCreated += Lt;
		_3AL.DeviceReset += Lt;
		_3AL.DeviceDisposing += Lt;
	}

	private void Lt(object P_0, EventArgs P_1)
	{
		_3A_0018 = true;
	}
}
