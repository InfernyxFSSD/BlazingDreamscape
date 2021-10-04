using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
    public class SuppressionRifleCardController : CardController
    {
        //Power: Cyberknight hits up to three targets for 1 projectile, then may zap self to prevent the next damage dealt by non-heroes dealt damage this way

        public SuppressionRifleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = GetPowerNumeral(0, 1);
            int powerNumeral2 = GetPowerNumeral(1, 1);
            List<DealDamageAction> targetsShot = new List<DealDamageAction>();
            IEnumerator shootTargets = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, DamageType.Projectile, new int?(3), false, new int?(0), storedResultsDamage: targetsShot, cardSource: base.GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shootTargets);
            }
            else
            {
                GameController.ExhaustCoroutine(shootTargets);
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
                IEnumerator hitSelf = base.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => c == base.CharacterCard, powerNumeral2, DamageType.Lightning, true, zapSelf, cardSource: base.GetCardSource(null));
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
                    List<Card> damagedTargets = new List<Card>();
                    if(targetsShot.Any())
                    { 
                        foreach(DealDamageAction dda in targetsShot)
                        {
                            if(dda.DidDealDamage && !dda.Target.IsHero)
                            {
                                damagedTargets.Add(dda.Target);
                            }
                        }
                    }
                    foreach (Card c in damagedTargets)
                    {
                        if (!c.IsHero)
                        {
                            CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
                            cddse.IsPreventEffect = true;
                            cddse.NumberOfUses = 1;
                            cddse.SourceCriteria.IsSpecificCard = c;
                            cddse.CardDestroyedExpiryCriteria.Card = c;
                            IEnumerator preventNextDamage = base.AddStatusEffect(cddse);
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(preventNextDamage);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(preventNextDamage);
                            }
                        }
                    }
                }
            }
            yield break;
        }
    }
}