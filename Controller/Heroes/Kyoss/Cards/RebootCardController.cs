using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class RebootCardController : CardController
	{
		public RebootCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
			IEnumerator coroutine = base.SelectAndDiscardCards(this.DecisionMaker, null, false, new int?(0), storedResults, false, null, null, null, SelectionType.DiscardCard, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			int numberOfCardsDiscarded = base.GetNumberOfCardsDiscarded(storedResults);
			if (numberOfCardsDiscarded > 0)
			{
				coroutine = base.DrawCards(this.DecisionMaker, numberOfCardsDiscarded, false, false, null, true, null);
			}
			else
			{
				coroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " did not discard any cards, so no cards will be drawn.", Priority.High, base.GetCardSource(null), null, true);
			}
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (numberOfCardsDiscarded >= 4)
            {
				PreventPhaseActionStatusEffect ppase = new PreventPhaseActionStatusEffect();
				ppase.ToTurnPhaseCriteria.Phase = Phase.PlayCard;
				ppase.ToTurnPhaseCriteria.TurnTaker = this.TurnTaker;
				ppase.NumberOfUses = 1;
				ppase.CardSource = this.Card;
				coroutine = AddStatusEffect(ppase, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
		}
	}
}