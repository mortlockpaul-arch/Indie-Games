using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

internal sealed class _003CSystem_Transactions_DtcProxyShim_Phase0NotifyShim_003EFCAF6D6532B6DEAD97BAF81D6C20E91901C9925D7D5BEB7FC9E4ABC65DCB5F23D__ComClassInformation : IComExposedClass
{
	private unsafe static volatile ComWrappers.ComInterfaceEntry* s_vtables;

	public unsafe static ComWrappers.ComInterfaceEntry* GetComInterfaceEntries(out int count)
	{
		count = 1;
		if (s_vtables == null)
		{
			ComWrappers.ComInterfaceEntry* ptr = (ComWrappers.ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(_003CSystem_Transactions_DtcProxyShim_Phase0NotifyShim_003EFCAF6D6532B6DEAD97BAF81D6C20E91901C9925D7D5BEB7FC9E4ABC65DCB5F23D__ComClassInformation), sizeof(ComWrappers.ComInterfaceEntry));
			IIUnknownDerivedDetails iUnknownDerivedDetails = StrategyBasedComWrappers.DefaultIUnknownInterfaceDetailsStrategy.GetIUnknownDerivedDetails(typeof(ITransactionPhase0NotifyAsync).TypeHandle);
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
