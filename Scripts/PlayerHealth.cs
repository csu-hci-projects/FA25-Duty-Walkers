using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public float maxHitPoints = 100f;
    public float hitPoints = 100f;
    public float regenerationSkill = 3.5f;
    public float shieldSkill = 0f;

    public AudioSource audio;
    public AudioClip painSound;
    public AudioClip deathSound;

    public Image bloodOverlay;
    public Image playerHealthBarImage;

    private float time = 0f;

    [SerializeField] private PlayerHealthBar playerHealthBar;

    private void Start()
    {
        hitPoints = maxHitPoints;

        if(playerHealthBar == null)
        {
            Debug.LogError("PlayerHealthBar not found in children of " + gameObject.name);
            return;
        }
        playerHealthBar.SetHealthBar(1f);
    }

    private void Update()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            Regenerate();
        }
        if(hitPoints > maxHitPoints) hitPoints = maxHitPoints;

        UpdateBloodOverlay();
    }

    private void UpdateBloodOverlay()
    {
        if(bloodOverlay == null) return;
        if(playerHealthBar == null) return;

        float healthPercent = hitPoints / maxHitPoints;
        //playerHealthBar.fillAmount = healthPercent;
        playerHealthBar.SetHealthBar(healthPercent); 
        
        float transparency = 1f - healthPercent;
        Color imageColor = bloodOverlay.color;
        imageColor.a = transparency;
        bloodOverlay.color = imageColor;

    }

    private void Regenerate()
    {
        if(hitPoints <= 0.0) return;
        if(hitPoints < maxHitPoints)
        {
            hitPoints += Time.deltaTime * regenerationSkill;
        }
    }
    private void PlayerDamage(int damage)
    {
        if(hitPoints < 0.0f) return;

        damage -= (int)shieldSkill;

        if(damage > 0)
        {
            hitPoints -= damage;
            if(audio != null && painSound != null) audio.PlayOneShot(painSound, 1.0f / audio.volume);
            time = 2.0f;

            if(hitPoints <= 0.0f) StartCoroutine(Die());
        }
        else
        {
            damage = 0;
        }
    }

    private IEnumerator Die()
    {
        Debug.Log("The Horde Has Consumed You!");

        if (audio && deathSound)
            audio.PlayOneShot(deathSound);

        // Disable controls
        // FirstPersonController controller = GetComponent<FirstPersonController>();
        // if (controller != null)
        //     controller.enabled = false;

        // OPTIONAL: ragdoll, animation trigger, or fade screen
        // animator.SetTrigger("Die");

        yield return new WaitForSeconds(3f);

        // Restart the scene or load game over
        //UnityEngine.SceneManagement.SceneManager.LoadScene(
            //UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        //);
    }
}