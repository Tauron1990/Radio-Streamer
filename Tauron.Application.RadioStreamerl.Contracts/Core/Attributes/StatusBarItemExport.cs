using Tauron.Application.Ioc;

namespace Tauron.Application.RadioStreamer.Contracts.Core.Attributes
{
	public sealed class StatusBarItemExport : ExportAttribute
	{
		public const string StatusBarItem = "StatusBarItem";

		public StatusBarItemExport()
			: base(typeof(object))
		{
		    ContractName = StatusBarItem;
		}
	}

	public sealed class StatusBarItemImport : InjectAttribute
	{
		public StatusBarItemImport()
			: base(typeof(object))
		{
		    ContractName = StatusBarItemExport.StatusBarItem;
		}
	}
}
