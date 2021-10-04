using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    //At the end of your turn, Whirlwind deals a target 1 Radiant damage. Then, up to X targets deal themselves 1 radiant damage each, where X is the number of Weather Effects in play minus 1.

    public class VigilanteGalesCardController : MicroWeatherCardController
    {
        public VigilanteGalesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
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
            var hitSelfCount = this.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsWeatherEffect && c.IsInPlayAndHasGameText));
            List<Card> selectedTargets = new List<Card>();
            int targetsChosen = 0;
            foreach (Card weather in hitSelfCount)
            {
                int quickCheck = this.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsWeatherEffect && c.IsInPlayAndHasGameText)).Count<Card>() - 1;
                //If there aren't enough weather effects in play, don't continue with having targets hit themselves
                if (targetsChosen >= quickCheck)
                {
                    break;
                }
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                IEnumerator hitSelf = this.GameController.SelectTargetsToDealDamageToSelf(this.DecisionMaker, 1, DamageType.Radiant, new int?(1), false, new int?(0), additionalCriteria: (Card c) => !selectedTargets.Contains(c), storedResultsDecisions: storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(hitSelf);
                }
                else
                {
                    GameController.ExhaustCoroutine(hitSelf);
                }
                Card chosenCard = this.GetSelectedCard(storedResults);
                selectedTargets.Add(chosenCard);
                targetsChosen++;
                storedResults = null;
            }
            yield break;
        }
    }
}