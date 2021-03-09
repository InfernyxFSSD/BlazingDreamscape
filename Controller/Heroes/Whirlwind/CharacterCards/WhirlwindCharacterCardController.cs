using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class WhirlwindCharacterCardController : HeroCharacterCardController
    {

        public WhirlwindCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One hero may use a power now.
                        IEnumerator usePower = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(usePower);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(usePower);
                        }
                        break;
                    }
                case 1:
                    {
                        //Up to 2 non-hero targets deal themselves 1 radiant damage
                        List<Card> selectedTargets = new List<Card>();
                        for (int i = 0; i < 2; i++)
                        {
                            //First, select a target
                            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                            IEnumerable<Card> choices = FindCardsWhere((Card c) => !c.IsHero && c.IsInPlay && c.IsTarget && !selectedTargets.Contains(c), false, null, false);
                            IEnumerator selectTarget = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTarget, choices, storedResults, cardSource: GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(selectTarget);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(selectTarget);
                            }
                            Card card = (from d in storedResults select d.SelectedCard).FirstOrDefault<Card>();
                            if (card != null)
                            {
                                //Then, the target hits itself
                                selectedTargets.Add(card);
                                IEnumerator hitSelf = DealDamage(card, card, 1, DamageType.Radiant, cardSource: GetCardSource());
                                if (UseUnityCoroutines)
                                {
                                    yield return GameController.StartCoroutine(hitSelf);
                                }
                                else
                                {
                                    GameController.ExhaustCoroutine(hitSelf);
                                }
                            }
                            else
                            {
                                yield break;
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //Reduce damage dealt to hero targets by 1 until the start of your next turn.
                        ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                        rdse.TargetCriteria.IsHero = true;
                        rdse.UntilStartOfNextTurn(TurnTaker);
                        IEnumerator applyStatus = AddStatusEffect(rdse);
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

        public override IEnumerator UsePower(int index = 0)
        {
            //Either Whirlwind deals a target 1 Projectile damage, or you draw a card.
            int powerNumeral = GetPowerNumeral(0, 1);
            List<Function> list = new List<Function>();
            List<SelectFunctionDecision> results = new List<SelectFunctionDecision>();
            IEnumerable<Card> choices = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndNotUnderCard));
            list.Add(new Function(DecisionMaker, $"Deal a target {powerNumeral} projectile damage", SelectionType.DealDamage, () => GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), powerNumeral, DamageType.Projectile, new int?(1), false, new int?(1), cardSource: GetCardSource()), repeatDecisionText: "Deal a target 1 Projectile damage"));
            list.Add(new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => GameController.DrawCard(HeroTurnTaker, cardSource: GetCardSource()), repeatDecisionText: "Draw a card"));
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, true, cardSource: GetCardSource());
            IEnumerator makeDecision = GameController.SelectAndPerformFunction(selectFunction, results);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(makeDecision);
            }
            else
            {
                GameController.ExhaustCoroutine(makeDecision);
            }
            yield break;
        }
    }
}