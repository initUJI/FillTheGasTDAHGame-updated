using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoatDrops_Trigger : MonoBehaviour
{
    public Text DEBUG;
    public GameObject fillingIndicator_model;
    public FluidGameManager gameManager;
    public Transform cameraTransform, spawnPoint;
    public GameObject waterDrop;
    public float indicatorScale;

    int maxParticles;
    int actualParticles;
    float percent;
    Coroutine c_dropping;

    public float poat_rotation;
    public float _rotationLeft;
    public float _rotationRight;
    public bool activated;
    public bool _dropping;

    private void Awake()
    {
        maxParticles = 100;
        actualParticles = 0; // 0
        indicatorScale = 0; // 0 
        percent = 0; // 0
    }

    private void Update()
    {
        poat_rotation = cameraTransform.localEulerAngles.z;
        _rotationLeft = 90 - percent * 0.6f;
        _rotationRight = 270 + percent * 0.6f;
        // DEBUG.text = cameraTransform.localEulerAngles.z.ToString("F0") + " Right: " + _rotationRight + "; Left: " + _rotationLeft;
        if (gameManager._diestro)
        {

            if (poat_rotation >= _rotationLeft && poat_rotation <= _rotationRight - 45)
            {
                if (!activated)
                {
                    c_dropping = StartCoroutine(SpawnWater());
                }
            }
            else
            {
                StopCoroutine(c_dropping);
                activated = false;
            }
        }
        else
        {
            if (poat_rotation <= _rotationRight && poat_rotation >= _rotationLeft + 45)
            {
                if (!activated)
                {
                    c_dropping = StartCoroutine(SpawnWater());
                }
            }
            else
            {
                StopCoroutine(c_dropping);
                activated = false;
            }
        }

    }

    IEnumerator SpawnWater()
    {
        activated = true;

        while (activated && gameManager.hasStarted && indicatorScale >= 0)
        {
            GameObject newObject = (GameObject)Instantiate(waterDrop, spawnPoint.position, spawnPoint.rotation);
            actualParticles--;
            percent = ((float)actualParticles / maxParticles) * 100;
            actualizarEscalaIndicador();
            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "particleObject")
        {
            actualParticles = actualParticles > 100 ? 100 : actualParticles + 1;
            percent = ((float)actualParticles / maxParticles) * 100;
            if (actualizarEscalaIndicador())
            {
                Destroy(other.gameObject);
            }
        }
    }

    private bool actualizarEscalaIndicador()
    {
        indicatorScale = (float) actualParticles / maxParticles;

        if(indicatorScale >= 1)
        {
            indicatorScale = 1;
            fillingIndicator_model.transform.localScale = new Vector3(fillingIndicator_model.transform.localScale.x, indicatorScale, fillingIndicator_model.transform.localScale.z);
            return false;
        }else
        {
            fillingIndicator_model.transform.localScale = new Vector3(fillingIndicator_model.transform.localScale.x, indicatorScale, fillingIndicator_model.transform.localScale.z);
            return true;
        }
    }
}
