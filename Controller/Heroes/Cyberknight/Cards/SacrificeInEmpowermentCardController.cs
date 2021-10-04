using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
    public class SacrificeInEmpowermentCardController : CardController
    {
        //May use an extra power during power phase, increase damage dealt by 2, at end of next turn, hit self for 3 infernal and break this card.

        public SacrificeInEmpowermentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
        }
        public override void AddTriggers()
        {
            base.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsSameCard(base.CharacterCard), 2);
            base.AddIncreaseDamageTrigger((DealDamageAction dda) => dda.Target == base.CharacterCard, 2);
            base.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(EndOfTurnResponse), new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            bool playedThisTurn = this.WasThisCardPlayedThisTurn(pca.ToPhase.TurnTaker);
            if (!playedThisTurn)
            {
                IEnumerator hitSelf = base.DealDamage(base.CharacterCard, base.CharacterCard, 3, DamageType.Infernal);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(hitSelf);
                }
                else
                {
                    GameController.ExhaustCoroutine(hitSelf);
                }
                IEnumerator breakSelf = base.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: base.GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(breakSelf);
                }
                else
                {
                    GameController.ExhaustCoroutine(breakSelf);
                }
            }
            yield break;
        }

        private bool WasThisCardPlayedThisTurn(TurnTaker tt)
        {
            return base.Journal.PlayCardEntriesThisTurn().Any((PlayCardJournalEntry pcje) => pcje.CardPlayed.Title == this.Card.Title);
        }

        public override IEnumerator Play()
        {
            IEnumerator increasePowers = base.IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, Phase.UsePower, 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(increasePowers);
            }
            else
            {
                GameController.ExhaustCoroutine(increasePowers);
            }
            yield break;
        }

        public override bool DoesHaveActivePlayMethod
        {
            get
            {
                return false;
            }
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == base.TurnTaker;
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            return base.GameController.ActiveTurnPhase.IsUsePower && this.ShouldIncreasePhaseActionCount(base.GameController.ActiveTurnTaker);
        }
    }
}