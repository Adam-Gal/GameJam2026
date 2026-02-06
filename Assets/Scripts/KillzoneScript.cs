using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillzoneScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
