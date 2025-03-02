using System;
using Pathfinding;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] fallingPlatforms;
    [SerializeField] public GameObject door;
    [SerializeField] public GameObject endPoint;

    private Vector3[] initialPosition;
    private bool isDoorRemoved = false; // Biến kiểm tra đã xóa cửa chưa

    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        // Save the initial positions of the enemies
        initialPosition = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                initialPosition[i] = enemies[i].transform.position;
            }
        }
    }

    private void Update()
    {
        if (!isDoorRemoved && isPermission())
        {
            RemoveDoor();
        }

        if (isPermission())
        {
            ShowEndPoint();
        }
    }

    public void ActivateRoom(bool _status)
    {
        if (_status)
        {
            ActiveDoor();
        }

        // Activate/deactivate enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].SetActive(_status);
                enemies[i].GetComponent<Health>().Respawn();
                enemies[i].transform.position = initialPosition[i];
            }

            // 🔹 Reset lại AIPath của A* Pathfinding
            AIPath aiPath = enemies[i].GetComponent<AIPath>();
            if (aiPath != null)
            {
                aiPath.enabled = false; // Tắt để reset
                aiPath.Teleport(initialPosition[i]); // Reset vị trí trong hệ thống pathfinding
                aiPath.enabled = true; // Bật lại AI
            }

            EnableEnemyScripts(enemies[i], _status);


            // 🔹 Cập nhật Graph của A* để tránh lỗi đường đi
            AstarPath.active.Scan(); // Quét lại bản đồ để cập nhật

            // 🔹 Reset Rigidbody2D (nếu có)
            Rigidbody2D rb = enemies[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Reset vận tốc
                rb.simulated = true; // Bật lại vật lý
            }

            // 🔹 Reset Animator (nếu có)
            Animator anim = enemies[i].GetComponent<Animator>();
            if (anim != null)
            {
                anim.Rebind(); // Reset animation
                anim.Update(0f);
            }

        }
    }

    public bool isPermission()
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }


    private void RemoveDoor()
    {
        if (door != null)
        {
            door.SetActive(false);
            isDoorRemoved = true; // Đánh dấu đã xóa cửa
        }
    }

    public void ActiveDoor()
    {
        if (door != null)
        {
            door.SetActive(true);
            isDoorRemoved = false;
        }
    }



    public void ShowEndPoint()
    {
        if (endPoint != null)
        {
            endPoint.SetActive(true);
        }
    }


    // 🔥 Hàm bật lại tất cả script trên Enemy
    private void EnableEnemyScripts(GameObject enemy, bool status)
    {
        MonoBehaviour[] scripts = enemy.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            Type scriptType = script.GetType();
            if (scriptType.Name == "GoblinEnemy" || scriptType.Name == "FlyingEyeEnemy" || scriptType.Name == "MeleeEnemy"
            || scriptType.Name == "SkeletonEnemy" || scriptType.Name == "WizardEnemy" || scriptType.Name == "GrimReaperEnemy")
            {
                script.enabled = status;
            }
        }
    }
}
