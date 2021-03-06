﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using VehicleEffects.GameExtensions;

namespace VehicleEffects
{
    /// <summary>
    /// Handles the collection of custom particle effects loaded via config files.
    /// </summary>
    class CustomParticlesManager
    {
        private Dictionary<string, ParticleEffect> effects = new Dictionary<string, ParticleEffect>();

        public void Destroy()
        {
            foreach(var entry in effects)
            {
                GameObject.Destroy(entry.Value.gameObject);
            }
            effects.Clear();
        }

        public void Reset()
        {
            Destroy();
        }

        public bool AddEffect(ParticleEffect effect)
        {
            if(effects.ContainsKey(effect.name))
            {
                return false;
            }
            effects[effect.name] = effect;
            return true;
        }

        private static void ApplyNullableSetting<T>(ref T target, T? setting) where T : struct
        {
            if(setting != null)
            {
                target = setting.Value;
            }
        }

        private static void ApplyNullableSettingWithDefault<T>(ref T target, T? setting, T def) where T : struct
        {
            if(setting != null)
            {
                target = setting.Value;
            }
            else
            {
                target = def;
            }
        }

        public static ParticleEffect CreateParticleEffect(ParticleEffectParams settings)
        {
            if(settings == null)
            {
                Logging.LogError("Null passed to CreateParticleEffect");
                return null;
            }

            if(settings == null || string.IsNullOrEmpty(settings.Base) || string.IsNullOrEmpty(settings.Name))
            {
                Logging.LogError("Invalid particle effect settings, name or base is empty");
                return null;
            }

            var baseEffect = VehicleEffectsMod.FindEffect(settings.Base) as ParticleEffect;
            if(baseEffect == null)
            {
                Logging.LogError("Unable to find base particle effect " + settings.Base + " for effect " + settings.Name);
                return null;
            }


            // Create effect
            var effect = new GameObject().AddComponent<CustomMovementParticleEffect>();
            // Apply general settings
            ApplyNullableSettingWithDefault(ref effect.m_canUseBezier, settings.m_canUseBezier, baseEffect.m_canUseBezier);
            ApplyNullableSettingWithDefault(ref effect.m_canUseMeshData, settings.m_canUseMeshData, baseEffect.m_canUseMeshData);
            ApplyNullableSettingWithDefault(ref effect.m_canUsePositions, settings.m_canUsePositions, baseEffect.m_canUsePositions);
            ApplyNullableSettingWithDefault(ref effect.m_extraRadius, settings.m_extraRadius, baseEffect.m_extraRadius);
            effect.m_intensityCurve = baseEffect.m_intensityCurve;
            ApplyNullableSettingWithDefault(ref effect.m_maxLifeTime, settings.m_maxLifeTime, baseEffect.m_maxLifeTime);
            ApplyNullableSettingWithDefault(ref effect.m_maxSpawnAngle, settings.m_maxSpawnAngle, baseEffect.m_maxSpawnAngle);
            ApplyNullableSettingWithDefault(ref effect.m_maxStartSpeed, settings.m_maxStartSpeed, baseEffect.m_maxStartSpeed);
            ApplyNullableSettingWithDefault(ref effect.m_maxVisibilityDistance, settings.m_maxVisibilityDistance, baseEffect.m_maxVisibilityDistance);
            ApplyNullableSettingWithDefault(ref effect.m_minLifeTime, settings.m_minLifeTime, baseEffect.m_minLifeTime);
            ApplyNullableSettingWithDefault(ref effect.m_minSpawnAngle, settings.m_minSpawnAngle, baseEffect.m_minSpawnAngle);
            ApplyNullableSettingWithDefault(ref effect.m_minStartSpeed, settings.m_minStartSpeed, baseEffect.m_minStartSpeed);
            ApplyNullableSettingWithDefault(ref effect.m_renderDuration, settings.m_renderDuration, baseEffect.m_renderDuration);
            ApplyNullableSettingWithDefault(ref effect.m_useSimulationTime, settings.m_useSimulationTime, baseEffect.m_useSimulationTime);

            var baseMovementEffect = baseEffect as MovementParticleEffect;
            if(baseMovementEffect != null)
            {
                ApplyNullableSettingWithDefault(ref effect.m_magnitudeAccelerationMultiplier,
                    settings.m_magnitudeAccelerationMultiplier,
                    baseMovementEffect.m_magnitudeAccelerationMultiplier);
                ApplyNullableSettingWithDefault(ref effect.m_magnitudeSpeedMultiplier, 
                    settings.m_magnitudeSpeedMultiplier, 
                    baseMovementEffect.m_magnitudeSpeedMultiplier);
                ApplyNullableSettingWithDefault(ref effect.m_minMagnitude, settings.m_minMagnitude, baseMovementEffect.m_minMagnitude);
            }
            else
            {
                ApplyNullableSetting(ref effect.m_magnitudeAccelerationMultiplier, settings.m_magnitudeAccelerationMultiplier);
                ApplyNullableSetting(ref effect.m_magnitudeSpeedMultiplier, settings.m_magnitudeSpeedMultiplier);
                ApplyNullableSetting(ref effect.m_minMagnitude, settings.m_minMagnitude);
            }

            var baseCustomMovementEffect = baseEffect as CustomMovementParticleEffect;
            if(baseCustomMovementEffect != null)
            {
                ApplyNullableSettingWithDefault(ref effect.m_velocityMultiplier, settings.m_velocityMultiplier, baseCustomMovementEffect.m_velocityMultiplier);
                ApplyNullableSettingWithDefault(ref effect.m_spawnAreaRadius, settings.m_spawnAreaRadius, baseCustomMovementEffect.m_spawnAreaRadius);
            }
            else
            {
                ApplyNullableSetting(ref effect.m_velocityMultiplier, settings.m_velocityMultiplier);
                ApplyNullableSetting(ref effect.m_spawnAreaRadius, settings.m_spawnAreaRadius);
            }

            effect.name = settings.Name;

            // Specify particle system to use
            effect.ParticleSystemOverride = baseEffect.gameObject.GetComponent<ParticleSystem>();

            return effect;
        }
    }
}
