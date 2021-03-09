using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class CritPrimerNanobotsCardController : CardController
    {
        //Kyoss 2.1 deals each villain target 1 irreducible lightning damage. Until the start of your next turn, damage dealt to any targets dealt damage this way is irreducible.
        public CritPrimerNanobotsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //Bap all villain targets
            IEnumerator lightningBap = DealDamage(CharacterCard, (Card card) => card.IsVillain, 1, DamageType.Lightning, isIrreducible: true, addStatusEffect: new Func<DealDamageAction, IEnumerator>(TargetsDealtDamageThisWayTakeIrreducibleUntilStartOfNextTurnResponse));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(lightningBap);
            }
            else
            {
                GameController.ExhaustCoroutine(lightningBap);
            }
            yield break;
        }

        private IEnumerator TargetsDealtDamageThisWayTakeIrreducibleUntilStartOfNextTurnResponse(DealDamageAction dda)
        {
            if (dda.DidDealDamage)
            {
                //If a villain target was bapped, damage dealt to them is irreducible until the start of your next turn
                MakeDamageIrreducibleStatusEffect mdise = new MakeDamageIrreducibleStatusEffect();
                mdise.TargetCriteria.IsSpecificCard = dda.Target;
                mdise.UntilStartOfNextTurn(TurnTaker);
                mdise.UntilCardLeavesPlay(dda.Target);
                IEnumerator applyStatus = AddStatusEffect(mdise);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(applyStatus);
                }
                else
                {
                    GameController.ExhaustCoroutine(applyStatus);
                }
            }
            yield break;
        }
    }
}