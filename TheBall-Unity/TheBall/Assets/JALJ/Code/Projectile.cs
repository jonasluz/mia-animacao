using UnityEngine;

/// <summary>
/// Projétil de lançamento oblíquo.
/// </summary>
public class Projectile : MonoBehaviour {

    #region Public variables

    [Header("Ball")]

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

    [Header("Oblique Movement")]

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
    /// Tolerância da restituição.
    /// </summary>
    [Tooltip("Tolerância para da restituição (novo salto), em termos de percentual do diâmetro da bola.")]
    [Range(.1f, 1f)]
    public float restitutionTolerance = .1f;

    #endregion Public variables

    #region Private variables

    // Posição inicial da bola.
    private Vector3 m_initialPosition;

    // Se deve continuar simulação.
    private bool m_simulate = false;

    // Tempo do lançamento.
    private float m_time;
    // Componentes iniciais da velocidade.
    private float m_vx0, m_vy0;
    // Velocidade instantânea em y. 
    private float m_vy;
    // Altura máxima atingida.
    private float m_H;
    // Alcance máximo atingido.
    private float m_A;
    // Energia potencial máxima (no topo).
    private float m_potential;
    // Energia cinética.
    private float m_kinetic, m_kinecticFactor;

    // Gravidade e seu fator de multiplicação.
    private float m_gravity, m_gravityFactor;
    
    // Raio da bola.
    private float ray;

    #endregion Private variables

    #region Monobehaviour

    void Awake()
    {
        // Calcula o raio da bola.
        ray = GetComponent<MeshRenderer>().bounds.size.y / 2;
        // Inicializa.
        Simulate();
    }

    void FixedUpdate()
    {
        if (m_simulate)
        {
            // Atualiza o contador de tempo.
            m_time += Time.fixedDeltaTime;
            // Determina e atualiza a nova posição e rotação da bola.
            ObliqueMovement(Time.fixedDeltaTime);
        }
    }

    #endregion Monobehaviour 

    public void Simulate()
    {
        // Vai para a posição inicial.
        transform.localPosition = new Vector3(0, ray, 0);

        // Gravidade do mundo.
        m_gravity = WorldController.WC.gravity;

        // Calcula o fator de multiplicação da gravidade.
        m_gravityFactor = m_gravity / 2;
        // Calcula o fator de cálculo da energia cinética.
        m_kinecticFactor = mass / 2;

        // Inicializa o primeiro lançamento.
        InitMovement(true);
        m_simulate = true;
    }

    /* Inicializa o próximo lançamento. */
    void InitMovement(bool first = false)
    {
        // Zera o contador de tempo.
        m_time = 0;
        // Atualiza a nova posição inicial.
        m_initialPosition = transform.localPosition;

        // Calcula a restituição do movimento.
        if (!first) initialVelocity *= restitution;

        // Calcula as componentes iniciais da velocidade.
        m_vx0 = initialVelocity * Mathf.Cos(Mathf.Deg2Rad * slope);
        m_vy0 = initialVelocity * Mathf.Sin(Mathf.Deg2Rad * slope);

        // Calcula a altura máxima.
        m_H = m_vy0 * m_vy0 / (2 * m_gravity);
        // Calcula o alcance máximo.
        m_A = initialVelocity * initialVelocity * Mathf.Sin(Mathf.Deg2Rad * slope * 2) / m_gravity;

        // Determina as flags de continuidade ...
        float tolerance = (restitutionTolerance * ray * 2);
        // ... continua a restituir saltos?
        if (m_H <= tolerance)
            slope = 0;
        // ... continua a simulação?
        m_simulate = m_vx0 > tolerance * tolerance;

        // Calcula a energia potencial máxima (no topo).
        m_potential = mass * m_gravity * m_H;

    }

    /* Coordenadas do movimento oblíquo. */
    private void ObliqueMovement(float deltaTime)
    {
        float x, y;

        if (slope != 0)
        { // há restituição (novo salto).
            if (transform.localPosition.y < ray)
            {   // Bola no chão...
                // inicia novo salto.
                InitMovement();
                // y é ajustado.
                y = ray - transform.localPosition.y;
            }
            else
            {   // Bola no ar...
                // y é função parabólica em função do tempo, na forma a*t + b*t^2.
                y = m_vy0 * m_time - m_gravityFactor * m_time * m_time;
            }
            // Atualiza a velocidade em y.
            m_vy = m_vy0 + m_gravity * m_time;
            // Atualiza a energia cinética.
            m_kinetic = m_kinecticFactor * m_vy * m_vy;

            // x é função linear do tempo a partir da velocidade inicial em x, constante.
            x = m_vx0 * m_time;
        }
        else
        { // sem restituição (novo salto).
            // usa para atualizar x.
            InitMovement();
            //  em y, tudo zera.
            m_kinetic = m_vy = y = 0;

            // x é função linear do tempo a partir da velocidade inicial em x, constante.
            x = m_vx0 * deltaTime;
        }

        transform.localPosition = m_initialPosition + new Vector3(x, y, 0);
        transform.Rotate(0, 0, -x / ray);
    }
}
