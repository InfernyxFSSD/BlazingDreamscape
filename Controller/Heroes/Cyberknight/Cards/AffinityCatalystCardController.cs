using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class AffinityCatalystCardController : CardController
	{
		//When Cyberknight is dealt lightning, she may draw or play a card

		public AffinityCatalystCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard && dda.DamageType == DamageType.Lightning && !dda.IsPretend && dda.DidDealDamage, new Func<DealDamageAction, IEnumerator>(this.DrawOrPlayResponse), new TriggerType[] { TriggerType.DealDamage, TriggerType.DrawCard, TriggerType.PlayCard }, TriggerTiming.After);
		}

		private IEnumerator DrawOrPlayResponse(DealDamageAction _)
        {
			IEnumerator drawOrPlay = base.DrawACardOrPlayACard(this.DecisionMaker, true);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(drawOrPlay);
			}
			else
			{
				GameController.ExhaustCoroutine(drawOrPlay);
			}
			yield break;
		}
	}
}