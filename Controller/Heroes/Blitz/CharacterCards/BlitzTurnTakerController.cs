using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace SybithosInfernyx.Blitz
{
    public class BlitzTurnTakerController : HeroTurnTakerController
    {
        public BlitzTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }
    }
}
