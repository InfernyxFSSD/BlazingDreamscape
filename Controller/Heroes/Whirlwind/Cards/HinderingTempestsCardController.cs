using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class HinderingTempestsCardController : CardController
    {
        //Destroy two Microstorms. If you do, skip either the next villain play phase or the next environment play phase.

        public HinderingTempestsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Destroy two Microstorms.
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator destroyMicrostorms = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm"), 2, false, 2, storedResultsAction: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyMicrostorms);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyMicrostorms);
            }
            if(GetNumberOfCardsDestroyed(storedResults) == 2)
            {
                //If you did destroy two microstorms...
                //Set up the status effects to skip either the next villain phase or the next environment phase
                var vilTurnTaker = GameController.AllTurnTakers.Where((TurnTaker tt) => tt.IsVillain || tt.IsVillainTeam);
                var vilToSkip = vilTurnTaker.FirstOrDefault();
                PreventPhaseActionStatusEffect ppase = new PreventPhaseActionStatusEffect();
                ppase.ToTurnPhaseCriteria.Phase = new Phase?(Phase.PlayCard);
                ppase.ToTurnPhaseCriteria.TurnTaker = vilToSkip;
                ppase.UntilEndOfPhase(vilToSkip, Phase.PlayCard);
                ppase.NumberOfUses = 1;

                PreventPhaseActionStatusEffect ppase2 = new PreventPhaseActionStatusEffect();
                ppase2.ToTurnPhaseCriteria.Phase = new Phase?(Phase.PlayCard);
                ppase2.ToTurnPhaseCriteria.TurnTaker = FindEnvironment(null).TurnTaker;
                ppase2.UntilEndOfPhase(FindEnvironment(null).TurnTaker, Phase.PlayCard);
                ppase2.NumberOfUses = 1;

                List<Function> list = new List<Function>();
                List<SelectFunctionDecision> results = new List<SelectFunctionDecision>();
                //Choose between villain or environment to skip the next play phase of
                IEnumerable<Card> choices = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndNotUnderCard));
                list.Add(new Function(DecisionMaker, "Skip the next villain play phase", SelectionType.None, () => AddStatusEffect(ppase), repeatDecisionText: "Skip the next villain play phase"));
                list.Add(new Function(DecisionMaker, "Skip the next environment play phase", SelectionType.None, () => AddStatusEffect(ppase2), repeatDecisionText: "Skip the next environment play phase"));
                SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, false, cardSource: GetCardSource());
                IEnumerator makeDecision = GameController.SelectAndPerformFunction(selectFunction, results);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(makeDecision);
                }
                else
                {
                    GameController.ExhaustCoroutine(makeDecision);
                }
            }
            else
            {
                //If not enough microstorms were destroyed...
                IEnumerator failState = GameController.SendMessageAction("Not enough microstorms were destroyed!", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(failState);
                }
                else
                {
                    GameController.ExhaustCoroutine(failState);
                }
            }
            yield break;
        }
    }
}