using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class ArctyxCharacterCardController : HeroCharacterCardController
    {
        public ArctyxCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //Reduce damage dealt by a target by 1 until the start of your next turn.
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator selectWeakling = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target"), storedResults, false, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(selectWeakling);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(selectWeakling);
                        }
                        SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
                        Card selectedCard = GetSelectedCard(storedResults);
                        ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                        rdse.SourceCriteria.IsSpecificCard = selectedCard;
                        rdse.UntilStartOfNextTurn(TurnTaker);
                        rdse.UntilCardLeavesPlay(selectedCard);
                        IEnumerator applyStatus = AddStatusEffect(rdse);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(applyStatus);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(applyStatus);
                        }
                        break;
                    }
                case 1:
                    {
                        //Increase damage dealt by a target by 1 until the start of your next turn.
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator selectStrongman = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "target"), storedResults, false, cardSource: GetCardSource());;
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(selectStrongman);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(selectStrongman);
                        }
                        Card selectedCard = GetSelectedCard(storedResults);
                        IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                        idse.SourceCriteria.IsSpecificCard = selectedCard;
                        idse.UntilStartOfNextTurn(TurnTaker);
                        idse.UntilCardLeavesPlay(selectedCard);
                        IEnumerator applyStatus = AddStatusEffect(idse);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(applyStatus);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(applyStatus);
                        }
                        break;
                    }
                case 2:
                    {
                        //One player may draw a card.
                        IEnumerator drawCard = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(drawCard);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(drawCard);
                        }
                        break;
                    }
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Arctyx deals a target 2 melee damage.
            int powerNumeral = GetPowerNumeral(0, 2);
            IEnumerator bap = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), powerNumeral, DamageType.Melee, new int?(1), false, new int?(1), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(bap);
            }
            else
            {
                GameController.ExhaustCoroutine(bap);
            }
            yield break;
        }
    }
}