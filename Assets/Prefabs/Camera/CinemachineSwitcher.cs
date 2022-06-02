using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineSwitcher : MonoBehaviour
{
    [SerializeField]

    private Animator animator;

    private bool Hotel = true;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SwitchState()
    {
        if (Hotel)
        {
            animator.Play("Lobby");
        }
        else
        {
            animator.Play("Hotel");

        }
        Hotel = !Hotel;


    }

    public void SwitchStateOverview()
    {
        if (Hotel)
        {
            animator.Play("Lobby");
        }
        else
        {
            animator.Play("Hotel Overview");

        }
        Hotel = !Hotel;


        //private void Update() 
        //{
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     SwitchState();
        // }   
        //}
    }
}