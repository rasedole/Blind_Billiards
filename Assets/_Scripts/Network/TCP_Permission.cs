using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class TCP_Permission : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermissions(new string[] { Permission.FineLocation });
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
