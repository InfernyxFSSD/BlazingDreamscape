using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class TacticalDirectionCardController : CardController
	{
		public TacticalDirectionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
			IEnumerator coroutine = base.GameController.SelectLocation(this.DecisionMaker, new LocationChoice[]
			{
				new LocationChoice(base.TurnTaker.Deck, null, false),
				new LocationChoice(base.TurnTaker.Trash, null, false)
			}, SelectionType.SearchLocation, storedResults, false, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (base.DidSelectLocation(storedResults))
			{
				Location selectedLocation = base.GetSelectedLocation(storedResults);
				coroutine = base.PlayCardFromLocation(selectedLocation, "EmpowermentOfFriendship", true, null, false, true, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			List<DiscardCardAction> storedResults2 = new List<DiscardCardAction>();
			IEnumerator coroutine2 = base.SelectAndDiscardCards(base.HeroTurnTakerController, new int?(1), false, null, storedResults2, false, null, null, null, SelectionType.DiscardCard, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			if (base.GetNumberOfCardsDiscarded(storedResults2) == 1)
			{
				IEnumerator coroutine3 = base.SelectHeroToPlayCard(base.HeroTurnTakerController, false, true, false, null, null, new LinqTurnTakerCriteria((TurnTaker h) => h != base.TurnTaker, () => "active heroes other than " + base.TurnTaker.Name), false, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
				}
			}
				yield break;
		}
	}
}