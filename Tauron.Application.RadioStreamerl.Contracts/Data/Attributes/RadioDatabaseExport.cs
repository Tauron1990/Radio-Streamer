using System;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Attributes
{
    [PublicAPI, MeansImplicitUse(ImplicitUseKindFlags.Assign)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
	public sealed class RadioDatabaseExport : ExportAttribute
	{
		public RadioDatabaseExport()
			: base(typeof(IRadioDatabase))
		{

		}
	}

    [PublicAPI]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter), MeansImplicitUse(ImplicitUseKindFlags.Assign)]
	public sealed class InjectRadioDatabase : InjectAttribute
	{
		public InjectRadioDatabase()
			: base(typeof(IRadioDatabase))
		{

		}
	}
}
