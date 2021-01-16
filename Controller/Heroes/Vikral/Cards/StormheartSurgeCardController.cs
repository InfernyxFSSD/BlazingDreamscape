using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace SybithosInfernyx.Vikral
{
	public class StormheartSurgeCardController : MicrostormCardController
	{
		public StormheartSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
			int numOfMicrostormsInPlay = base.TurnTaker.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsMicrostorm(c)).Count();
			if (numberOfCardsDiscarded > 0 && numOfMicrostormsInPlay > 0)
			{
				int X = 0;
				while (X < numberOfCardsDiscarded)
                {
					List<SelectCardDecision> storedResults2 = new List<SelectCardDecision>();
					coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.AddTokens, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsMicrostorm(c), "microstorm", false, false, null, null, false), storedResults2, true, false, null, true, GetCardSource(null));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
					Card selectedCard = base.GetSelectedCard(storedResults2);
					Card card = base.TurnTaker.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == selectedCard.Identifier).FirstOrDefault();
					string torrentPoolIdentifier = $"{card.Identifier}TorrentPool";
					TokenPool torrentPool = card.FindTokenPool(torrentPoolIdentifier);
					coroutine = base.GameController.AddTokensToPool(torrentPool, 1, GetCardSource(null));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
					X++;
				}
			}
			else if (numberOfCardsDiscarded == 0)
            {
				coroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " did not discard any cards, so no tokens will be added.", Priority.High, base.GetCardSource(null), null, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			else if (numOfMicrostormsInPlay == 0)
            {
				coroutine = base.GameController.SendMessageAction("There are no Microstorms in play!", Priority.High, base.GetCardSource(null), null, true);
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