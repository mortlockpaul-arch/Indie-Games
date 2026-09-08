using System.Security.AccessControl;

namespace System.Threading;

public static class ThreadingAclExtensions
{
	public static EventWaitHandleSecurity GetAccessControl(this EventWaitHandle handle)
	{
		return new EventWaitHandleSecurity(handle.GetSafeWaitHandle(), AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
	}

	public static void SetAccessControl(this EventWaitHandle handle, EventWaitHandleSecurity eventSecurity)
	{
		ArgumentNullException.ThrowIfNull(eventSecurity, "eventSecurity");
		eventSecurity.Persist(handle.GetSafeWaitHandle());
	}

	public static MutexSecurity GetAccessControl(this Mutex mutex)
	{
		return new MutexSecurity(mutex.GetSafeWaitHandle(), AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
	}

	public static void SetAccessControl(this Mutex mutex, MutexSecurity mutexSecurity)
	{
		ArgumentNullException.ThrowIfNull(mutexSecurity, "mutexSecurity");
		mutexSecurity.Persist(mutex.GetSafeWaitHandle());
	}

	public static SemaphoreSecurity GetAccessControl(this Semaphore semaphore)
	{
		return new SemaphoreSecurity(semaphore.GetSafeWaitHandle(), AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
	}

	public static void SetAccessControl(this Semaphore semaphore, SemaphoreSecurity semaphoreSecurity)
	{
		ArgumentNullException.ThrowIfNull(semaphoreSecurity, "semaphoreSecurity");
		semaphoreSecurity.Persist(semaphore.GetSafeWaitHandle());
	}
}
