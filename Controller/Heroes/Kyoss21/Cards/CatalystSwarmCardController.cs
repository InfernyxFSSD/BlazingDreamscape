using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class CatalystSwarmCardController : CardController
    {
        //When this card enters play, place it next to a hero.
        //Increase damage dealt by that hero by 1.
        //After that hero uses a power, they may draw or play a card.

        public CatalystSwarmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator selectHero = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, "hero character card"), storedResults, isPutIntoPlay, decisionSources);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectHero);
            }
            else
            {
                GameController.ExhaustCoroutine(selectHero);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //Increase the damage dealt by the hero this card is next to
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card == GetCardThisCardIsNextTo(), 1);
            //After the hero this card is next to uses a power, they may draw or play a card
            AddTrigger<UsePowerAction>((UsePowerAction p) => GetCardThisCardIsNextTo().Owner.IsHero && p.Power.TurnTakerController == FindHeroTurnTakerController(GetCardThisCardIsNextTo().Owner.ToHero()) && (!p.Power.TurnTakerController.HasMultipleCharacterCards || p.Power.CardController.Card == GetCardThisCardIsNextTo()), (UsePowerAction p) => DrawOrPlayResponse(p), new TriggerType[] { TriggerType.DrawCard, TriggerType.PlayCard }, TriggerTiming.After);
        }

        //"After that hero uses a power, they may draw or play a card."
        private IEnumerator DrawOrPlayResponse(UsePowerAction upa)
        {
            List<Function> list = new List<Function>();
            HeroTurnTaker htt = GetCardThisCardIsNextTo().Owner.ToHero();
            HeroTurnTakerController thisHero = FindHeroTurnTakerController(htt);
            list.Add(new Function(thisHero, "Draw a card", SelectionType.DrawCard, () => DrawCard(htt)));
            list.Add(new Function(thisHero, "Play a card", SelectionType.PlayCard, () => SelectAndPlayCardFromHand(thisHero)));
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, thisHero, list, true, cardSource: GetCardSource());
            IEnumerator drawOrPlay = GameController.SelectAndPerformFunction(selectFunction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(drawOrPlay);
            }
            else
            {
                GameController.ExhaustCoroutine(drawOrPlay);
            }
            yield break;
        }
    }
}