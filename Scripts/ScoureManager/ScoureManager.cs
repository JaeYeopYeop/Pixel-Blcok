using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoureManager : MonoBehaviour
{
    [SerializeField] private GameView scGameView;
    [SerializeField] private Convert3D scConvert3D;
    [SerializeField] private GameObject scStarView;
    [SerializeField] private ParticleSystem scLeftUpFire;
    [SerializeField] private ParticleSystem scLeftDownFire;
    [SerializeField] private ParticleSystem scRightUpFire;
    [SerializeField] private ParticleSystem scRightDownFire;

    public float scoure { get; set; }

    public float fevertime { get; set; }

    public bool Isfever {  get; set; }
    public bool Ismegafever {  get; set; }


    public void StartParticle()
    {
        scLeftDownFire.Play();
        scLeftUpFire.Play();
        scRightUpFire.Play();
        scRightDownFire.Play();
    }

    public void StopParticle()
    {
        if (scLeftDownFire.isPlaying)
        {
            scLeftDownFire.Stop();
            scLeftUpFire.Stop();
            scRightUpFire.Stop();
            scRightDownFire.Stop();
        }
    }

    public void ChangeParticleColor()
    {
        if (Isfever)
        {
            ParticleSystem.MainModule main = scLeftDownFire.main;
            main.startColor = Color.red;

            main = scLeftUpFire.main;
            main.startColor = Color.red;

            main = scRightUpFire.main;
            main.startColor = Color.red;

            main = scRightDownFire.main;
            main.startColor = Color.red;
        }
        else if (Ismegafever)
        {
            ParticleSystem.MainModule main = scLeftDownFire.main;
            main.startColor = Color.blue;

            main = scLeftUpFire.main;
            main.startColor = Color.blue;

            main = scRightUpFire.main;
            main.startColor = Color.blue;

            main = scRightDownFire.main;
            main.startColor = Color.blue;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (scConvert3D.IsPlayingGame && scConvert3D.IsBlockClickOK)
        {
            scoure -= Time.deltaTime;
        }

        if (scConvert3D.IsPlayingGame&&(Isfever || Ismegafever))
        {
            
            fevertime -= Time.deltaTime;
        }


        if (scConvert3D.IsPlayingGame && fevertime <= 0)
        {
            Isfever = false;
            Ismegafever = false;
            StopParticle();
        }
    }
}
