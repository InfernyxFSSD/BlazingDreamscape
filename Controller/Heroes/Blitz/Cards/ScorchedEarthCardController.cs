using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Blitz
{
	public class ScorchedEarthCardController : CardController
	{
		public ScorchedEarthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			IEnumerator burn = base.GameController.DealDamage(this.DecisionMaker, base.CharacterCard, (Card c) => c.IsTarget && c.IsInPlayAndHasGameText, 2, DamageType.Fire, false, false, null, null, null, false, null, null, false, false, GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(burn);
			}
			else
			{
				base.GameController.ExhaustCoroutine(burn);
			}
			int conduitCount = base.FindCardsWhere((Card c) => c.DoKeywordsContain("conduit", false, false) && c.IsInPlayAndHasGameText).Count<Card>();
			if (conduitCount > 2)
			{
				IEnumerator burn2 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 4, DamageType.Fire, 1, false, 1, false, false, false, (Card c) => c.IsVillainTarget && c.IsInPlayAndHasGameText, null, null, null, null, false, null, null, false, null, GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(burn2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(burn2);
				}
			}
			yield break;
		}
	}
}