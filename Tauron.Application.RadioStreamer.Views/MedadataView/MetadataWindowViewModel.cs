using System;
using System.Collections.ObjectModel;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;

namespace Tauron.Application.RadioStreamer.Views.MedadataView
{
	public class MetadataWindowViewModel : ObservableObject
	{
		public MetadataWindowViewModel(RadioEntry entry)
		{
            if(entry.IsEmpty) return;

			_metadata = new ObservableCollection<string>();

			var meta = entry.Metadata;
			foreach (var item in meta)
			{
				_metadata.Add(String.Format("{0}={1}", item, meta[item]));
			}
		}

		private ObservableCollection<string> _metadata;
		public ObservableCollection<string> Metadata
		{
			get { return _metadata; }
			set
			{
				if (_metadata == value) return;

				_metadata = value;
				OnPropertyChanged();
			}
		}
		
	}
}
