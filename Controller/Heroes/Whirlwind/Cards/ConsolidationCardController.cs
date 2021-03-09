using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
	public class ConsolidationCardController : CardController
	{
		//Power: Destroy a microstorm. If you do, select a target. Increase damage dealt to that target by 1 until the start of your next turn.

		public ConsolidationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeral = GetPowerNumeral(0, 1);
			List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
			//Destroys a microstorm
			IEnumerator destroyMicrostorms = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm"), 1, false, 1, storedResultsAction: storedResults, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(destroyMicrostorms);
			}
			else
			{
				GameController.ExhaustCoroutine(destroyMicrostorms);
			}
			if (DidDestroyCards(storedResults, 1))
			{
				//If a microstorm was destroyed, select a target
				List<SelectCardDecision> storedResults2 = new List<SelectCardDecision>();
				IEnumerator selectTarget = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText, "target"), storedResults2, false, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(selectTarget);
				}
				else
				{
					GameController.ExhaustCoroutine(selectTarget);
				}
				Card selectedCard = GetSelectedCard(storedResults2);
				if (selectedCard != null)
				{
					//Increase damage dealt to that target
					IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(powerNumeral);
					idse.TargetCriteria.IsSpecificCard = selectedCard;
					idse.UntilStartOfNextTurn(TurnTaker);
					idse.UntilCardLeavesPlay(selectedCard);
					IEnumerator applyStatus = AddStatusEffect(idse);
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(applyStatus);
					}
					else
					{
						GameController.ExhaustCoroutine(applyStatus);
					}
				}
			}
			yield break;
		}
	}
}