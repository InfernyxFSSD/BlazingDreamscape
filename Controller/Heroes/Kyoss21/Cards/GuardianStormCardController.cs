using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
	public class GuardianStormCardController : CardController
	{
		//Reduce damage dealt to hero targets by 2.
		//At the start of your turn, destroy this card.

		public GuardianStormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Reduce damage to hero targets by 2
			AddReduceDamageTrigger((Card c) => c.IsHero && c.IsTarget, 2);
			//Destroy this card at the start of your turn
			AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, new Func<PhaseChangeAction, IEnumerator>(DestroyThisCardResponse), TriggerType.DestroySelf);
		}
	}
}
