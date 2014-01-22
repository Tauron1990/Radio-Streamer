using System.Windows.Input;
using Tauron.Application.Ioc;
using Tauron.Application.RadioStreamer.Resources;

namespace Tauron.Application.RadioStreamer.Views
{
	[Export(typeof(IModule))]
	internal sealed class ViewModule : IModule
	{
	    public int Order { get { return 1000; } }

	    public void Initialize(CommonApplication application)
		{
			CommandBinder.Register(ApplicationCommands.Save);

			CommandBinder.Register(MediaCommands.Play);
			CommandBinder.Register(MediaCommands.Stop);
			CommandBinder.Register(MediaCommands.Record);
			CommandBinder.Register(MediaCommands.MuteVolume);

			CommandBinder.Register(RadioStreamerResources.ViewMetadataLebel, "Metadata");
			CommandBinder.Register(RadioStreamerResources.AddFavoritesLabel, "AddFavorites");
			CommandBinder.Register(RadioStreamerResources.RemoveFavoritesLabel, "RemoveFavorites");

			SimpleLocalize.Register(RadioStreamerResources.ResourceManager, GetType().Assembly);
		}
	}
}
