using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
    public class PolyburstGlaiveCardController : CardController
    {
        //Power: Cyberknight hits a target for 2 melee and 2 of type choice, may zap herself for 1 lightning to reduce the next damage dealt to her by 4

        public PolyburstGlaiveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = GetPowerNumeral(0, 2);
            int powerNumeral2 = GetPowerNumeral(1, 2);
            int powerNumeral3 = GetPowerNumeral(2, 1);
            int powerNumeral4 = GetPowerNumeral(3, 4);
            List<DealDamageAction> list = new List<DealDamageAction>
            {
                new DealDamageAction(base.GetCardSource(null), new DamageSource(base.GameController, base.CharacterCard), null, powerNumeral2, DamageType.Cold)
                {
                    UnknownDamageType = true
                }
            };
            List<SelectCardDecision> chosenVictim = new List<SelectCardDecision>();
            IEnumerator bapBap = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, DamageType.Melee, 1, false, 1, storedResultsDecisions: chosenVictim, cardSource: base.GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(bapBap);
            }
            else
            {
                GameController.ExhaustCoroutine(bapBap);
            }
            foreach (SelectCardDecision scd in chosenVictim)
            {
                Card target = scd.SelectedCard;
                if (target != null && target.IsInPlayAndHasGameText)
                {
                    List<SelectDamageTypeDecision> damageTypeChoice = new List<SelectDamageTypeDecision>();
                    IEnumerator chooseDamage = base.GameController.SelectDamageType(this.DecisionMaker, damageTypeChoice, cardSource: base.GetCardSource(null));
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(chooseDamage);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(chooseDamage);
                    }
                    DamageType? chosenType = base.GetSelectedDamageType(damageTypeChoice);
                    if (chosenType != null)
                    {
                        IEnumerator bapBap2 = base.DealDamage(base.CharacterCard, target, powerNumeral2, chosenType.Value, cardSource: base.GetCardSource(null));
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(bapBap2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(bapBap2);
                        }
                    }
                }
            }
            List<YesNoCardDecision> yesOrNo = new List<YesNoCardDecision>();
            IEnumerator makeDecision = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.DealDamageSelf, base.Card, storedResults: yesOrNo, cardSource: base.GetCardSource(null));
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
                List<DealDamageAction> zapSelf = new List<DealDamageAction>();
                IEnumerator hitSelf = base.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => c == base.CharacterCard, powerNumeral3, DamageType.Lightning, true, zapSelf, cardSource: base.GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(hitSelf);
                }
                else
                {
                    GameController.ExhaustCoroutine(hitSelf);
                }
                if (zapSelf != null && zapSelf.Count(dd => dd.Amount > 0) > 0)
                {
                    ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(powerNumeral4);
                    rdse.TargetCriteria.IsSpecificCard = base.CharacterCard;
                    rdse.NumberOfUses = 1;
                    rdse.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
                    IEnumerator reduceNextDamage = base.AddStatusEffect(rdse);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(reduceNextDamage);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(reduceNextDamage);
                    }
                }
            }
            yield break;
        }
    }
}