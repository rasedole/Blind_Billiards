using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BallDoll : MonoBehaviour
{
    private static readonly float hideTime = 1f;

    public Color showcaseColor;

    [SerializeField] 
    private MeshRenderer ballRenderer;
    [SerializeField]
    private Animator ballAnimator;
    [SerializeField]
    private ParticleSystem particle;
    [SerializeField]
    private BallShowMode showMode;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private AudioClip _audioClip;
    [SerializeField]
    private AudioSource _audioSource;

    private IEnumerator enumerator;

    // Start is called before the first frame update
    void Start()
    {
        // Just for main menu
        if (showMode == BallShowMode.Showcase)
        {
            Init(showcaseColor, showMode);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(Color baseColor, BallShowMode _mode)
    {
        // Set color
        Material material = ballRenderer.material;
        material.SetColor("_BaseColor", baseColor);

        MainModule particleMain = particle.main;
        particleMain.startColor = baseColor;

        // Set animation
        showMode = _mode;
        if (TCP_BallCore.networkMode == NetworkMode.Client) 
        {
            _rigidbody.isKinematic = true;
        }
        if (showMode == BallShowMode.MyPlayer)
        {
            ballAnimator.Play("Moving");
            //_rigidbody.isKinematic = false;
        }
        else if (showMode == BallShowMode.Showcase)
        {
            ballAnimator.Play("PopLoop");
        }
    }

    public void CollisionEvent()
    {
        if(showMode != BallShowMode.Showcase)
        {
            ballAnimator.Play("Pop");
        }
        particle.Stop();
        particle.Play();

        if (showMode == BallShowMode.OtherPlayer)
        {
            if (enumerator != null)
            {
                StopCoroutine(enumerator);
                enumerator = null;
            }
            enumerator = Hide();
            StartCoroutine(enumerator);
        }
        _audioSource.PlayOneShot(_audioClip);
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(hideTime);
        if(showMode == BallShowMode.OtherPlayer)
        {
            ballAnimator.Play("Hide");
        }
    }
}

public enum BallShowMode
{
    OtherPlayer,
    MyPlayer,
    Showcase,
}
