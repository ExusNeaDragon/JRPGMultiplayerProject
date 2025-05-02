using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource), typeof(Collider2D))]
public class AmbienceSound : MonoBehaviour
{
    private Collider2D area;
    private AudioSource audioSource;
    public float fadeSpeed = 1f;
    public float maxVolume = 1f;

    private float targetVolume = 0f;

    void Start()
    {
        area = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
        audioSource.Play();

        AmbienceManager.Instance?.Register(this);
    }

    void Update()
    {
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
    }

    public void SetTargetVolume(float volume)
    {
        targetVolume = volume;
    }

    public Vector2 GetClosestPoint(Vector2 position)
    {
        return area.ClosestPoint(position);
    }
}
