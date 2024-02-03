using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public Button Invite;
    
    // Update is called once per frame
    void Update()
    {
        Invite.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });
    }
}
