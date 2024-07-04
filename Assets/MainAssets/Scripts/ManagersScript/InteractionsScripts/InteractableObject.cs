using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    public float interactionRange = 3f;
    public GameObject childObject;
    public bool disactiveBillBoard = false;
    private bool hasInteracted = false;
    private bool isDisappearing = false;

    private Animator childAnimator;
    private float disappearAnimationDuration;

    private void Start()
    {
        InteractionManager.Instance.RegisterInteractable(this);
        if (childObject != null)
        {
            childAnimator = GetComponent<Animator>();
            // Recupera la durata dell'animazione billDisappear
            foreach (AnimationClip clip in childAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "billDisappear")
                {
                    disappearAnimationDuration = clip.length;
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        InteractionManager.Instance.UnregisterInteractable(this);
        // Debug.Log("Destroyed interactable: " + gameObject.name);
    }

    public void Interact()
    {
        if (disactiveBillBoard && hasInteracted) return;

        // Debug.Log("Interacted with " + gameObject.name);

        if (disactiveBillBoard)
        {
            hasInteracted = true;
            PlayDisappearAnimation();
        }

        var handler = GetComponent<IInteractionHandler>();
        if (handler != null)
        {
            handler.HandleInteraction(this);
        }
    }

    public void SetChildActive(bool isActive)
    {
        if (childObject != null)
        {
            if (isActive && (!hasInteracted || !disactiveBillBoard))
            {
                if (!isDisappearing)
                {
                    childObject.SetActive(true);
                    PlayAppearAnimation();
                }
            }
            else if (!isActive && !isDisappearing)
            {
                PlayDisappearAnimation();
            }
        }
    }

    private void PlayAppearAnimation()
    {
        if (childAnimator != null)
        {
            //Debug.Log("Playing appear animation");
            childAnimator.Play("billAppear");
        }
    }

    private void PlayDisappearAnimation()
    {
        if (childAnimator != null)
        {
            //Debug.Log("Playing disappear animation");
            isDisappearing = true;
            childAnimator.Play("billDisappear");
            StartCoroutine(WaitAndDeactivate(disappearAnimationDuration));
        }
    }

    private IEnumerator WaitAndDeactivate(float waitTime)
    {
        //Debug.Log("Waiting to deactivate: " + waitTime + " seconds");
        yield return new WaitForSeconds(waitTime);
        if (childObject != null)
        {
            //Debug.Log("Deactivating child object");
            childObject.SetActive(false);
        }
        isDisappearing = false;
    }
}
