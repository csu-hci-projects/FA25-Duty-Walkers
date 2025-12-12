using UnityEngine;
using UnityEngine.UI;

public class AimingCrossHair : MonoBehaviour{
    public Image crossHair;
    public Color defaultColor = Color.white;
    public Color enemyColor = Color.red;
    public float maxDistance = 100f;
    public LayerMask enemyLayer;

    public float normalScale = 1f;
    public float enemyScale = 1.3f;
    public float scaleSpeed = 8f;

    private bool isZooming = false;

    private RectTransform rect;

    private void Start()
    {
        rect = crossHair.GetComponent<RectTransform>();
    }

    private void Update()
    {
        checkZoom();

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool hoveringEnemy = Physics.Raycast(ray, out hit, maxDistance, enemyLayer);

        if (!isZooming)
        {
            if (hoveringEnemy)
            {
                crossHair.color = enemyColor;
                float newScale = Mathf.Lerp(rect.localScale.x, enemyScale, Time.deltaTime * scaleSpeed);
                rect.localScale = new Vector3(newScale, newScale, 1);
            }
            else
            {
                crossHair.color = defaultColor;
                float newScale = Mathf.Lerp(rect.localScale.x, normalScale, Time.deltaTime * scaleSpeed);
                rect.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }
    private void checkZoom()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isZooming = true;
            crossHair.enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            isZooming = false;
            crossHair.enabled = true;
        }
    }
}