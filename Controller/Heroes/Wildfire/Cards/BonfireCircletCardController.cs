using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace BlazingDreamscape.Wildfire
{
    public class BonfireCircletCardController : CardController
	{
		//Power: Discard two cards from your hand. If you did, search your deck or trash for an elemental and put it into play. If you searched your deck, shuffle it afterwards.

		public BonfireCircletCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			//How many elementals in your deck?
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental"), "elemental"));
			//How many elementals in your trash?
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental"), "elemental"));
		}

        public override IEnumerator UsePower(int index = 0)
		{
			//Discard two cards. 
			List<DiscardCardAction> discarded = new List<DiscardCardAction>();
			IEnumerator discardCards = SelectAndDiscardCards(DecisionMaker, new int?(2), storedResults: discarded);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(discardCards);
			}
			else
			{
				GameController.ExhaustCoroutine(discardCards);
			}
			if (GetNumberOfCardsDiscarded(discarded) == 2)
			{
				//If you did discard two, search for an elemental
				IEnumerator searchElemental = SearchForCards(DecisionMaker, true, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental"), "elemental"), true, false, false);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(searchElemental);
				}
				else
				{
					GameController.ExhaustCoroutine(searchElemental);
				}
			}
			yield break;
		}
	}
}
