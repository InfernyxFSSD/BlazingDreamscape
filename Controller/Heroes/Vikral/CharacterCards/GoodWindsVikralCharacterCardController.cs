using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class GoodWindsVikralCharacterCardController : HeroCharacterCardController
    {
        public GoodWindsVikralCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        /*"Prevent the end of turn effects of Microstorms this turn. For each Microstorm in play, one player may draw 1 card."
         * "One hero target regains 2 HP."
         * "Until the start of your next turn, whenever a hero deals damage, another hero regains 1 HP."
         * "Select a target. Until the start of your next turn, damage dealt to that target is irreducible."*/

        public override IEnumerator UsePower(int index = 0)
        {
            PreventPhaseEffectStatusEffect ppese = new PreventPhaseEffectStatusEffect(Phase.End);
            ppese.UntilStartOfNextTurn(base.GameController.FindNextTurnTaker());
            ppese.CardCriteria.HasAnyOfTheseKeywords = new List<string>() { "microstorm" };
            IEnumerator preventEnd = base.AddStatusEffect(ppese, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(preventEnd);
            }
            else
            {
                base.GameController.ExhaustCoroutine(preventEnd);
            }
            int X = base.FindCardsWhere((Card c) => c.DoKeywordsContain("microstorm", false, false) && c.IsInPlayAndHasGameText, false, null, false).Count<Card>();
            for (int i = 0; i < X; i++)
            {
                IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, false, true, false, null, null, new int?(1), base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        IEnumerator healHeroTarget = base.GameController.SelectAndGainHP(this.DecisionMaker, 2, false, (Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, 1, null, false, null, GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(healHeroTarget);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(healHeroTarget);
                        }
                        break;
                    }
                case 1:
                    {
                        OnDealDamageStatusEffect oddse = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(AnotherHeroHealsResponse), "Until the start of your next turn, whenever a hero deals damage, another hero regains 1 HP.", new TriggerType[] { TriggerType.GainHP }, base.HeroTurnTaker, this.Card);
                        oddse.SourceCriteria.IsHeroCharacterCard = true;
                        oddse.BeforeOrAfter = BeforeOrAfter.After;
                        IEnumerator damageMeansHealing = base.AddStatusEffect(oddse, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(damageMeansHealing);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(damageMeansHealing);
                        }
                        break;
                    }
                case 2:
                    {
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "targets in play", false, false, null, null, false), storedResults, false, false, null, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        Card card = storedResults.FirstOrDefault()?.SelectedCard;
                        if (card != null)
                        {
                            coroutine = base.AddMakeDamageIrreducibleStatusEffect(card, base.TurnTaker, Phase.Start);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
        }

        private IEnumerator AnotherHeroHealsResponse(DealDamageAction dd)
        {
            if(dd.DamageSource.IsHeroCharacterCard == true)
            {
                IEnumerator healHeroTarget = base.GameController.SelectAndGainHP(this.DecisionMaker, 1, false, (Card c) => c.IsHeroCharacterCard && c.IsTarget && c.IsInPlayAndHasGameText && c != dd.DamageSource.Card, 1, null, false, null, GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(healHeroTarget);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(healHeroTarget);
                }
            }
        }
    }
}