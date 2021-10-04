using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class OverclockedArsenalCardController : CardController
	{
		//May use an extra power during power phase, either hit self for 1 irreducible lightning or destroy this card at start of turn.

		public OverclockedArsenalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
		}
		public override void AddTriggers()
		{
			base.AddAdditionalPhaseActionTrigger((TurnTaker tt) => this.ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => base.DealDamageOrDestroyThisCardResponse(pca, base.CharacterCard, base.CharacterCard, 1, DamageType.Lightning, true), new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf });
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