using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] public string nextLevel;
    private GameObject endpoint;

    private void Start()
    {
        endpoint = GameObject.FindGameObjectWithTag("Endpoint");
        if (endpoint != null) endpoint.SetActive(false);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            LoadLevel();
        }
    }


    public void LoadLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }
}
