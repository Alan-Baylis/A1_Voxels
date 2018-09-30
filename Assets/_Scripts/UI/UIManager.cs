using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

[System.Serializable]
public class UIManager : MonoBehaviour
{
    [Header("Objects to Disable")]
    public GameObject reticle;
    public GameObject playerController;
    public GameObject playerCam;
    public GameObject descriptions;
    public GameObject achievementBox;

    [Header("Panel Settings")]
    public GameObject panel;
    public GameObject panelSpawnPoint;
    public float panelSpeed;
    public Animator animator;

    [Header("Panel Groups")]
    public List<Transform> inventoryList;
    public List<Transform> equippedList;


    // Use this for initialization
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (panel.activeInHierarchy)
            {
                StartCoroutine(this.ClosePanel());
            }
            else
            {
                panel.SetActive(true);
                reticle.SetActive(false);
                animator.SetInteger("AnimState", 1);
                playerController.GetComponent<FirstPersonController>().enabled = false;
                playerCam.GetComponent<BlockEditingSuite>().enabled = false;
                descriptions.SetActive(false);
                achievementBox.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private IEnumerator ClosePanel()
    {
        
        yield return new WaitForSeconds(0.2f);
        animator.SetInteger("AnimState", 0);

        yield return new WaitForSeconds(0.2f);
        descriptions.SetActive(true);
        playerCam.GetComponent<BlockEditingSuite>().enabled = true;
        playerController.GetComponent<FirstPersonController>().enabled = true;
        reticle.SetActive(true);
        panel.SetActive(false);
    }

}

