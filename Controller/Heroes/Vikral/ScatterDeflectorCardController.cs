using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace SybithosInfernyx.Vikral
{
    class ScatterDeflectorCardController : CardController
    {
        private static readonly string PreventAndScatterPropertyKey = "VikralPreventAndScatter";

        public ScatterDeflectorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddPreventDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.Amount >= 3 && base.GetCardPropertyJournalEntryBoolean(PreventAndScatterPropertyKey) == true, new Func<DealDamageAction, IEnumerator>(this.PreventDamageAndScatterResponse), new TriggerType[] { TriggerType.DealDamage }, true);
        }

        //Power: The next time Vikral would be dealt three or more damage, prevent that damage. Then, the source of that damage deals up to X targets 1 damage of that type each, where X is the amount of damage prevented this way. This power can not stack.
        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 1);
            IEnumerator coroutine;
            if (base.GetCardPropertyJournalEntryBoolean(PreventAndScatterPropertyKey) == true)
            {
                coroutine = base.GameController.SendMessageAction("This power is already active!", Priority.High, base.GetCardSource(null), null, true);
                if (UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }
            base.AddCardPropertyJournalEntry(PreventAndScatterPropertyKey, true);
            OnPhaseChangeStatusEffect opcse = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DoNothing), $"Prevent the next damage of 3 or more that would be dealt to {base.TurnTaker.Name}, and have the source of that damage deal damage.", new TriggerType[] { TriggerType.WouldBeDealtDamage }, this.Card);
            opcse.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
            opcse.NumberOfUses = powerNumeral;
            coroutine = AddStatusEffect(opcse, true);
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator PreventDamageAndScatterResponse(DealDamageAction dd)
        {
            StatusEffect[] statusEffects = base.Game.StatusEffects.Where((StatusEffect effect) => effect.CardSource == this.Card && effect.CardDestroyedExpiryCriteria.Card == base.CharacterCard).ToArray();
            if (statusEffects.Any())
            {
                foreach (StatusEffect status in statusEffects)
                {
                    var damageType = dd.DamageType;
                    int numOfTargets = dd.Amount;
                    int damageAmount = status.NumberOfUses.Value;
                    IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, dd.DamageSource, damageAmount, damageType, numOfTargets, false, 0, false, false, false, (Card c) => c != base.CharacterCard, null, null, null, null, false, null, null, false, null, GetCardSource(null));
                    if (UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    base.AddCardPropertyJournalEntry(PreventAndScatterPropertyKey, (bool?)null);
                    base.GameController.StatusEffectManager.RemoveStatusEffect(status);
                }
            }
            yield break;
        }
    }
}
