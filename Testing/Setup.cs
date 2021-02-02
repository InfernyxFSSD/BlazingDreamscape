using BlazingDreamscape.Arctyx;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;

namespace BlazingDreamscape.Testing
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            ModHelper.AddAssembly("BlazingDreamscape", typeof(ArctyxCharacterCardController).Assembly);
        }
    }
}
