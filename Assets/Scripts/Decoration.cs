using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Decoration : MonoBehaviour
{
    [SerializeField] float generateRadius;
    [SerializeField] Vector2 generateNumRange, sizeModRange, animationSpeedRange;
    [SerializeField] Gradient colorGradient;
    [SerializeField] List<GameObject> instantiated = new List<GameObject>();
    [SerializeField] List<GameObject> prefabs = new List<GameObject>();

    [Header("Options")]
    [SerializeField] bool matchY = true;
    [SerializeField] bool raycastDown;
    [SerializeField, ConditionalField(nameof(raycastDown))] LayerMask raycastLayermask;

    private void Start()
    {
        ColorInstanced();
    }

    void ColorInstanced()
    {
        foreach (var detail in instantiated) {
            var renderers = detail.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.material.color = colorGradient.Evaluate(Random.Range(0.0f, 1));

            var lights = detail.GetComponentsInChildren<Light>();
            foreach (var l in lights) l.color = colorGradient.Evaluate(Random.Range(0.0f, 1));
        }
    }

    [ButtonMethod]
    public void GenerateDetails()
    {
        DeleteDetails();
        var num = Random.Range(generateNumRange.x, generateNumRange.y);
        for (int i = 0; i < num; i++) GenerateDetail();
    }

    [ButtonMethod]
    void DeleteDetails()
    {
        for (int i = 0; i < instantiated.Count; i++) {
            DestroyImmediate(instantiated[i]);
        }
        instantiated.Clear();
    }

    void GenerateDetail()
    {
        var pos = transform.position;
        if (matchY) {
            var point2D = Random.insideUnitCircle * generateRadius;
            pos = transform.TransformPoint(new Vector3(point2D.x, 0, point2D.y));
        }
        else {
            pos = transform.TransformPoint(Random.insideUnitSphere * generateRadius);
        }
        if (raycastDown) {
            var ray = new Ray(pos, Vector3.down);
            bool hit = Physics.Raycast(ray, out var hitData);
            if (hit) pos = hitData.point;
            else return;
        }

        var chosenPrefab = prefabs[Random.Range(0, prefabs.Count)];
        var newDetail = Instantiate(chosenPrefab, transform);
        newDetail.transform.position = pos;
        newDetail.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);

        var anim = newDetail.GetComponentInChildren<Animator>();
        if (anim) anim.speed = Random.Range(animationSpeedRange.x, animationSpeedRange.y);

        newDetail.transform.localScale = Vector3.one * Random.Range(sizeModRange.x, sizeModRange.y);
        instantiated.Add(newDetail);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, generateRadius);
    }
}
