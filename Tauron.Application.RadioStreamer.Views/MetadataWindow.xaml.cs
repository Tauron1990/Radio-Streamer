#region Usings

using System.Windows;
using Tauron.Application.RadioStreamer.Contracts.Data.Enttitis;

#endregion

namespace Tauron.Application.RadioStreamer.Views.MedadataView
{
    /// <summary>
    ///     Interaktionslogik für MetadataWindow.xaml
    /// </summary>
    public partial class MetadataWindow : Window
    {
        public MetadataWindow(RadioEntry entry)
        {
            DataContext = new MetadataWindowViewModel(entry);

            InitializeComponent();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}