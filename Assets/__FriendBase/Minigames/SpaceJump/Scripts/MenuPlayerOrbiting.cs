using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuPlayerOrbiting : MonoBehaviour
{
    private Vector3 BasePosition;
    private Vector3 BaseRotation;
    [SerializeField] private Vector3 GoTo;
    [SerializeField] private bool rotate;
    [SerializeField] private bool backAndForth;
    [SerializeField] private float rotateAngle;
    [SerializeField] private float duration = 30;
    private DG.Tweening.Core.TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions> rotateTween;
    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> moveTween;

    private bool goingBack; //Only if back and forth is enabled

    private void Awake()
    {
        BasePosition = transform.position;
        BaseRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (backAndForth)
        {
            if (Vector3.Distance(this.transform.position,GoTo) < 0.1f && !goingBack)
            {
                goingBack = true;
                moveTween.Kill();
                rotateTween.Kill();
                moveTween = transform.DOLocalMove(BasePosition, duration);
            }
            else if (Vector3.Distance(this.transform.position, BasePosition) < 0.1f && goingBack)
            {
                goingBack = false;
                moveTween.Kill();
                rotateTween.Kill();
                moveTween = transform.DOLocalMove(GoTo, duration);
            }
        }
    }


    private void OnEnable()
    {
        moveTween = transform.DOLocalMove(GoTo, duration);
        if (rotate)
        {
            rotateTween = transform.DORotate(new Vector3(0, 0, rotateAngle), duration, RotateMode.FastBeyond360) ;
        }
    }   
    private void OnDisable()
    {
        transform.position = BasePosition;
        transform.eulerAngles = BaseRotation;
        moveTween.Kill();
        rotateTween.Kill();
    }

}