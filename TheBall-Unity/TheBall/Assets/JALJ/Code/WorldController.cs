using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {

    #region Static Data
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static WorldController WC
    {
        get; private set;
    }
    #endregion Static Data

    #region Public variables
    [Header("Settings")]

    public float gravity;

    [Range(.1f, 1f)]
    public float timeScale = 1f;

    #endregion Public variables

    #region Monobehaviour

    void Awake()
    {
        // Singleton.
        if (WC == null) WC = this;
        else Destroy(this);

        DontDestroyOnLoad(gameObject);

        Time.timeScale = timeScale;
    }

    #endregion Monobehaviour

}
