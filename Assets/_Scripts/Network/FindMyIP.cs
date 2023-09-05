using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class FindMyIP : MonoBehaviour
{
    public static string localIP
    {
        get
        {
            return instance._localIP;
        }
    }
    public static string publicIP
    {
        get
        {
            return instance._publicIP;
        }
    }

    private static FindMyIP instance;

    private string _localIP;
    private string _publicIP;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        // local
        IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in ipEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                _localIP = ip.ToString();
            }
        }

        // public
        _publicIP = new WebClient().DownloadString("http://ipinfo.io/ip").Trim();
        if (String.IsNullOrWhiteSpace(_publicIP))
        {
            _publicIP = _localIP;
        }

        //Debug.Log(_localIP + ", " + _publicIP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
