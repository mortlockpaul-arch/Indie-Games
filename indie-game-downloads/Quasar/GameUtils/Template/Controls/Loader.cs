using System;
using Quasar.GUI;

namespace Quasar.GameUtils.Template.Controls;

public abstract class Loader : Control
{
	public const string Type = "Loader";

	private ELoadState loadState;

	public override string ControlType => "Loader";

	public ELoadState LoadState => loadState;

	public event LoaderEvent LoadFinished;

	public event LoaderEvent LoadError;

	public Loader(Layout layout)
		: base(layout)
	{
	}

	public virtual void startLoading()
	{
		Load();
		PostLoad();
		if (LoadFinished != null)
		{
			LoadFinished();
		}
	}

	protected abstract void Load();

	protected abstract void PostLoad();

	private void loadScenario(object parameters)
	{
		try
		{
			Load();
			loadState = ELoadState.Finished;
		}
		catch (Exception)
		{
			loadState = ELoadState.Error;
		}
	}

	private void taskFinished(object parameters)
	{
		switch (loadState)
		{
		case ELoadState.Error:
			if (LoadError != null)
			{
				LoadError();
			}
			break;
		case ELoadState.Finished:
			PostLoad();
			if (LoadFinished != null)
			{
				LoadFinished();
			}
			break;
		}
	}

	protected void invokeLoadError()
	{
		if (LoadError != null)
		{
			LoadError();
		}
	}
}
