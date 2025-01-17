﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Networking;
using RoR2.Audio;
using R2API.Networking;
using R2API.Networking.Interfaces;
using EntityStates;

namespace Ursa.States
{
	class UrsaMain : GenericCharacterMain
	{
		private Transform model;
		private ChildLocator childLocator;
		private ParticleSystem.EmissionModule overpowerParticleSystemEmission; 

		public override void OnEnter()
		{
			base.OnEnter();
			this.model = base.GetModelTransform();
			this.childLocator = base.GetModelChildLocator();
			this.overpowerParticleSystemEmission = this.childLocator.FindChild("OverpowerParticles").GetComponent<ParticleSystem>().emission;
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			#region Change run animation if low health.
			if (base.healthComponent.combinedHealth <= base.healthComponent.fullCombinedHealth / 4)
			{
				this.GetModelAnimator().SetFloat("isLowHealth", 1, 0.1f, Time.fixedDeltaTime);
			}
			else
			{
				this.GetModelAnimator().SetFloat("isLowHealth", 0, 0.1f, Time.fixedDeltaTime);
			}
			#endregion

			#region Arms glow if have Overpower.
			if (base.isAuthority && base.characterBody.HasBuff(Survivor.Buffs.overpowerBuff))
			{
				base.GetModelChildLocator().FindChild("R_Hand").GetComponent<Light>().enabled = true;
				base.GetModelChildLocator().FindChild("L_Hand").GetComponent<Light>().enabled = true;
                this.overpowerParticleSystemEmission.enabled = true;
				Core.Utils.HandsGlowOnOtherClients(base.gameObject, true);
			}
			else if (base.isAuthority && !base.characterBody.HasBuff(Survivor.Buffs.overpowerBuff))
			{
				base.GetModelChildLocator().FindChild("R_Hand").GetComponent<Light>().enabled = false;
				base.GetModelChildLocator().FindChild("L_Hand").GetComponent<Light>().enabled = false;
				this.overpowerParticleSystemEmission.enabled = false;
				Core.Utils.HandsGlowOnOtherClients(base.gameObject, false);
			}
			#endregion

			#region Ursa grows when Enraged.
			if (base.isAuthority && base.characterBody.HasBuff(Survivor.Buffs.enrageBuff) && this.model.localScale.magnitude < (Core.Constants.BaseSize + Enrage.bonusSize).magnitude)
			{
				float n = Time.deltaTime;
				model.localScale += new Vector3(n, n, n);
				Core.Utils.GrowOnOtherClients(base.gameObject, (Core.Constants.BaseSize + Enrage.bonusSize));
			}
			else if (base.isAuthority && !base.characterBody.HasBuff(Survivor.Buffs.enrageBuff) && this.model.localScale.magnitude > Core.Constants.BaseSize.magnitude)
			{
				float n = Time.deltaTime;
				model.localScale -= new Vector3(n, n, n);
				Core.Utils.GrowOnOtherClients(base.gameObject, Core.Constants.BaseSize);

			}
            #endregion

            #region Resize Ursa to normal size
			// Unity animator for some reason resets Ursa's scale to 1 when it initializes, meaning that I can't just set his model's scale in CreateCharacter.
			// Instead, I just check if it is smaller than it is supposed to be and scale it up using the same method as Enrage resize.
			if(base.isAuthority && model.localScale.magnitude < Core.Constants.BaseSize.magnitude)
            {
				float n = Time.deltaTime;
				model.localScale += new Vector3(n, n, n);
			}
            #endregion
        }
    }
}


