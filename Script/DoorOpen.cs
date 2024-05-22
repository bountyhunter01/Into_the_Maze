using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorOpen : MonoBehaviour
{
    public KeyGet keyGet;
    public string sceneName;
    public GameObject needKeyText;

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (keyGet != null && keyGet.isKeyGet)
            {
                // �÷��̾ ���踦 ������ ������ �� �ε�
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                // �÷��̾ ���踦 ������ ���� ������ �ؽ�Ʈ�� ������
                StartCoroutine(TextTunedown());
            }
        }
    }
    IEnumerator TextTunedown()
    {
        needKeyText.SetActive(true);
        yield return new WaitForSeconds(1f);
        needKeyText.SetActive(false);
    }

}
