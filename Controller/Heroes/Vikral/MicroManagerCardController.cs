using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
	public abstract class MicroManagerCardController : MicrostormCardController
	{
		public MicroManagerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

        public override void AddTriggers()
        {
			base.AddWhenDestroyedTrigger((DestroyCardAction dc) => this.ResetTokenValue(), TriggerType.Hidden);
		}
		public IEnumerator ResetTokenValue()
		{
			string torrentPoolIdentifier = $"{base.Card.Identifier}TorrentPool";
			TokenPool tokenPool = base.Card.FindTokenPool(torrentPoolIdentifier);
			tokenPool.SetToInitialValue();
			yield return null;
			yield break;
		}

		public override IEnumerator Play()
		{
			Card card = base.TurnTaker.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == base.Card.Identifier).FirstOrDefault();
			string torrentPoolIdentifier = $"{card.Identifier}TorrentPool";
			TokenPool torrentPool = card.FindTokenPool(torrentPoolIdentifier);
			IEnumerator coroutine = base.GameController.AddTokensToPool(torrentPool, 3, GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public IEnumerator RemoveTokenFromMicrostorm(Card c)
		{
			if (c.IsInPlayAndHasGameText)
			{
				Card card = base.GameController.FindCardsWhere((Card d) => d.IsInPlayAndHasGameText && d.Identifier == c.Identifier).FirstOrDefault();
				string torrentPoolIdentifier = $"{card.Identifier}TorrentPool";
				TokenPool torrentPool = card.FindTokenPool(torrentPoolIdentifier);
				IEnumerator removeToken = base.GameController.RemoveTokensFromPool(torrentPool, 1, null, false, null, null);
				if (UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(removeToken);
				}
				else
				{
					base.GameController.ExhaustCoroutine(removeToken);
				}
			}
		}
			//return ((Card c) => c.IsInPlayAndHasGameText && IsMicrostorm(c));
	}
}