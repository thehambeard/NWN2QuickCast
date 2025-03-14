using Kingmaker.UI.Tooltip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;

namespace NWN2QuickCast.UI.WindowControl
{
    public class ScalableWindow : ControlBase
    {
        [SerializeField]
        private float _maxScale = 3f;

        [SerializeField]
        private float _minScale = .3f;

        public override void MoveAction(Vector2 vector)
        {
            var x = vector.x;

            if (x < 100f && x > 0f)
                _ownRectTransform.localScale = UIUtility.LimitScaleRectInRect(
                    UIUtility.MapValueVector(0f, 100f, _currentScale.x, _maxScale, x),
                    _parentRectTransform,
                    _ownRectTransform);

            if (x > -100f && x < 0f)
                _ownRectTransform.localScale = UIUtility.MapValueVector(0f, -100f, _currentScale.x, _minScale, x);
        }
    }
}
