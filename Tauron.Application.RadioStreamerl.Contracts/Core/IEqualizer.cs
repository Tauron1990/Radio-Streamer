using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Core
{
    [PublicAPI]
	public interface IEqualizer
	{
		float Band0 { get; set; }
		float Band1 { get; set; }
		float Band2 { get; set; }
		float Band3 { get; set; }
		float Band4 { get; set; }
		float Band5 { get; set; }
		float Band6 { get; set; }
		float Band7 { get; set; }
		float Band8 { get; set; }
		float Band9 { get; set; }
		bool Enabled { get; set; }
	}
}
