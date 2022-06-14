
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ChangeCamera : MonoBehaviour
{
    

    [SerializeField]
    private CinemachineVirtualCamera hotelCam; //HotelOverview

    [SerializeField]
    private CinemachineVirtualCamera playerCam; //Player

    [SerializeField]
    private bool Hotel = false;



    public void SwitchPriority()
    {
        if (Hotel)
        {
            hotelCam.Priority = 0;
            playerCam.Priority = 1;
        }
        else
        {
            hotelCam.Priority = 1;
            playerCam.Priority = 0;
        }
        Hotel = !Hotel;
    }
  
    public void SetHotelCam()
    {
        Hotel = true;
        hotelCam.Priority = 1;
        playerCam.Priority = 0;

    }

    public void SetPlayerCam()
    {
        Hotel = false;
        hotelCam.Priority = 0;
        playerCam.Priority = 1;

    }

    public bool IsHotelTrue()
    {
        return Hotel;
    }

}
