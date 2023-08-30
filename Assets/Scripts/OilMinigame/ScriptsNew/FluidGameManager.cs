using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FluidGameManager : MonoBehaviour
{
    //public Slider percentageSlider;
    public Text percentageText, failedDropsText, timeText, DEBUG; //QUITAR DEBUG
    public float particleCounter, particlesToReach;
    int failedDrops, totalDrops;

    public string difficulty;
    public GameObject mainPlayerPoat;
    public GameObject poatToFill;
    public bool movingPoat;

    public bool hasStarted = false;
    public bool _diestro = false;
    public GameObject gameplay_UI;

    public float gameplayTime, totalTimeToGo;
    public GameObject finishingPanel;
    private PartidaJugada partidaActual;

    public Animator a_poatAnimator;
    public GameObject _bigPoat;
    public GameObject _smallPoat;
    // Start is called before the first frame update
    void Start()
    {
        timeText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "time") + " 00";
        actualizarValorSlider(0);
        actualizarFailedDrops(0);
        DefineGameMode();
        StartCoroutine(StartCountdown());

        //Habilitar diestro/zurdo
        if(DataManager.instancia.isPlayerDiestro)
        {
            _diestro = true;
            mainPlayerPoat.transform.localScale = new Vector3(-mainPlayerPoat.transform.localScale.x, mainPlayerPoat.transform.localScale.y, mainPlayerPoat.transform.localScale.z);
        }
    }

    IEnumerator StartCountdown()
    {
        Transform countdownObject = GameObject.Find("Canvas/Countdown").transform;

        foreach (Transform child in countdownObject)
        {
            LeanTween.scale(child.gameObject, new Vector3(1, 1, 1), 0.75f);
            yield return new WaitForSeconds(0.75f);
            LeanTween.scale(child.gameObject, new Vector3(0, 0, 0), 0.25f);
            yield return new WaitForSeconds(0.25f);
        }
        hasStarted = true;
        gameplay_UI.SetActive(true);
        gameplayTime = 0;

        yield return null;
    }

    private void DefineGameMode()
    {
        if (SessionManager.instance == null || (SessionManager.instance != null && SessionManager.instance.playingSession == false))
        {
            //No se encuentra en una sesion
            poatToFill.transform.localScale = new Vector3(19.85877f, 21.01058f, 24.73227f);
            totalTimeToGo = 10;
            particlesToReach = 3500;
        }
        else
        {
            //Si que se encuentra en una sesion
            MinijuegoLevel thisLevel = SessionManager.instance.sucesionDeJuegos[0];
            partidaActual = new PartidaJugada(thisLevel.difficulty);
            DataManager.instancia.añadirPartidaGuardada(partidaActual);
            DataManager.instancia.Guardar();
            BezierFollow poatMovement = (BezierFollow)FindObjectOfType(typeof(BezierFollow));

            this.gameObject.GetComponent<UI_InGame_Manager>().setPauseScreenModeToPractise();


            difficulty = thisLevel.difficulty;
            switch (difficulty)
            {
                case "Easy":
                    {
                        totalTimeToGo = 10;
                        particlesToReach = 2500;
                        poatMovement.speedModifier = 0;
                        a_poatAnimator.SetBool("MovRot", false);
                        _bigPoat.SetActive(true);
                        _smallPoat.SetActive(false);
                        break;
                    }

                case "Medium":
                    {
                        totalTimeToGo = 120;
                        particlesToReach = 2500;
                        //poatMovement.speedModifier = 0.05f;   //  Quitar movimiento del recipiente a rellenar, es demasiado dificil para un niño
                        a_poatAnimator.SetBool("MovRot", true);
                        _bigPoat.SetActive(true);
                        _smallPoat.SetActive(false);
                        break;
                    }
                case "Hard":
                    {
                        particlesToReach = 2500;
                        totalTimeToGo = 120;
                        //poatMovement.speedModifier = 0.15f;
                        a_poatAnimator.SetBool("MovRot", true);
                        _bigPoat.SetActive(false);
                        _smallPoat.SetActive(true);
                        break;
                    }
            }
        }
    }

    private void Update()
    {
        if(hasStarted)
        {
            actualizarTiempo();
            comprobarFinalDeJuego();
        }
    }

    private void comprobarFinalDeJuego()
    {
        if(particleCounter/particlesToReach >= 1 || gameplayTime >= totalTimeToGo)
        {
            //Finish the game
            hasStarted = false;

            finishingPanel.SetActive(true);
            GetComponent<UI_InGame_Manager>().setPauseButtonState(false);


            LeanTween.scale(finishingPanel, new Vector3(2f, 2f, 1), 1);

            //Fill the panel with the info
            float valueToShow = (particleCounter / particlesToReach) * 100;
            float liters = particleCounter / 100;

            //Make buttons functional
            finishingPanel.transform.Find("Panel/Buttons/MenuButton").gameObject.GetComponent<Button>().onClick.AddListener(() => gameObject.GetComponent<UI_InGame_Manager>().exitButtonPressed());
            
            if (SessionManager.instance == null || (SessionManager.instance != null && SessionManager.instance.playingSession == false))
            {
                finishingPanel.transform.Find("Panel/Title").gameObject.GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "endText");

                //Editando texto de resultado
                finishingPanel.transform.Find("Panel/Information/PointsText").gameObject.GetComponent<Text>().text = "";
                finishingPanel.transform.Find("Panel/Information/PointsText/ResolutionText").gameObject.GetComponent<Text>().text = "";

                //Editando texto de resultado adicional
                finishingPanel.transform.Find("Panel/Information/AdditionalText").gameObject.GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "drops") + " " + particleCounter;

                double percent = (particleCounter / (particleCounter + failedDrops)) * 100;
                finishingPanel.transform.Find("Panel/Information/TimeText").gameObject.GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "dataText") + " "
                    + liters.ToString("F2") + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "litres");


                finishingPanel.transform.Find("Panel/Buttons/NextButton/Information2").GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "menu");
                finishingPanel.transform.Find("Panel/Buttons/NextButton").gameObject.GetComponent<Button>().onClick.AddListener(() => gameObject.GetComponent<UI_InGame_Manager>().exitButtonPressed());
            }
            else
            {
                SessionManager.instance.SumarTiempoAlTotal((int)gameplayTime);

                partidaActual.aciertos = (int)particleCounter;
                partidaActual.fallos = failedDrops;
                partidaActual.juegoTerminado = true;
                DataManager.instancia.sustituirPartidaGuardada(partidaActual);
                DataManager.instancia.Guardar();

                string resultadoPrueba = "";
                if (particleCounter >= 190)
                {
                    resultadoPrueba = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "highResultText");
                    SessionManager.instance.SumarPuntosAlTotal(100);
                }
                else if (particleCounter >= 150)
                {
                    resultadoPrueba = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "midResultText");
                    SessionManager.instance.SumarPuntosAlTotal(50);
                }
                else if (particleCounter >= 1)
                {
                    resultadoPrueba = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "lowResultText");
                    SessionManager.instance.SumarPuntosAlTotal(25);
                }
                else
                {
                    resultadoPrueba = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "failResultText");
                }

                SessionManager.instance.sucesionDeJuegos.RemoveAt(0);
                //Editando titulo de la pantalla final
                int completed = SessionManager.instance.totalGamesInSession - SessionManager.instance.sucesionDeJuegos.Count;
                finishingPanel.transform.Find("Panel/Title").gameObject.GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "endText") + " " + completed + " / " + SessionManager.instance.totalGamesInSession;

                //Editando texto de resultado
                // finishingPanel.transform.Find("Panel/Information/PointsText").gameObject.GetComponent<Text>().text = "Resultado: ";
                finishingPanel.transform.Find("Panel/Information/PointsText/ResolutionText").gameObject.GetComponent<Text>().text = resultadoPrueba;

                //Editando texto de resultado adicional
                finishingPanel.transform.Find("Panel/Information/AdditionalText").gameObject.GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "drops") + " " + particleCounter;

                //Editando texto de puntos
                /*finishingPanel.transform.Find("Panel/Information/TimeText").gameObject.GetComponent<Text>().text = "Gotas falladas: "
                    + SessionManager.instance.puntosTotales + "/" + SessionManager.instance.puntosAConseguir;*/
                double percent = (particleCounter / (particleCounter + failedDrops)) * 100;
                finishingPanel.transform.Find("Panel/Information/TimeText").gameObject.GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "dataText") + " "
                    + liters.ToString("F2") + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "litres");

                finishingPanel.transform.Find("Panel/Buttons/NextButton").gameObject.GetComponent<Button>().onClick.AddListener(() => SessionManager.instance.chargeNextScene());
                finishingPanel.transform.Find("Panel/Buttons/NextButton/Information2").GetComponent<Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "next");
                
            }
        }
    }

    private void actualizarTiempo()
    {
        gameplayTime += Time.deltaTime;
        float timeToShow = totalTimeToGo - gameplayTime;
        if (timeToShow <= 0)
        {
            timeToShow = 0;
        }
        timeText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "time") + " " + Mathf.Floor(timeToShow).ToString("00");
    }

    public void actualizarValorSlider(int newCount)
    {
        particleCounter = newCount;

        float valueToShow = particleCounter / 100;
        
        //percentageSlider.value = valueToShow;
        percentageText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "dataText") + " " + valueToShow.ToString("F2") + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UI Game Text", "litres");
    }

    public void actualizarFailedDrops(int newCount)
    {
        failedDrops = newCount;
        //DESACTIVADO PORQUE NO SE QUIERE QUE SE MUESTRE LOS FALLOS AL JUGADOR
        //failedDropsText.text = newCount.ToString();
    }

    void playAgain()
    {
        SceneManager.LoadScene("Fluid");
    }
}
