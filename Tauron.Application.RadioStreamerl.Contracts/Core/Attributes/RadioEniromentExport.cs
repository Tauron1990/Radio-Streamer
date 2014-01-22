using Tauron.Application.Ioc;

namespace Tauron.Application.RadioStreamer.Contracts.Core.Attributes
{
	public sealed class RadioEniromentExport : ExportAttribute
	{
		public RadioEniromentExport()
			: base(typeof(IRadioEnvironment))
		{

		}
	}

	public sealed class InjectRadioEnviroment : InjectAttribute
	{
		public InjectRadioEnviroment()
			: base(typeof(IRadioEnvironment))
		{

		}
	}
}
