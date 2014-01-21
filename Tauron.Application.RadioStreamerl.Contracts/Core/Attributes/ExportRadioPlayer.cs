using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Player;

namespace Tauron.Application.RadioStreamer.Contracts.Core.Attributes
{
	public sealed class ExportRadioPlayer : ExportAttribute
	{
		public ExportRadioPlayer()
			: base(typeof(IRadioPlayer))
		{

		}
	}

	public sealed class InjectRadioPlayer : InjectAttribute
	{
		public InjectRadioPlayer()
			: base(typeof(IRadioPlayer))
		{

		}
	}
}
