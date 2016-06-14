using System;
using System.Collections.Generic;
using System.Threading;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    public sealed class RadioCreatedEventArgs : EventArgs
    {
        public RadioEntry RadioEntry { get; private set; }

        public RadioCreatedEventArgs(RadioEntry radioEntry)
        {
            RadioEntry = radioEntry;
        }
    }

    public sealed class ManyRadiosCreatedEvent : EventArgs
    {
        [NotNull]
        public IEnumerable<RadioEntry> Entries { get; private set; }

        public ManyRadiosCreatedEvent([NotNull] IEnumerable<RadioEntry> entries)
        {
            if (entries == null) throw new ArgumentNullException("entries");
            Entries = entries;
        }
    }

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
        event EventHandler DatabaseCleared;

        event EventHandler<RadioCreatedEventArgs> RadioAdded;

        event EventHandler<ManyRadiosCreatedEvent> RadiosAdded;

        int Count { get; }

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
