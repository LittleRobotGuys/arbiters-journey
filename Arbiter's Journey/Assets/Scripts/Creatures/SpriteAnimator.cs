using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    private int animationIndex = 0, animationNumber = 0, animationsPerCycle = 4;
    [SerializeField]
    private Sprite baseSprite;
    private string baseSpriteName;
    [SerializeField]
    private Sprite[] frontSprites, backSprites;
    public Sprite[] atlas;
    private bool animating = false;
    private float animSpeed = 4f;

    // private bool animating = false;

    void Awake()
    {
        // Try to get it from the SpriteRenderer under it:
        if (baseSprite == null)
        {
            baseSprite = GetComponentInChildren<SpriteRenderer>().sprite;
        }
        // Couldn't find it?  Destroy!
        if (baseSprite == null)
        {
            Debug.LogError(this.name + " Creature created but had no baseSprite assigned, destroying itself.");
            GameObject.Destroy(this);
        }
        else
        {
            baseSpriteName = baseSprite.name.Substring(0, baseSprite.name.IndexOf('_'));
            animationNumber = Int32.Parse(baseSprite.name.Substring(baseSpriteName.Length + 1));
            animationIndex = 0;

            //Debug.Log("Creature created with sprite: " + baseSpriteName);
            //Debug.Log("Creature sprite begins with index: " + animationIndex);
        }

        frontSprites = new Sprite[animationsPerCycle];
        backSprites = new Sprite[animationsPerCycle];

        atlas = Resources.LoadAll<Sprite>("AllAvatars/" + baseSpriteName);

        //Debug.Log("Created an atlas of length: " + atlas.Length);
        
        for(int k = 0; k < atlas.Length; k++)
        {
            //Debug.Log("Sprite in atlas at index " + k + atlas[k].name);
        }

        for (int i = 0; i < animationsPerCycle; i++)
        {
            frontSprites[i] = atlas.Single(s => s.name == baseSpriteName + '_' + (i + animationNumber));
            backSprites[i] = atlas.Single(s => s.name == baseSpriteName + '_' + (i + animationNumber + animationsPerCycle));
        }
    }

    // This is the T-Pose of each sprite.
    internal void ResetAnimation()
    {
        animating = false;
        GetComponentInChildren<SpriteRenderer>().sprite = frontSprites[0];
    }

    public void StopAnimating()
    {
        animating = false;
    }

    public void Animate(Vector3 dir, float time)
    {
        if (!animating)
        {
            StartCoroutine(StartAnimating(dir, time * animSpeed));
        }
    }

    private IEnumerator StartAnimating(Vector3 direction, float t)
    {
        animating = true;
        while (animating)
        {
            if (animationIndex >= animationsPerCycle)
            {
                animationIndex = 0;
            }
            
            loadCorrectSpriteFrame(direction, animationIndex);
        

            // flip X if needed
            if (direction.x < 0)
            {
                GetComponentInChildren<SpriteRenderer>().flipX = true;
            }
            else
            {
                GetComponentInChildren<SpriteRenderer>().flipX = false;
            }

            animationIndex++;
            yield return new WaitForSeconds(t);
        }
    }

    private void loadCorrectSpriteFrame(Vector3 dir, int ndx)
    {
        if (dir.y > 0)
        {
            if (animationIndex >= animationsPerCycle)
            {
                animationIndex = 0;
            }
            GetComponentInChildren<SpriteRenderer>().sprite = backSprites[ndx];
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().sprite = frontSprites[ndx];
        }
    }
}
