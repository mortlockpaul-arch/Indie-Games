using System;

namespace XnaToFna.StubXDK.GamerServices;

public class TitleServiceDirectory
{
	private EventHandler<FindServicesCompletedArgs> _FindServicesCompleted;

	public bool IsBusy => false;

	public event EventHandler<FindServicesCompletedArgs> FindServicesCompleted
	{
		add
		{
			_FindServicesCompleted = (EventHandler<FindServicesCompletedArgs>)Delegate.Combine(_FindServicesCompleted, value);
		}
		remove
		{
			_FindServicesCompleted = (EventHandler<FindServicesCompletedArgs>)Delegate.Remove(_FindServicesCompleted, value);
		}
	}

	public void FindServicesAsync()
	{
	}
}
