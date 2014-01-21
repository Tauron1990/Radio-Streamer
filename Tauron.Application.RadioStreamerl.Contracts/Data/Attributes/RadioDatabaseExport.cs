using System;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data.Attributes
{
    [PublicAPI]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
	public sealed class RadioDatabaseExport : ExportAttribute
	{
		public RadioDatabaseExport()
			: base(typeof(IRadioDatabase))
		{

		}
	}

    [PublicAPI]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public sealed class InjectRadioDatabase : InjectAttribute
	{
		public InjectRadioDatabase()
			: base(typeof(IRadioDatabase))
		{

		}
	}
}
