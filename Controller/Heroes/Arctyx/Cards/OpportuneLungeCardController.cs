using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public class OpportuneLungeCardController : CardController
	{
		public OpportuneLungeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator bop = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
            List<Function> list = new List<Function>
            {
                new Function(this.DecisionMaker, "Draw a card", SelectionType.DrawCard, () => base.DrawCard(base.HeroTurnTaker, false, null, true), null, null, null),
                new Function(this.DecisionMaker, "Play a card", SelectionType.PlayCard, () => base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, null, false, false, true, null), null, null, null)
            };
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, list, true, null, null, null, base.GetCardSource(null));
			IEnumerator drawOrPlay = base.GameController.SelectAndPerformFunction(selectFunction, null, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(bop);
				yield return base.GameController.StartCoroutine(drawOrPlay);
			}
			else
			{
				base.GameController.ExhaustCoroutine(bop);
				base.GameController.ExhaustCoroutine(drawOrPlay);
			}
			yield break;
		}
	}
}
