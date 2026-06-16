using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // ── Speed ────────────────────────────────────────────────────────────────
    [Header("Speed")]
    [SerializeField] float baseSpeed     = 5f;    // always-on forward speed
    [SerializeField] float boostPerPress = 1f;    // boostLevel added per Space press
    [SerializeField] float maxBoostLevel = 5f;    // cap — 5 stacked presses = max surge
    [SerializeField] float boostToSpeed  = 2.5f;  // extra units/sec per boostLevel

    // ── Decay ────────────────────────────────────────────────────────────────
    [Header("Decay")]
    [SerializeField] float boostDecay = 0.3f;  // boostLevel/sec lost naturally
    [SerializeField] float brakeDecay = 1.5f;  // boostLevel/sec lost while B held

    // ── Steering ─────────────────────────────────────────────────────────────
    [Header("Steering")]
    [SerializeField] float maxTurnRate   = 120f;  // degrees/sec at full mouse deflection
    [SerializeField] float turnDrag      = 0.4f;  // boostLevel/sec drained at max turn
    [SerializeField] float mouseDeadzone = 0.05f; // fraction of half-screen to ignore

    // ── Internal state ────────────────────────────────────────────────────────
    // FUTURE: change to GameState.Start when the start screen is ready.
    // Space handling below already checks this and has a stub for the transition.
    enum GameState { Start, Playing }
    GameState gameState = GameState.Playing;

    float boostLevel = 0f;
    float CurrentSpeed => baseSpeed + boostLevel * boostToSpeed;

    // ─────────────────────────────────────────────────────────────────────────

    void Update()
    {
        if (gameState != GameState.Playing) return;

        HandleBoost();
        float turnInput = GetTurnInput();
        ApplyTurn(turnInput);
        ApplyTurnDrag(turnInput);
        ApplyBoostDecay();
        Move();
    }

    void HandleBoost()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // FUTURE: if (gameState == GameState.Start) { gameState = GameState.Playing; return; }
            boostLevel = Mathf.Min(boostLevel + boostPerPress, maxBoostLevel);
        }
    }

    // Returns -1 (full left) to 1 (full right) based on mouse X vs screen centre.
    float GetTurnInput()
    {
        float mouseX    = Mouse.current.position.ReadValue().x;
        float halfWidth = Screen.width * 0.5f;
        float raw       = (mouseX - halfWidth) / halfWidth;

        if (Mathf.Abs(raw) < mouseDeadzone) return 0f;
        return Mathf.Sign(raw) * (Mathf.Abs(raw) - mouseDeadzone) / (1f - mouseDeadzone);
    }

    void ApplyTurn(float turnInput)
    {
        transform.Rotate(0f, turnInput * maxTurnRate * Time.deltaTime, 0f);
    }

    // Sharp turns bleed boost — the tighter the arc, the more surge you lose.
    void ApplyTurnDrag(float turnInput)
    {
        float sharpness = Mathf.Abs(turnInput); // 0 = straight, 1 = maximum turn
        boostLevel = Mathf.Max(0f, boostLevel - sharpness * turnDrag * Time.deltaTime);
    }

    void ApplyBoostDecay()
    {
        float decay = Keyboard.current.bKey.isPressed ? brakeDecay : boostDecay;
        boostLevel  = Mathf.Max(0f, boostLevel - decay * Time.deltaTime);
    }

    void Move()
    {
        transform.Translate(0f, 0f, CurrentSpeed * Time.deltaTime);
    }

    // ── Debug info (remove when done) ────────────────────────────────────────
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Boost: {boostLevel:F1} / {maxBoostLevel}");
        GUI.Label(new Rect(10, 30, 200, 20), $"Speed: {CurrentSpeed:F1} u/s");
    }
}
