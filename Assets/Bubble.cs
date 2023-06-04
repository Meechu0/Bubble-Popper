using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bubble : MonoBehaviour

{
    public GameManager gameManager;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float breatheSpeed = 1f;
    
    private bool isBreathing = false;
    private Coroutine breatheCoroutine;

    public void StartBreathing()
    {
        if (!isBreathing)
        {
            isBreathing = true;
            breatheCoroutine = StartCoroutine(Breathe());
        }
    }

    public void StopBreathing()
    {
        if (isBreathing)
        {
            isBreathing = false;
            if (breatheCoroutine != null)
            {
                StopCoroutine(breatheCoroutine);
            }
            transform.localScale = Vector3.one; 
        }
    }

    IEnumerator Breathe()
    {
        float t = 0;
        while (true)
        {
            t = Mathf.PingPong(Time.time * breatheSpeed, 1);
            float scale = Mathf.Lerp(minScale, maxScale, t);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    private void OnMouseOver()
    {
        List<GameObject> matchingBubbles = gameManager.gameBoard.GetMatchingBubbles(this.gameObject);
        if (matchingBubbles.Count > 1)
        {
            foreach (GameObject bubble in matchingBubbles)
            {
                bubble.GetComponent<Bubble>().StartBreathing();
            }
        }
    }

    private void OnMouseExit()
    {
        List<GameObject> matchingBubbles = gameManager.gameBoard.GetMatchingBubbles(this.gameObject);
        foreach (GameObject bubble in matchingBubbles)
        {
            bubble.GetComponent<Bubble>().StopBreathing();
        }
    }

    private void OnMouseDown()
    {
        if (gameManager != null)
            gameManager.BubbleClicked(this.gameObject);
    }
}
