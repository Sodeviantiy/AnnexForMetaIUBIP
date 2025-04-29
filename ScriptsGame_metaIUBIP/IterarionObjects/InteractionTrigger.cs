using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;
using Photon.Pun;
using System.Collections;

public class InteractionTrigger : MonoBehaviour
{
    [SerializeField] public ModalWindowManager myModalWindow;
    public GameObject interactionUI; 
    public GameObject infoPanel;  
    private Coroutine closeInfoPanelCoroutine; 
    private bool isPlayerNearby = false;

    void Start()
    {
        interactionUI.SetActive(false);
        infoPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            infoPanel.SetActive(true); 
            myModalWindow.Open(); 
            interactionUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                interactionUI.SetActive(true); 
                isPlayerNearby = true;
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                interactionUI.SetActive(false); 
                Cursor.visible = false;
                myModalWindow.Close();

                if (infoPanel.activeSelf) 
                {
                    closeInfoPanelCoroutine = StartCoroutine(CloseInfoPanelAfterDelay(3f)); 
                } 

                isPlayerNearby = false;
            }
             
        }
    }
    IEnumerator CloseInfoPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); 
        infoPanel.SetActive(false); 

}
