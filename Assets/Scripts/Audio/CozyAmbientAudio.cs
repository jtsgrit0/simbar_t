using UnityEngine;

public class CozyAmbientAudio : MonoBehaviour
{
    public AudioClip ambientLoop;
    public AudioSource source;

    private void Awake()
    {
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0.35f;
    }

    private void Start()
    {
        if (ambientLoop == null)
        {
            CreateFallbackTone();
        }

        if (source != null && ambientLoop != null)
        {
            source.clip = ambientLoop;
            source.Play();
        }
    }

    private void CreateFallbackTone()
    {
        const int sampleRate = 22050;
        const int lengthSeconds = 2;
        var samples = new float[sampleRate * lengthSeconds];

        for (int i = 0; i < samples.Length; i++)
        {
            float t = i / (float)sampleRate;
            float tone = Mathf.Sin(2f * Mathf.PI * 220f * t) * 0.25f;
            float pad = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.15f;
            samples[i] = tone + pad;
        }

        var clip = AudioClip.Create("FallbackCozyTone", samples.Length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        ambientLoop = clip;
    }
}
