using System;
using System.Collections.Generic;
using Lean.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using Particle = UnityEngine.ParticleSystem.Particle;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemPrefabSpawner : MonoBehaviour
{
    [SerializeField] public ParticleSystem particles;
    
    private Particle[] _particleStorage = Array.Empty<Particle>();
    
    private void Awake()
    {
        Array.Resize(ref _particleStorage, particles.main.maxParticles);
    }
    
    [Button]
    public List<T> Spawn<T>(T prefab, int count) where T : Component
    {
        gameObject.SetActive(true);
        
        particles.Emit(count);
        
        particles.GetParticles(_particleStorage, count, particles.particleCount - count);
        
        List<T> spawnedPrefabs = new(count);
        
        for (int i = 0; i < count; i++)
        {
            Particle particle = _particleStorage[i];
            
            spawnedPrefabs.Add(LeanPool.Spawn(prefab, particle.position, Quaternion.FromToRotation(Vector3.up, particle.velocity)));
        }
        
        //gameObject.SetActive(false);  // TODO: may need to disable after delay if we play the particle system/run coroutine
        
        return spawnedPrefabs;
    }
    
    private void Reset()
    {
        particles = GetComponent<ParticleSystem>();
    }
}
