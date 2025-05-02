using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;
    public MovementInput moveScript;
    public float speedBoost = 6;
    public Animator animator;
    public float animSpeedBoost = 1.5f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    [Header("VFX Related")]
    public VisualEffect vfxGraphParticlesTrail;
    public VisualEffect vfxGraphInitialImpact;


    private SkinnedMeshRenderer[] skinnedRenderers;
    private bool isTrailActive;
    private float normalSpeed;
    private float normalAnimSpeed;

    private void Start()
    {
        vfxGraphParticlesTrail.Stop();
        vfxGraphInitialImpact.Stop();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine (ActivateTrail(activeTime));
        }
    }

    IEnumerator ActivateTrail (float timeActivated)
    {
        vfxGraphParticlesTrail.Play();
        vfxGraphInitialImpact.Play();
        normalSpeed = moveScript.movementSpeed;
        moveScript.movementSpeed = speedBoost;
        normalAnimSpeed = animator.GetFloat("animSpeed");
        animator.SetFloat("animSpeed", animSpeedBoost);

        while (timeActivated > 0)
        {
            timeActivated -= meshRefreshRate;

            if (skinnedRenderers == null)
                skinnedRenderers = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i=0; i<skinnedRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                skinnedRenderers[i].BakeMesh(m);

                mf.mesh = m;
                mr.material = mat;

                StartCoroutine (AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestroyDelay);
            }
            yield return new WaitForSeconds(meshRefreshRate);
        }

        vfxGraphParticlesTrail.Stop();
        vfxGraphInitialImpact.Stop();
        moveScript.movementSpeed = normalSpeed;
        animator.SetFloat("animSpeed", normalAnimSpeed);
        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat (Material m, float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVarRef);

        while (valueToAnimate > valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }

}
