using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Gameplay.SkillSystem
{
    // public class SkillAreaConfig : ScriptableObject
    // {
    //     public int Radius;
    //     public int MaxCount;
    // }
    
    [Serializable]
    public class SkillAreaConfig 
    {
        public int Radius;
        public int MaxCount;
    }

    public class SkillAreaInputData
    {
        public List<Vector3> InputPoints = new ();
    }
    
    public class InputReaderSkillAreaView : MonoBehaviour
    {
        private const float k_skillPreviewOffsetY = 0.1f;
        private SkillAreaInputData m_inputPoints = new();

        [SerializeField] private SkillAreaConfig m_config;
        [SerializeField] private LineRenderer m_lineRender;

        private async void Start()
        {
            await DrawSkillAreaPreview();
            Debug.LogError("Render end");
        }

        public void Init(SkillAreaConfig config)
        {
            //m_config = config;
            m_config = new SkillAreaConfig()
            {
                MaxCount = 2,
                Radius = 200
            };
        }

        private void Update()
        {
            if (Mouse.current.press.wasPressedThisFrame)
            {
                CheckAndAddInputPoint();
            }
        }

        private void CheckAndAddInputPoint()
        {
            if (GetCurMouseRaycastHit(out var hitInfo))
            {
                var count = m_inputPoints.InputPoints.Count;
                if (count >= m_config.MaxCount)
                {
                    return;
                }

                if (count > 0)
                {
                    if (Vector2.Distance(m_inputPoints.InputPoints[count - 1], hitInfo.point) < 0.001f)
                    {
                        return;
                    }
                }
            
                m_inputPoints.InputPoints.Add(hitInfo.point);
            }
        }

        private async Task DrawSkillAreaPreview()
        {
            bool needExit = false;
            while (!needExit && m_inputPoints.InputPoints.Count < m_config.MaxCount)
            {
                var count = m_inputPoints.InputPoints.Count;
                if (count == 0)
                {
                    DrawStepOne();
                }
                else
                {
                    DrawSkillArea(count);
                }
                await Task.Delay(1);

                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    needExit = true;
                }
            }
        }

        private void DrawStepOne()
        {
            if (!GetCurMouseRaycastHit(out var hitInfo))
            {
                return;
            }
            
            var target = hitInfo.point;
            target.y += k_skillPreviewOffsetY;
            
            m_lineRender.positionCount = 361;
            
            for (int i = 0, angle = -180; i < 360; i++, angle++)
            {
                m_lineRender.SetPosition(i, target + Quaternion.Euler(0, angle, 0) * transform.forward * m_config.Radius);
            }
            
            m_lineRender.SetPosition(360, target);

            transform.position = target;
        }

        private void DrawSkillArea(int inputPointCount)
        {
            if (!GetCurMouseRaycastHit(out var hitInfo))
            {
                Debug.LogError("[DrawSkillArea] Don't find any hit info.");
                return;
            }
            
            m_lineRender.positionCount = inputPointCount + 1;
            for (int i = 0; i < inputPointCount; i++)
            {
                var pos = m_inputPoints.InputPoints[i];
                pos.y += k_skillPreviewOffsetY;
                m_lineRender.SetPosition(i, pos);
            }

            var previewPoint = hitInfo.point;
            previewPoint.y += k_skillPreviewOffsetY;
            m_lineRender.SetPosition(inputPointCount, previewPoint);
            
            Debug.LogError("[DrawSkillArea] Render Skill area end.");
        }
 
        private bool GetCurMouseRaycastHit(out RaycastHit hitInfo)
        {
            var ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
            if (!Physics.Raycast(ray, out hitInfo))
            {
                Debug.LogError("Don't hit anything!");
                return false;
            }

            return true;
        }
    }
}
