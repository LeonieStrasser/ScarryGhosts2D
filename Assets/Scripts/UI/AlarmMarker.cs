using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmMarker : MonoBehaviour
{
    [SerializeField]
    Transform followTarget;
    Camera usedCamera;
    [SerializeField]
    GameObject marker;
    [SerializeField]
    RectTransform markerAnchor;
    [SerializeField]
    Canvas markerCanvas;
    [SerializeField]
    Vector2 borderHorizontal = new Vector2(25f, 25f); // So viel Pixel soll die Marker Anchor Position vom Rand weg sein
    [SerializeField]
    Vector2 borderVertical = new Vector2(25f, 25f);

    bool alarmIsActive = true;

    private GameManager gm;

    private void Awake()
    {
        usedCamera = Camera.main;
        gm = FindObjectOfType<GameManager>();
    }
    private void Update()
    {

        if (followTarget != null)
        {
            if (alarmIsActive)
                UpdatePosition();
        }
        else
        {
            Destroy(markerCanvas.gameObject);
        }


        alarmIsActive = gm.waitingNPCs.Count > 0;
    }

    private void UpdatePosition()
    {
        // Gibt die Position relativ zur kamera zur�ck - in einem normalizeten Space (0,0 -> 1,1 etc.)
        Vector3 transformRelativePosition = usedCamera.WorldToViewportPoint(followTarget.position);
        // Setzt den marker Aktiv, wenn ein Koordinatenwert au�erhalb des screens ist <3
        marker.SetActive(transformRelativePosition.x < 0 || transformRelativePosition.x > 1 || transformRelativePosition.y < 0 || transformRelativePosition.y > 1);
        // Wenn der Marker nicht an ist - brauchen wir den Rest nicht
        if (!marker.activeSelf)
            return;


        Vector3 screenPosition = usedCamera.ViewportToScreenPoint(transformRelativePosition);

        float canvasScale = markerCanvas.transform.localScale.y;

        // Clamp "beschneidet" einen Wert auf ein Minimum oder ein Maximum
        screenPosition.x = Mathf.Clamp(screenPosition.x, borderHorizontal.x * canvasScale, markerCanvas.pixelRect.width - borderHorizontal.y * canvasScale);
        screenPosition.y = Mathf.Clamp(screenPosition.y, borderVertical.x * canvasScale, markerCanvas.pixelRect.height - borderVertical.y * canvasScale);
        screenPosition.z = 0;


        // Sorgt daf�r dass die Screenposition mit der Scalierung vom Canvas verrechnet wird - Sonst funktioniert es only f�r fullHD
        screenPosition.x /= markerCanvas.transform.localScale.x;
        screenPosition.y /= markerCanvas.transform.localScale.y;


        Debug.Log("relative Position: " + transformRelativePosition + " - Screen Position: " + screenPosition);
        markerAnchor.anchoredPosition = screenPosition;
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }
}
