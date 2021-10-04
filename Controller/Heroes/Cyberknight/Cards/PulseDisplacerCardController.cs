using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class PulseDisplacerCardController : CardController
	{
		//Environment can't play cards, hero targets are immune to damage from environment cards, destroy this card at the start of your turn

		public PulseDisplacerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.CannotPlayCards((TurnTakerController ttc) => ttc != null && ttc.TurnTaker == base.FindEnvironment(null).TurnTaker && base.GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, base.GetCardSource(null)), (Card c) => c.IsEnvironment && base.GameController.IsCardVisibleToCardSource(c, base.GetCardSource(null)));
			base.AddImmuneToDamageTrigger((DealDamageAction dda) => dda.Target.IsHero && dda.DamageSource.IsEnvironmentCard);
			base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
		}
	}
}