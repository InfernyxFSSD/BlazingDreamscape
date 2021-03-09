using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    //At the end of your turn, Whirlwind deals a target 1 Radiant damage. Then, up to X targets deal themselves 1 radiant damage each, where X is the number of Microstorms in play minus 1.

    public class VigilanteGalesCardController : CardController
    {
        public VigilanteGalesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many Microstorms are in play?
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm", false, singular: "microstorm", plural: "microstorms"));
        }

        public override void AddTriggers()
        {
            //At the end of your turn, deal damage, then make others deal themselves damage
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EndOfTurnResponse(), new TriggerType[] { TriggerType.DealDamage, TriggerType.ReduceDamage });
        }

        private IEnumerator EndOfTurnResponse()
        {
            //First, Whirlwind deals a target damage
            IEnumerator hitTarget = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Radiant, 1, false, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(hitTarget);
            }
            else
            {
                GameController.ExhaustCoroutine(hitTarget);
            }
            //See if (microstorms in play minus 1) is greater than 0
            int hitSelfCount = FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm", false, false) && c.IsInPlayAndHasGameText)).Count() - 1;
            if (hitSelfCount > 0)
            {
                //If so, select targets to hit themselves
                IEnumerator damageSelf = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 1, DamageType.Radiant, hitSelfCount, false, 0, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(damageSelf);
                }
                else
                {
                    GameController.ExhaustCoroutine(damageSelf);
                }
            }
            yield break;
        }
    }
}