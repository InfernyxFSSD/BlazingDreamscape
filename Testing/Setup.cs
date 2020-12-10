using SybithosInfernyx.Arctyx;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;

namespace SybithosInfernyx.Testing
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            ModHelper.AddAssembly("SybithosInfernyx", typeof(ArctyxCharacterCardController).Assembly);
        }
    }
}
