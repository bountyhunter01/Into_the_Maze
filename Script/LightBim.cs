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
        KeyDepoly();  // keyList �迭 �ʱ�ȭ

    }
    
    void KeyDepoly()
    {
        keyList = new KeyCode[10];

        //�ڵ尪 �ɽ��ϱ�
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
                flashLight.enabled = !flashLight.enabled;  // ������ �Ѱ� ����
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
                break;  // Ű�� ������ ���� ����
            }
        }

        return result;

    }
}
