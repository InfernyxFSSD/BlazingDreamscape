using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class LightningStormCardController : MicroWeatherCardController
    {
        //At the end of your turn, Whirlwind deals a target 1 Lightning damage. Then, up to X players may play a card, where X is the number of Weather Effects in play minus 2.

        public LightningStormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            //Do the end of turn damage and play check at the end of your turn
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EndOfTurnResponse(), new TriggerType[] { TriggerType.DealDamage, TriggerType.ReduceDamage });
        }

        private IEnumerator EndOfTurnResponse()
        {
            //First, Whirlwind deals a target damage
            IEnumerator hitTarget = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Lightning, 1, false, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(hitTarget);
            }
            else
            {
                GameController.ExhaustCoroutine(hitTarget);
            }
            var givePlayCount = this.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsWeatherEffect && c.IsInPlayAndHasGameText));
            List<TurnTaker> selectedHeroes = new List<TurnTaker>();
            int playsGiven = 0;
            foreach (Card weather in givePlayCount)
            {
                int quickCheck = this.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsWeatherEffect && c.IsInPlayAndHasGameText)).Count<Card>() - 2;
                //If there aren't enough weather effects in play, don't continue with giving out plays
                if (playsGiven >= quickCheck)
                {
                    break;
                }
                List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                IEnumerator givePlay = GameController.SelectHeroToPlayCard(DecisionMaker, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !selectedHeroes.Contains(tt)), storedResultsTurnTaker: storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(givePlay);
                }
                else
                {
                    GameController.ExhaustCoroutine(givePlay);
                }
                TurnTaker chosenHero = GetSelectedTurnTaker(storedResults);
                selectedHeroes.Add(chosenHero);
                playsGiven++;
                storedResults = null;
            }
            yield break;
        }
    }
}