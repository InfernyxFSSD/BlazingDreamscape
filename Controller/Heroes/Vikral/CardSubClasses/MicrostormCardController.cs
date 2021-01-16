using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
	public abstract class MicrostormCardController : CardController
	{
		public MicrostormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public bool IsMicrostorm(Card card)
		{
			return card.DoKeywordsContain("microstorm", false, false);
		}

		public TokenPool GetTokenPool(Card card)
        {
			return base.TurnTaker.FindCard(card.Identifier).FindTokenPool("TorrentPool");
		}

		public string BladedGaleIdentifier = "BladedGale";
		public string FlaringBlazeIdentifier = "FlaringBlaze";
		public string HailFlurryIdentifier = "HailFlurry";
		public string ScathingSandstormIdentifier = "ScathingSandstorm";
		public string OverloadPulseIdentifier = "OverloadPulse";
		public string PsionicTorrentIdentifier = "PsionicTorrent";
		public string ThunderStormIdentifier = "ThunderStorm";
		public string ToxicCloudIdentifier = "ToxicCloud";
		public string ViciousRetributionIdentifier = "ViciousRetribution";
		public string VigilanteJusticeIdentifier = "VigilanteJustice";
	}
}