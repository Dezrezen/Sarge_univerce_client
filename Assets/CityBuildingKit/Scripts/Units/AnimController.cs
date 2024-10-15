using UnityEngine;

public class AnimController : MonoBehaviour
{
    //public tk2dSpriteAnimation attackAnimation, buildAnimation, idleAnimation, walkAnimation;

    public tk2dSpriteAnimation spriteAnimation;

    public string soldierType = ""; //EventTrigger

    private string action, direction; //animation controller for builders/soldiers

    private tk2dSpriteAnimator animator;

    private Component soundFX;

    private void Start()
    {
        animator = GetComponent<tk2dSpriteAnimator>();

        action = "Walk";
        direction = "W";

        soundFX = GameObject.Find("SoundFX")
            .GetComponent<SoundFX>(); // //connects to SoundFx - a sound source near the camera

        if (soldierType == "EventTrigger") animator.AnimationEventTriggered += FireWeapon;
    }

    //order: change anim / turn /update char anim 


    public void ChangeAnim(string anim)
    {
        action = anim;
        /*
        switch (anim) 
        {
        case "Idle":
            animator.Library = idleAnimation;
            break;
        case "Walk":
            animator.Library = walkAnimation;
            break;
        case "Attack":
            animator.Library = attackAnimation;
            break;
        case "Build":
            animator.Library = buildAnimation;	
            break;
        }	
        */
    }


    public void Turn(string dir)
    {
        direction = dir;
    }

    public void UpdateCharacterAnimation()
    {
        animator.Play(action + "_" + direction);
    }


    private void FireWeapon(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frameNo)
    {
        if (soldierType == "EventTrigger")
            ((SoundFX)soundFX).CopFire();
        //.CannonFire ();
    }
}