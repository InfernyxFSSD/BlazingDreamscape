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
            int powerNumeral2 = base.GetPowerNumeral(1, 1);
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
            ReduceDamageStatusEffect rdse2 = new ReduceDamageStatusEffect(powerNumeral2);
            rdse2.TargetCriteria.IsSpecificCard = selectedCard;
            rdse2.DamageTypeCriteria.AddType(DamageType.Projectile);
            rdse2.UntilStartOfNextTurn(base.TurnTaker);
            rdse2.UntilCardLeavesPlay(selectedCard);
            IEnumerator coroutine2 = base.AddStatusEffect(rdse2, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }
    }
}