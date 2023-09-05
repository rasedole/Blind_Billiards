using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveForConvenience : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField ip;
    [SerializeField]
    private TMP_InputField port;
    [SerializeField]
    private TMP_InputField id;
    [SerializeField]
    private TMP_InputField room;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LastClient()
    {
        ip.text = PlayerPrefs.GetString("ipClient", FindMyIP.localIP);
        port.text = PlayerPrefs.GetString("portClient", "5000");
        id.text = PlayerPrefs.GetString("idClient", "Guest");
    }
    public void SubmitClient()
    {
        PlayerPrefs.SetString("ipClient", ip.text);
        PlayerPrefs.SetString("portClient", port.text);
        PlayerPrefs.SetString("idClient", id.text);
    }

    public void LastServer()
    {
        ip.text = FindMyIP.localIP;
        port.text = PlayerPrefs.GetString("portServer", "5000");
        id.text = PlayerPrefs.GetString("idServer", "Guest");
    }
    public void SubmitServer()
    {
        PlayerPrefs.SetString("portServer", port.text);
        PlayerPrefs.SetString("idServer", id.text);
    }

    public void LastRoom()
    {
        room.text = PlayerPrefs.GetString("room", "4");
    }
    public void SubmitRoom()
    {
        PlayerPrefs.SetString("room", room.text);
    }
}
