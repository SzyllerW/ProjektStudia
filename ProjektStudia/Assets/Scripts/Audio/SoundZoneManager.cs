using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class SoundZoneManager : MonoBehaviour
{
    //script where audio clip is assigned
    [SerializeField] private MonoBehaviour targetScript;
    
    //within this zone audio clip is audible
    private float soundZoneRadius;

    private CircleCollider2D circleCollider;
    private Camera mainCamera;
    private FieldInfo playSoundField;
    private bool shouldPlaySound;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        mainCamera = Camera.main;

        if (targetScript != null)
        {
            //pull variable soundZoneRadius value from target script
            var field = targetScript.GetType().GetField("soundZoneRadius");

            if (field != null)
            {
                soundZoneRadius = (float)field.GetValue(targetScript);
                circleCollider.radius = soundZoneRadius;
            }

            //assign variable playSound value from target script to playSoundField
            playSoundField = targetScript.GetType().GetField("playSound");
        }
    }

    void Update()
    {
        if (targetScript == null || mainCamera == null || playSoundField == null) return;

        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        
        //if disctance to camera is equal or less than sound zone radius then return true
        shouldPlaySound = distanceToCamera <= soundZoneRadius;

        playSoundField.SetValue(targetScript, shouldPlaySound);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") && targetScript != null)
        {
            shouldPlaySound = true;
            playSoundField.SetValue(targetScript, shouldPlaySound);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetScript != null)
        {
            shouldPlaySound = false;
            playSoundField.SetValue(targetScript, shouldPlaySound);
        }
    }
}
