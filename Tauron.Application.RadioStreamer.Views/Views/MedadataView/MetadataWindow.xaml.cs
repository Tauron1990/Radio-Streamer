#region Usings

using System.Windows;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.RadioStreamer.Views.MedadataView
{
    /// <summary>
    ///     Interaktionslogik für MetadataWindow.xaml
    /// </summary>
    public partial class MetadataWindow
    {
        public MetadataWindow(RadioEntry entry)
        {
            DataContext = new MetadataWindowViewModel(entry);

            InitializeComponent();
        }

        private void CloseClick([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Close();
        }
    }
}