using UnityEngine;

public class BreakDebrisEffect : MonoBehaviour
{
    [SerializeField] private FrameEvent_Audio frameEventAudio;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private Material particleMaterial;
    [SerializeField] private Texture2D debrisTexture;
    [SerializeField] private int particleCount;
    [SerializeField] private Vector2 particleSize;
    [SerializeField] private Vector2 horizontalSpeed;
    [SerializeField] private Vector2 verticalSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float particleLifetime;

    public void Spawn(Bounds bounds, SpriteRenderer source)
    {
        BreakDebrisEffect effect = Instantiate(this, bounds.center, Quaternion.identity);
        effect.Play(bounds.extents, source);
    }

    private void Play(Vector3 extents, SpriteRenderer source)
    {
        ParticleSystem particles = gameObject.AddComponent<ParticleSystem>();
        particles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = particles.main;
        main.playOnAwake = false;
        main.loop = false;
        main.duration = particleLifetime;
        main.maxParticles = particleCount;
        main.gravityModifier = gravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particles.emission;
        emission.enabled = false;
        var shape = particles.shape;
        shape.enabled = false;

        var rotation = particles.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-5f, 5f);

        var fade = particles.colorOverLifetime;
        fade.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0.55f), new GradientAlphaKey(0f, 1f) });
        fade.color = gradient;

        var debrisRenderer = GetComponent<ParticleSystemRenderer>();
        debrisRenderer.sharedMaterial = particleMaterial;
        if (source != null)
        {
            debrisRenderer.sortingLayerID = source.sortingLayerID;
            debrisRenderer.sortingOrder = source.sortingOrder + 1;
        }

        var properties = new MaterialPropertyBlock();
        properties.SetTexture("_MainTex", debrisTexture);
        debrisRenderer.SetPropertyBlock(properties);

        particles.Play();
        for (int i = 0; i < particleCount; i++)
        {
            var fragment = new ParticleSystem.EmitParams
            {
                position = transform.position + new Vector3(
                    Random.Range(-extents.x, extents.x),
                    Random.Range(-extents.y, extents.y)),
                velocity = new Vector3(Random.Range(horizontalSpeed.x, horizontalSpeed.y),
                    Random.Range(verticalSpeed.x, verticalSpeed.y)),
                rotation = Random.Range(0f, 360f),
                startSize = Random.Range(particleSize.x, particleSize.y),
                startLifetime = Random.Range(particleLifetime * 0.65f, particleLifetime)
            };
            particles.Emit(fragment, 1);
        }

        frameEventAudio.PlayClip(breakSound);
        Destroy(gameObject, Mathf.Max(particleLifetime, breakSound.length) + 0.1f);
    }
}
