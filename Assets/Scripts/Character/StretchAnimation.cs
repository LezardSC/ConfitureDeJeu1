using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchAnimation : MonoBehaviour
{
    public GameObject characterModel;

    public void DoStretch(string coroutineName)
    {
        StartCoroutine(coroutineName);
    }

    IEnumerator StretchJumpAnimation()
    {
        void DoPositiveStuff()
        {
            characterModel.transform.localScale += new Vector3(-0.01f, 0.05f, -0.01f);
        }

        void DoNegativeStuff()
        {
            characterModel.transform.localScale -= new Vector3(-0.01f, 0.05f, -0.01f);
        }

        for (int i = 0; i < 8; i++)
        {
            DoPositiveStuff();
            yield return new WaitForSeconds(0.025f);
        }
        for (int i = 0; i < 8; i++)
        {
            DoNegativeStuff();
            yield return new WaitForSeconds(0.025f);
        }
    }
}
