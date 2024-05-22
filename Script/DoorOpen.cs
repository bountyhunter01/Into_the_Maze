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
                // 플레이어가 열쇠를 가지고 있으면 씬 로드
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                // 플레이어가 열쇠를 가지고 있지 않으면 텍스트를 보여줌
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
