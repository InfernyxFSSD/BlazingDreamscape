using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    //At the end of your turn, Whirlwind deals a target 1 Toxic damage. Until the start of your next turn, reduce damage dealt by a target dealt damage this way by X, where X is the number of Weather Effects in play minus 2.

    public class ToxicCloudCardController : MicroWeatherCardController
    {
        public ToxicCloudCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of your turn, deal damage and maybe reduce damage
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EndOfTurnResponse(), new TriggerType[] { TriggerType.DealDamage, TriggerType.ReduceDamage });
        }

        private IEnumerator EndOfTurnResponse()
        {
            //First, Whirlwind deals a target damage
            IEnumerator hitTarget = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Toxic, 1, false, 1, addStatusEffect: new Func<DealDamageAction, IEnumerator>(ReduceDamageResponse), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(hitTarget);
            }
            else
            {
                GameController.ExhaustCoroutine(hitTarget);
            }
            yield break;
        }

        private IEnumerator ReduceDamageResponse(DealDamageAction d)
        {
            //See if (weather effects in play minus 2) is greater than 0
            int reduceAmount = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsWeatherEffect && c.IsInPlayAndHasGameText)).Count<Card>() - 2;
            if (d.DidDealDamage && reduceAmount > 0)
            {
                //If Whirlwind dealt a target damage and there are enough weather effects in play to matter
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(reduceAmount);
                rdse.SourceCriteria.IsSpecificCard = d.Target;
                rdse.UntilStartOfNextTurn(this.TurnTaker);
                rdse.UntilCardLeavesPlay(d.Target);
                IEnumerator reduceDamage = AddStatusEffect(rdse);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(reduceDamage);
                }
                else
                {
                    GameController.ExhaustCoroutine(reduceDamage);
                }
            }
            yield break;
        }
    }
}