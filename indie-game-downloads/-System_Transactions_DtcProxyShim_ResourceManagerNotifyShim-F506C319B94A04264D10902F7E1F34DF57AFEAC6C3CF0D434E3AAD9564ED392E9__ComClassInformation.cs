using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

internal sealed class _003CSystem_Transactions_DtcProxyShim_ResourceManagerNotifyShim_003EF506C319B94A04264D10902F7E1F34DF57AFEAC6C3CF0D434E3AAD9564ED392E9__ComClassInformation : IComExposedClass
{
	private unsafe static volatile ComWrappers.ComInterfaceEntry* s_vtables;

	public unsafe static ComWrappers.ComInterfaceEntry* GetComInterfaceEntries(out int count)
	{
		count = 1;
		if (s_vtables == null)
		{
			ComWrappers.ComInterfaceEntry* ptr = (ComWrappers.ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(_003CSystem_Transactions_DtcProxyShim_ResourceManagerNotifyShim_003EF506C319B94A04264D10902F7E1F34DF57AFEAC6C3CF0D434E3AAD9564ED392E9__ComClassInformation), sizeof(ComWrappers.ComInterfaceEntry));
			IIUnknownDerivedDetails iUnknownDerivedDetails = StrategyBasedComWrappers.DefaultIUnknownInterfaceDetailsStrategy.GetIUnknownDerivedDetails(typeof(IResourceManagerSink).TypeHandle);
			*ptr = new ComWrappers.ComInterfaceEntry
			{
				IID = iUnknownDerivedDetails.Iid,
				Vtable = (nint)iUnknownDerivedDetails.ManagedVirtualMethodTable
			};
			s_vtables = ptr;
		}
		return s_vtables;
	}
}
