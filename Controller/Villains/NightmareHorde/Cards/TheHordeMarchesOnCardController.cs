using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class TheHordeMarchesOnCardController : CardController
    {
        public TheHordeMarchesOnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //Fragments are indestructible
            if (card.DoKeywordsContain("fragment"))
            {
                return true;
            }
            return base.AskIfCardIsIndestructible(card);
        }

        public override void AddTriggers()
        {
            //End of each turn, if there are (H) - 1 fragments in play at or below 0 HP, destroy this card
            base.AddEndOfTurnTrigger((TurnTaker tt) => true, new Func<PhaseChangeAction, IEnumerator>(this.CheckLowHealthFragments), new TriggerType[] { TriggerType.Other });
            base.AddAfterLeavesPlayAction((GameAction ga) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(false, GetCardSource()), TriggerType.DestroyCard);
        }

        private IEnumerator CheckLowHealthFragments(PhaseChangeAction p)
        {
            if (base.FindCardsWhere((Card c) => c.DoKeywordsContain("fragment") && c.IsInPlayAndHasGameText && c.HitPoints <= 0).Count<Card>() >= base.H - 1)
            {
                IEnumerator breakSelf = base.GameController.DestroyCard(this.DecisionMaker, base.Card);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(breakSelf);
                }
                else
                {
                    GameController.ExhaustCoroutine(breakSelf);
                }
                yield break;
            }
        }
    }
}