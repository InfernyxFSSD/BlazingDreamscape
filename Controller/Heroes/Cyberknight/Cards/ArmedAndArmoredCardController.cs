using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class ArmedAndArmoredCardController : CardController
	{
		//Draw a card, search your deck for an arsenal and an armor, put one into play and the other in your hand, shuffle afterwards

		public ArmedAndArmoredCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

        public override IEnumerator Play()
        {
			IEnumerator drawCard = base.DrawCard();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(drawCard);
            }
            else
            {
                GameController.ExhaustCoroutine(drawCard);
            }
            List<SelectCardDecision> foundArsenal = new List<SelectCardDecision>();
            IEnumerator findArsenal = base.SearchForCards(base.HeroTurnTakerController, true, false, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("arsenal"), "arsenal"), false, false, false, storedResults: foundArsenal);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findArsenal);
            }
            else
            {
                GameController.ExhaustCoroutine(findArsenal);
            }
            List<SelectCardDecision> foundArmor = new List<SelectCardDecision>();
            IEnumerator findArmor = base.SearchForCards(base.HeroTurnTakerController, true, false, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("armor"), "armor"), false, false, false, storedResults: foundArmor);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findArmor);
            }
            else
            {
                GameController.ExhaustCoroutine(findArmor);
            }
            List<Card> foundCards = new List<Card>();
            if (foundArsenal != null)
            {
                foreach(SelectCardDecision scd in foundArsenal)
                {
                    foundCards.Add(scd.SelectedCard);
                }
            }
            if (foundArmor != null)
            {
                foreach(SelectCardDecision scd in foundArmor)
                {
                    foundCards.Add(scd.SelectedCard);
                }
            }
            if(foundCards.Any())
            {
                if (foundCards.Count() == 1)
                {
                    IEnumerator putInPlay = base.GameController.PlayCard(this.DecisionMaker, foundCards.FirstOrDefault(), isPutIntoPlay: true, cardSource: base.GetCardSource(null));
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(putInPlay);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(putInPlay);
                    }

                }
                else
                {
                    IEnumerator putIntoPlay = base.GameController.SelectAndPlayCard(DecisionMaker, foundCards, isPutIntoPlay: true, cardSource: base.GetCardSource(null));
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(putIntoPlay);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(putIntoPlay);
                    }
                    Card otherCard = foundCards.FirstOrDefault();
                    if (otherCard != null)
                    {
                        IEnumerator putIntoHand = GameController.MoveCard(DecisionMaker, otherCard, FindCardController(otherCard).HeroTurnTaker.Hand, showMessage: true, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(putIntoHand);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(putIntoHand);
                        }
                    }
                }
            }
            IEnumerator shuffleDeck = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shuffleDeck);
            }
            else
            {
                GameController.ExhaustCoroutine(shuffleDeck);
            }
            yield break;
        }
    }
}