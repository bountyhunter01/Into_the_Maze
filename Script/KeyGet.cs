using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyGet : MonoBehaviour
{
    public GameObject keyGetText;
    public bool isKeyGet;

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (keyGetText != null)
            {
                ShowKeyGetText();
            }
            isKeyGet = true;
            gameObject.SetActive(false);
        }
    }

    ///<summary>
    ///키를 얻으면 나오는 텍스트를 활성화
    ///</summary>
    void ShowKeyGetText()
    {
        keyGetText.SetActive(true); // 텍스트를 활성화
        Invoke("HideKeyGetText", 1f); // 1초 후에 텍스트를 비활성화
        
    }

    ///<summary>
    ///키를 얻으면 나오는 텍스트를 비활성화
    ///</summary>
    void HideKeyGetText()
    {
        keyGetText.SetActive(false); // 텍스트를 비활성화
       
    }
}
