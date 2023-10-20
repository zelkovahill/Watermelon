using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    public AudioSource ss;

    public void Anykey()
    {
     
            ss.Play();
            SceneManager.LoadScene("Game");
            //StartCoroutine(Start());
        
        
    }
    //private IEnumerator Start()
    //{
    //    yield return new WaitForSeconds(1f);
    //    Debug.Log("game");
        
    //}


    public void GameOut()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

}
