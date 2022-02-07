using Photon.Pun;
using UnityEngine;


public class MyPlayerAnimatorManager : MonoBehaviourPun
{
    #region Private Fields

    [SerializeField]
    private float directionDampTime = 0.25f;
    Animator animator;

    #endregion
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        
        if (!animator)
        {
            return;
        }
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("Base Layer.Run"))
        {
            if (Input.GetButtonDown("Fire2")) animator.SetTrigger("Jump"); 
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if( v < 0 )
        {
            v = 0;
        }

        animator.SetFloat( "Speed", h*h+v*v );
        animator.SetFloat( "Direction", h, directionDampTime, Time.deltaTime );
    }
}
