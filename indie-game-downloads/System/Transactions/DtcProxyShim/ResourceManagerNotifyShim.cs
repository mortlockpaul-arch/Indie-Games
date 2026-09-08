using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

namespace System.Transactions.DtcProxyShim;

[GeneratedComClass]
[ComExposedClass<_003CSystem_Transactions_DtcProxyShim_ResourceManagerNotifyShim_003EF506C319B94A04264D10902F7E1F34DF57AFEAC6C3CF0D434E3AAD9564ED392E9__ComClassInformation>]
internal sealed class ResourceManagerNotifyShim : NotificationShimBase, IResourceManagerSink
{
	internal ResourceManagerNotifyShim(DtcProxyShimFactory shimFactory, object enlistmentIdentifier)
		: base(shimFactory, enlistmentIdentifier)
	{
	}

	public void TMDown()
	{
		NotificationType = ShimNotificationType.ResourceManagerTmDownNotify;
		ShimFactory.NewNotification(this);
	}
}
