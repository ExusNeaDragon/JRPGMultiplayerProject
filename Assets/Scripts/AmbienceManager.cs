using System.Collections.Generic;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    public static AmbienceManager Instance;
    private List<AmbienceSound> allZones = new();
    private GameObject player;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player == null || allZones.Count == 0) return;

        AmbienceSound closest = null;
        float minDist = float.MaxValue;

        foreach (var zone in allZones)
        {
            float dist = Vector2.Distance(player.transform.position, zone.GetClosestPoint(player.transform.position));
            if (dist < minDist)
            {
                minDist = dist;
                closest = zone;
            }
        }

        foreach (var zone in allZones)
        {
            zone.SetTargetVolume(zone == closest ? zone.maxVolume : 0f);
        }
    }

    public void Register(AmbienceSound zone)
    {
        if (!allZones.Contains(zone))
            allZones.Add(zone);
    }
}
