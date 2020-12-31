using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class ScanAndCounteractionCardController : CardController
	{
		public ScanAndCounteractionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		//Reveal the top card of a deck, then replace it or discard it. If you replaced it, one player may use a power now.
		public override IEnumerator Play()
		{
			List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
			IEnumerator coroutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location l) => true, storedResults, true, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Location deck = base.GetSelectedLocation(storedResults);
			if (deck != null)
			{
				List<Card> storedResultsCard = new List<Card>();
				coroutine = base.GameController.RevealCards(base.TurnTakerController, deck, 1, storedResultsCard, true, RevealedCardDisplay.None, null, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				Card card = storedResultsCard.FirstOrDefault<Card>();
				if (card != null)
				{
					YesNoDecision yesNo = new YesNoCardDecision(GameController, base.HeroTurnTakerController, SelectionType.DiscardCard, card);
					List<IDecision> decisionSources = new List<IDecision>
		{
			yesNo
		};
					IEnumerator coroutine2 = GameController.MakeDecisionAction(yesNo);
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(coroutine2);
					}
					else
					{
						GameController.ExhaustCoroutine(coroutine2);
					}
					if (DidPlayerAnswerYes(yesNo))
					{
						IEnumerator coroutine3 = GameController.DiscardCard(base.HeroTurnTakerController, card, decisionSources, null, null, GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine3);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine3);
						}
					}
					if (!DidPlayerAnswerYes(yesNo))
                    {
						IEnumerator coroutine5 = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine5);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine5);
						}
					}
					if (yesNo != null && yesNo.Completed && yesNo.Answer.HasValue)
					{
						decisionSources.Add(yesNo);
						if (!yesNo.Answer.Value)
						{
							IEnumerator coroutine4 = GameController.MoveCard(TurnTakerController, card, deck, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, decisionSources, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
							if (UseUnityCoroutines)
							{
								yield return GameController.StartCoroutine(coroutine4);
							}
							else
							{
								GameController.ExhaustCoroutine(coroutine4);
							}
						}
					}

					/*List<MoveCardDestination> list = new List<MoveCardDestination>
                    {
                        new MoveCardDestination(deck, false, false, false),
                        new MoveCardDestination(deck, true, false, false)
                    };
                    coroutine = base.GameController.SelectLocationAndMoveCard(this.DecisionMaker, card, list, false, true, null, null, null, false, false, null, false, base.GetCardSource(null));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}*/
				}
				coroutine = base.CleanupCardsAtLocations(new List<Location>
				{
					deck.OwnerTurnTaker.Revealed
				}, deck, false, true, false, false, false, true, storedResultsCard);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				storedResultsCard = null;
			}
			yield break;
		}
	}
}