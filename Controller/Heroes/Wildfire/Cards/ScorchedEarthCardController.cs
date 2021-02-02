using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
	public class ScorchedEarthCardController : CardController
	{
		public ScorchedEarthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			IEnumerator burn = base.GameController.DealDamage(this.DecisionMaker, base.CharacterCard, (Card c) => c.IsTarget && c.IsInPlayAndHasGameText && !c.IsHero, 2, DamageType.Fire, false, false, null, null, null, false, null, null, false, false, GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(burn);
			}
			else
			{
				base.GameController.ExhaustCoroutine(burn);
			}
			yield break;
		}
	}
}