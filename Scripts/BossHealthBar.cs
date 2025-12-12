/*

FOR BOSS FIGHT

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BossHealthBar : MonoBehaviour
{
    public Gradient colorGradient;
    LineRenderer healthLine;

    private void Start()
    {
        healthLine = this.GetComponent<LineRenderer>();
    }

    public void SetHealthBar(float targetPercentage)
    {
        healthLine.positionCount = 2;
        healthLine.SetPosition(0, Vector3.zero);

        float currentPercentage = healthLine.GetPosition(1).x;
        targetPercentage = Mathf.Clamp(targetPercentage, 0, 1);
        StartCoroutine(AnimateHealthBar(currentPercentage, targetPercentage, .3f));
    }

    IEnumerator AnimateHealthBar(float startingPercentage, float targetPercentage, float animationTime)
    {
        float timer = 0.0f;
        while(timer < animationTime)
        {
            timer += Time.deltaTime;
            float timePercentage = timer / animationTime;
            float currentPercentage = Mathf.Lerp(startingPercentage, targetPercentage, timePercentage);
            healthLine.SetPosition(1, new Vector3(currentPercentage, 0, 0));
            AssignColorAtPercent(currentPercentage);

            yield return null;
        }

        healthLine.SetPosition(1, new Vector3(targetPercentage, 0, 0));
        AssignColorAtPercent(targetPercentage);

        yield return null;
    }

    private void AssignColorAtPercent(float percent)
    {
        Color c = colorGradient.Evaluate(percent);
        healthLine.startColor = c;
        healthLine.endColor = c;
    }
}

*/