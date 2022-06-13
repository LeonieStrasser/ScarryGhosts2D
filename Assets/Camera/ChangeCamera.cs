
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField]

    private InputAction action;

    [SerializeField]
    private CinemachineVirtualCamera vcam1; //HotelOverview

    [SerializeField]
    private CinemachineVirtualCamera vcam2; //Player

    private bool Hotel = true;


    void Start()
    {
        action.performed +=_=> SwitchPriority();
    }

    private void SwitchPriority()
    {
        if (Hotel)
        {
            vcam1.Priority = 0;
            vcam2.Priority = 1;
        }
        else
        {
            vcam1.Priority = 1;
            vcam2.Priority = 0;
        }
        Hotel = !Hotel;
    }
    private void Update() 
    {
    if(Input.GetKeyDown(KeyCode.Space))
    {
        SwitchPriority();
    }   
    }


}
