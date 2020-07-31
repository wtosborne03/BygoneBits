using UnityEngine;
using System.Collections;

public static class AudioFadeOut
{

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

    }
    public static IEnumerator PitchOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.pitch;

        while (audioSource.pitch > 0)
        {
            audioSource.pitch -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

    }
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume < 1)
        {
            audioSource.volume += 1 * Time.deltaTime / FadeTime;

            yield return null;
        }


    }

}
public static class CGFade
{

    public static IEnumerator FadeOut(CanvasGroup cg, float FadeTime)
    {
        float startAlpha = cg.alpha;

        while (cg.alpha > 0)
        {
            cg.alpha -= startAlpha * Time.deltaTime / FadeTime;

            yield return null;
        }

    }
    public static IEnumerator FadeIn(CanvasGroup cg, float FadeTime)
    {
        float startAlpha = cg.alpha;

        while (cg.alpha < 1)
        {
            cg.alpha += 1 * Time.deltaTime / FadeTime;

            yield return null;
        }


    }
}
public static class savetool
{

    public static int savebool(bool b)
    {
        if (b)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    public static bool loadbool(int b)
    {
        if (b == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}