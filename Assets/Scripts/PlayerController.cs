using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int collectibleCount = 0;

    void Start()
    {
        if (PlayerPrefs.HasKey("CollectibleCount"))
        {
            collectibleCount = PlayerPrefs.GetInt("CollectibleCount");
        }
        else
        {
            collectibleCount = 0;
        }
    }

    private void Save()
    {
        PlayerPrefs.SetInt("CollectibleCount", collectibleCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            collectibleCount++;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Bed"))
        {
            SceneManager.LoadScene("List 1");
        }
    }
}
