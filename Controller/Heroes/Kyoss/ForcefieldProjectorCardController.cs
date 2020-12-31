using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
    public class ForcefieldProjectorCardController : CardController
    {
        public ForcefieldProjectorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 1);
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget, "hero target in play", false, false, null, null, false), storedResults, true, false, null, true, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card selectedCard = base.GetSelectedCard(storedResults);
            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(powerNumeral);
            rdse.TargetCriteria.IsSpecificCard = selectedCard;
            rdse.UntilStartOfNextTurn(base.TurnTaker);
            rdse.UntilCardLeavesPlay(selectedCard);
            coroutine = base.AddStatusEffect(rdse, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}