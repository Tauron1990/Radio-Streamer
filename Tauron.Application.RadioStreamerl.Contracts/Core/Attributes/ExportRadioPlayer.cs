using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Contracts.Player;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core.Attributes
{
	public sealed class ExportRadioPlayer : ExportAttribute
	{
		public ExportRadioPlayer()
			: base(typeof(IRadioPlayer))
		{

		}
	}

    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
	public sealed class InjectRadioPlayer : InjectAttribute
	{
		public InjectRadioPlayer()
			: base(typeof(IRadioPlayer))
		{

		}
	}
}
