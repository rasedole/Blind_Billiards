using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JoystickHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform wrapper;
    [SerializeField]
    private RectTransform background;
    [SerializeField]
    private RectTransform handle;

    private Vector2 lastScreenSize;

    // Start is called before the first frame update
    void Start()
    {
        lastScreenSize = new Vector2 (Screen.width, Screen.height);
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastScreenSize.x != Screen.width && lastScreenSize.y != Screen.height)
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            Init();
        }
    }

    public void Init()
    {
        SizeResetByAnchor(background);
        background.anchorMin = Vector2.zero;
        background.anchorMax = Vector2.zero;
        background.offsetMin = Vector2.zero;
        background.offsetMax = new Vector2(wrapper.rect.width, wrapper.rect.height);

        SizeResetByAnchor(handle);
        handle.anchorMin = new Vector2(0.5f, 0.5f);
        handle.anchorMax = new Vector2(0.5f, 0.5f);
        handle.offsetMin = Vector2.zero;
        handle.offsetMax = new Vector2(wrapper.rect.width / 2, wrapper.rect.height / 2);
    }

    private void SizeResetByAnchor(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
