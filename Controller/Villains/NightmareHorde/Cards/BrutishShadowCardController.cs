using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class BrutishShadowCardController : FragmentCardController
    {
        public BrutishShadowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected const string HasBeenHitThisTurn = "HasBeenHitThisTurn";


        public override void AddTriggers()
        {
            //End of villain turn, hit highest hero for 2 irreducible melee
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHeroCharacterCard, TargetType.HighestHP, 2, DamageType.Melee, isIrreducible: true);
            //The first time each turn this card is hit, NightmareHorde recovers HP
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.Target == base.Card && !base.HasBeenSetToTrueThisTurn(HasBeenHitThisTurn), this.NightmareHordeHealsResponse, new TriggerType[] { TriggerType.GainHP }, TriggerTiming.After);
        }

        private IEnumerator NightmareHordeHealsResponse(DealDamageAction dd)
        {
            base.SetCardPropertyToTrueIfRealAction(HasBeenHitThisTurn);
            IEnumerator healIt = base.GameController.GainHP(base.CharacterCard, 2, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(healIt);
            }
            else
            {
                GameController.ExhaustCoroutine(healIt);
            }
            yield break;
        }
    }
}