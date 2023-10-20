using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class SwitchController : MonoBehaviour
{
    [SerializeField] bool setOn, setOff, forceOn, forceOff;
    [SerializeField] Vector3 onPos, offPos;
    [SerializeField] Quaternion onRot, offRot;
    bool on;
    [SerializeField] float animateTime = 1;
    [SerializeField] Sound toggleSound;
    [SerializeField] AnimationCurve curve;
    [SerializeField] bool startOn = true;

    [Header("Events")]
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    public void ForceOff()
    {
        on = false;
        forceOff = true;
    }

    private void Start()
    {
        on = startOn;
        forceOn = startOn;
        forceOff = !startOn;

        if (Application.isPlaying) toggleSound = Instantiate(toggleSound);
    }

    private void Update()
    {
        if (setOn) {
            setOn = false;
            onRot = transform.localRotation; onPos = transform.localPosition;
        }
        if (setOff) {
            setOff = false;
            offRot = transform.localRotation; offPos = transform.localPosition;
        }
        if (forceOn) {
            forceOn = false;
            transform.localRotation = onRot;
            transform.localPosition = onPos;
        }
        if (forceOff) {
            forceOff = false;
            transform.localRotation = offRot;
            transform.localPosition = offPos;
        }
    }

    public void Toggle()
    {
        on = !on;
        if (on) OnActivate.Invoke();
        else OnDeactivate.Invoke();
        toggleSound.Play(transform);

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        var startRot = transform.localRotation;
        var endRot = on ? onRot : offRot;
        var startPos = transform.localPosition;
        var endPos = on ? onPos : offPos;

        float timePassed = 0;
        while (timePassed < animateTime) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            float progress = curve.Evaluate(timePassed / animateTime);
            transform.localRotation = Quaternion.Lerp(startRot, endRot, progress);
            transform.localPosition = Vector3.Lerp(startPos, endPos, progress);
        }
        transform.localPosition = endPos;
        transform.localRotation = endRot;
    }

}
