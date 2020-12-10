using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace SybithosInfernyx.Arctyx
{
    public class ArctyxTurnTakerController : HeroTurnTakerController
    {
        public ArctyxTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }
    }
}
