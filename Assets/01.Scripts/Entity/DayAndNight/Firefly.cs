using UnityEngine;
public class Firefly : MonoBehaviour
{
    private void OnEnable()
    {
        DayNightCycle.OnNightStarted += SpawnSelf;
        DayNightCycle.OnDayStarted += DespawnSelf;
    }

    private void OnDisable()
    {
        DayNightCycle.OnNightStarted -= SpawnSelf;
        DayNightCycle.OnDayStarted -= DespawnSelf;
    }

    private void SpawnSelf()
    {
        gameObject.SetActive(true);
    }

    private void DespawnSelf()
    {
        gameObject.SetActive(false);
    }
}
