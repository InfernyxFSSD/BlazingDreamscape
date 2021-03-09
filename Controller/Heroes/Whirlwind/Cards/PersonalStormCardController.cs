using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
	public class PersonalStormCardController : CardController
	{
		//When Whirlwind would be dealt 3 or more damage at once, you may prevent that damage. If you do, destroy a microstorm or this card.
		//When this card is destroyed, Whirlwind deals up to 3 targets 1 projectile damage each.

		public PersonalStormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//How many Microstorms are in play?
			SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm", false, singular: "microstorm", plural: "microstorms"));
		}

		public override void AddTriggers()
		{
			//When Whirlwind would be dealt 3+ damage at once, maybe do the thing
			AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == CharacterCard && !dd.IsPretend && dd.Amount >= 3 && dd.DamageSource.IsInPlayAndHasGameText && !IsBeingDestroyed, new Func<DealDamageAction, IEnumerator>(PreventDamageResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.DestroyCard }, TriggerTiming.Before);
			//When this card is destroyed, Whirlwind deals damage
			AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(OnDestroyResponse), new TriggerType[] { TriggerType.DealDamage });
		}

		private IEnumerator OnDestroyResponse(DestroyCardAction _)
        {
			//Whirlwind deals damage
			IEnumerator flingRocks = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Projectile, 3, false, 0, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(flingRocks);
			}
			else
			{
				GameController.ExhaustCoroutine(flingRocks);
			}
		}

		public Guid? PerformDestroyForDamage { get; set; }

		private IEnumerator PreventDamageResponse(DealDamageAction dd)
        {
			//Ask if you want to prevent that damage
			List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
			IEnumerator yesNo = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.PreventDamage, Card, storedResults: storedResults, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(yesNo);
			}
			else
			{
				GameController.ExhaustCoroutine(yesNo);
			}
			if(DidPlayerAnswerYes(storedResults))
            {
				//If you did choose to prevent that damage, prevent it
				IEnumerator preventDamage = CancelAction(dd, isPreventEffect: true);
				//Destroy either a microstorm or this card
				IEnumerator destroyCard = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => (c.DoKeywordsContain("microstorm") || c == Card) && c.IsInPlayAndHasGameText, "a microstorm or this card"), false, responsibleCard: Card, cardSource: GetCardSource(null));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(preventDamage);
					yield return GameController.StartCoroutine(destroyCard);
				}
				else
				{
					GameController.ExhaustCoroutine(preventDamage);
					GameController.ExhaustCoroutine(destroyCard);
				}
			}
			yield break;
		}
	}
}