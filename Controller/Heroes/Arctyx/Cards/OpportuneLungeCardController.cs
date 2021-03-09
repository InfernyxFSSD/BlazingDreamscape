using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public class OpportuneLungeCardController : CardController
	{
		//Arctyx deals a target 2 Melee damage.
		//You may draw or play a card now.

		public OpportuneLungeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//Arctyx bops a target for 2 melee
			IEnumerator bop = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, new int?(1), false, new int?(1), cardSource: GetCardSource());
            List<Function> list = new List<Function>
            {
                new Function(this.DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(HeroTurnTaker)),
                new Function(this.DecisionMaker, "Play a card", SelectionType.PlayCard, () => SelectAndPlayCardFromHand(DecisionMaker))
            };
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, true, cardSource: GetCardSource());
			//Then you draw or play a card
			IEnumerator drawOrPlay = GameController.SelectAndPerformFunction(selectFunction);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(bop);
				yield return GameController.StartCoroutine(drawOrPlay);
			}
			else
			{
				GameController.ExhaustCoroutine(bop);
				GameController.ExhaustCoroutine(drawOrPlay);
			}
			yield break;
		}
	}
}
