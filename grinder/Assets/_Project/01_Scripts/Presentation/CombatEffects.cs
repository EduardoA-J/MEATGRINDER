using Meatgrinder.Core;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    public sealed class CombatEffects : MonoBehaviour
    {
        [SerializeField] private Material sparkMaterial;
        [SerializeField] private AudioClip explosionSound;
        readonly Transform[] sparks = new Transform[48];
        readonly Vector3[] velocity = new Vector3[48];
        readonly float[] lifetime = new float[48];
        MaterialPropertyBlock block;
        LocalProjectileSpawner spawner;
        PlayerImpactFeedback feedback;
        LocalGameSession session;
        int cursor;
        AudioSource source;
        void Awake()
        {
            block = new MaterialPropertyBlock();
            spawner = GetComponent<LocalProjectileSpawner>(); feedback = GetComponent<PlayerImpactFeedback>();
            source = gameObject.AddComponent<AudioSource>();
            for (int i = 0; i < sparks.Length; i++)
            {
                var spark = GameObject.CreatePrimitive(PrimitiveType.Sphere); spark.name = "PooledCombatSpark";
                Destroy(spark.GetComponent<Collider>()); spark.GetComponent<Renderer>().sharedMaterial = sparkMaterial;
                sparks[i] = spark.transform; spark.SetActive(false);
            }
        }
        void Start() => session = FindFirstObjectByType<LocalGameSession>();
        void OnEnable() { if (spawner != null) spawner.Spawned += Spawned; if (feedback != null) feedback.PresentedAt += Impact; }
        void OnDisable() { if (spawner != null) spawner.Spawned -= Spawned; if (feedback != null) feedback.PresentedAt -= Impact; }
        void Spawned(RocketProjectile projectile) => projectile.Exploded += Explosion;
        void Explosion(Vector3 point)
        {
            Burst(point, new Color(1, .3f, .04f), 18, 8);
            if (explosionSound != null) source.PlayOneShot(explosionSound, .8f);
        }
        void Impact(Vector3 point, bool target) => Burst(point, target ? new Color(.6f, .035f, .02f) : new Color(1, .7f, .3f), target ? 4 : 2, 2);
        void Burst(Vector3 point, Color color, int count, float speed)
        {
            if (session != null && session.ReduceFlashes) count = Mathf.Max(1, count / 3);
            for (int i = 0; i < count; i++)
            {
                int index = cursor++ % sparks.Length;
                var spark = sparks[index]; spark.position = point; spark.localScale = Vector3.one * (speed > 3 ? .22f : .065f);
                velocity[index] = Random.onUnitSphere * speed + Vector3.up * 2;
                lifetime[index] = speed > 3 ? .65f : .3f;
                block.SetColor("_BaseColor", color); block.SetColor("_EmissionColor", color * .8f);
                spark.GetComponent<Renderer>().SetPropertyBlock(block); spark.gameObject.SetActive(true);
            }
        }
        void Update()
        {
            for (int i = 0; i < sparks.Length; i++)
            {
                if (lifetime[i] <= 0) continue;
                lifetime[i] -= Time.deltaTime;
                sparks[i].position += velocity[i] * Time.deltaTime;
                velocity[i] += Vector3.down * (9 * Time.deltaTime);
                if (lifetime[i] <= 0) sparks[i].gameObject.SetActive(false);
            }
        }
        void OnDestroy() { foreach (var spark in sparks) if (spark != null) Destroy(spark.gameObject); }
    }
}
