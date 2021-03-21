using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    public GameObject speechBubble;
    public GameObject textBox;
    private bool touchingPlayer;
    private Dialog dialog;
    private bool opened;

    private void Start()
    {
        dialog = GetComponent<Dialog>();
    }

    private void Update()
    {
        if (touchingPlayer)
        {
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!opened)
                {
                    
                    StartCoroutine(OpenText());
                    
                }
                else if (dialog.getIndex() < dialog.getSentencesLength() - 1)
                {
                    dialog.NextSentence();

                }
                else
                {
                    StartCoroutine(CloseText());
                    
                }
                    
            }
        }
        else
        {
            StartCoroutine(CloseText());
        }
    }

    IEnumerator CloseText()
    {
        textBox.GetComponent<Animator>().SetTrigger("Close");
        yield return new WaitForSeconds(0.417f);
        dialog.Clear();
        textBox.SetActive(false);
        opened = false;
    }

    IEnumerator OpenText()
    {
        textBox.SetActive(true);
        yield return new WaitForSeconds(0.417f);
        dialog.StartDiaglog();
        opened = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            speechBubble.GetComponent<SpriteRenderer>().enabled = true;
            touchingPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            speechBubble.GetComponent<SpriteRenderer>().enabled = false;
            touchingPlayer = false;
        }
    }
}

