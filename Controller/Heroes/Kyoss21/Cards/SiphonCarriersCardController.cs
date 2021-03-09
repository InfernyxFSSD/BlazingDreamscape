using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class SiphonCarriersCardController : CardController
    {
        //Whenever a villain card would be played, Kyoss 2.1 may deal herself 3 psychic damage. If she takes damage this way, play the top card of a hero deck instead.
        //At the start of your turn, destroy this card.

        public SiphonCarriersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Potentially hit yourself when a villain card would be played
            AddTrigger<PlayCardAction>((PlayCardAction pc) => IsVillain(pc.CardToPlay) && !pc.IsPutIntoPlay, new Func<PlayCardAction, IEnumerator>(MaybePlayHeroResponse), new TriggerType[] { TriggerType.DealDamage, TriggerType.CancelAction, TriggerType.PlayCard }, TriggerTiming.Before);
            //Destroy this card at the start of your turn
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource()), TriggerType.DestroySelf);
        }

        public IEnumerator MaybePlayHeroResponse(PlayCardAction pc)
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            //Decide whether to hit yourself
            IEnumerator hitSelf = DealDamage(CharacterCard, CharacterCard, 3, DamageType.Psychic, optional: true, storedResults: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(hitSelf);
            }
            else
            {
                GameController.ExhaustCoroutine(hitSelf);
            }
            if (DidDealDamage(storedResults, CharacterCard))
            {
                //If you took damage, cancel the villain play
                IEnumerator cancelVillainPlay = CancelAction(pc);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(cancelVillainPlay);
                }
                else
                {
                    GameController.ExhaustCoroutine(cancelVillainPlay);
                }
                List<SelectTurnTakerDecision> storedResults2 = new List<SelectTurnTakerDecision>();
                //Then, select a hero to play the top card of their deck
                IEnumerator selectTurnTaker = GameController.SelectTurnTaker(DecisionMaker, SelectionType.Custom, storedResults2, additionalCriteria: (TurnTaker tt) => tt.IsHero, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(selectTurnTaker);
                }
                else
                {
                    GameController.ExhaustCoroutine(selectTurnTaker);
                }
                SelectTurnTakerDecision selectTurnTakerDecision = storedResults2.FirstOrDefault<SelectTurnTakerDecision>();
                if (selectTurnTakerDecision != null && selectTurnTakerDecision.SelectedTurnTaker != null)
                {
                    IEnumerator playTopCard = GameController.PlayTopCard(DecisionMaker, FindTurnTakerController(selectTurnTakerDecision.SelectedTurnTaker), cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(playTopCard);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(playTopCard);
                    }
                }
            }
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("Play the top card of which deck?", "Play the top card of which deck?", "Vote for which deck should have the top card played.", "play the top card of which deck?");
        }
    }
}