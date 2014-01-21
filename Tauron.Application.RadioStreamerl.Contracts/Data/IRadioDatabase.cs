﻿using System.Collections.Generic;
using System.Threading;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    [PublicAPI]
	public class RadioQualityPair
	{
        [NotNull]
        public string RadioName { get; private set; }

        [NotNull]
        public IQualityDatabaseInterface Qualitys { get; private set; }

		public RadioQualityPair([NotNull] string radioName, [NotNull] IQualityDatabaseInterface @interface)
		{
		    RadioName = radioName;
		    Qualitys = @interface;
		}
	}
    [PublicAPI]
	public interface IRadioDatabase
	{
        [NotNull]
        ManualResetEventSlim StartLock { get; }

        [NotNull]
        IRadioEntryFactory GetEntryFactory();

        [NotNull]
        IEnumerable<RadioQualityPair> GetQualitys();

        [NotNull]
        IEnumerable<RadioEntry> GetRadios();
		
		RadioEntry OpenRadio([NotNull] string name);
		RadioEntry CreateRadio([NotNull] string name, out bool created);
		void DeleteRadio([NotNull] string name);

		bool ExisRadio([NotNull] string name);
		void Save();
		void Clear();
	}
}
