using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class PsionicTorrentCardController : CardController
    {
        //At the end of your turn, Whirlwind deals a target 1 psychic damage. Then, up to X players may use a power, where X is the number of microstorms in play minus 2.

        public PsionicTorrentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many Microstorms are in play?
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm", false, singular: "microstorm", plural: "microstorms"));
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
            //See if (microstorms in play minus 2) is greater than 0
            int usePowerCount = FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm") && c.IsInPlayAndHasGameText)).Count() - 2;
            if (usePowerCount > 0)
            {
                //If enough microstarms are in play, start handing out powers
                List<TurnTaker> selectedHeroes = new List<TurnTaker>();
                for (int i = 0; i < usePowerCount; i++)
                {
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
                    storedResults = null;
                }
            }
            yield break;
        }
    }
}