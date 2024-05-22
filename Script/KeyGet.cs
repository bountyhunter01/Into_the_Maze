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
    ///Ű�� ������ ������ �ؽ�Ʈ�� Ȱ��ȭ
    ///</summary>
    void ShowKeyGetText()
    {
        keyGetText.SetActive(true); // �ؽ�Ʈ�� Ȱ��ȭ
        Invoke("HideKeyGetText", 1f); // 1�� �Ŀ� �ؽ�Ʈ�� ��Ȱ��ȭ
        
    }

    ///<summary>
    ///Ű�� ������ ������ �ؽ�Ʈ�� ��Ȱ��ȭ
    ///</summary>
    void HideKeyGetText()
    {
        keyGetText.SetActive(false); // �ؽ�Ʈ�� ��Ȱ��ȭ
       
    }
}
