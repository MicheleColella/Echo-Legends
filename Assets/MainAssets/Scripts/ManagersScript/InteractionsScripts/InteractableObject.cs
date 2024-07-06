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
            foreach (AnimationClip clip in childAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "billPressedDisappear")
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
    }

    public void Interact()
    {
        if (disactiveBillBoard && hasInteracted) return;

        if (disactiveBillBoard)
        {
            hasInteracted = true;
            PlayDisappearAnimation();
            PlayBillboardInteractAnimation();
        }
        else
        {
            PlayInteractAnimation();
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
            childAnimator.Play("billAppear");
        }
    }

    private void PlayDisappearAnimation()
    {
        if (childAnimator != null)
        {
            isDisappearing = true;
            childAnimator.Play("billDisappear");
            StartCoroutine(WaitAndDeactivate(disappearAnimationDuration));
        }
    }

    private void PlayInteractAnimation()
    {
        if (childAnimator != null)
        {
            childAnimator.Play("billPressed");
        }
    }

    private void PlayBillboardInteractAnimation()
    {
        if (childAnimator != null)
        {
            childAnimator.Play("billPressedDisappear");
        }
    }

    private IEnumerator WaitAndDeactivate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (childObject != null)
        {
            childObject.SetActive(false);
        }
        isDisappearing = false;
    }
}
