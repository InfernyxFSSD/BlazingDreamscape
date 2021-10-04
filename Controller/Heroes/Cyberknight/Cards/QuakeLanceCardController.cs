using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
    public class QuakeLanceCardController : CardController
    {
        //Power: Cyberknight may hit herself for 1 irreducible. If she takes damage this way, Cyberknight deals a target 4 irreducible melee. Otherwise, she deals a target 3 melee.

        public QuakeLanceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = GetPowerNumeral(0, 1);
            int powerNumeral2 = GetPowerNumeral(1, 4);
            int powerNumeral3 = GetPowerNumeral(2, 3);
            bool isHitIrreducible = false;
            int damageToDeal = powerNumeral3;
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
                IEnumerator hitSelf = base.GameController.DealDamageToSelf(this.DecisionMaker, (Card c) => c == base.CharacterCard, powerNumeral, DamageType.Lightning, true, zapSelf, cardSource: base.GetCardSource(null));
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
                    isHitIrreducible = true;
                    damageToDeal = powerNumeral2;
                }
            }
            IEnumerator poke = GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damageToDeal, DamageType.Melee, 1, false, 1, isIrreducible: isHitIrreducible, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(poke);
            }
            else
            {
                GameController.ExhaustCoroutine(poke);
            }
            yield break;
        }
    }
}
