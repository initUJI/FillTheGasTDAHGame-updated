using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenu, exerciseSelectionMenu, optionsMenu, instructionsInformation, informationPanel,
        startExercise, selectSessionToDo, actualPanel, playButton, secondRoundText, sessionNotAvailable_Text;

    public Toggle diestroToggle;
    public instructionInformationManager informationManager;


    // Start is called before the first frame update
    void Start()
    {
        actualPanel = mainMenu;
        diestroToggle.isOn = DataManager.instancia.isPlayerDiestro;
        int partidasJugadas = DataManager.instancia.comprobarPartidasDelDia();
        //Debug.Log("Partidas jugadas: " + partidasJugadas);
        if (partidasJugadas >= 3 && partidasJugadas < 99)
        {
            secondRoundText.SetActive(true);
        }
        else if(partidasJugadas >= 99) //Para deshabilitar el botón de jugar cuando se hayan realizado "x" partidas
        {
            playButton.GetComponent<Button>().interactable = false;
            sessionNotAvailable_Text.SetActive(true);
        }
        playButton.GetComponent<Button>().onClick.AddListener(() => { SessionManager.instance.setSessionMode("Corta"); });
    }


    public void changePanel(string newPanel)
    {
        actualPanel.SetActive(false);
        
        switch (newPanel)
        {
            case "mainMenu":
                actualPanel = mainMenu;
                actualPanel.SetActive(true);
                break;

            case "exerciseSelectionMenu":
                actualPanel = exerciseSelectionMenu;
                actualPanel.SetActive(true);
                break;

            case "optionsMenu":
                actualPanel = optionsMenu;
                actualPanel.SetActive(true);
                break;

            case "dataInformation":
                actualPanel = informationPanel;
                actualPanel.SetActive(true);
                break;
            case "microphoneSettings":
                actualPanel = instructionsInformation;
                actualPanel.SetActive(true);
                break;
            case "startExercise":
                actualPanel = startExercise;
                actualPanel.SetActive(true);
                break;
            case "selectSessionToDo":
                actualPanel = selectSessionToDo;
                actualPanel.SetActive(true);
                break;
            case "exitGame":
                Application.Quit();
                break;
        }
        
    }

    public void getInstructionsPanel(string gameMode)
    {
        actualPanel.SetActive(false);
        actualPanel = instructionsInformation;
        actualPanel.SetActive(true);

        switch (gameMode)
        {
            case "Avion":
                informationManager.setInformation("Avion");
                break;
            case "Fluid":
                informationManager.setInformation("Fluid");
                break;
            case "Pipes":
                informationManager.setInformation("Pipes");
                break;
            case "Graffiti":
                informationManager.setInformation("Graffiti");
                break;
        }
    }

    public void exerciseSelection(string nameOfExercise)
    {
        switch (nameOfExercise)
        {
            case "airplaneGame":
                SceneManager.LoadScene("Avion");
                break;

            case "fluidGame":
                SceneManager.LoadScene("Fluid");
                break;

            case "pipeDirtyGame":
                SceneManager.LoadScene("Pipes");
                break;

            case "blowingGame":
                SceneManager.LoadScene("BlowingGame");
                break;
            case "paintingGame":
                SceneManager.LoadScene("Graffiti");
                break;
        }
    }

    public void changeDiestroPlayer()
    {
        bool isDiestro = diestroToggle.isOn;

        DataManager.instancia.isPlayerDiestro = isDiestro;
        DataManager.instancia.Guardar();
    }

    public void tutorialButtonSelected()
    {
        StartCoroutine(WaitAndLaunchScene(1.5f));
    }

    IEnumerator WaitAndLaunchScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("TutorialScene");
    }
}
