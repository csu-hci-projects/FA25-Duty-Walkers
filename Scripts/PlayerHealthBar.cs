using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Gradient colorGradient;
    public Image fillImage;
    Coroutine currentAnimation;

    private void Awake()
    {
        if(fillImage == null) fillImage = GetComponent<Image>();
    }

    public void SetHealthBar(float targetPercentage)
    {
       targetPercentage = Mathf.Clamp01(targetPercentage);

       if(currentAnimation != null) StopCoroutine(currentAnimation);

        float currentPercentage = fillImage.fillAmount;
        currentAnimation = StartCoroutine(AnimateHealthBar(currentPercentage, targetPercentage, 0.3f));
    }

    IEnumerator AnimateHealthBar(float startingPercentage, float targetPercentage, float animationTime)
    {
        float timer = 0.0f;
        while(timer < animationTime)
        {
            timer += Time.deltaTime;
            float timePercentage = timer / animationTime;
            float currentPercentage = Mathf.Lerp(startingPercentage, targetPercentage, timePercentage);

            fillImage.fillAmount = currentPercentage;
            fillImage.color = colorGradient.Evaluate(currentPercentage);

            yield return null;
        }
        fillImage.fillAmount = targetPercentage;
        fillImage.color = colorGradient.Evaluate(targetPercentage);
    }
}