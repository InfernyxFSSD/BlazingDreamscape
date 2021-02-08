using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace BlazingDreamscape.Wildfire
{
    public class BonfireCircletCardController : CardController
    {
        public BonfireCircletCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			base.SpecialStringMaker.ShowNumberOfCardsAtLocation(this.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), null, false);
			base.SpecialStringMaker.ShowNumberOfCardsAtLocation(this.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), null, false);
		}

        public override IEnumerator UsePower(int index = 0)
		{
			//Discard two cards. If you did, search your deck or trash for an Elemental and put it into play. If you searched your deck, shuffle it afterwards.
			List<DiscardCardAction> discarded = new List<DiscardCardAction>();
			IEnumerator discardCards = base.SelectAndDiscardCards(this.DecisionMaker, new int?(2), false, null, discarded, false, null, null, null, SelectionType.DiscardCard, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(discardCards);
			}
			else
			{
				base.GameController.ExhaustCoroutine(discardCards);
			}
			if (base.GetNumberOfCardsDiscarded(discarded) == 2)
			{
				IEnumerator searchElemental = base.SearchForCards(this.DecisionMaker, true, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), true, false, false, false, null, false, true, null);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(searchElemental);
				}
				else
				{
					base.GameController.ExhaustCoroutine(searchElemental);
				}
			}
			yield break;
		}
	}
}
