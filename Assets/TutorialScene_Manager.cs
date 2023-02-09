using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class TutorialScene_Manager : MonoBehaviour
{
    public float Speed;
    bool isFinished = true;
    [SerializeField] Material filmStrip_Material;
    [SerializeField] VideoPlayer videoPlayer;


    private void Awake()
    {
        videoPlayer.loopPointReached += loadPractiseScene;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFinished)
        {
            float offset_Move = Speed * Time.time;
            filmStrip_Material.SetTextureOffset("_MainTex", new Vector2(0, offset_Move));
        }
    }

    private void loadPractiseScene(VideoPlayer source)
    {
        isFinished = false;
        source.Stop();
        StartCoroutine(WaitAndPrint(1.5f));
    }

    IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("Fluid");

        Debug.Log("Ahora carga la escena");
    }
}
