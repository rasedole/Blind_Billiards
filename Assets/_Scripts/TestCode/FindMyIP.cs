using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class FindMyIP : MonoBehaviour
{
    private string localIP;
    private string publicIP;

    // Start is called before the first frame update
    void Start()
    {
        // local
        IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in ipEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }
        }

        // public
        publicIP = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();
        if (String.IsNullOrWhiteSpace(publicIP))
        {
            publicIP = localIP;
        }

        Debug.Log(localIP + ", " + publicIP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
