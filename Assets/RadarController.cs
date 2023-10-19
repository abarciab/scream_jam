using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class TransformMarkerPair
{
    public Transform worldTransform;
    public Transform markerTransform;
    public Sound beepSound;

    public TransformMarkerPair()
    {}

    public TransformMarkerPair(Transform world, Transform marker, Sound beepSound)
    {
        worldTransform = world;
        markerTransform = marker;
        this.beepSound = GameObject.Instantiate(beepSound);
    }
}

public class RadarController : MonoBehaviour
{
    [SerializeField] float turnSpeed;
    [SerializeField] Transform spinLineParent;
    [SerializeField] float maxDist, scaleMod;

    [SerializeField] List<Transform> obstacles = new List<Transform>();
    List<TransformMarkerPair> obstacleData = new List<TransformMarkerPair>();
    [SerializeField] Transform playerMarker, markerParent;
    [SerializeField] GameObject markerPrefab;
    [SerializeField] float angleThreshold = 10;
    Transform player;
    [SerializeField] float currentAngle;

    [Header("Sounds")]
    [SerializeField] Sound radarBeepSound;

    private void Start()
    {
        player = PlayerManager.Instance.submarine.transform;
    }

    void Update()
    {
        spinLineParent.transform.localEulerAngles -= new Vector3(0, 0, turnSpeed * Time.deltaTime);
        if (spinLineParent.transform.localEulerAngles.z < 0) spinLineParent.transform.localEulerAngles += Vector3.forward * 360;
        currentAngle += turnSpeed * Time.deltaTime;
        if (currentAngle > 360) currentAngle -= 360;

        playerMarker.localEulerAngles = new Vector3(0, 0, -player.eulerAngles.y);

        obstacles = new List<Transform> (EnvironmentManager.current.monsters);
        if (obstacles.Count > 0) {
            foreach (var o in obstacles) AddNewMarker(o);
            obstacles.Clear();
        }
        foreach (var o in obstacleData) DisplayObstacle(o, currentAngle);
    }

    void AddNewMarker(Transform worldTransform)
    {
        foreach (var o in obstacleData) if (o.worldTransform == worldTransform) return;
        var newMarker = Instantiate(markerPrefab, markerParent);
        newMarker.SetActive(false);
        var newData = new TransformMarkerPair(worldTransform, newMarker.transform, radarBeepSound);
        obstacleData.Add(newData);
    }

    void DisplayObstacle(TransformMarkerPair data, float currentAngle)
    {
        var worldPos = data.worldTransform.position;
        worldPos.y = 0;
        var playerPos = player.position;
        playerPos.y = 0;
        var dir = (worldPos - playerPos) * scaleMod;
        var newPos = playerMarker.localPosition + new Vector3(dir.x, dir.z, 0);
        data.markerTransform.localPosition = newPos;

        float distanceFactor = Vector2.Distance(worldPos, playerPos) / maxDist;
        if (distanceFactor > 1) return;

        float markerAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        if (markerAngle < 0) markerAngle += 360;

        if (!data.markerTransform.gameObject.activeInHierarchy && Mathf.Abs(markerAngle - currentAngle) < angleThreshold) {
            data.markerTransform.gameObject.SetActive(true);
            data.beepSound.PlaySilent(transform);
            data.beepSound.PercentVolume(1-distanceFactor);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDist);
    }
}
