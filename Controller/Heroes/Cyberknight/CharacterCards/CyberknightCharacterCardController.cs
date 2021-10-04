using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
    public class CyberknightCharacterCardController : HeroCharacterCardController
    {
        public CyberknightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Cyberknight deals a target 2 Energy damage. You may discard up to 5 cards. For each card discarded this way, Cyberknight deals a target 1 Energy damage.
            int powerNumeral = base.GetPowerNumeral(0, 2);
            int powerNumeral2 = base.GetPowerNumeral(1, 5);
            int powerNumeral3 = base.GetPowerNumeral(2, 1);
            IEnumerator shootTarget = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), powerNumeral, DamageType.Energy, new int?(1), false, new int?(1), cardSource: base.GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shootTarget);
            }
            else
            {
                GameController.ExhaustCoroutine(shootTarget);
            }
            base.AddToTemporaryTriggerList(base.AddTrigger<DiscardCardAction>((DiscardCardAction dca) => dca.ResponsibleTurnTaker == base.TurnTaker && dca.CardSource.Card == base.Card, delegate (DiscardCardAction dca)
            {
                return base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), powerNumeral3, DamageType.Energy, new int?(1), false, new int?(1), cardSource: base.GetCardSource(null));
            }, TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified));
                IEnumerator discardCards = base.SelectAndDiscardCards(base.HeroTurnTakerController, new int?(powerNumeral2), requiredDecisions: new int?(0), selectionType: SelectionType.DiscardCard, responsibleTurnTaker: base.TurnTaker);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCards);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCards);
            }
            base.RemoveTemporaryTriggers();
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //Select a hero target. Until the start of your next turn, reduce damage dealt to that target by 1.List<SelectCardDecision> selectedHeroTarget = new List<SelectCardDecision>();
                        List<SelectCardDecision> selectedHeroTarget = new List<SelectCardDecision>();
                        IEnumerator selectTarget = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlay && c.IsHero && c.IsTarget, "hero target", useCardsSuffix: false), selectedHeroTarget, optional: false, cardSource: base.GetCardSource(null));
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(selectTarget);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(selectTarget);
                        }
                        SelectCardDecision selection = selectedHeroTarget.FirstOrDefault<SelectCardDecision>();
                        if (selection != null && selection.SelectedCard != null)
                        {
                            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                            rdse.UntilStartOfNextTurn(base.TurnTaker);
                            rdse.TargetCriteria.IsSpecificCard = selection.SelectedCard;
                            IEnumerator reduceDamage = base.AddStatusEffect(rdse);
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(reduceDamage);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(reduceDamage);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        //Select a hero target. The next damage dealt by that target is irreducible and increased by 1.
                        List<SelectCardDecision> selectedHeroTarget = new List<SelectCardDecision>();
                        IEnumerator selectTarget = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlay && c.IsHero && c.IsTarget, "hero target", useCardsSuffix: false), selectedHeroTarget, optional: false, cardSource: base.GetCardSource(null));
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(selectTarget);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(selectTarget);
                        }
                        SelectCardDecision selection = selectedHeroTarget.FirstOrDefault<SelectCardDecision>();
                        if (selection != null && selection.SelectedCard != null)
                        {
                            IEnumerator makeNextHitIrreducible = base.AddStatusEffect(new MakeDamageIrreducibleStatusEffect
                            {
                                SourceCriteria =
                                {
                                    IsSpecificCard = selection.SelectedCard
                                },
                                NumberOfUses = new int?(1)
                            });
                            IEnumerator increaseNextHit = base.AddStatusEffect(new IncreaseDamageStatusEffect(1)
                            {
                                SourceCriteria =
                                {
                                    IsSpecificCard = selection.SelectedCard
                                },
                                NumberOfUses = new int?(1)
                            });
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(makeNextHitIrreducible);
                                yield return GameController.StartCoroutine(increaseNextHit);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(makeNextHitIrreducible);
                                GameController.ExhaustCoroutine(increaseNextHit);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //One player may draw a card.
                        IEnumerator selectDrawer = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: base.GetCardSource(null));
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(selectDrawer);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(selectDrawer);
                        }
                        break;
                    }
            }
        }
    }
}