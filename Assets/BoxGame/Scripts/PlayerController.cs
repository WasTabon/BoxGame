using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Follow")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 10, -5);
    [SerializeField] private float cameraFollowSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;   // Ссылка на Animator
    [SerializeField] private string runBoolName = "Run"; // Название параметра в Animator

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector2 input = joystick.Direction;
        Vector3 move = new Vector3(input.x, 0f, input.y);

        // Двигаем игрока
        rb.velocity = move * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        // Поворот игрока в сторону движения
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Обновляем анимацию (бежит / стоит)
        if (animator != null)
        {
            bool isRunning = move.sqrMagnitude > 0.01f;
            animator.SetBool(runBoolName, isRunning);
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Целевая позиция камеры
        Vector3 targetPosition = transform.position + cameraOffset;

        // Плавное перемещение
        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            targetPosition,
            cameraFollowSpeed * Time.deltaTime
        );
    }
}