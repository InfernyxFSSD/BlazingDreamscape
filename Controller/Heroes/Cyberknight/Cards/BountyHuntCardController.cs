using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class BountyHuntCardController : CardController
	{
		//Whenever a villain target is reduced to 2 HP or less, destroy it, then you may draw a card

		public BountyHuntCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>(delegate (DealDamageAction dda)
			{
				if (dda.DidDealDamage && dda.Target.IsVillain)
				{
					int? hitPoints = dda.Target.HitPoints;
					int checkHP = 2;
					return hitPoints.GetValueOrDefault() <= checkHP & hitPoints != null;
				}
				return false;
			}, new Func<DealDamageAction, IEnumerator>(this.DestroyIfTwoHPResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
		}

		private IEnumerator DestroyIfTwoHPResponse(DealDamageAction dda)
        {
			int? hp = dda.Target.HitPoints;
			int checkHP = 2;
			if (hp.GetValueOrDefault() <= checkHP & hp != null)
            {
				IEnumerator breakTarget = base.GameController.DestroyCard(this.DecisionMaker, dda.Target, cardSource: base.GetCardSource(null));
				IEnumerator sendMessage = base.GameController.SendMessageAction(string.Concat(new string[] { dda.Target.Title, " was reduced to 2 HP or less, so Bounty Hunt destroys ", dda.Target.Title, "!" }), Priority.Low, base.GetCardSource(null));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(breakTarget);
					yield return GameController.StartCoroutine(sendMessage);
				}
				else
				{
					GameController.ExhaustCoroutine(breakTarget);
					GameController.ExhaustCoroutine(sendMessage);
				}
				IEnumerator drawCard = base.DrawCard();
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(drawCard);
				}
				else
				{
					GameController.ExhaustCoroutine(drawCard);
				}
			}
			yield break;
        }
	}
}