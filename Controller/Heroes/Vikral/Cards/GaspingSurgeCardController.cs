using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
	public class GaspingSurgeCardController : MicrostormCardController
	{
		public GaspingSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => IsMicrostorm(d.CardToDestroy.Card) && d.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(DealDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		private IEnumerator DealDamageResponse(DestroyCardAction d)
        {
			var microstormIdentifier = d.CardToDestroy.Card.Identifier;
            var damageType = new DamageType();
			if (microstormIdentifier == HailFlurryIdentifier)
			{
				damageType = DamageType.Cold;
			}
			else if (microstormIdentifier == FlaringBlazeIdentifier)
			{
				damageType = DamageType.Fire;
			}
			else if (microstormIdentifier == ScathingSandstormIdentifier)
			{
				damageType = DamageType.Projectile;
			}
			else if (microstormIdentifier == BladedGaleIdentifier)
			{
				damageType = DamageType.Sonic;
			}
			else if (microstormIdentifier == VigilanteJusticeIdentifier)
			{
				damageType = DamageType.Radiant;
			}
			else if (microstormIdentifier == ViciousRetributionIdentifier)
			{
				damageType = DamageType.Infernal;
			}
			else if (microstormIdentifier == ThunderStormIdentifier)
			{
				damageType = DamageType.Lightning;
			}
			else if (microstormIdentifier == ToxicCloudIdentifier)
			{
				damageType = DamageType.Toxic;
			}
			else if (microstormIdentifier == PsionicTorrentIdentifier)
			{
				damageType = DamageType.Psychic;
			}
			else if (microstormIdentifier == OverloadPulseIdentifier)
			{
				damageType = DamageType.Energy;
			}
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, damageType, 1, false, 0, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, new LinqCardCriteria((Card c) => IsMicrostorm(c)), true, false, true, null);
			if (base.UseUnityCoroutines)
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