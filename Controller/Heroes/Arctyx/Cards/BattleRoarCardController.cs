using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class BattleRoarCardController : CardController
    {
        //Until the start of your next turn, redirect damage dealt by villain targets to Arctyx

        public BattleRoarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //Status that redirects damage dealt by villain targets to Arctyx
            RedirectDamageStatusEffect rdse = new RedirectDamageStatusEffect
            {
                RedirectTarget = CharacterCard
            };
            rdse.SourceCriteria.IsVillain = true;
            rdse.TargetCriteria.IsNotSpecificCard = CharacterCard;
            rdse.UntilStartOfNextTurn(TurnTaker);
            rdse.TargetRemovedExpiryCriteria.Card = CharacterCard;
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