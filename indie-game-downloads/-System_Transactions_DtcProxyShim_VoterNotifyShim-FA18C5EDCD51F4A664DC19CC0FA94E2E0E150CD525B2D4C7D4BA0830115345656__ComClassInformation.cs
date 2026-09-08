using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

internal sealed class _003CSystem_Transactions_DtcProxyShim_VoterNotifyShim_003EFA18C5EDCD51F4A664DC19CC0FA94E2E0E150CD525B2D4C7D4BA0830115345656__ComClassInformation : IComExposedClass
{
	private unsafe static volatile ComWrappers.ComInterfaceEntry* s_vtables;

	public unsafe static ComWrappers.ComInterfaceEntry* GetComInterfaceEntries(out int count)
	{
		count = 1;
		if (s_vtables == null)
		{
			ComWrappers.ComInterfaceEntry* ptr = (ComWrappers.ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(_003CSystem_Transactions_DtcProxyShim_VoterNotifyShim_003EFA18C5EDCD51F4A664DC19CC0FA94E2E0E150CD525B2D4C7D4BA0830115345656__ComClassInformation), sizeof(ComWrappers.ComInterfaceEntry));
			IIUnknownDerivedDetails iUnknownDerivedDetails = StrategyBasedComWrappers.DefaultIUnknownInterfaceDetailsStrategy.GetIUnknownDerivedDetails(typeof(ITransactionVoterNotifyAsync2).TypeHandle);
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
