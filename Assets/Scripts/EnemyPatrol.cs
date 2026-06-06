using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    private Transform currentTarget;
    private Rigidbody2D rb;

    void Start()
    {
        currentTarget = pointA;
        rb = GetComponent<Rigidbody2D>(); // Подключаем физику
    }

    // Вся физика пишется строго в FixedUpdate
    void FixedUpdate()
    {
        // 1. Вычисляем направление. Если цель правее нас — идем вправо (1), если левее — влево (-1)
        float direction = currentTarget.position.x > transform.position.x ? 1f : -1f;

        // 2. Двигаем врага через физическую скорость (как мы это делали в скрипте Player)
        rb.velocity = new Vector2(direction * speed, rb.velocity.y);

        // 3. Проверяем дистанцию ТОЛЬКО по оси X (Mathf.Abs отсекает минусы)
        // Теперь неважно, на какой высоте стоят точки A и B. Враг не будет пытаться вкопаться в землю.
        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.2f)
        {
            if (currentTarget == pointA)
            {
                currentTarget = pointB;
                Flip(1f);
            }
            else
            {
                currentTarget = pointA;
                Flip(-1f);
            }
        }
    }

    void Flip(float direction)
    {
        Vector3 localScale = transform.localScale;
        
        if (direction > 0f)
            localScale.x = Mathf.Abs(localScale.x);
        else
            localScale.x = -Mathf.Abs(localScale.x);
            
        transform.localScale = localScale;
    }
}