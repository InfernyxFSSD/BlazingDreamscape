using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class PsionicTorrentCardController : MicroWeatherCardController
    {
        //At the end of your turn, Whirlwind deals a target 1 psychic damage. Then, up to X players may use a power, where X is the number of Weather Effects in play minus 2.

        public PsionicTorrentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Do the end of turn damage and power check at the end of your turn
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EndOfTurnResponse(), new TriggerType[] { TriggerType.DealDamage, TriggerType.ReduceDamage });
        }

        private IEnumerator EndOfTurnResponse()
        {
            //First, Whirlwind deals a target damage
            IEnumerator hitTarget = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Psychic, 1, false, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(hitTarget);
            }
            else
            {
                GameController.ExhaustCoroutine(hitTarget);
            }
            var givePowerCount = this.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsWeatherEffect && c.IsInPlayAndHasGameText));
            List<TurnTaker> selectedHeroes = new List<TurnTaker>();
            int powersGiven = 0;
            foreach (Card weather in givePowerCount)
            {
                int quickCheck = this.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsWeatherEffect && c.IsInPlayAndHasGameText)).Count<Card>() - 2;
                //If there aren't enough weather effects in play, don't continue with giving out powers
                if (powersGiven >= quickCheck)
                {
                    break;
                }
                List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                IEnumerator usePower = GameController.SelectHeroToUsePower(DecisionMaker, storedResultsDecision: storedResults, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !selectedHeroes.Contains(tt)), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(usePower);
                }
                else
                {
                    GameController.ExhaustCoroutine(usePower);
                }
                TurnTaker chosenHero = GetSelectedTurnTaker(storedResults);
                selectedHeroes.Add(chosenHero);
                powersGiven++;
                storedResults = null;
            }
            yield break;
        }
    }
}