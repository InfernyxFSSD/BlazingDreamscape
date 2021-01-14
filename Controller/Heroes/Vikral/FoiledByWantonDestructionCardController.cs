using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class FoiledByWantonDestructionCardController : MicrostormCardController
    {
        public FoiledByWantonDestructionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => (c.IsOngoing && c.IsVillain) || (c.IsDevice && c.HitPoints <= 6) || (c.IsRelic && c.HitPoints <= 6), "villain ongoing, device with 6 or less HP, or relic with 6 or less HP", false, false, "villain ongoing card, device with 6 or less HP, or relic with 6 or less HP", "villain ongoing cards, devices with 6 or less HP, or relics with 6 or less HP", false), true, storedResults, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            List<DestroyCardAction> dca = new List<DestroyCardAction>();
            coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => IsMicrostorm(c) && c.IsInPlayAndHasGameText, "microstorm", false, false, null, null, false), true, dca, null, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDestroyCard(dca))
            {
                var vilTurnTaker = GameController.AllTurnTakers.Where((TurnTaker tt) => tt.IsVillain || tt.IsVillainTeam);
                var vilToSkip = vilTurnTaker.FirstOrDefault();
                PreventPhaseActionStatusEffect ppase = new PreventPhaseActionStatusEffect();
                ppase.ToTurnPhaseCriteria.Phase = new Phase?(Phase.PlayCard);
                ppase.ToTurnPhaseCriteria.TurnTaker = vilToSkip;
                ppase.UntilEndOfPhase(vilToSkip, Phase.PlayCard);
                ppase.NumberOfUses = 1;
                coroutine = AddStatusEffect(ppase, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = base.GameController.SendMessageAction("Skip the next villain play phase!", Priority.High, GetCardSource(null), null, false);
                if (UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }
        }
    }
}
