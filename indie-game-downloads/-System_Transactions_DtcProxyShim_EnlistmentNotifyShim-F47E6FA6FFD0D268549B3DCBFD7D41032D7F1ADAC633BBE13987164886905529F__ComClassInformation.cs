using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

internal sealed class _003CSystem_Transactions_DtcProxyShim_EnlistmentNotifyShim_003EF47E6FA6FFD0D268549B3DCBFD7D41032D7F1ADAC633BBE13987164886905529F__ComClassInformation : IComExposedClass
{
	private unsafe static volatile ComWrappers.ComInterfaceEntry* s_vtables;

	public unsafe static ComWrappers.ComInterfaceEntry* GetComInterfaceEntries(out int count)
	{
		count = 1;
		if (s_vtables == null)
		{
			ComWrappers.ComInterfaceEntry* ptr = (ComWrappers.ComInterfaceEntry*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(_003CSystem_Transactions_DtcProxyShim_EnlistmentNotifyShim_003EF47E6FA6FFD0D268549B3DCBFD7D41032D7F1ADAC633BBE13987164886905529F__ComClassInformation), sizeof(ComWrappers.ComInterfaceEntry));
			IIUnknownDerivedDetails iUnknownDerivedDetails = StrategyBasedComWrappers.DefaultIUnknownInterfaceDetailsStrategy.GetIUnknownDerivedDetails(typeof(ITransactionResourceAsync).TypeHandle);
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
