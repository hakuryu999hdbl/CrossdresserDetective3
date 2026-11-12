using System.Collections.Generic;
using UnityEngine;

// Shared by all observers of this player. Reloading the level resets exposure.
public class PlayerSuspicion : MonoBehaviour
{
    public float CurrentSuspicion { get; private set; }
    public bool IsExposed { get; private set; }
    public bool CombatLocked { get; private set; }

    public void BeginUndercover()
    {
        if (!IsExposed) CombatLocked = true;
    }
    private readonly List<SurveillanceMonitor> monitors = new List<SurveillanceMonitor>();
    private PlayerController player;
    private float maximum = 100, recovery = 10;

    public void Register(SurveillanceMonitor monitor)
    {
        if (IsExposed) { monitor.Reveal(); return; }
        if (monitors.Contains(monitor)) return;
        if (monitors.Count == 0)
        {
            maximum = Mathf.Max(1, monitor.maxSuspicion);
            recovery = monitor.recoveryPerSecond;
        }
        monitors.Add(monitor);
    }

    private void Awake() { player = GetComponent<PlayerController>(); }

    private void LateUpdate()
    {
        if (UIManager.instance != null)
            UIManager.instance.SetUndercoverCombatPrompts(player, CombatLocked);
        if (IsExposed) return;
        if (player.isDead || player.isInCutscene ||
            (GameManager.instance != null && (GameManager.instance.gameOver || GameManager.instance.PlayerWin)))
        {
            if (UIManager.instance != null) UIManager.instance.HideSuspicionBar();
            return;
        }
        float gain = 0;
        monitors.RemoveAll(m => m == null);
        foreach (var monitor in monitors)
            if (monitor.CanSee(player)) gain = Mathf.Max(gain, monitor.suspicionPerSecond);
        CurrentSuspicion = Mathf.Clamp(CurrentSuspicion + (gain > 0 ? gain : -recovery) * Time.deltaTime, 0, maximum);
        if (CurrentSuspicion >= maximum) { Expose(); return; }
        if (UIManager.instance != null)
            UIManager.instance.UpdateSuspicionBar(CurrentSuspicion, maximum, monitors.Count > 0);
    }

    public void Expose()
    {
        if (IsExposed) return;
        IsExposed = true;
        CombatLocked = false;
        foreach (var enemy in FindObjectsByType<EnemyController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (enemy.IsNpc) enemy.RevealAsHostile();
        foreach (var monitor in monitors)
            if (monitor != null) monitor.Reveal();
        if (UIManager.instance != null)
        {
            UIManager.instance.HideSuspicionBar();
            UIManager.instance.SetUndercoverCombatPrompts(player, false);
            UIManager.instance.ShowDiscoveredAlert();
        }
    }

    private void OnDisable()
    {
        if (UIManager.instance != null) UIManager.instance.HideSuspicionBar();
        if (UIManager.instance != null) UIManager.instance.SetUndercoverCombatPrompts(player, false);
    }
}
