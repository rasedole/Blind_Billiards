using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void PlayWithLocal()
    {
        SceneManager.LoadScene("GuestHostPlayScene");
    }

    public void JoinRoom()
    {
        SceneManager.LoadScene("SampleRoomScene");
    }
}
