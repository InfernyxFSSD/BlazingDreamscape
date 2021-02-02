using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace BlazingDreamscape.Wildfire
{
    public class WildfireTurnTakerController : HeroTurnTakerController
    {
        public WildfireTurnTakerController(TurnTaker turnTaker, GameController gameController)
            : base(turnTaker, gameController)
        {
        }
    }
}
