using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
	public class ElementalWrathCardController : CardController
	{
		public ElementalWrathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsAtLocation(this.TurnTaker.PlayArea, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false) && c.IsInPlayAndHasGameText, "elemental", true, false, null, null, false), null, false);
		}
		public override IEnumerator Play()
		{
			//Each elemental in your play area deals a target 1 Fire damage.
			IEnumerator elemPoke = base.GameController.SelectCardsAndDoAction(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("elemental", false, false) && c.Location.IsPlayAreaOf(this.TurnTaker), "elemental in your play area", false, false, null, null, false), SelectionType.CardToDealDamage, (Card c) => this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, c), 1, DamageType.Fire, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null)), null, false, null, null, true, null, GetCardSource(null), false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(elemPoke);
			}
			else
			{
				base.GameController.ExhaustCoroutine(elemPoke);
			}
			yield break;
		}
	}
}