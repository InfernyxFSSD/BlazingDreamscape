using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class SmolderingSalamanderCardController : CardController
    {
        public SmolderingSalamanderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => true, TargetType.SelectTarget, 2, DamageType.Fire, false, false, 1, 1, null, null);
            base.AddWhenDestroyedTrigger((DestroyCardAction dc) => base.DrawCard(base.TurnTaker.ToHero(), true, null, true), TriggerType.DrawCard);
        }
    }
}