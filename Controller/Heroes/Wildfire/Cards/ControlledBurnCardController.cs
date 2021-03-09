using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
	public class ControlledBurnCardController : CardController
	{
		//Wildfire either deals one target 4 fire damage, or deals up to three targets 2 fire damage each

		public ControlledBurnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<Function> list = new List<Function>();
			//Make the option and apply the effect for the given selection
			list.Add(new Function(DecisionMaker, "Deal 1 target 4 fire damage", SelectionType.DealDamage, () => GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, CharacterCard), 4, DamageType.Fire, 1, false, 1, cardSource: GetCardSource()), repeatDecisionText: "Deal 1 target 4 fire damage"));

			list.Add(new Function(DecisionMaker, "Deal up to 3 targets 2 fire damage each", SelectionType.DealDamage, () => GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Fire, 3, false, 1, cardSource: GetCardSource()), repeatDecisionText: "Deal up to 3 targets 2 fire damage each"));

			SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, false, cardSource: GetCardSource());
			IEnumerator howMuchBurn = GameController.SelectAndPerformFunction(selectFunction);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(howMuchBurn);
			}
			else
			{
				GameController.ExhaustCoroutine(howMuchBurn);
			}
			yield break;
		}
	}
}
