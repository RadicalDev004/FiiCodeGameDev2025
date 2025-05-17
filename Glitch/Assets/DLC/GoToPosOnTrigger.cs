using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class GoToPosOnTrigger : MonoBehaviour
{
    public Transform goTo;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerBehaviour playerBehaviour))
        {
            var controller = playerBehaviour.GetComponent<CharacterController>();
            controller.enabled = false;
            playerBehaviour.transform.position = goTo.transform.position;
            controller.enabled = true;
        }
        
    }
}
