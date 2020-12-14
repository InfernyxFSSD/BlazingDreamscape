using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class ArmorBurstCardController : CardController
    {
        public ArmorBurstCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura", false, false), "aura", true, false, null, null, false), null, null, null, false);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = base.DestroyCardsAndDoActionBasedOnNumberOfCardsDestroyed(this.DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura", false, false), "aura", true, false, null, null, false), new Func<int, IEnumerator>(this.DealDamageResponseAndIncreaseDamageTaken), true, null);
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

        private IEnumerator DealDamageResponseAndIncreaseDamageTaken(int X)
        {
            if (X > 0)
            {
                List<SelectDamageTypeDecision> storedResults = new List<SelectDamageTypeDecision>();
                IEnumerator chooseDamageType = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults, new DamageType[]
                {
                    DamageType.Fire,
                    DamageType.Cold
                }, null, SelectionType.DamageType, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(chooseDamageType);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(chooseDamageType);
                }
                DamageType value = storedResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;
                IEnumerator dealAuraDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, value, new int?(X), false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealAuraDamage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealAuraDamage);
                }
                IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(X);
                increaseDamageStatusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
                increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                IEnumerator increaseDamageTaken = base.AddStatusEffect(increaseDamageStatusEffect, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(increaseDamageTaken);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(increaseDamageTaken);
                }
                yield break;
            }
            else
            {
                yield break;
            }
        }
    }
}
