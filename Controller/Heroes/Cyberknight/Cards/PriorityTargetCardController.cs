using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
    public class PriorityTargetCardController : CardController
    {
        //Play next to a target, when that target is destroyed everyone may draw, when any target is destroyed break this card

        public PriorityTargetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator chooseTarget = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText, "target", useCardsSuffix: false), storedResults, isPutIntoPlay, decisionSources);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseTarget);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseTarget);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy.Card == base.GetCardThisCardIsNextTo(true) && dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget, new Func<DestroyCardAction, IEnumerator>(this.EveryoneDrawsResponse), TriggerType.DrawCard, TriggerTiming.After);
            base.AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy.Card.IsTarget, new Func<DestroyCardAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator EveryoneDrawsResponse(DestroyCardAction dca)
        {
            IEnumerator everyoneDraws = base.EachPlayerDrawsACard(optional: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(everyoneDraws);
            }
            else
            {
                GameController.ExhaustCoroutine(everyoneDraws);
            }
            yield break;
        }
    }
}