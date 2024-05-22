using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBim : MonoBehaviour
{
    Light flashLight;
   new Transform transform;
    KeyCode[] keyList;
   public GameObject player;

    private void Awake()
    {
        flashLight = GetComponent<Light>();
        transform = player.transform;
        KeyDepoly();  // keyList 배열 초기화

    }
    
    void KeyDepoly()
    {
        keyList = new KeyCode[10];

        //코드값 케싱하기
        keyList[0] = KeyCode.F;
        keyList[1] = KeyCode.Escape;
    }
    private void Update()
    {
        KeyCode result = User_Input();
        if (result == keyList[0])
        {
            if (flashLight != null)
            {
                flashLight.enabled = !flashLight.enabled;  // 손전등 켜고 끄기
                Debug.Log("Flashlight toggled: " + flashLight.enabled);
            }
        }
    }
    KeyCode User_Input()
    {
        KeyCode result = KeyCode.None;

        for (int i = 0; i < keyList.Length; i++)
        {
            if (Input.GetKeyDown(keyList[i]))
            {
                result = keyList[i];
                break;  // 키가 눌리면 루프 종료
            }
        }

        return result;

    }
}
