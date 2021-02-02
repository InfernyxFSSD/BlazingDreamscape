using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
	public class ControlledBurnCardController : CardController
	{
		public ControlledBurnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			List<Function> list = new List<Function>();

			string option1Message = "Deal 1 target 4 fire damage";
			IEnumerator option1Effect = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 4, DamageType.Fire, 1, false, 1, cardSource: base.GetCardSource());
			list.Add(new Function(this.DecisionMaker, option1Message, SelectionType.DealDamage, () => option1Effect, repeatDecisionText: option1Message));

			string option2Message = "Deal up to 3 targets 2 fire damage each";
			IEnumerator option2Effect = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Fire, 3, false, 1, cardSource: base.GetCardSource());
			list.Add(new Function(this.DecisionMaker, option2Message, SelectionType.DealDamage, () => option2Effect, repeatDecisionText: option2Message));

			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list, false, cardSource: base.GetCardSource());
			IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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
