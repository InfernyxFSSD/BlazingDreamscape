using System;
using NUnit.Framework;
using System.Reflection;
using BlazingDreamscape.Arctyx;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace BlazingDreamscape.Testing
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            var assembly = Assembly.GetAssembly(typeof(ArctyxCharacterCardController));
            ModHelper.AddAssembly("BlazingDreamscape", assembly);
        }
    }
}
