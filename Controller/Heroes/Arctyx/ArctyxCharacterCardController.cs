using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class ArctyxCharacterCardController : HeroCharacterCardController
    {
        public string str;

        public ArctyxCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target", true, false, null, null, false), storedResults, false, false, null, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
                        Card selectedCard = base.GetSelectedCard(storedResults);
                        ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
                        reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = selectedCard;
                        reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                        reduceDamageStatusEffect.UntilCardLeavesPlay(selectedCard);
                        coroutine2 = base.AddStatusEffect(reduceDamageStatusEffect, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 1:
                    {
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target", true, false, null, null, false), storedResults, false, false, null, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
                        Card selectedCard = base.GetSelectedCard(storedResults);
                        IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
                        increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = selectedCard;
                        increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                        increaseDamageStatusEffect.UntilCardLeavesPlay(selectedCard);
                        coroutine2 = base.AddStatusEffect(increaseDamageStatusEffect, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, false, true, false, null, null, null, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 2);
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), powerNumeral, DamageType.Melee, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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
    }
}