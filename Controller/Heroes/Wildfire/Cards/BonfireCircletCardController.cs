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
        }

        public override IEnumerator UsePower(int index = 0)
		{
            //Search your deck or trash for an Elemental and put it into play. If you searched your deck, shuffle it afterwards.
            IEnumerator searchElemental = base.SearchForCards(this.DecisionMaker, true, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), true, false, false, false, null, false, true, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(searchElemental);
			}
			else
			{
				base.GameController.ExhaustCoroutine(searchElemental);
			}
			yield break;
		}
	}
}
