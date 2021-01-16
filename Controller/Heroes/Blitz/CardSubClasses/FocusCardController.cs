using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Blitz
{
	public abstract class FocusCardController : CardController
	{
		public FocusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.GameController.MoveCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("focus", false, false) && c.IsInPlayAndHasGameText && c != base.Card && c.Location.IsPlayAreaOf(this.TurnTaker), "focus", true, false, null, null, false), (Card c) => base.HeroTurnTaker.Hand, false, SelectionType.MoveCardToHand, null, false, null, true, false, true, null, null, false, MoveCardDisplay.None, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}
	}
}
