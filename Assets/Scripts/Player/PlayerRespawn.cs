using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpoint;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uiManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    private void Update()
    {
        // Kiểm tra nếu rơi xuống dưới -8 thì respawn
        if (transform.position.y < -8f)
        {
            CheckRespawn();
            // GetComponent<Room>().ActiveDoor();
        }
    }

    public void CheckRespawn()
    {
        // Check if check point available
        if (currentCheckpoint == null)
        {
            // Show game over screen
            uiManager.GameOver();

            return;
        }

        playerHealth.Respawn(); //Restore player health and reset animation
        transform.position = currentCheckpoint.position; //Move player to checkpoint location

        // Lấy phòng chứa checkpoint
        Room room = currentCheckpoint.parent.GetComponent<Room>();

        // Đóng cửa lại khi respawn
        room.ActiveDoor();

        room.ActivateRoom(true);

        //Move the camera to the checkpoint's room
        Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckpoint.parent);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform;
            SoundManager.instance.PlaySound(checkpoint);
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("appear");
        }
    }
}
