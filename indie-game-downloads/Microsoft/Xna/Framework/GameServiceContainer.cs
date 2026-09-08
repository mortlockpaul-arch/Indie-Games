using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework;

public class GameServiceContainer : IServiceProvider
{
	private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

	public void AddService(Type type, object provider)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type", "The service type cannot be null.");
		}
		if (provider == null)
		{
			throw new ArgumentNullException("provider", "The service provider instance cannot be null.");
		}
		if (services.ContainsKey(type))
		{
			throw new ArgumentException("Container already contains a service of this type.", "type");
		}
		if (!type.IsAssignableFrom(provider.GetType()))
		{
			throw new ArgumentException("Service provider object of type " + provider.GetType().FullName + " must be assignable to service type " + type.FullName + ".");
		}
		services.Add(type, provider);
	}

	public object GetService(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type", "The service type cannot be null.");
		}
		return INTERNAL_GetService(type);
	}

	public void RemoveService(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type", "The service type cannot be null.");
		}
		services.Remove(type);
	}

	internal void INTERNAL_AddService(Type type, object provider)
	{
		if (services.ContainsKey(type))
		{
			throw new ArgumentException("Container already contains a service of this type.", "type");
		}
		services.Add(type, provider);
	}

	internal void INTERNAL_RemoveService(Type type)
	{
		services.Remove(type);
	}

	internal object INTERNAL_GetService(Type type)
	{
		services.TryGetValue(type, out var value);
		return value;
	}
}
