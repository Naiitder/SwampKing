using UnityEngine;

public class Platform : MonoBehaviour
{
    public float velocidad = 2f;
    public float vaivenAmplitude = 0.1f;
    public float vaivenSpeed = 2f;
    public float delayAntesDeVolver = 2f;

    private Vector3 startPos;
    [SerializeField] private Vector3 targetPos;
    private Vector3 destinoActual;
    private bool enMovimiento = false;
    private bool jugadorEncima = false;
    private float tiempoFuera = 0f;
    private float tiempoTotal = 0f;
    private bool yaLlegoADestino = false;

    void Start()
    {
        startPos = transform.position;
        destinoActual = startPos;
    }

    void Update()
    {
        tiempoTotal += Time.deltaTime;
        float vaiven = Mathf.Sin(tiempoTotal * vaivenSpeed) * vaivenAmplitude;
        
        Vector3 objetivoConVaiven = new Vector3(destinoActual.x, destinoActual.y + vaiven, destinoActual.z);

        if (enMovimiento)
        {
            transform.position = Vector3.MoveTowards(transform.position, objetivoConVaiven, velocidad * Time.deltaTime);

            if (Vector3.Distance(transform.position, destinoActual) < 0.01f)
            {
                enMovimiento = false;

                if (destinoActual == targetPos)
                {
                    yaLlegoADestino = true;
                }
            }
        }
        else
        {
            transform.position = new Vector3(transform.position.x, objetivoConVaiven.y, transform.position.z);
        }
        
        if (!jugadorEncima && !yaLlegoADestino)
        {
            tiempoFuera += Time.deltaTime;

            if (tiempoFuera >= delayAntesDeVolver)
            {
                destinoActual = startPos;
                enMovimiento = true;
                tiempoFuera = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEncima = true;
            tiempoFuera = 0f;

            if (yaLlegoADestino)
            {
                yaLlegoADestino = false;
                destinoActual = startPos;
                enMovimiento = true;
            }
            else
            {
                destinoActual = targetPos;
                enMovimiento = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEncima = false;
            tiempoFuera = 0f;
        }
    }
}