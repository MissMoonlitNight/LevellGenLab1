using UnityEngine;

/// <summary>
/// Простое управление игроком с помощью клавиш WASD и пробела.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.6f;

    private Rigidbody rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Получаем ввод с клавиатуры (WASD или стрелки)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Создаём вектор движения
        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        // Применяем движение к Rigidbody
        rb.velocity = new Vector3(move.x * speed, rb.velocity.y, move.z * speed);

        // Проверка: стоит ли игрок на земле
        isGrounded = Physics.CheckSphere(transform.position - Vector3.up * 0.5f, 0.3f, groundMask);

        // Прыжок (пробел)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Отладка: рисуем сферу проверки земли в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * 0.5f, 0.3f);
    }
}