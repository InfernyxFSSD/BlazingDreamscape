using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class SupplementaryBatchCardController : CardController
    {
        //Power: One player may play a card, use a power, and draw a card in any order now. Destroy this card.

        public SupplementaryBatchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //One player may play a card, use a power, and draw a card in any order now.
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator selectPlayer = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.SelectTargetFriendly, false, false, storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectPlayer);
            }
            else
            {
                GameController.ExhaustCoroutine(selectPlayer);
            }
            if (DidSelectTurnTaker(storedResults))
            {
                HeroTurnTaker htt = GetSelectedTurnTaker(storedResults).ToHero();
                HeroTurnTakerController hero = FindHeroTurnTakerController(htt);
                bool drewCard = false;
                bool playedCard = false;
                bool usedPower = false;
                List<SelectFunctionDecision> storedFunction = new List<SelectFunctionDecision>();
                //Send a message stating if that hero can't take one of those actions
                Func<string> getNoFunctionString = delegate
                {
                    List<string> list = new List<string>();
                    if (!usedPower && (!GameController.CanUsePowers(hero, GetCardSource()) || GameController.GetUsablePowersThisTurn(hero).Count() < 1))
                    {
                        list.Add("use a power");
                    }
                    if (!playedCard && !CanPlayCardsFromHand(hero))
                    {
                        list.Add("play a card");
                    }
                    if (!drewCard && !CanDrawCards(hero))
                    {
                        list.Add("draw a card");
                    }
                    return "That hero cannot " + list.ToCommaList(useWordAnd: false, useWordOr: true) + ".";
                };
                for (int i = 0; i < 3; i++)
                {
                    //Let them choose from options that they haven't chosen yet until are selected
                    List<Function> list = new List<Function>
                    {
                        new Function(hero, "Draw a card", SelectionType.DrawCard, () => DrawCard(htt), !drewCard && CanDrawCards(hero)),
                        new Function(hero, "Play a card", SelectionType.PlayCard, () => SelectAndPlayCardFromHand(hero, optional: false), !playedCard && CanPlayCardsFromHand(hero)),
                        new Function(hero, "Use a power", SelectionType.UsePower, () => GameController.SelectAndUsePower(hero, optional: false, cardSource: GetCardSource()), !usedPower && GameController.CanUsePowers(hero, GetCardSource()) && GameController.GetUsablePowersThisTurn(hero).Count() > 0)
                    };
                    SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, hero, list, optional: true, cardSource: GetCardSource());
                    IEnumerator makeDecision = GameController.SelectAndPerformFunction(selectFunction, storedFunction);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(makeDecision);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(makeDecision);
                    }
                    Function selectedFunction = GetSelectedFunction(storedFunction);
                    if (selectedFunction != null)
                    {
                        switch (selectedFunction.SelectionType)
                        {
                            case SelectionType.DrawCard:
                                drewCard = true;
                                break;
                            case SelectionType.PlayCard:
                                playedCard = true;
                                break;
                            case SelectionType.UsePower:
                                usedPower = true;
                                break;
                        }
                        continue;
                    }
                }
            }
            //Destroy this card after all of that^
            IEnumerator destroySelf = GameController.DestroyCard(DecisionMaker, Card, optional: false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroySelf);
            }
            else
            {
                GameController.ExhaustCoroutine(destroySelf);
            }
            yield break;
        }
    }
}