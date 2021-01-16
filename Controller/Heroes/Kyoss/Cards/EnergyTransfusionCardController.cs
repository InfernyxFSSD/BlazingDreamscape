using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class EnergyTransfusionCardController : CardController
	{
		public EnergyTransfusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
        {
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
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.SourceCriteria.IsSpecificCard = selectedCard;
            idse.UntilStartOfNextTurn(base.TurnTaker);
            idse.UntilCardLeavesPlay(selectedCard);
            coroutine = base.AddStatusEffect(idse, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IncreaseDamageStatusEffect idse2 = new IncreaseDamageStatusEffect(1);
            idse2.SourceCriteria.IsSpecificCard = selectedCard;
            idse2.UntilCardLeavesPlay(selectedCard);
            idse2.NumberOfUses = 1;
            IEnumerator coroutine2 = base.AddStatusEffect(idse2, true);
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