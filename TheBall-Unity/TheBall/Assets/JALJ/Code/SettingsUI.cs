using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsUI : MonoBehaviour {

    #region Public variables - Default Settings
    [Header("Default Settings")]

    /// <summary>
    /// A massa da bola.
    /// </summary>
    [Tooltip("Massa da bola")]
    public float mass = 1f;

    /// <summary>
    /// O coeficiente de restituição do material da bola.
    /// </summary>
    [Tooltip("Coeficiente de restituição")]
    [Range(0, 1)]
    public float restitution = .6f;

    /// <summary>
    /// Velocidade inicial.
    /// </summary>
    [Tooltip("Velocidade inicial")]
    [Range(1, 25)]
    public float initialVelocity = 10f;

    /// <summary>
    /// Ângulo do lançamento.
    /// </summary>
    [Tooltip("Ângulo do lançamento")]
    [Range(0, 90)]
    public float slope = 60f;

    /// <summary>
    /// Escala do tempo.
    /// </summary>
    [Tooltip("Escala do tempo")]
    [Range(.1f, 1f)]
    public float timeScale = 1f;

    #endregion Public variables

    #region Public Variables - References

    [Header("Object References")]

    public Projectile theBall;

    [Header("UI References")]

    public InputField uiInputMass;
    public Slider uiSliderRestitution;
    public Slider uiSliderSlope;
    public InputField uiInputVelocity;
    public Slider uiSliderTimeScale;

    #endregion Public Variables

    void Awake()
    {
        LoadDefaults();
        SyncSettings();
    }

    public void Simulate()
    {
        SyncSettings();
        theBall.Simulate();
    }

    public void LoadDefaults()
    {
        uiInputMass.text = mass.ToString();
        uiSliderRestitution.value = restitution;
        uiSliderSlope.value = slope;
        uiInputVelocity.text = initialVelocity.ToString();
        uiSliderTimeScale.value = timeScale;
    }

    void SyncSettings()
    {
        theBall.mass = float.Parse(uiInputMass.text);
        theBall.restitution = uiSliderRestitution.value;
        theBall.slope = uiSliderSlope.value;
        theBall.initialVelocity = float.Parse(uiInputVelocity.text);

        WorldController.WC.timeScale = uiSliderTimeScale.value;
        Time.timeScale = WorldController.WC.timeScale;
    }
}
