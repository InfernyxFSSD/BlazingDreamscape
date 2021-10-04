using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class BurstEnergizerCardController : CardController
	{
		//When Cyberknight would be dealt damage by a target other than herself, she may use a power, and if that power deals damage, increase it by 3. If she uses a power this way, destroy this card.

		public BurstEnergizerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target == base.CharacterCard && !dda.IsPretend && dda.Amount > 0 && dda.DamageSource.IsInPlayAndHasGameText && !base.IsBeingDestroyed && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget, new Func<DealDamageAction, IEnumerator>(this.UsePowerResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.UsePower }, TriggerTiming.Before);
		}

		public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{
			return new CustomDecisionText("Use a power?", "Use a power?", "Vote for whether a power should be used", "use a power?");
		}

		private IEnumerator UsePowerResponse(DealDamageAction dda)
		{
			List<YesNoCardDecision> yesOrNo = new List<YesNoCardDecision>();
			IEnumerator makeDecision = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.Custom, base.Card, storedResults: yesOrNo, cardSource: base.GetCardSource(null));
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(makeDecision);
			}
			else
			{
				GameController.ExhaustCoroutine(makeDecision);
			}
			if (base.DidPlayerAnswerYes(yesOrNo))
            {
				IEnumerator useAPower = this.UsePowerAndModifyIfDealsDamage(this.DecisionMaker, (Func<DealDamageAction, bool> c) => base.AddIncreaseDamageTrigger(c, 3, null, new TriggerPriority?(TriggerPriority.Low)), new int?(3));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(useAPower);
				}
				else
				{
					GameController.ExhaustCoroutine(useAPower);
				}
				IEnumerator breakSelf = base.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: base.GetCardSource(null));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(breakSelf);
				}
				else
				{
					GameController.ExhaustCoroutine(breakSelf);
				}
			}
			yield break;
		}

		private IEnumerator UsePowerAndModifyIfDealsDamage(HeroTurnTakerController decisionMaker, Func<Func<DealDamageAction, bool>, ITrigger> addDealDamageTrigger, int? increaseDamageAmount = null, LinqTurnTakerCriteria additionalCriteria = null)
        {
			base.AddToTemporaryTriggerList(base.AddTrigger<MakeDecisionAction>((MakeDecisionAction mda) => mda.Decision is UsePowerDecision && mda.Decision.CardSource.CardController == this, (MakeDecisionAction mda2) => this.ModifyDamageForTurnTakerResponse(mda2, addDealDamageTrigger, increaseDamageAmount), new TriggerType[] { TriggerType.Hidden }, TriggerTiming.After));
			IEnumerator usePower = base.GameController.SelectAndUsePower(this.DecisionMaker, optional: false, cardSource: GetCardSource(null));
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(usePower);
			}
			else
			{
				GameController.ExhaustCoroutine(usePower);
			}
			base.RemoveTemporaryTriggers();
			base.RemoveTemporaryVariables();
			yield break;
		}

		private IEnumerator ModifyDamageForTurnTakerResponse(MakeDecisionAction mda, Func<Func<DealDamageAction, bool>, ITrigger> addDealDamageTrigger, int? increaseDamageAmount = null)
        {
			base.RemoveTemporaryTriggers();
			base.AddToTemporaryTriggerList(base.AddTrigger<UsePowerAction>((UsePowerAction upa) => upa.Power.TurnTakerController == this.DecisionMaker, (UsePowerAction upa) => this.ModifyDamageFromPowerResponse(upa, addDealDamageTrigger, increaseDamageAmount), TriggerType.Hidden, TriggerTiming.Before, ActionDescription.Unspecified));
			yield return null;
			yield break;
        }

		private IEnumerator ModifyDamageFromPowerResponse(UsePowerAction upa, Func<Func<DealDamageAction, bool>, ITrigger> addDealDamageTrigger, int? increaseDamageAmount = null)
        {
			RemoveTemporaryTriggers();
			CardController powerCardController = (CardController)AddTemporaryVariable("SelectedPowerCardControllerForDealDamageModification", upa.Power.IsContributionFromCardSource ? upa.Power.CardSource.CardController : upa.Power.CardController);
			Func<DealDamageAction, bool> arg = (DealDamageAction dda) => dda.CardSource.PowerSource != null && dda.CardSource.PowerSource == upa.Power && (dda.CardSource.CardController == powerCardController || dda.CardSource.AssociatedCardSources.Any((CardSource cs) => cs.CardController == powerCardController)) && !dda.DamageModifiers.Select((ModifyDealDamageAction mdda) => mdda.CardSource.CardController).Contains(this) && !powerCardController.Card.IsBeingDestroyed;
			AddToTemporaryTriggerList(addDealDamageTrigger(arg));
			AddToTemporaryTriggerList(AddTrigger((AddStatusEffectAction asea) => asea.StatusEffect.DoesDealDamage && asea.CardSource.PowerSource != null && asea.CardSource.PowerSource == upa.Power, (AddStatusEffectAction asea) => IncreaseDamageFromEffectResponse(asea, 3, upa.Power), TriggerType.Hidden, TriggerTiming.Before));
			yield return null;
		}

		private IEnumerator IncreaseDamageFromEffectResponse(AddStatusEffectAction asea, int increaseAmount, Power power)
        {
			IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(increaseAmount);
			idse.StatusEffectCriteria.Effect = asea.StatusEffect;
			if (power != null && power.CardController != null)
			{
				idse.StatusEffectCriteria.CardWithPower = power.CardController.Card;
			}
			IEnumerator applyStatus = base.AddStatusEffect(idse);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(applyStatus);
			}
			else
			{
				GameController.ExhaustCoroutine(applyStatus);
			}
			yield break;
		}
	}
}