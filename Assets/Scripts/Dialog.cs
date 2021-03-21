using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text text;
    public string[] sentences;
    private int index;
    public float typingSpeed;

    public void StartDiaglog()
    {
        StartCoroutine(Type());
    }

    IEnumerator Type()
    {
        foreach(char letter in sentences[index].ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {
        if(index < sentences.Length - 1)
        {
            StopAllCoroutines();
            index++;
            text.text = "";
            StartCoroutine(Type());
        }
    }

    public void Clear()
    {
        text.text = "";
        index = 0;
    }

    public int getIndex()
    {
        return index;
    }

    public int getSentencesLength()
    {
        return sentences.Length;
    }
}
