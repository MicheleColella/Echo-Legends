using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCollector : MonoBehaviour
{
    ParticleSystem ps;
    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
    WeaponSystem weaponSystem; // Riferimento a WeaponSystem

    // Proprietà pubblica per specificare il numero di particelle da creare
    public int numberOfParticlesToCreate = 10;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();

        // Trova lo script WeaponSystem nella scena
        weaponSystem = FindObjectOfType<WeaponSystem>();
        if (weaponSystem == null)
        {
            Debug.LogError("WeaponSystem non trovato nella scena");
            return;
        }

        // Usa Find per trovare il Collider con il nome "AmmoCollector"
        GameObject triggerObject = GameObject.Find("AmmoCollector");
        if (triggerObject != null)
        {
            Collider triggerCollider = triggerObject.GetComponent<Collider>();
            if (triggerCollider != null)
            {
                var trigger = ps.trigger;
                trigger.SetCollider(0, triggerCollider);
            }
            else
            {
                Debug.LogError("Collider non trovato nell'oggetto AmmoCollector");
            }
        }
        else
        {
            Debug.LogError("Oggetto AmmoCollector non trovato nella scena");
        }

        // Assicurati che il ParticleSystem sia fermo prima di impostare le particelle
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        CreateParticles(numberOfParticlesToCreate + 1);
    }

    private void OnParticleTrigger()
    {
        int triggeredParticles = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        for (int i = 0; i < triggeredParticles; i++)
        {
            ParticleSystem.Particle p = particles[i];
            p.remainingLifetime = 0;
            Debug.Log("Collected particle");
            particles[i] = p;

            // Chiama AddAmmo su WeaponSystem
            weaponSystem.AddAmmo(1);
        }

        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }

    public void CreateParticles(int numParticles)
    {
        var main = ps.main;
        var emission = ps.emission;

        // Calcola la durata dell'emissione in base al numero di particelle
        float duration = CalculateEmissionDuration(numParticles);

        main.duration = duration;
        main.loop = false;

        // Imposta la rate over time in modo che sia proporzionale al numero di particelle e alla durata
        emission.rateOverTime = numParticles / duration;
        emission.rateOverTime = Mathf.Ceil(emission.rateOverTime.constant); // Arrotonda per eccesso

        // Assicurati che il sistema non emetta più particelle del necessario
        emission.SetBursts(new ParticleSystem.Burst[0]); // Rimuovi tutti i burst

        // Avvia il sistema delle particelle
        ps.Play();

        // Utilizza una coroutine per fermare il sistema dopo che tutte le particelle sono state emesse
        StartCoroutine(StopParticlesAfterDuration(duration));
    }

    private float CalculateEmissionDuration(int numParticles)
    {
        // Definisci una proporzione base, ad esempio, 1 secondo ogni 10 particelle
        float baseDurationPerTenParticles = 1.0f;

        // Calcola la durata proporzionale al numero di particelle
        float duration = (numParticles / 10.0f) * baseDurationPerTenParticles;

        // Aggiungi una durata minima per garantire che il sistema abbia il tempo di emettere le particelle
        float minDuration = 1.1f;
        return Mathf.Max(duration, minDuration);
    }

    private IEnumerator StopParticlesAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        ps.Stop();
    }
}
