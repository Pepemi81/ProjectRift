using UnityEngine;
using UnityEngine.UI;

namespace ProjectRift.Rendering
{
    [ExecuteAlways]
    public class OverlayRenderOrder : MonoBehaviour
    {
        [Header("Order")]
        [SerializeField] private int _baseRenderQueue = 4000;
        [SerializeField] private int _overlayOrder;

        [Header("Targets")]
        [SerializeField] private bool _includeChildren;
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Graphic[] _graphics;

        public int BaseRenderQueue
        {
            get => _baseRenderQueue;
            set
            {
                _baseRenderQueue = value;
                ApplyRenderQueue();
            }
        }

        public int OverlayOrder
        {
            get => _overlayOrder;
            set
            {
                _overlayOrder = value;
                ApplyRenderQueue();
            }
        }

        public int RenderQueue => _baseRenderQueue + _overlayOrder;

        private void Reset()
        {
            FindTargets();
        }

        private void OnEnable()
        {
            if (!HasTargets()) FindTargets();
            ApplyRenderQueue();
        }

        private void OnValidate()
        {
            if (!HasTargets()) FindTargets();
            ApplyRenderQueue();
        }

        [ContextMenu("Find Targets")]
        public void FindTargets()
        {
            _renderers = _includeChildren
                ? GetComponentsInChildren<Renderer>(true)
                : GetComponents<Renderer>();

            _graphics = _includeChildren
                ? GetComponentsInChildren<Graphic>(true)
                : GetComponents<Graphic>();
        }

        [ContextMenu("Apply Render Queue")]
        public void ApplyRenderQueue()
        {
            ApplyToRenderers();
            ApplyToGraphics();
        }

        #region Internal

        private bool HasTargets()
        {
            return HasAny(_renderers) || HasAny(_graphics);
        }

        private static bool HasAny<T>(T[] values) where T : Object
        {
            if (values == null) return false;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null) return true;
            }

            return false;
        }

        private void ApplyToRenderers()
        {
            if (_renderers == null) return;

            for (int i = 0; i < _renderers.Length; i++)
            {
                Renderer targetRenderer = _renderers[i];
                if (targetRenderer == null) continue;

                Material[] materials = targetRenderer.sharedMaterials;
                for (int j = 0; j < materials.Length; j++)
                {
                    if (materials[j] == null) continue;
                    materials[j].renderQueue = RenderQueue;
                }
            }
        }

        private void ApplyToGraphics()
        {
            if (_graphics == null) return;

            for (int i = 0; i < _graphics.Length; i++)
            {
                Graphic graphic = _graphics[i];
                if (graphic == null || graphic.material == null) continue;

                graphic.material.renderQueue = RenderQueue;
            }
        }

        #endregion
    }
}
